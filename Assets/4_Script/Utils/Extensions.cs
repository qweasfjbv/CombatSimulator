using UnityEngine;

namespace Defense.Utils
{
	public static class Extensions
	{
		public static float AbsSum(this Vector3 v)
		{
			return Mathf.Abs(v.x + v.y + v.z);
		}

		public static float GetAnimationClipLength(this Animator animator, string clipName)
		{
			foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
			{
				if (clip.name == clipName)
				{
					return clip.length;
				}
			}
			return 0f;
		}
	}
}
