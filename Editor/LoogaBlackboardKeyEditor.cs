using LoogaSoft.Blackboard;
using UnityEditor;
using UnityEngine;

namespace LoogaSoft.Blackboard.Editor
{
    [CustomEditor(typeof(LoogaBlackboardKey))]
    public sealed class LoogaBlackboardKeyEditor : UnityEditor.Editor
    {
        private SerializedProperty _script;
        private SerializedProperty _useCustomDisplayName;
        private SerializedProperty _displayName;
        private SerializedProperty _valueType;
        private SerializedProperty _description;

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _useCustomDisplayName = serializedObject.FindProperty("_useCustomDisplayName");
            _displayName = serializedObject.FindProperty("_displayName");
            _valueType = serializedObject.FindProperty("_valueType");
            _description = serializedObject.FindProperty("_description");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_script);
            }

            EditorGUILayout.PropertyField(_useCustomDisplayName);
            using (new EditorGUI.DisabledScope(!_useCustomDisplayName.boolValue))
            {
                EditorGUILayout.PropertyField(_displayName);
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_valueType);
            }

            EditorGUILayout.LabelField(_description.displayName);
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.MinHeight(58f));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
