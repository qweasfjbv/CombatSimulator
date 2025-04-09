
using UnityEngine;

namespace Defense.Utils
{
	public static class Constants
	{
		public static readonly int ENEMY_LEVEL_MAX = 20;
		public static readonly int TOWER_LEVEL_MAX = 5;

		public static readonly string ANIM_PARAM_ATTACK = "IsAttack";
		public static readonly string ANIM_PARAM_DAMAGED = "IsDamaged";
		public static readonly string ANIM_PARAM_DIED = "IsDied";
		public static readonly string ANIM_PARAM_SKILL = "IsSkill";
		public static readonly string ANIM_PARAM_ATTACK_MT = "AttackMultiplier";
		public static readonly string ANIM_PARAM_DAMAGED_MT = "DamagedMultiplier";
		public static readonly string ANIM_PARAM_DEATH_MT = "DeathMultiplier";
		public static readonly string ANIM_PARAM_SKILL_MT = "SkillMultiplier";
		public static readonly string ANIM_PARAM_SPEED = "Speed";

		public static readonly string ANIM_NAME_ATTACK = "Attack";
		public static readonly string ANIM_NAME_DAMAGE = "Damage";
		public static readonly string ANIM_NAME_DEATH = "Death";
		public static readonly string ANIM_NAME_SKILL = "Skill";

		public static readonly int LAYER_SLOT = 1 << 13;

		public static readonly Color COLOR_SLOT_START = new Color(1f, 0, 0, 0.7f);
		public static readonly Color COLOR_SLOT_END = new Color(0, 1f, 0, 0.7f);

	}
}
