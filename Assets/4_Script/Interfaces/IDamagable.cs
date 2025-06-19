using Autobattler.Utils;

namespace Autobattler.Interfaces
{
	public interface IDamagable
	{
		public bool IsAbleToTargeted(float duration);

		/// <summary>
		/// 데미지를 예약함.
		/// 이를 통해 상대는 죽을 타겟이라는걸 미리 알고 다른 타겟을 찾음
		/// </summary>
		public void ReserveDamage(DamageType type, float damage, float duration);

		/// <summary>
		/// 데미지를 입력으로 받아서 defense 로직 처리 후 hp 감소
		/// </summary>
		public void GetImmediateDamage(DamageType type, float damage);

		public void CheckIfDied();
	}
}
