using System;
using System.Collections.Generic;
using System.Reflection;
using LoogaSoft.Blackboard;
using UnityEditor;
using UnityEngine;

namespace LoogaSoft.Blackboard.Editor
{
    [CustomEditor(typeof(LoogaBlackboardDefinition))]
    public sealed class LoogaBlackboardDefinitionEditor : UnityEditor.Editor
    {
        private const float RowHorizontalPadding = 5f;
        private const float RowHeight = 25f;
        private const float FooterButtonHeight = 22f;
        private const float FoldoutContentTrailingPadding = 2f;
        private const float ColumnGap = 4f;
        private const float RuntimeColumnWidth = 130f;

        private static readonly Color RowBackgroundColor = new(0.195f, 0.195f, 0.195f, 1f);
        private static readonly Color HoveredRowColor = new(1f, 1f, 1f, 0.06f);
        private static readonly Color SelectedRowColor = new(0.24f, 0.44f, 0.68f, 0.35f);

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
            expanded = LoogaInspectorFoldoutBridge.DrawSmall(
                new GUIContent($"{title} ({keys.arraySize})"),
                expanded,
                () =>
            {
                DrawColumnHeader();

                for (int i = 0; i < keys.arraySize; i++)
                {
                    SerializedProperty element = keys.GetArrayElementAtIndex(i);
                    LoogaBlackboardKey key = element.objectReferenceValue as LoogaBlackboardKey;
                    DrawKeyRow(type, key, i);
                }

                EditorGUILayout.Space(RowHorizontalPadding);
                using (new EditorGUI.DisabledScope(Application.isPlaying))
                {
                    Rect buttonRowRect = EditorGUILayout.GetControlRect(false, FooterButtonHeight);
                    buttonRowRect = new Rect(
                        buttonRowRect.x + RowHorizontalPadding,
                        buttonRowRect.y,
                        Mathf.Max(0f, buttonRowRect.width - RowHorizontalPadding * 2f),
                        buttonRowRect.height);

                    float buttonWidth = Mathf.Max(0f, (buttonRowRect.width - ColumnGap) * 0.5f);
                    Rect addButtonRect = new(buttonRowRect.x, buttonRowRect.y, buttonWidth, buttonRowRect.height);
                    Rect removeButtonRect = new(addButtonRect.xMax + ColumnGap, buttonRowRect.y, buttonWidth, buttonRowRect.height);

                    if (GUI.Button(addButtonRect, $"Add {type} Key"))
                    {
                        RequestCreateKey(type, keys.propertyPath);
                    }

                    using (new EditorGUI.DisabledScope(!HasSelectedIndex(type, keys)))
                    {
                        if (GUI.Button(removeButtonRect, "Remove Selected"))
                        {
                            RemoveSelectedKey(type, keys);
                        }
                    }
                }
                EditorGUILayout.Space(Mathf.Max(0f, RowHorizontalPadding - FoldoutContentTrailingPadding));

                if (Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("Key creation and deletion are disabled in Play Mode.", MessageType.None);
                }
            },
                keys);
        }

        private static void DrawColumnHeader()
        {
            Rect rect = EditorGUILayout.GetControlRect();
            GetKeyRowRects(rect, out Rect nameRect, out Rect runtimeRect);

            EditorGUI.LabelField(nameRect, "Key", EditorStyles.miniBoldLabel);
            EditorGUI.LabelField(runtimeRect, "Runtime Value", EditorStyles.miniBoldLabel);
        }

        private void DrawKeyRow(LoogaBlackboardValueType sectionType, LoogaBlackboardKey key, int index)
        {
            Rect rect = PixelAlign(EditorGUILayout.GetControlRect(false, RowHeight));
            bool selected = GetSelectedIndex(sectionType) == index;
            bool hovered = rect.Contains(Event.current.mousePosition);

            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(rect, RowBackgroundColor);

                if (hovered)
                {
                    EditorGUI.DrawRect(rect, HoveredRowColor);
                }

                if (selected)
                {
                    EditorGUI.DrawRect(rect, SelectedRowColor);
                }
            }

            if (Event.current.type == EventType.MouseDown && hovered)
            {
                _selectedIndices[sectionType] = index;
                Repaint();
                Event.current.Use();
            }

            GetKeyRowRects(rect, out Rect nameRect, out Rect runtimeRect);

            string keyName = key != null ? key.DisplayName : "Missing Key";
            EditorGUI.LabelField(nameRect, keyName);

