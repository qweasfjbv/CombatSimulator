#if UNITY_EDITOR
using IUtil.Utils;
using UnityEditor;
using UnityEngine;

namespace IUtil.CustomAttribute
{
	[CustomPropertyDrawer(typeof(ShowIfAttribute), true)]
	public class ShowIfDrawer : PropertyDrawer
	{
		private bool isActive = false;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			isActive = property.GetBoolean("ShowIf", (attribute as ShowIfAttribute).Condition);
			if (isActive)
			{
				EditorGUI.PropertyField(position, property, label);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return isActive ?
				EditorGUI.GetPropertyHeight(property, label) : 0f;
		}
	}
}
#endif