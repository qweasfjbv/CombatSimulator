#if UNITY_EDITOR
using IUtil.Utils;
using UnityEditor;
using UnityEngine;

namespace IUtil.CustomAttribute
{
	[CustomPropertyDrawer(typeof(ReadOnlyIfAttribute), true)]
	public class ReadOnlyIfDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = !property.GetBoolean("ReadOnlyIf", (attribute as ReadOnlyIfAttribute).Condition);
			EditorGUI.PropertyField(position, property, label);
			GUI.enabled = true;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label);
		}
	}
}
#endif