            EditorGUI.LabelField(runtimeRect, GetRuntimeValueText(key));
        }

        private static void GetKeyRowRects(Rect rowRect, out Rect nameRect, out Rect runtimeRect)
        {
            Rect contentRect = CenterVertically(rowRect, EditorGUIUtility.singleLineHeight);
            contentRect.x = rowRect.x + RowHorizontalPadding;
            contentRect.width = Mathf.Max(0f, rowRect.width - RowHorizontalPadding * 2f);

            runtimeRect = new(
                contentRect.xMax - RuntimeColumnWidth,
                contentRect.y,
                RuntimeColumnWidth,
                contentRect.height);

            nameRect = new(
                contentRect.x,
                contentRect.y,
                Mathf.Max(0f, runtimeRect.x - ColumnGap - contentRect.x),
                contentRect.height);
        }

        private static float SnapToPixel(float value)
        {
            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            return Mathf.Floor(value * pixelsPerPoint + 0.5f) / pixelsPerPoint;
        }

        private static Rect CenterVertically(Rect container, float height)
        {
            float y = SnapToPixel(container.y + (container.height - height) * 0.5f);
            return new Rect(container.x, y, container.width, height);
        }

        private static Rect PixelAlign(Rect rect)
        {
            return new Rect(
                SnapToPixel(rect.x),
                SnapToPixel(rect.y),
                SnapToPixel(rect.width),
                SnapToPixel(rect.height));
        }

        private void RequestCreateKey(LoogaBlackboardValueType type, string keysPropertyPath)
        {
            UnityEngine.Object blackboardDefinition = target;
            EditorApplication.delayCall += () => CreateKey(type, blackboardDefinition, keysPropertyPath);
        }

        private void CreateKey(LoogaBlackboardValueType type, UnityEngine.Object blackboardDefinition, string keysPropertyPath)
        {
            if (blackboardDefinition == null || string.IsNullOrWhiteSpace(keysPropertyPath))
                return;

            string path = EditorUtility.SaveFilePanelInProject(
                $"Create {type} Blackboard Key",
                $"New {type} Key",
                "asset",
                "Choose where to save the new blackboard key.",
                GetDefaultKeyDirectory(blackboardDefinition));

            if (string.IsNullOrWhiteSpace(path))
                return;

            LoogaBlackboardKey key = CreateInstance<LoogaBlackboardKey>();
            SerializedObject keyObject = new(key);
            keyObject.FindProperty("_valueType").enumValueIndex = (int)type;
            keyObject.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(key, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);

            SerializedObject blackboardObject = new(blackboardDefinition);
            SerializedProperty keys = blackboardObject.FindProperty(keysPropertyPath);
            if (keys != null)
            {
                AddKeyReference(keys, key);
                blackboardObject.ApplyModifiedProperties();
                _selectedIndices[type] = keys.arraySize - 1;
            }

            EditorGUIUtility.PingObject(key);
            Repaint();
        }

        private static string GetDefaultKeyDirectory(UnityEngine.Object blackboardDefinition)
        {
            string assetPath = AssetDatabase.GetAssetPath(blackboardDefinition);
            if (string.IsNullOrWhiteSpace(assetPath))
                return "Assets";

            string directory = System.IO.Path.GetDirectoryName(assetPath);
            if (string.IsNullOrWhiteSpace(directory))
                return "Assets";

            return directory.Replace('\\', '/');
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

        private static class LoogaInspectorFoldoutBridge
        {
            private const string FoldoutTypeName = "LoogaSoft.Inspector.Editor.LoogaEditorFoldouts, LoogaSoft.Inspector.Editor";

            private static Type _foldoutType;
            private static MethodInfo _smallFoldoutMethod;

            public static bool DrawSmall(GUIContent label, bool expanded, Action content, SerializedProperty property)
            {
                if (TryGetSmallFoldoutMethod(out MethodInfo method))
                {
                    object[] args = { label, expanded, content, property };
                    return (bool)method.Invoke(null, args);
                }

                return DrawFallback(label, expanded, content);
            }

            private static bool DrawFallback(GUIContent label, bool expanded, Action content)
            {
                EditorGUILayout.Space(1f);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    expanded = EditorGUILayout.Foldout(expanded, label, true);
                    if (expanded)
                    {
                        EditorGUILayout.Space(2f);
                        content?.Invoke();
                        EditorGUILayout.Space(2f);
                    }
                }
                EditorGUILayout.Space(1f);

                return expanded;
            }

            private static bool TryGetSmallFoldoutMethod(out MethodInfo method)
            {
                method = _smallFoldoutMethod;
                if (method != null)
                    return true;

                _foldoutType ??= Type.GetType(FoldoutTypeName);
                if (_foldoutType == null)
                    return false;

                _smallFoldoutMethod = _foldoutType.GetMethod(
                    "LoogaFoldoutSmall",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(GUIContent), typeof(bool), typeof(Action), typeof(SerializedProperty) },
                    null);

                method = _smallFoldoutMethod;
                return method != null;
            }
        }
    }
}
