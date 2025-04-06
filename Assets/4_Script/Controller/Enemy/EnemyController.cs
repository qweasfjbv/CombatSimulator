using System.Collections.Generic;
using UnityEngine;
using Defense.Utils;
using Defense.Manager;
using Defense.Interfaces;
using NUnit.Framework.Internal;

namespace Defense.Controller
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider))]
	public partial class EnemyController : MonoBehaviour
		,IAttackable
		,IDamagable
	{

		[SerializeField]
		private ParticleSystem hitParticle;

		/** Components **/
		private Transform myTransform;
		private Animator animator = null;

		private IAttackable attackable = null;
		private IDamagable damagable = null;

		/** SO Datas **/
		private EnemyData enemyData = null;
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

		private int animIDAttack = 0;
		private int animIDAttackMT = 0;
		private int animIDDamagedMT = 0;
		private int animIDDeathMT = 0;
		private int animIDSpeed = 0;
		private int animIDDamaged = 0;
		private int animIDDeath = 0;

		/** State Variables **/
		private float currentAttackCooltime = 0f;
		private int currentWaypointIndex = 0;
		private bool isChasing = false;
		private bool isAttacking = false;


		private void Awake()
		{
			myTransform = GetComponent<Transform>();
			animator = GetComponent<Animator>();

			attackable = (IAttackable)this;
			damagable = (IDamagable)this;

			attackClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_ATTACK);
			damagedClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_DAMAGE);
			deathClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_DEATH);

			animIDAttack = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK);
			animIDAttackMT = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK_MT);
			animIDDamagedMT = Animator.StringToHash(Constants.ANIM_PARAM_DAMAGED_MT);
			animIDDeathMT = Animator.StringToHash(Constants.ANIM_PARAM_DEATH_MT);
			animIDSpeed = Animator.StringToHash(Constants.ANIM_PARAM_SPEED);
			animIDDamaged = Animator.StringToHash(Constants.ANIM_PARAM_DAMAGED);
			animIDDeath = Animator.StringToHash(Constants.ANIM_PARAM_DIED);
		}

		private void Update()
		{
			if (enemyData == null) return;
			attackable.UpdateCooltimeTick();
			OnUpdateKnockbackRemainedTime();

			if (IsKnockBack || isEnemyDead) return;

			CheckNearbyTarget();

			if (isAttacking)
			{
				if (attackable.IsAbleToAttack())
					attackable.StartAttackAnim();
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

		public void InitEnemy(int routeId, int enemyId)
		{
			routeData = Managers.Resource.GetRouteData(routeId);
			enemyData = Managers.Resource.GetEnemyData(enemyId);
			CacheStatData(enemyData.StatsByLevel[0]);

			targets = new Collider[enemyData.MaxDetectCounts];
			waypoints = routeData.Waypoints;

			GetRandomStartPosition();
			targetPosition = waypoints[currentWaypointIndex + 1] + widthOffset * Calculation.GetConsistentBisector(
				-(waypoints[currentWaypointIndex + 1] - waypoints[currentWaypointIndex]),
				GetDirFromPath(currentWaypointIndex + 1)
			);
		}
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
			targetCounts = Physics.OverlapSphereNonAlloc(transform.position, enemyData.SearchRange, targets, enemyData.TargetLayer);
			if (targetCounts > 0)
			{
				float minDistance = float.MaxValue;
				Transform closestTarget = null;

				for (int i = 0; i < targetCounts; i++)
				{
					if (targets[i] == null) break;
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

			if (targetTransform != null && Vector3.SqrMagnitude(transform.position- targetTransform.position) <= enemyData.AttackRange* enemyData.AttackRange)
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

			myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, enemyData.RotationSpeed * Time.deltaTime);
			myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, enemyData.MoveSpeed* Time.deltaTime);

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
			myTransform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, enemyData.RotationSpeed* Time.deltaTime);
			myTransform.position = Vector3.MoveTowards(transform.position, targetTransform.position, enemyData.MoveSpeed * Time.deltaTime);

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
			attackable.Attack(targetTransform);
		}

		private void OnDrawGizmos()
		{
			if (enemyData == null) return;

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, enemyData.SearchRange);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);
		}

	}
}
