using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UnselectableButton), true)]
[CanEditMultipleObjects]
/// <summary>
///   Custom Editor for the Button Component.
///   Extend this class to write a custom editor for a component derived from Button.
/// </summary>
public class UnselectableButtonEditor  : UnselectableEditor
{
    SerializedProperty m_OnClickProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnClickProperty);
        serializedObject.ApplyModifiedProperties();
    }

    // [MenuItem("GameObject/UI/Unselectable Button", false)]
    // private static void CreateUnselectableButton()
    // {
    //     // Canvas
    //     Canvas canvas = FindObjectOfType<Canvas>();
    //     if (canvas == null)
    //     {
    //         GameObject canvasObject = new GameObject("Canvas");
    //         canvas = canvasObject.AddComponent<Canvas>();
    //         canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    //         canvas.gameObject.AddComponent<GraphicRaycaster>();
    //         Undo.RegisterCreatedObjectUndo(canvasObject, "Create " + canvasObject.name);
    //     }
    //
    //     //The Button
    //     GameObject button = new GameObject("Button");
    //     RectTransform rectTransform = button.AddComponent<RectTransform>();
    //     rectTransform.sizeDelta = new Vector2(160, 30);
    //     GameObjectUtility.SetParentAndAlign(button, canvas.gameObject);
    //     button.AddComponent<CanvasRenderer>();
    //     var img = button.AddComponent<Image>();
    //     img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite> ("UI/Skin/UISprite.psd");
    //     img.type = Image.Type.Sliced;
    //
    //     button.AddComponent<UnselectableButton>();
    //     
    //     //Text
    //     GameObject textGameObject = new GameObject("Text");
    //     RectTransform textRectTransform = textGameObject.AddComponent<RectTransform>();
    //     textRectTransform.anchorMin = new Vector2(0, 0);
    //     textRectTransform.anchorMax = new Vector2(1, 1);
    //     textRectTransform.offsetMin = Vector2.zero;
    //     textRectTransform.offsetMax = Vector2.zero;
    //     textGameObject.AddComponent<CanvasRenderer>();
    //     Text text = textGameObject.AddComponent<Text>();
    //     text.text = "Unselectable Button";
    //     text.color = new Color(0.1960784f,0.1960784f,0.1960784f,1);
    //     text.alignment = TextAnchor.MiddleCenter;
    //     GameObjectUtility.SetParentAndAlign(textGameObject, button.gameObject);
    //     
    // }
}
