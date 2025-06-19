using UnityEngine;

namespace Autobattler.Interfaces
{
	public interface IAttackable
	{
		bool IsAbleToAttack();
		/// <summary>
		/// Animation 실행
		/// Attack Animation 에 Event 를 통해 실제 HP 조작 함수 호출
		/// </summary>
		void StartAttackAnim();
		/// <summary>
		/// Attack은 Animation Event 로만 호출
		/// </summary>
		void Attack(Transform target);
		void UpdateCooltimeTick();
	}
}
