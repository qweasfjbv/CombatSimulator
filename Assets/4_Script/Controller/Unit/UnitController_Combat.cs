using Combat.Utils;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using Combat.Manager;
using Combat.Interfaces;
using DG.Tweening;

namespace Combat.Controller
{
	/// <summary>
	/// UnitController의 전투 관련 인터페이스 구현 및 함수 구현
	/// </summary>
	public partial class UnitController
	{
		private float currentHP = 0f;
		private float currentAtk = 0f;
		private float currentDef = 0f;
		private float currentMP = 0f;
		private float maxMP = float.MaxValue;

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
			maxMP = stat.MaxMP;
		}
		private void InitCombat()
		{
			isEnemyDead = false;
			animator.SetBool(animIDDeath, false);
			currentMP = 0f;
			CacheStatData(unitData.StatsByLevel[0]);
		}

		private Transform attackTarget = null;
		private int skillTargetCount = 0;
		private Transform[] skillTargets = new Transform[10];

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

			attackTarget = targetTransform;
			transform.LookAt(attackTarget);
			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDAttack);
			animator.SetFloat(animIDAttackMT, attackClipLength / unitData.AttackCooltime);

			currentAttackCooltime = unitData.AttackCooltime;
		}
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

		/** ISkillable Interface **/
		public bool IsAbleToUseSkill()
		{
			// HACK - 특정 레벨 이상에만 열리도록?
			return currentMP >= maxMP;
		}
		public void StartSkillAnim()
		{
			if (targetTransform == null)
			{
				isAttacking = false;
				isChasing = false;
				return;
			}

			// HACK - 임시 스킬 테스트용
			skillTargets[0] = targetTransform;
			skillTargetCount = 1;

			transform.LookAt(targetTransform);
			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDSkill);
			animator.SetFloat(animIDSkillMT, skillClipLength / unitData.SkillDuration);
			currentMP = 0;
			currentAttackCooltime = unitData.SkillDuration;
		}

		/** Pre-Calculate Damage System **/
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
				gameObject.SetActive(false);
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
