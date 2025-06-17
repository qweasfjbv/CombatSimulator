using System.Collections.Generic;
using UnityEngine;
using Defense.Utils;
using Defense.Manager;
using Defense.Interfaces;
using IUtil;
using DG.Tweening;
using Defense.Props;

namespace Defense.Controller
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider))]
	public abstract partial class UnitController : MonoBehaviour
		,IAttackable
		,IDamagable
		,ISkillable
	{
		/** Components **/
		private Transform myTransform;
		private Animator animator = null;

		/** SO Datas **/
		protected UnitData unitData = null;

		/** Target Infos **/
		private Collider[] targets;
		private Transform targetTransform = null;
		private Vector3 targetPosition = Vector3.zero;

		/** Pre-load Variables **/
		private PlacementSlot mySlot = null;
		public PlacementSlot MySlot { get => mySlot; set => mySlot = value; }

		private float attackClipLength = 0f;
		private float damagedClipLength = 0f;
		private float deathClipLength = 0f;
		private float skillClipLength = 0f;

		private int animIDAttack = 0;
		private int animIDAttackMT = 0;
		private int animIDDamagedMT = 0;
		private int animIDDeathMT = 0;
		private int animIDSpeed = 0;
		private int animIDDamaged = 0;
		private int animIDDeath = 0;
		private int animIDSkill = 0;
		private int animIDSkillMT = 0;

		/** State Variables **/
		private float currentAttackCooltime = 0f;
		private bool isChasing = false;
		private bool isAttacking = false;

		public abstract bool IsSameUnit(int unitId, int rarity);
		public abstract void Attack(Transform target);
		protected abstract void ExecuteSkill(Transform target);

		private void Awake()
		{
			myTransform = GetComponent<Transform>();
			animator = GetComponent<Animator>();

			attackClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_ATTACK);
			damagedClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_DAMAGE);
			deathClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_DEATH);
			skillClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_SKILL);

			animIDAttack = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK);
			animIDAttackMT = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK_MT);
			animIDDamagedMT = Animator.StringToHash(Constants.ANIM_PARAM_DAMAGED_MT);
			animIDDeathMT = Animator.StringToHash(Constants.ANIM_PARAM_DEATH_MT);
			animIDSpeed = Animator.StringToHash(Constants.ANIM_PARAM_SPEED);
			animIDDamaged = Animator.StringToHash(Constants.ANIM_PARAM_DAMAGED);
			animIDDeath = Animator.StringToHash(Constants.ANIM_PARAM_DIED);
			animIDSkill = Animator.StringToHash(Constants.ANIM_PARAM_SKILL);
			animIDSkillMT = Animator.StringToHash(Constants.ANIM_PARAM_SKILL_MT);
		}

		[Header("DEBUG")]
		public int enemyId;

		private void Update()
		{
			if (unitData == null) return;
			UpdateCooltimeTick();
			UpdateKnockbackRemainedTime();

			if (IsKnockBack || isEnemyDead) return;
			OnUpdateUnit();
		}

		/// <summary>
		/// Update logics for walking/chasing on the ground
		/// </summary>
		public void OnUpdateUnit()
		{
			CheckNearbyTarget();

			if (isAttacking)
			{
				if (IsAbleToAttack())
				{
					if (IsAbleToUseSkill())
					{
						StartSkillAnim();
					}
					else
					{
						StartAttackAnim();
					}
				}
			}
			else if (isChasing)
			{
				ChaseTarget();
			}
		}

		[Button(nameof(enemyId))]
		public void InitUnit(int enemyId)
		{
			unitData = Managers.Resource.GetEnemyData(enemyId);
			CacheStatData(unitData.StatsByLevel[0]);

			targets = new Collider[unitData.MaxDetectCounts];
		}

		private int targetCounts = 0;
		private void CheckNearbyTarget()
		{
			targetCounts = Physics.OverlapSphereNonAlloc(transform.position, unitData.SearchRange, targets, unitData.TargetLayer);
			if (targetCounts > 0)
			{
				float minDistance = float.MaxValue;
				Transform closestTarget = null;

				for (int i = 0; i < targetCounts; i++)
				{
					if (targets[i] == null) break; 
					if (targets[i].GetComponent<IDamagable>() == null ||
					!targets[i].GetComponent<IDamagable>().IsAbleToTargeted()) continue;

					float distance = Vector3.SqrMagnitude(transform.position - targets[i].transform.position);

					if (distance < minDistance)
					{
						minDistance = distance;
						closestTarget = targets[i].transform;
					}
				}

				targetTransform = closestTarget;
				if (targetTransform == null) return;
				isChasing = true;
				isAttacking = false;
			}
			else
			{
				targetTransform = null;
				isChasing = false;
				isAttacking = false;
			}

			if (targetTransform != null && Vector3.SqrMagnitude(transform.position- targetTransform.position) <= unitData.AttackRange* unitData.AttackRange)
			{
				isAttacking = true;
				isChasing = false;
			}
		}
		private void ChaseTarget()
		{
			if (targetTransform == null)
			{
				isChasing = false;
				return;
			}

			Vector3 dir = targetTransform.position - myTransform.position;
			dir.y = 0;

			Quaternion targetRotation = Quaternion.LookRotation(dir);
			myTransform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, unitData.RotationSpeed* Time.deltaTime);
			myTransform.position = Vector3.MoveTowards(transform.position, targetTransform.position, unitData.MoveSpeed * Time.deltaTime);

			GetComponent<Animator>().SetFloat(animIDSpeed, (targetTransform.position - myTransform.position).AbsSum());
		}

		private void OnDrawGizmos()
		{
			if (unitData == null) return;

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, unitData.SearchRange);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, unitData.AttackRange);
		}

		public static float hoverHeight = 1f;
		public static float hoverDuration = 0.2f;
		public static float moveDuration = 0.2f;

		private Tween currentTween = null;
		private bool isDragging = false;

		public void PickUp(float baseHeight)
		{
			if (currentTween != null) currentTween.Kill();

			currentTween = transform.DOMoveY(baseHeight + hoverHeight, hoverDuration)
				.SetEase(Ease.OutQuad);
		}
		public void DropTo(Vector3 targetSlotPos)
		{
			isDragging = false;

			if (currentTween != null) currentTween.Kill();
			transform.position = new Vector3(targetSlotPos.x, targetSlotPos.y + hoverHeight, targetSlotPos.z);

			Sequence seq = DOTween.Sequence();
			seq.Append(transform.DOMoveY(targetSlotPos.y, hoverDuration).SetEase(Ease.InQuad));
			currentTween = seq;
		}

	}
}
