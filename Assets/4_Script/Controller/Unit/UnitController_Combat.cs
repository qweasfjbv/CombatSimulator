using Defense.Utils;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using Defense.Manager;
using Defense.Interfaces;

namespace Defense.Controller
{
	public partial class UnitController
	{
		private float currentHP = 0f;
		private float currentAtk = 0f;
		private float currentDef = 0f;
		private float currentMP = 0f;

		private float afterHP = 0f;
		
		private Dictionary<int, CancellationTokenSource> reservedDamage = new();
		private int damageId = 0;

		private bool isEnemyDead = false;

		private void CacheStatData(LevelStat stat)
		{
			damageId = 0;
			currentHP = stat.MaxHealth;
			afterHP = stat.MaxHealth;
			currentAtk = stat.AttackPower;
			currentDef = stat.DefensePower;
		}
		private void InitCombat()
		{
			isEnemyDead = false;
			animator.SetBool(animIDDeath, false);
			currentMP = 0f;
		}


		/** IAttackable Interface **/
		public bool IsAbleToAttack()
		{
			return currentAttackCooltime < 0f;
		}
		public void StartAttackAnim()
		{
			if (targetTransform == null)
			{
				isAttacking = false;
				isChasing = false;
				return;
			}

			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDAttack);
			animator.SetFloat(animIDAttackMT, attackClipLength / unitData.AttackCooltime);

			currentAttackCooltime = unitData.AttackCooltime;
		}
		/// <summary>
		/// 자식 클래스에서 Detail 구현
		/// </summary>
		public abstract void Attack(Transform target);
		public void UpdateCooltimeTick()
		{
			if (currentAttackCooltime >= 0f)
				currentAttackCooltime -= Time.deltaTime;
		}

		/** IDamagable Interface **/
		public bool IsAbleToTargeted()
		{
			return afterHP > 0f;
		}
		public void ReserveDamage(DamageType type, float damage, float duration)
		{
			if (isEnemyDead) return;

			float trueDamage = Calculation.CalculateDamage(unitData.StatsByLevel[0], type, damage);
			afterHP -= trueDamage;

			var cts = new CancellationTokenSource();
			reservedDamage[damageId] = cts;

			DelayedDamage(type, trueDamage, duration, cts.Token).Forget();
		}

		/// <summary>
		/// Delay 된 데미지를 입히는 함수
		/// 취소 시 catch 부분 실행됨
		/// </summary>
		private async UniTaskVoid DelayedDamage(DamageType type, float damage, float duration, CancellationToken ct)
		{
			try
			{
				await UniTask.Delay((int)(duration * 1000));

				if (!ct.IsCancellationRequested)
					GetDelayedDamage(type, damage);
			}
			catch (OperationCanceledException)
			{
				// Cancelled
			}
			finally
			{
				reservedDamage.Remove(damageId);
			}
		}
		public void GetDelayedDamage(DamageType type, float trueDamage)
		{
			if (isEnemyDead) return;

			currentHP -= trueDamage; 

			UIManager.Instance.GameUI.ShowDamage(transform.position + Vector3.up * 1.8f, trueDamage, type, HitResultType.Normal);
			CheckIfDied();
			ApplyKnockback();
		}
		public void GetImmediateDamage(DamageType type, float damage)
		{
			if (isEnemyDead) return;

			float trueDamage = Calculation.CalculateDamage(unitData.StatsByLevel[0], type, damage);
			afterHP -= trueDamage;
			currentHP -= trueDamage;

			UIManager.Instance.GameUI.ShowDamage(transform.position + Vector3.up * 1.8f, trueDamage, type, HitResultType.Normal);
			CheckIfDied();
			ApplyKnockback();
		}
		public void CheckIfDied()
		{
			if (currentHP <= 0f)
			{
				OnDead();
			}
		}

		/** Dying System **/
		private void OnDead()
		{
			if (isEnemyDead) return;
			isEnemyDead = true;
			animator.SetFloat(animIDSpeed, 0f);
			animator.SetFloat(animIDDeathMT, deathClipLength / unitData.DeathAnimDuration);
			animator.SetBool(animIDDeath, true);

			GetComponent<Outline>().OffOutline();

			DelayedDestroy(unitData.DeathAnimDuration, unitData.FadeOutDuration).Forget();
		}
		public async UniTask DelayedDestroy(float deathDuration, float fadeDuration)
		{
			try
			{
				await UniTask.Delay((int)(deathDuration * 1000));
				// TODO - FadeOut
				await UniTask.Delay((int)(fadeDuration * 1000));
				Destroy(this.gameObject);
			}
			catch (System.OperationCanceledException)
			{
				Debug.Log("Delay was cancelled");
			}
		}

		/** Knockback System **/
		private bool IsKnockBack { get => unitData.UseKnockback && knockbackRemainedTime > Mathf.Epsilon; }
		private float knockbackRemainedTime = 0f;
		private void ApplyKnockback()
		{
			knockbackRemainedTime = unitData.KnockbackDuration;

			animator.SetFloat(animIDSpeed, 0f);
			animator.SetFloat(animIDDamagedMT, damagedClipLength / knockbackRemainedTime);
			animator.SetTrigger(animIDDamaged);
			PoolingManager.Instance.SpawnParticle(ParticleType.Hit, myTransform.position);
		}
		private void UpdateKnockbackRemainedTime()
		{
			if (IsKnockBack)
				knockbackRemainedTime -= Time.deltaTime;

			animator.SetLayerWeight(1, knockbackRemainedTime > Mathf.Epsilon ? 1f : 0f);
		}

		private void OnDestroy()
		{
			foreach (var cts in reservedDamage.Values)
				cts.Cancel();
		}
	}
}
