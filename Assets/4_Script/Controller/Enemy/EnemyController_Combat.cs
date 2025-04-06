using Defense.Utils;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using Defense.Interfaces;
using UnityEngine;
using Defense.Manager;

namespace Defense.Controller
{
	public partial class EnemyController
	{
		private float currentHP = 0f;
		private float currentAtk = 0f;
		private float currentDef = 0f;

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
		}

		bool IAttackable.IsAbleToAttack()
		{
			return currentAttackCooltime < 0f;
		}
		void IAttackable.StartAttackAnim()
		{
			if (targetTransform == null)
			{
				isAttacking = false;
				isChasing = false;
				return;
			}

			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDAttack);
			animator.SetFloat(animIDAttackMT, attackClipLength / enemyData.AttackCooltime);

			currentAttackCooltime = enemyData.AttackCooltime;
		}
		void IAttackable.Attack(Transform target)
		{
			// TODO - Attack 로직 필요
		}

		void IAttackable.UpdateCooltimeTick()
		{
			if (currentAttackCooltime >= 0f)
				currentAttackCooltime -= Time.deltaTime;
		}

		bool IDamagable.IsAbleToTargeted()
		{
			return afterHP > 0f;
		}
		void IDamagable.ReserveDamage(DamageType type, float damage, float duration)
		{
			if (isEnemyDead) return;

			float trueDamage = Calculation.CalculateDamage(currentDef, type, damage);
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
					damagable.GetDelayedDamage(type, damage);
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

		void IDamagable.GetDelayedDamage(DamageType type, float trueDamage)
		{
			if (isEnemyDead) return;

			currentHP -= trueDamage;
			damagable.CheckIfDied();
			ApplyKnockback();
		}
		void IDamagable.GetImmediateDamage(DamageType type, float damage)
		{
			if (isEnemyDead) return;

			float trueDamage = Calculation.CalculateDamage(currentDef, type, damage);
			afterHP -= trueDamage;
			currentHP -= trueDamage;

			damagable.CheckIfDied();
			ApplyKnockback();
		}
		void IDamagable.CheckIfDied()
		{
			if (currentHP <= 0f)
			{
				OnDead();
			}
		}

		private void OnDead()
		{
			if (isEnemyDead) return;
			isEnemyDead = true;
			animator.SetFloat(animIDSpeed, 0f);
			animator.SetFloat(animIDDeathMT, deathClipLength / enemyData.DeathAnimDuration);
			animator.SetBool(animIDDeath, true);

			GetComponent<Outline>().OffOutline();

			DelayedDestroy(enemyData.DeathAnimDuration, enemyData.FadeOutDuration).Forget();
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

		private bool IsKnockBack { get => enemyData.UseKnockback && knockbackRemainedTime > Mathf.Epsilon; }
		private float knockbackRemainedTime = 0f;
		private void ApplyKnockback()
		{
			knockbackRemainedTime = enemyData.KnockbackDuration;

			animator.SetFloat(animIDSpeed, 0f);
			animator.SetFloat(animIDDamagedMT, damagedClipLength / knockbackRemainedTime);
			animator.SetTrigger(animIDDamaged);
			ParticleManager.Instance.SpawnParticle(ParticleType.Hit, myTransform.position);
		}
		private void OnUpdateKnockbackRemainedTime()
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
