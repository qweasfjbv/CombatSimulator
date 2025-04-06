#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using IUtil;
using IUtil.Utils;

[CustomPropertyDrawer(typeof(PopupOptionAttribute))]
public class PopupOptionDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var popupAttribute = attribute as PopupOptionAttribute;
		if (popupAttribute == null) return;

		System.Array valueArray = property.GetArray(popupAttribute.ArrayName);
		if (valueArray == null)
		{
			EditorGUI.PropertyField(position, property, label);
			return;
		}

		// Find matching index
		int selectedIndex = -1;
		object currentValue = GetPropertyValue(property);
		for (int i = 0; i < valueArray.Length; i++)
		{
			if (valueArray.GetValue(i).Equals(currentValue))
			{
				selectedIndex = i;
				break;
			}
		}

		// Set default index
		if(selectedIndex < 0)
		{
			selectedIndex = popupAttribute.DefaultIndex;
		}

		string[] options = new string[valueArray.Length];
		for (int i = 0; i < valueArray.Length; i++)
			options[i] = valueArray.GetValue(i).ToString();

		// Draw Popup
		EditorGUI.BeginChangeCheck();
		selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);
		if (EditorGUI.EndChangeCheck() && selectedIndex >= 0)
		{
			SetPropertyValue(property, valueArray.GetValue(selectedIndex));
		}
	}

	private object GetPropertyValue(SerializedProperty property)
	{
		switch (property.propertyType)
		{
			case SerializedPropertyType.Integer: return property.intValue;
			case SerializedPropertyType.Float: return property.floatValue;
			case SerializedPropertyType.String: return property.stringValue;
			default:
				IUtilDebug.TypeError("PopupOption", property.propertyType.ToString());
				return null;
		}
	}

	private void SetPropertyValue(SerializedProperty property, object value)
	{
		switch (property.propertyType)
		{
			case SerializedPropertyType.Integer:
				property.intValue = (int)value;
				break;
			case SerializedPropertyType.Float:
				property.floatValue = (float)value;
				break;
			case SerializedPropertyType.String:
				property.stringValue = (string)value;
				break;
			default:
				IUtilDebug.TypeError("PopupOption", property.propertyType.ToString());
				break;
		}
	}
}
#endif