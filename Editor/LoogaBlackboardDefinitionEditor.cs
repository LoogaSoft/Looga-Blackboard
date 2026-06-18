using System.Collections.Generic;
using LoogaSoft.Blackboard;
using UnityEditor;
using UnityEngine;

namespace LoogaSoft.Blackboard.Editor
{
    [CustomEditor(typeof(LoogaBlackboardDefinition))]
    public sealed class LoogaBlackboardDefinitionEditor : UnityEditor.Editor
    {
        private readonly Dictionary<LoogaBlackboardValueType, int> _selectedIndices = new();

        private SerializedProperty _boolKeys;
        private SerializedProperty _intKeys;
        private SerializedProperty _floatKeys;
        private SerializedProperty _stringKeys;

        private bool _showBoolKeys = true;
        private bool _showIntKeys = true;
        private bool _showFloatKeys = true;
        private bool _showStringKeys = true;

        private void OnEnable()
        {
            _boolKeys = serializedObject.FindProperty("_boolKeys");
            _intKeys = serializedObject.FindProperty("_intKeys");
            _floatKeys = serializedObject.FindProperty("_floatKeys");
            _stringKeys = serializedObject.FindProperty("_stringKeys");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(2f);
            EditorGUILayout.LabelField("Looga Blackboard", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Defines typed blackboard keys. This asset stores key definitions only; runtime values live in a runtime blackboard instance.", MessageType.Info);

            DrawKeySection("Bool Keys", LoogaBlackboardValueType.Bool, _boolKeys, ref _showBoolKeys);
            DrawKeySection("Int Keys", LoogaBlackboardValueType.Int, _intKeys, ref _showIntKeys);
            DrawKeySection("Float Keys", LoogaBlackboardValueType.Float, _floatKeys, ref _showFloatKeys);
            DrawKeySection("String Keys", LoogaBlackboardValueType.String, _stringKeys, ref _showStringKeys);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawKeySection(string title, LoogaBlackboardValueType type,
            SerializedProperty keys, ref bool expanded)
        {
            EditorGUILayout.Space(6f);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                Rect headerRect = EditorGUILayout.GetControlRect();
                expanded = EditorGUI.Foldout(headerRect, expanded, $"{title} ({keys.arraySize})", true, EditorStyles.foldoutHeader);

                if (!expanded)
                    return;

                DrawColumnHeader();

                for (int i = 0; i < keys.arraySize; i++)
                {
                    SerializedProperty element = keys.GetArrayElementAtIndex(i);
                    LoogaBlackboardKey key = element.objectReferenceValue as LoogaBlackboardKey;
                    DrawKeyRow(type, key, i);
                }

                EditorGUILayout.Space(4f);
                using (new EditorGUI.DisabledScope(Application.isPlaying))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button($"Add {type} Key"))
                        {
                            CreateKey(type, keys);
                        }

                        using (new EditorGUI.DisabledScope(!HasSelectedIndex(type, keys)))
                        {
                            if (GUILayout.Button("Remove Selected"))
                            {
                                RemoveSelectedKey(type, keys);
                            }
                        }
                    }
                }

