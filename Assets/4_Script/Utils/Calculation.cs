using System.Runtime.CompilerServices;
using UnityEngine;

namespace Autobattler.Utils
{
	public static class Calculation
	{
		/// <summary>
		/// defense, damage, type 을 통해 줄어들 HP를 계산합니다.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CalculateDamage(LevelStat stat , DamageType type, float damage)
		{
			float calculatedDamage = damage * (1 - (stat.DefensePower / (100 + stat.DefensePower)));
			return calculatedDamage < 0 ? 0 : calculatedDamage;
		}

		public static Vector3 GetConsistentBisector(Vector3 dir1, Vector3 dir2)
		{
			Vector3 d1 = new Vector3(dir1.x, 0, dir1.z).normalized;
			Vector3 d2 = new Vector3(dir2.x, 0, dir2.z).normalized;

			if (Vector3.Dot(d1, d2) < -0.9999f)
			{
				return new Vector3(-d1.z, 0, d1.x);
			}

			Vector3 bisector = (d1 + d2).normalized;
			float crossY = d1.x * d2.z - d1.z * d2.x;

			if (crossY < 0)
			{
				bisector = -bisector;
			}

			return bisector;
		}

		public static Vector3 CalculateBezierPoint(Vector3 startPoint, Vector3 endPoint, float height, float t)
		{
			Vector3 mid = (startPoint + endPoint) * 0.5f;
			mid.y += height;
			float u = 1 - t;

			return (u * u * startPoint) + (2 * u * t * mid) + (t * t * endPoint);
		}

	}
}
