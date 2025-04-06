#if UNITY_EDITOR
using IUtil.Utils;
using UnityEditor;
using UnityEngine;

namespace IUtil.CustomAttribute
{
	[CustomPropertyDrawer(typeof(HideIfAttribute), true)]
	public class HideIfDrawer : PropertyDrawer
	{
		private bool isActive = false;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			isActive = property.GetBoolean("HideIf", (attribute as HideIfAttribute).Condition);
			if(!isActive)
			{
				EditorGUI.PropertyField(position, property, label);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return isActive ?
				0f : EditorGUI.GetPropertyHeight(property, label);
		}
	}
}
#endif