#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace IUtil.CustomAttribute
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute), true)]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = !Application.isPlaying && (attribute as ReadOnlyAttribute).OnlyInRuntime;
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