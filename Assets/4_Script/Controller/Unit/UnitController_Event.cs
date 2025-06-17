
namespace Defense.Controller
{
	/// <summary>
	/// UnitController의 이벤트 관련 함수들을 담습니다.
	/// </summary>
	public partial class UnitController
	{

		/** Animation Events **/
		public void OnAttack()
		{
			Attack(targetTransform);
			currentMP += unitData.MPPerAttack;
		}
		public void OnSkill()
		{
			ExecuteSkill(targetTransform);
		}

		/** Game Cycle Events **/
		public void OnEndStage()
		{
			gameObject.SetActive(true);
			InitCombat();

			isAttacking = false;
			isChasing = false;
		}
		public void OnStartStage()
		{

		}

	}
}
