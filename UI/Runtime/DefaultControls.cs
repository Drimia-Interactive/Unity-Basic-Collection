using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class DefaultControls
{
	static IFactoryControls m_CurrentFactory = DefaultRuntimeFactory.Default;

	public static IFactoryControls factory
	{
		get { return m_CurrentFactory; }
#if UNITY_EDITOR
		set { m_CurrentFactory = value; }
#endif
	}

	/// <summary>
	/// Factory interface to create a GameObject in this class.
	/// It is necessary to use this interface in the whole class so MenuOption and Editor can work using ObjectFactory and default Presets.
	/// </summary>
	/// <remarks>
	/// The only available method is CreateGameObject.
	/// It needs to be called with every Components the created Object will need because of a bug with Undo and RectTransform.
	/// Adding a UI component on the created GameObject may crash if done after Undo.SetTransformParent,
	/// So it's better to prevent such behavior in this class by asking for full creation with all the components.
	/// </remarks>
	public interface IFactoryControls
	{
		GameObject CreateGameObject(string name, params Type[] components);
	}

	private class DefaultRuntimeFactory : IFactoryControls
	{
		public static IFactoryControls Default = new DefaultRuntimeFactory();

		public GameObject CreateGameObject(string name, params Type[] components)
		{
			return new GameObject(name, components);
		}
	}

	/// <summary>
	/// Object used to pass resources to use for the default controls.
	/// </summary>
	public struct Resources
	{
		/// <summary>
		/// The primary sprite to be used for graphical UI elements, used by the button, toggle, and dropdown controls, among others.
		/// </summary>
		public Sprite standard;

		/// <summary>
		/// Sprite used for background elements.
		/// </summary>
		public Sprite background;

		/// <summary>
		/// Sprite used as background for input fields.
		/// </summary>
		public Sprite inputField;

		/// <summary>
		/// Sprite used for knobs that can be dragged, such as on a slider.
		/// </summary>
		public Sprite knob;

		/// <summary>
		/// Sprite used for representation of an "on" state when present, such as a checkmark.
		/// </summary>
		public Sprite checkmark;

		/// <summary>
		/// Sprite used to indicate that a button will open a dropdown when clicked.
		/// </summary>
		public Sprite dropdown;

		/// <summary>
		/// Sprite used for masking purposes, for example to be used for the viewport of a scroll view.
		/// </summary>
		public Sprite mask;
	}

	private const float kWidth = 160f;
	private const float kThickHeight = 30f;
	private const float kThinHeight = 20f;
	private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
	private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
	private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
	private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
	private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
	private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

	// Helper methods at top

	private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components)
	{
		GameObject child = factory.CreateGameObject(name, components);
		RectTransform rectTransform = child.GetComponent<RectTransform>();
		rectTransform.sizeDelta = size;
		return child;
	}

	private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components)
	{
		GameObject go = factory.CreateGameObject(name, components);
		SetParentAndAlign(go, parent);
		return go;
	}

	private static void SetDefaultTextValues(Text lbl)
	{
		// Set text values we want across UI elements in default controls.
		// Don't set values which are the same as the default values for the Text component,
		// since there's no point in that, and it's good to keep them as consistent as possible.
		lbl.color = s_TextColor;

		// // Reset() is not called when playing. We still want the default font to be assigned
		// lbl.AssignDefaultFont();
	}

	private static void SetDefaultColorTransitionValues(Selectable slider)
	{
		ColorBlock colors = slider.colors;
		colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
		colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
		colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
	}
	
	private static void SetDefaultColorTransitionValues(Unselectable slider)
	{
		ColorBlock colors = slider.colors;
		colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
		colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
		colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
	}

	private static void SetParentAndAlign(GameObject child, GameObject parent)
	{
		if (parent == null)
			return;

#if UNITY_EDITOR
		Undo.SetTransformParent(child.transform, parent.transform, "");
#else
            child.transform.SetParent(parent.transform, false);
#endif
		SetLayerRecursively(child, parent.layer);
	}

	private static void SetLayerRecursively(GameObject go, int layer)
	{
		go.layer = layer;
		Transform t = go.transform;
		for (int i = 0; i < t.childCount; i++)
			SetLayerRecursively(t.GetChild(i).gameObject, layer);
	}
	
	
	/// <summary>
	/// Create the basic UI unselectable button.
	/// </summary>
	/// <remarks>
	/// Hierarchy:
	/// (root)
	///     UnselectableButton
	///         -Text
	/// </remarks>
	/// <param name="resources">The resources to use for creation.</param>
	/// <returns>The root GameObject of the created element.</returns>
	public static GameObject CreateUnselectableButton(Resources resources)
	{
		GameObject buttonRoot = CreateUIElementRoot("UnselectableButton", s_ThickElementSize, typeof(Image), typeof(UnselectableButton));

		GameObject childText = CreateUIObject("Text", buttonRoot, typeof(Text));

		Image image = buttonRoot.GetComponent<Image>();
		image.sprite = resources.standard;
		image.type = Image.Type.Sliced;
		image.color = s_DefaultSelectableColor;

		UnselectableButton bt = buttonRoot.GetComponent<UnselectableButton>();
		SetDefaultColorTransitionValues(bt);

		Text text = childText.GetComponent<Text>();
		text.text = "Unselectable Button";
		text.alignment = TextAnchor.MiddleCenter;
		SetDefaultTextValues(text);

		RectTransform textRectTransform = childText.GetComponent<RectTransform>();
		textRectTransform.anchorMin = Vector2.zero;
		textRectTransform.anchorMax = Vector2.one;
		textRectTransform.sizeDelta = Vector2.zero;

		return buttonRoot;
	}
	
	/// <summary>
	/// Create the basic UI OptionsSideList.
	/// </summary>
	/// <remarks>
	/// Hierarchy:
	/// (root)
	///     OptionsSideList
	///         - Label
	///         - ForwardButton
	///             - Image
	///         - BackwardButton
	///             - Image
	/// </remarks>
	/// <param name="resources">The resources to use for creation.</param>
	/// <returns>The root GameObject of the created element.</returns>
	public static GameObject CreateOptionsSideList(Resources resources)
	{
		GameObject root = CreateUIElementRoot("OptionsSideList", s_ThickElementSize, typeof(Image), typeof(OptionsSideList));

		GameObject label = CreateUIObject("Label", root, typeof(Text));
		GameObject forwardButton = CreateUIObject("ForwardButton", root, typeof(RectTransform), typeof(UnselectableButton));
		GameObject forwardImage = CreateUIObject("Image", forwardButton, typeof(Image));
		GameObject backwardButton = CreateUIObject("BackwardButton", root, typeof(RectTransform), typeof(UnselectableButton));
		GameObject backwardImage = CreateUIObject("Image", backwardButton, typeof(Image));
		

		OptionsSideList optionsSideList = root.GetComponent<OptionsSideList>();
		
		var forwardImageImage = forwardImage.GetComponent<Image>();
		var backwardImageImage = backwardImage.GetComponent<Image>();
		
		// Setup forwardButton
		var forwardButtonButton = forwardButton.GetComponent<UnselectableButton>();
		forwardButtonButton.image = forwardImageImage;

		var backwardButtonButton = backwardButton.GetComponent<UnselectableButton>();
		backwardButtonButton.image = backwardImageImage;

		optionsSideList.forwardButton = forwardButtonButton;
		optionsSideList.backwardButton = backwardButtonButton;
		
#if UNITY_EDITOR
		UnityEditor.Events.UnityEventTools.AddPersistentListener(forwardButtonButton.onClick, optionsSideList.Forward);
		UnityEditor.Events.UnityEventTools.AddPersistentListener(backwardButtonButton.onClick, optionsSideList.Backward);
#endif
		
		// Setup dropdown UI components.

		Text labelText = label.GetComponent<Text>();
		SetDefaultTextValues(labelText);
		labelText.alignment = TextAnchor.MiddleCenter;
		
		Image forwardButtonImage = forwardButton.GetComponentInChildren<Image>();
		forwardButtonImage.sprite = resources.dropdown;
		
		Image backwardButtonImage = backwardButton.GetComponentInChildren<Image>();
		backwardButtonImage.sprite = resources.dropdown;

		Image backgroundImage = root.GetComponent<Image>();
		backgroundImage.sprite = resources.standard;
		backgroundImage.color = s_DefaultSelectableColor;
		backgroundImage.type = Image.Type.Sliced;

		Dropdown dropdown = root.GetComponent<Dropdown>();
		dropdown.targetGraphic = backgroundImage;
		SetDefaultColorTransitionValues(dropdown);
		dropdown.captionText = labelText;

		// Setting default Item list.
		dropdown.options.Add(new Dropdown.OptionData {text = "Option A"});
		dropdown.options.Add(new Dropdown.OptionData {text = "Option B"});
		dropdown.options.Add(new Dropdown.OptionData {text = "Option C"});
		dropdown.RefreshShownValue();

		// Set up RectTransforms.

		RectTransform labelRT = label.GetComponent<RectTransform>();
		labelRT.anchorMin           = Vector2.zero;
		labelRT.anchorMax           = Vector2.one;
		labelRT.offsetMin           = new Vector2(20, 0);
		labelRT.offsetMax           = new Vector2(-20, 0);
		
		RectTransform forwardButtonRT = forwardButton.GetComponent<RectTransform>();
		forwardButtonRT.anchorMin           = new Vector2(1, 0.5f);
		forwardButtonRT.anchorMax           = new Vector2(1, 0.5f);
		forwardButtonRT.sizeDelta           = new Vector2(20, 20);
		forwardButtonRT.anchoredPosition    = new Vector2(0, 0);
		forwardButtonRT.pivot               = new Vector2(1, 0.5f);

		RectTransform forwardImageRT = forwardImage.GetComponent<RectTransform>();
		forwardImageRT.anchorMin           = new Vector2(0, 0);
		forwardImageRT.anchorMax           = new Vector2(1, 1);
		forwardImageRT.anchoredPosition    = new Vector2(0, 0);
		forwardImageRT.pivot               = new Vector2(0.5f, 0.5f);
		forwardImageRT.sizeDelta           = new Vector2(0, 0);
		forwardImageRT.rotation            = Quaternion.Euler(0,0,90);
		
		RectTransform backwardButtonRT = backwardButton.GetComponent<RectTransform>();
		backwardButtonRT.anchorMin           = new Vector2(0, 0.5f);
		backwardButtonRT.anchorMax           = new Vector2(0, 0.5f);
		backwardButtonRT.sizeDelta           = new Vector2(20, 20);
		backwardButtonRT.anchoredPosition    = new Vector2(0, 0);
		backwardButtonRT.pivot               = new Vector2(0, 0.5f);

		RectTransform backwardImageRT = backwardImage.GetComponent<RectTransform>();
		backwardImageRT.anchorMin           = new Vector2(0, 0);
		backwardImageRT.anchorMax           = new Vector2(1, 1);
		backwardImageRT.anchoredPosition    = new Vector2(0, 0);
		backwardImageRT.pivot               = new Vector2(0.5f, 0.5f);
		backwardImageRT.sizeDelta           = new Vector2(0, 0);
		backwardImageRT.rotation            = Quaternion.Euler(0,0,-90);

		


		return root;
	}
}