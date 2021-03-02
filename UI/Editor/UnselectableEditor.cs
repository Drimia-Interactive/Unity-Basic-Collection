using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEditor.Animations;

[CustomEditor(typeof(Unselectable), true)]
    /// <summary>
    /// Custom Editor for the UnSelectable Component.
    /// Extend this class to write a custom editor for a component derived from Selectable.
    /// </summary>
    public class UnselectableEditor : Editor
    {
        SerializedProperty m_Script;
        SerializedProperty m_InteractableProperty;
        SerializedProperty m_TargetGraphicProperty;
        SerializedProperty m_TransitionProperty;
        SerializedProperty m_ColorBlockProperty;
        SerializedProperty m_SpriteStateProperty;
        SerializedProperty m_AnimTriggerProperty;


        AnimBool m_ShowColorTint       = new AnimBool();
        AnimBool m_ShowSpriteTrasition = new AnimBool();
        AnimBool m_ShowAnimTransition  = new AnimBool();

        private static List<UnselectableEditor> s_Editors = new List<UnselectableEditor>();

        // Whenever adding new SerializedProperties to the Unselectable and UnselectableEditor
        // Also update this guy in OnEnable. This makes the inherited classes from Unselectable not require a CustomEditor.
        private string[] m_PropertyPathToExcludeForChildClasses;

        protected virtual void OnEnable()
        {
            m_Script                = serializedObject.FindProperty("m_Script");
            m_InteractableProperty  = serializedObject.FindProperty("m_Interactable");
            m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
            m_TransitionProperty    = serializedObject.FindProperty("m_Transition");
            m_ColorBlockProperty    = serializedObject.FindProperty("m_Colors");
            m_SpriteStateProperty   = serializedObject.FindProperty("m_SpriteState");
            m_AnimTriggerProperty   = serializedObject.FindProperty("m_AnimationTriggers");

            m_PropertyPathToExcludeForChildClasses = new[]
            {
                m_Script.propertyPath,
                m_TransitionProperty.propertyPath,
                m_ColorBlockProperty.propertyPath,
                m_SpriteStateProperty.propertyPath,
                m_AnimTriggerProperty.propertyPath,
                m_InteractableProperty.propertyPath,
                m_TargetGraphicProperty.propertyPath,
            };

            var trans = GetTransition(m_TransitionProperty);
            m_ShowColorTint.value       = (trans == Unselectable.Transition.ColorTint);
            m_ShowSpriteTrasition.value = (trans == Unselectable.Transition.SpriteSwap);
            m_ShowAnimTransition.value  = (trans == Unselectable.Transition.Animation);

            m_ShowColorTint.valueChanged.AddListener(Repaint);
            m_ShowSpriteTrasition.valueChanged.AddListener(Repaint);

            s_Editors.Add(this);
        }

        protected virtual void OnDisable()
        {
            m_ShowColorTint.valueChanged.RemoveListener(Repaint);
            m_ShowSpriteTrasition.valueChanged.RemoveListener(Repaint);

            s_Editors.Remove(this);
        }

        static Unselectable.Transition GetTransition(SerializedProperty transition)
        {
            return (Unselectable.Transition)transition.enumValueIndex;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_InteractableProperty);

            var trans = GetTransition(m_TransitionProperty);

            var graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
            if (graphic == null)
                graphic = (target as Unselectable).GetComponent<Graphic>();

            var animator = (target as Unselectable).GetComponent<Animator>();
            m_ShowColorTint.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Unselectable.Transition.ColorTint);
            m_ShowSpriteTrasition.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Unselectable.Transition.SpriteSwap);
            m_ShowAnimTransition.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Unselectable.Transition.Animation);

            EditorGUILayout.PropertyField(m_TransitionProperty);

            ++EditorGUI.indentLevel;
            {
                if (trans == Unselectable.Transition.ColorTint || trans == Unselectable.Transition.SpriteSwap)
                {
                    EditorGUILayout.PropertyField(m_TargetGraphicProperty);
                }

                switch (trans)
                {
                    case Unselectable.Transition.ColorTint:
                        if (graphic == null)
                            EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
                        break;

                    case Unselectable.Transition.SpriteSwap:
                        if (graphic as Image == null)
                            EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
                        break;
                }

                if (EditorGUILayout.BeginFadeGroup(m_ShowColorTint.faded))
                {
                    EditorGUILayout.PropertyField(m_ColorBlockProperty);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowSpriteTrasition.faded))
                {
                    EditorGUILayout.PropertyField(m_SpriteStateProperty);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowAnimTransition.faded))
                {
                    EditorGUILayout.PropertyField(m_AnimTriggerProperty);

                    if (animator == null || animator.runtimeAnimatorController == null)
                    {
                        Rect buttonRect = EditorGUILayout.GetControlRect();
                        buttonRect.xMin += EditorGUIUtility.labelWidth;
                        if (GUI.Button(buttonRect, "Auto Generate Animation", EditorStyles.miniButton))
                        {
                            var controller = GenerateSelectableAnimatorContoller((target as Unselectable).animationTriggers, target as Unselectable);
                            if (controller != null)
                            {
                                if (animator == null)
                                    animator = (target as Unselectable).gameObject.AddComponent<Animator>();
                            
                                // Animations.AnimatorController.SetAnimatorController(animator, controller);
                            }
                        }
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;

            EditorGUILayout.Space();

            
            EditorGUI.BeginChangeCheck();
            Rect toggleRect = EditorGUILayout.GetControlRect();
            toggleRect.xMin += EditorGUIUtility.labelWidth;
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

            // We do this here to avoid requiring the user to also write a Editor for their Unselectable-derived classes.
            // This way if we are on a derived class we dont draw anything else, otherwise draw the remaining properties.
            ChildClassPropertiesGUI();

            serializedObject.ApplyModifiedProperties();
        }

        // Draw the extra SerializedProperties of the child class.
        // We need to make sure that m_PropertyPathToExcludeForChildClasses has all the Unselectable properties and in the correct order.
        // TODO: find a nicer way of doing this. (creating a InheritedEditor class that automagically does this)
        private void ChildClassPropertiesGUI()
        {
            if (IsDerivedUnselectableEditor())
                return;

            DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
        }

        private bool IsDerivedUnselectableEditor()
        {
            return GetType() != typeof(UnselectableEditor);
        }

        private static UnityEditor.Animations.AnimatorController GenerateSelectableAnimatorContoller(AnimationTriggers animationTriggers, Unselectable target)
        {
            if (target == null)
                return null;
        
            // Where should we create the controller?
            var path = GetSaveControllerPath(target);
            if (string.IsNullOrEmpty(path))
                return null;
        
            // figure out clip names
            var normalName = string.IsNullOrEmpty(animationTriggers.normalTrigger) ? "Normal" : animationTriggers.normalTrigger;
            var highlightedName = string.IsNullOrEmpty(animationTriggers.highlightedTrigger) ? "Highlighted" : animationTriggers.highlightedTrigger;
            var pressedName = string.IsNullOrEmpty(animationTriggers.pressedTrigger) ? "Pressed" : animationTriggers.pressedTrigger;
            var selectedName = string.IsNullOrEmpty(animationTriggers.selectedTrigger) ? "Selected" : animationTriggers.selectedTrigger;
            var disabledName = string.IsNullOrEmpty(animationTriggers.disabledTrigger) ? "Disabled" : animationTriggers.disabledTrigger;
        
            // Create controller and hook up transitions.
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);
            GenerateTriggerableTransition(normalName, controller);
            GenerateTriggerableTransition(highlightedName, controller);
            GenerateTriggerableTransition(pressedName, controller);
            GenerateTriggerableTransition(selectedName, controller);
            GenerateTriggerableTransition(disabledName, controller);
        
            AssetDatabase.ImportAsset(path);
        
            return controller;
        }

        private static string GetSaveControllerPath(Unselectable target)
        {
            var defaultName = target.gameObject.name;
            var message = string.Format("Create a new animator for the game object '{0}':", defaultName);
            return EditorUtility.SaveFilePanelInProject("New Animation Contoller", defaultName, "controller", message);
        }

        private static void SetUpCurves(AnimationClip highlightedClip, AnimationClip pressedClip, string animationPath)
        {
            string[] channels = { "m_LocalScale.x", "m_LocalScale.y", "m_LocalScale.z" };

            var highlightedKeys = new[] { new Keyframe(0f, 1f), new Keyframe(0.5f, 1.1f), new Keyframe(1f, 1f) };
            var highlightedCurve = new AnimationCurve(highlightedKeys);
            foreach (var channel in channels)
                AnimationUtility.SetEditorCurve(highlightedClip, EditorCurveBinding.FloatCurve(animationPath, typeof(Transform), channel), highlightedCurve);

            var pressedKeys = new[] { new Keyframe(0f, 1.15f) };
            var pressedCurve = new AnimationCurve(pressedKeys);
            foreach (var channel in channels)
                AnimationUtility.SetEditorCurve(pressedClip, EditorCurveBinding.FloatCurve(animationPath, typeof(Transform), channel), pressedCurve);
        }

        private static string BuildAnimationPath(Unselectable target)
        {
            // if no target don't hook up any curves.
            var highlight = target.targetGraphic;
            if (highlight == null)
                return string.Empty;

            var startGo = highlight.gameObject;
            var toFindGo = target.gameObject;

            var pathComponents = new Stack<string>();
            while (toFindGo != startGo)
            {
                pathComponents.Push(startGo.name);

                // didn't exist in hierarchy!
                if (startGo.transform.parent == null)
                    return string.Empty;

                startGo = startGo.transform.parent.gameObject;
            }

            // calculate path
            var animPath = new StringBuilder();
            if (pathComponents.Count > 0)
                animPath.Append(pathComponents.Pop());

            while (pathComponents.Count > 0)
                animPath.Append("/").Append(pathComponents.Pop());

            return animPath.ToString();
        }

        private static AnimationClip GenerateTriggerableTransition(string name, UnityEditor.Animations.AnimatorController controller)
        {
            // Create the clip
            var clip = UnityEditor.Animations.AnimatorController.AllocateAnimatorClip(name);
            AssetDatabase.AddObjectToAsset(clip, controller);
        
            // Create a state in the animatior controller for this clip
            var state = controller.AddMotion(clip);
        
            // Add a transition property
            controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
        
            // Add an any state transition
            var stateMachine = controller.layers[0].stateMachine;
            var transition = stateMachine.AddAnyStateTransition(state);
            transition.AddCondition(AnimatorConditionMode.If, 0, name);
            return clip;
        }
    }
