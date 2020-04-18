using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomPropertyDrawer(typeof(vgAnimationEvent))]
public class vgAnimationEventEditor : PropertyDrawer
{

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if( property.isExpanded )
		{
			return EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight;
		}

		return EditorGUI.GetPropertyHeight(property);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PropertyField(position, property, label, true);

		if( property.isExpanded )
		{
			Rect newPosition = new Rect(position);
			newPosition.yMin += EditorGUI.GetPropertyHeight(property);
			newPosition.yMax = newPosition.yMin + EditorGUIUtility.singleLineHeight;

			EditorGUI.indentLevel++;

			SerializedProperty parameterType = property.FindPropertyRelative("parameterToPass");

			vgAnimationEvent.ParameterType choosenType = (vgAnimationEvent.ParameterType)parameterType.enumValueIndex;

			SerializedProperty parameterToPass = null;
			switch( choosenType )
			{
				case vgAnimationEvent.ParameterType.floatParameter:
					parameterToPass = property.FindPropertyRelative("floatToPass");
					break;

				case vgAnimationEvent.ParameterType.intParameter:
					parameterToPass = property.FindPropertyRelative("intToPass");
					break;

				case vgAnimationEvent.ParameterType.stringParameter:
					parameterToPass = property.FindPropertyRelative("stringToPass");
					break;

				case vgAnimationEvent.ParameterType.objectParameter:
					parameterToPass = property.FindPropertyRelative("objectReferenceToPass");
					break;

				default:
					break;
			}

			if( parameterToPass != null )
			{
				EditorGUI.PropertyField(newPosition, parameterToPass);
			}

			EditorGUI.indentLevel--;
		}
	}
}
