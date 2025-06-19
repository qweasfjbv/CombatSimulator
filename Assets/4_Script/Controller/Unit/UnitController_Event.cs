

namespace Autobattler.Controller
{
	/// <summary>
	/// UnitController의 이벤트 관련 함수들을 담습니다.
	/// </summary>
	public partial class UnitController
	{

		/** Animation Events **/
		public void OnAttack()
		{
			if (attackTarget == null) return;
			Attack(attackTarget);
			currentMP += unitData.MPPerAttack;
		}

		public void OnSkill()
		{
			ExecuteSkill(skillTargets, skillTargetCount);
		}

		public void OnEndAttack()
		{
			isAttacking = false;
		}

		/** Game Cycle Events **/
		public void OnEndStage()
		{
			gameObject.SetActive(true);
			InitCombat();

			isInGame = false;
			isAttacking = false;
			isChasing = false;
		}
		public void OnStartStage()
		{
			isInGame = true;
		}

	}
}
