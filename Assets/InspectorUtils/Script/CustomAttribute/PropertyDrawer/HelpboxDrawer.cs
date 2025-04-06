#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace IUtil.CustomAttribute
{
	[CustomPropertyDrawer(typeof(HelpBoxAttribute), true)]
	public class HelpboxDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			float width = EditorGUIUtility.currentViewWidth;
			float height = EditorStyles.helpBox.CalcHeight(new GUIContent((attribute as HelpBoxAttribute).Content), width);

			Rect propertyRect = position;
			propertyRect.height -= height;

			EditorGUI.PropertyField(propertyRect, property, label);

			GUILayout.Space(-15f);

			EditorGUILayout.HelpBox((attribute as HelpBoxAttribute).Content, (UnityEditor.MessageType)(attribute as HelpBoxAttribute).MessageType);

			GUILayout.Space(5f);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float width = EditorGUIUtility.currentViewWidth;
			float height = EditorStyles.helpBox.CalcHeight(new GUIContent((attribute as HelpBoxAttribute).Content), width);
			return EditorGUI.GetPropertyHeight(property, label) + height;
		}
	}
}
#endif