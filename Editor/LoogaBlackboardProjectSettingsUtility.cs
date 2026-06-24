using LoogaSoft.Blackboard;
using UnityEditor;
using UnityEngine;

namespace LoogaSoft.Blackboard.Editor
{
    internal static class LoogaBlackboardProjectSettingsUtility
    {
        public const string SettingsAssetPath = "Assets/Project/Databases/Blackboard/Resources/LoogaBlackboardProjectSettings.asset";
        public const string ProjectSettingsPath = "Project/LoogaSoft/Blackboard";

        public static LoogaBlackboardProjectSettings FindSettings()
        {
            return AssetDatabase.LoadAssetAtPath<LoogaBlackboardProjectSettings>(SettingsAssetPath);
        }

        public static LoogaBlackboardProjectSettings GetOrCreateSettings()
        {
            LoogaBlackboardProjectSettings settings = FindSettings();
            if (settings != null)
                return settings;

            EnsureSettingsFolder();

            settings = ScriptableObject.CreateInstance<LoogaBlackboardProjectSettings>();
            AssetDatabase.CreateAsset(settings, SettingsAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(SettingsAssetPath);

            return settings;
        }

        public static void AssignProjectBlackboard(LoogaBlackboardDefinition definition)
        {
            if (definition == null)
                return;

            LoogaBlackboardProjectSettings settings = GetOrCreateSettings();
            SerializedObject settingsObject = new(settings);
            SerializedProperty definitionProperty = settingsObject.FindProperty("_blackboardDefinition");
            definitionProperty.objectReferenceValue = definition;
            settingsObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        public static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(ProjectSettingsPath);
        }

        private static void EnsureSettingsFolder()
        {
            EnsureFolder("Assets", "Project");
            EnsureFolder("Assets/Project", "Databases");
            EnsureFolder("Assets/Project/Databases", "Blackboard");
            EnsureFolder("Assets/Project/Databases/Blackboard", "Resources");
        }

        private static void EnsureFolder(string parentFolder, string childFolder)
        {
            string fullPath = $"{parentFolder}/{childFolder}";
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parentFolder, childFolder);
            }
        }
    }
}
