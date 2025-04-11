using System.Collections.Generic;
using UnityEngine;
using Defense.Utils;
using Defense.Manager;
using Defense.Interfaces;
using IUtil;
using DG.Tweening;

namespace Defense.Controller
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider))]
	public abstract partial class UnitController : MonoBehaviour
		,IAttackable
		,IDamagable
	{
		/** Components **/
		private Transform myTransform;
		private Animator animator = null;

		/** SO Datas **/
		protected UnitData unitData = null;
		private RouteData routeData = null;

		/** Target Infos **/
		private Collider[] targets;
		private Transform targetTransform = null;
		private Vector3 targetPosition = Vector3.zero;

		/** Pre-load Variables **/
		private List<Vector3> waypoints = new List<Vector3>();
		private float widthOffset = 0f;

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
		private int currentWaypointIndex = 0;
		private bool isChasing = false;
		private bool isAttacking = false;


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
		public bool isTowerUnit = false;
		public int routeId;
		public int enemyId;

		private void Update()
		{
			if (unitData == null) return;
			UpdateCooltimeTick();
			UpdateKnockbackRemainedTime();

			if (IsKnockBack || isEnemyDead) return;

			if (isTowerUnit)
			{
				OnUpdateTowerUnit();
			}
			else
			{
				OnUpdateInfantry();
			}
		}

		/// <summary>
		/// Update logics for walking/chasing on the ground
		/// </summary>
		public void OnUpdateInfantry()
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
			else
			{
				FollowPath();
			}
		}

		/// <summary>
		/// Update logics for tower unit
		/// </summary>
		private void OnUpdateTowerUnit()
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
		}

		[Button(nameof(routeId), nameof(enemyId))]
		public void InitEnemy(int routeId, int enemyId)
		{
			routeData = Managers.Resource.GetRouteData(routeId);
			unitData = Managers.Resource.GetEnemyData(enemyId);
			CacheStatData(unitData.StatsByLevel[0]);

			targets = new Collider[unitData.MaxDetectCounts];
			waypoints = routeData.Waypoints;

			if (isTowerUnit) return;
			GetRandomStartPosition();
			targetPosition = waypoints[currentWaypointIndex + 1] + widthOffset * Calculation.GetConsistentBisector(
				-(waypoints[currentWaypointIndex + 1] - waypoints[currentWaypointIndex]),
				GetDirFromPath(currentWaypointIndex + 1)
			);
		}
		public abstract bool IsSameUnit(int unitId, int rarity);
		private void GetRandomStartPosition()
		{
			Vector3 dir = waypoints[1] - waypoints[0];
			dir = (Quaternion.Euler(0, 90, 0) * dir).normalized;

			widthOffset = Random.Range(-routeData.WayWidth / 2f, routeData.WayWidth / 2f);
			transform.position = waypoints[0] + dir * widthOffset;
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

		private Vector3 dir = Vector3.zero;
		private Quaternion targetRotation = Quaternion.identity;
		private void FollowPath()
		{
			dir = waypoints[currentWaypointIndex + 1] - waypoints[currentWaypointIndex];
			targetRotation = Quaternion.LookRotation(targetPosition - myTransform.position);

			myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, unitData.RotationSpeed * Time.deltaTime);
			myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, unitData.MoveSpeed* Time.deltaTime);

			if (Vector3.Dot(dir, targetPosition - myTransform.position) < Mathf.Epsilon)
			{
				currentWaypointIndex++;
				if (currentWaypointIndex >= waypoints.Count - 1) { Destroy(this.gameObject); return; }

				targetPosition = waypoints[currentWaypointIndex + 1] + widthOffset * Calculation.GetConsistentBisector(
					-(waypoints[currentWaypointIndex + 1] - waypoints[currentWaypointIndex]),
					GetDirFromPath(currentWaypointIndex + 1)
				);
			}

			GetComponent<Animator>().SetFloat(animIDSpeed, (targetPosition - myTransform.position).AbsSum());
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

		public Vector3 GetDirFromPath(int index)
		{
			if (index + 1 == waypoints.Count)
			{
				return waypoints[index] - waypoints[index - 1];
			}

			return waypoints[index + 1] - waypoints[index];
		}

		/** Animation Events **/
		public void OnAttack()
		{
			Attack(targetTransform);
			// HACK
			currentMP += 30f;
		}
		public void OnSkill()
		{
			ExecuteSkill(targetTransform);
		}

		private void StartSkillAnim()
		{
			if (targetTransform == null)
			{
				isAttacking = false;
				isChasing = false;
				return;
			}

			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDSkill);
			animator.SetFloat(animIDSkillMT, skillClipLength / unitData.SkillDuration);
			currentMP = 0;
			currentAttackCooltime = unitData.SkillDuration;
		}
		private bool IsAbleToUseSkill()
		{
			// HACK - 특정 레벨 이상에만 열림
			return currentMP >= 10f;
		}
		protected abstract void ExecuteSkill(Transform target);

		private void OnDrawGizmos()
		{
			if (unitData == null) return;

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, unitData.SearchRange);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, unitData.AttackRange);
		}


		public float hoverHeight = 1f;
		public float hoverDuration = 0.2f;
		public float moveDuration = 0.2f;

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
