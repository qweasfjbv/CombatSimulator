using Defense.Interfaces;
using Defense.Manager;
using Defense.Utils;
using UnityEngine;

namespace Defense.Controller
{
	public abstract class TowerUnitController : MonoBehaviour
		, IAttackable
	{

		public Transform weaponSocket;
		private Transform prevTarget;
		
		/** Components **/
		private IAttackable attackable;
		private Animator unitAnimator;

		/** SO Datas **/
		protected TowerData towerData = null;

		/** State Variables **/
		private Transform targetTransform = null;
		private float currentCooltime = 0f;

		/** Pre-load Variables **/
		private float attackClipLength = 0f;

		private int animIDAttack = 0;
		private int animIDAttackMT = 0;

		private void Awake()
		{
			attackable = (IAttackable)this;;

			unitAnimator = GetComponent<Animator>();

			animIDAttack = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK);
			animIDAttackMT = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK_MT);

			attackClipLength = unitAnimator.GetAnimationClipLength(Constants.ANIM_NAME_ATTACK);
		}

		private Vector3 lookDir = Vector3.zero;
		private void Update()
		{
			UpdateCooltimeTick();
			if (SearchEnemy() && IsAbleToAttack())
				StartAttackAnim();
			if (prevTarget != null)
			{
				lookDir = prevTarget.position - transform.position;
				lookDir.y = 0f;
				transform.rotation = Quaternion.LookRotation(lookDir);
			}
		}

		public void InitTowerUnit(int towerId)
		{
			towerData = Managers.Resource.GetTowerData(towerId);
			targets = new Collider[towerData.MaxDetectCounts];
		}

		/** Animation Events **/
		public void OnAttack()
		{
			if (prevTarget == null) return;

			Attack(prevTarget);
		}

		private Collider[] targets = null;
		private int targetCounts = 0;
		private bool SearchEnemy()
		{
			targetCounts = Physics.OverlapSphereNonAlloc(transform.position, towerData.TowerRange, targets, towerData.TargetLayer);
			targetTransform = FindTargetBySearchType(targets);

			return targetTransform != null;
		}

		private Transform FindTargetBySearchType(Collider[] targets)
		{
			if (targets.Length == 0) return null;

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

			return closestTarget;
		}

		public bool IsAbleToAttack()
		{
			if (currentCooltime <= 0f) return true;
			else return false;
		}
		public void StartAttackAnim()
		{
			unitAnimator.SetTrigger(animIDAttack);
			unitAnimator.SetFloat(animIDAttackMT, attackClipLength / towerData.AttackCooltime);

			currentCooltime = towerData.AttackCooltime;
			prevTarget = targetTransform;
		}

		public abstract void Attack(Transform target);

		public void UpdateCooltimeTick()
		{
			if (currentCooltime >= 0f)
				currentCooltime -= Time.deltaTime;
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, towerData.TowerRange);
		}
	}
}