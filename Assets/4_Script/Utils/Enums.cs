
namespace Defense.Utils
{
	public enum GameInputState
	{
		Main,
		PrePlay,
		Playing,
		Paused,
	}

	public enum SearchEnemyType
	{
		Front,
		Back,
		Near,
		Strong,
		Weak,
	}

	public enum DamageType
	{
		Physics,
		Magic,
		True
	}

	public enum HitResultType
	{
		Miss,
		Normal,
		Critical
	}

	public enum ParticleType
	{
		Build,
		Destroy,
		Hit,
		Lightning,
		Meteor
	}

	public enum ProjectileType
	{
		Arrow,
	}
}
