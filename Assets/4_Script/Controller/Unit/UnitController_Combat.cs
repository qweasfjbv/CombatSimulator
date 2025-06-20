using Autobattler.Utils;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using Autobattler.Manager;

namespace Autobattler.Controller
{

	public class ReservationKey : IComparable<ReservationKey>
	{
		public int dID;
		public float time;

		public ReservationKey(int dID, float time)
		{
			this.dID = dID;
			this.time = time;
		}

		public int CompareTo(ReservationKey other)
		{
			if (other == null) return 1;

			int timeComparison = time.CompareTo(other.time);
			if (timeComparison != 0)
				return timeComparison;

			return dID.CompareTo(other.dID);
		}
	}

	public class DamageReservation
	{
		public float damage;
		public DamageType type;
		public CancellationTokenSource cancellationTokenSource;

		public DamageReservation(CancellationTokenSource cancellationTokenSource, float damage, DamageType type)
		{
			this.cancellationTokenSource = cancellationTokenSource;
			this.damage = damage;
			this.type = type;
		}
	}

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
		
		private SortedDictionary<ReservationKey, DamageReservation> reservedDamage = new();
		private int damageId = 0;

		private bool isEnemyDead = false;

		private void CacheStatData(LevelStat stat)
		{
			damageId = 0;
			currentHP = stat.MaxHealth;
			currentAtk = stat.AttackPower;
			currentDef = stat.DefensePower;
			maxMP = stat.MaxMP;
		}
		private void InitCombat()
		{
			isEnemyDead = false;
			isTargetFlagDirty = true;
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

		private bool isTargetFlagDirty = true;
		private bool isAbleToTargeted = false;

		/** IDamagable Interface **/
		public bool IsAbleToTargeted(float duration)
		{
			if (currentHP <= 0f) return false;
			if (!isTargetFlagDirty) return isAbleToTargeted;

			float tmpHP = currentHP;
			float lastTime = 0;

			foreach (var res in reservedDamage)
			{
				tmpHP -= Calculation.CalculateDamage(unitData.StatsByLevel[0], res.Value.type, res.Value.damage);
				if (tmpHP <= 0f)
				{
					lastTime = res.Key.time;
					break;
				}
			}

			isAbleToTargeted = (tmpHP > 0f || (tmpHP <= 0f && Time.time + duration < lastTime));
			isTargetFlagDirty = false;
			return isAbleToTargeted;
		}
		public void ReserveDamage(DamageType type, float damage, float duration)
		{
			if (isEnemyDead) return;

			var cts = new CancellationTokenSource();
			ReservationKey resKey = new ReservationKey(++damageId, Time.time + duration);
			reservedDamage[resKey] = new DamageReservation(cts, damage, type);
			isTargetFlagDirty = true;

			DelayedDamage(type, damage, duration, resKey).Forget();
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
		private async UniTaskVoid DelayedDamage(DamageType type, float damage, float duration, ReservationKey resKey)
		{
			try
			{
				await UniTask.Delay((int)(duration * 1000));

				if (!reservedDamage[resKey].cancellationTokenSource.IsCancellationRequested)
					GetImmediateDamage(type, damage);
			}
			catch (OperationCanceledException)
			{
				// Cancelled
			}
			finally
			{
				reservedDamage.Remove(resKey);
			}
		}

		public void GetImmediateDamage(DamageType type, float damage)
		{
			if (isEnemyDead) return;

			float trueDamage = Calculation.CalculateDamage(unitData.StatsByLevel[0], type, damage);
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
			foreach (var res in reservedDamage.Values)
				res.cancellationTokenSource.Cancel();
		}
	}
}
