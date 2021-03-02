using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(OptionsSideList), true)]
[CanEditMultipleObjects]
public class OptionsSideListEditor : SelectableEditor
{
	SerializedProperty m_CaptionText;
	SerializedProperty m_CaptionImage;
	SerializedProperty m_Value;
	SerializedProperty m_Options;
	SerializedProperty m_AlphaFadeSpeed;

	SerializedProperty forwardButton;
	SerializedProperty backwardButton;
	SerializedProperty forwardDirection;
	SerializedProperty backwardDirection;
	SerializedProperty cyclic;
	SerializedProperty m_OnValueChangedDynamic;

	protected override void OnEnable()
	{
		base.OnEnable();
		m_CaptionText = serializedObject.FindProperty("m_CaptionText");
		m_CaptionImage = serializedObject.FindProperty("m_CaptionImage");
		m_Value = serializedObject.FindProperty("m_Value");
		m_Options = serializedObject.FindProperty("m_Options");
		m_AlphaFadeSpeed = serializedObject.FindProperty("m_AlphaFadeSpeed");

		forwardButton = serializedObject.FindProperty("forwardButton");
		backwardButton = serializedObject.FindProperty("backwardButton");
		forwardDirection = serializedObject.FindProperty("forwardDirection");
		backwardDirection = serializedObject.FindProperty("backwardDirection");
		cyclic = serializedObject.FindProperty("cyclic");
		m_OnValueChangedDynamic = serializedObject.FindProperty("m_OnValueChangedDynamic");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.Space();

		serializedObject.Update();
		EditorGUILayout.PropertyField(m_CaptionText);
		EditorGUILayout.PropertyField(m_CaptionImage);
		EditorGUILayout.PropertyField(m_Value);
		EditorGUILayout.PropertyField(m_AlphaFadeSpeed);
		EditorGUILayout.PropertyField(m_Options);
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(forwardButton);
		EditorGUILayout.PropertyField(backwardButton);
		EditorGUILayout.PropertyField(forwardDirection);
		EditorGUILayout.PropertyField(backwardDirection);
		EditorGUILayout.PropertyField(cyclic);
		EditorGUILayout.PropertyField(m_OnValueChangedDynamic);
		
		serializedObject.ApplyModifiedProperties();
	}
}