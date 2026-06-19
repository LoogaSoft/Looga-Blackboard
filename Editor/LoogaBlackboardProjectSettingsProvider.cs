using LoogaSoft.Blackboard;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LoogaSoft.Blackboard.Editor
{
    internal sealed class LoogaBlackboardProjectSettingsProvider : SettingsProvider
    {
        private SerializedObject _settingsObject;
        private SerializedProperty _script;
        private SerializedProperty _blackboardDefinition;
        private SerializedProperty _autoRegisterRuntimeBlackboard;
        private SerializedProperty _persistAcrossScenes;

        private LoogaBlackboardProjectSettingsProvider()
            : base(LoogaBlackboardProjectSettingsUtility.ProjectSettingsPath, SettingsScope.Project)
        {
            label = "Blackboard";
            keywords = new[] { "Looga", "Blackboard", "Runtime", "State" };
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new LoogaBlackboardProjectSettingsProvider();
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            BindSettings();
        }

        public override void OnGUI(string searchContext)
        {
            if (_settingsObject == null || _settingsObject.targetObject == null)
            {
                BindSettings();
            }

            _settingsObject.Update();

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("Looga Blackboard", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Assign the project blackboard definition here. At runtime, the package can automatically create and register the active runtime blackboard.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_script);
            }

            EditorGUILayout.PropertyField(_blackboardDefinition, new GUIContent("Project Blackboard"));
            EditorGUILayout.PropertyField(_autoRegisterRuntimeBlackboard);

            using (new EditorGUI.DisabledScope(!_autoRegisterRuntimeBlackboard.boolValue))
            {
                EditorGUILayout.PropertyField(_persistAcrossScenes);
            }

            _settingsObject.ApplyModifiedProperties();
        }

        private void BindSettings()
        {
            LoogaBlackboardProjectSettings settings = LoogaBlackboardProjectSettingsUtility.GetOrCreateSettings();
            _settingsObject = new SerializedObject(settings);
            _script = _settingsObject.FindProperty("m_Script");
            _blackboardDefinition = _settingsObject.FindProperty("_blackboardDefinition");
            _autoRegisterRuntimeBlackboard = _settingsObject.FindProperty("_autoRegisterRuntimeBlackboard");
            _persistAcrossScenes = _settingsObject.FindProperty("_persistAcrossScenes");
        }
    }
}