                if (Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("Key creation and deletion are disabled in Play Mode.", MessageType.None);
                }
            }
        }

        private static void DrawColumnHeader()
        {
            Rect rect = EditorGUILayout.GetControlRect();
            float typeWidth = 70f;
            float runtimeWidth = 130f;

            Rect nameRect = new(rect.x + 4f, rect.y, rect.width - typeWidth - runtimeWidth - 12f, rect.height);
            Rect typeRect = new(nameRect.xMax + 4f, rect.y, typeWidth, rect.height);
            Rect runtimeRect = new(typeRect.xMax + 4f, rect.y, runtimeWidth, rect.height);

            EditorGUI.LabelField(nameRect, "Key", EditorStyles.miniBoldLabel);
            EditorGUI.LabelField(typeRect, "Type", EditorStyles.miniBoldLabel);
            EditorGUI.LabelField(runtimeRect, "Runtime Value", EditorStyles.miniBoldLabel);
        }

        private void DrawKeyRow(LoogaBlackboardValueType sectionType, LoogaBlackboardKey key, int index)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight + 4f);
            bool selected = GetSelectedIndex(sectionType) == index;

            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.helpBox.Draw(rect, GUIContent.none, false, false, false, false);

                if (selected)
                {
                    EditorGUI.DrawRect(new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f),
                        new Color(0.24f, 0.44f, 0.68f, 0.35f));
                }
            }

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                _selectedIndices[sectionType] = index;
                Repaint();
                Event.current.Use();
            }

            Rect contentRect = new(rect.x + 5f, rect.y + 2f, rect.width - 10f, EditorGUIUtility.singleLineHeight);
            float typeWidth = 70f;
            float runtimeWidth = 130f;

            Rect nameRect = new(contentRect.x, contentRect.y, contentRect.width - typeWidth - runtimeWidth - 8f, contentRect.height);
            Rect typeRect = new(nameRect.xMax + 4f, contentRect.y, typeWidth, contentRect.height);
            Rect runtimeRect = new(typeRect.xMax + 4f, contentRect.y, runtimeWidth, contentRect.height);

            string keyName = key != null ? key.DisplayName : "Missing Key";
            EditorGUI.LabelField(nameRect, keyName);
            EditorGUI.LabelField(typeRect, key != null ? key.ValueType.ToString() : sectionType.ToString());

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.TextField(runtimeRect, GetRuntimeValueText(key));
            }
        }

        private void CreateKey(LoogaBlackboardValueType type, SerializedProperty keys)
        {
            string path = EditorUtility.SaveFilePanelInProject(
                $"Create {type} Blackboard Key",
                $"New {type} Key",
                "asset",
                "Choose where to save the new blackboard key.");

            if (string.IsNullOrWhiteSpace(path))
                return;

            LoogaBlackboardKey key = CreateInstance<LoogaBlackboardKey>();
            SerializedObject keyObject = new(key);
            keyObject.FindProperty("_valueType").enumValueIndex = (int)type;
            keyObject.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(key, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);

            AddKeyReference(keys, key);
            _selectedIndices[type] = keys.arraySize - 1;
            EditorGUIUtility.PingObject(key);
        }

        private void AddKeyReference(SerializedProperty keys, LoogaBlackboardKey key)
        {
            for (int i = 0; i < keys.arraySize; i++)
            {
                if (keys.GetArrayElementAtIndex(i).objectReferenceValue == key)
                    return;
            }

            keys.InsertArrayElementAtIndex(keys.arraySize);
            keys.GetArrayElementAtIndex(keys.arraySize - 1).objectReferenceValue = key;
        }

        private void RemoveSelectedKey(LoogaBlackboardValueType type, SerializedProperty keys)
        {
            int index = GetSelectedIndex(type);
            if (index < 0 || index >= keys.arraySize)
                return;

            LoogaBlackboardKey key = keys.GetArrayElementAtIndex(index).objectReferenceValue as LoogaBlackboardKey;
            string keyName = key != null ? key.DisplayName : "missing key";
            string message = $"Remove '{keyName}' from this blackboard and delete its asset?";

            if (!EditorUtility.DisplayDialog("Delete Blackboard Key", message, "Delete", "Cancel"))
                return;

            string path = key != null ? AssetDatabase.GetAssetPath(key) : string.Empty;
            keys.DeleteArrayElementAtIndex(index);

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }

            _selectedIndices[type] = Mathf.Clamp(index - 1, -1, keys.arraySize - 1);
        }

        private bool HasSelectedIndex(LoogaBlackboardValueType type, SerializedProperty keys)
        {
            int index = GetSelectedIndex(type);
            return index >= 0 && index < keys.arraySize;
        }

        private int GetSelectedIndex(LoogaBlackboardValueType type)
        {
            return _selectedIndices.TryGetValue(type, out int index) ? index : -1;
        }

        private static string GetRuntimeValueText(LoogaBlackboardKey key)
        {
            if (!Application.isPlaying)
                return "Edit Mode";

            LoogaBlackboard blackboard = LoogaBlackboardRuntimeRegistry.Active;
            if (key == null || blackboard == null || !blackboard.TryGetValue(key, out LoogaBlackboardValue value))
                return "Unset";

            return value.type switch
            {
                LoogaBlackboardValueType.Bool => value.boolValue.ToString(),
                LoogaBlackboardValueType.Int => value.intValue.ToString(),
                LoogaBlackboardValueType.Float => value.floatValue.ToString("0.###"),
                LoogaBlackboardValueType.String => value.stringValue,
                _ => "Unset"
            };
        }
    }
}
