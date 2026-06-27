using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace LoogaSoft.Blackboard.Editor
{
    internal static class LoogaBlackboardR3SupportMenu
    {
        private const string MenuPath = "LoogaSoft/Blackboard/Enable R3 Support";
        private const string GeneratedFolder = "Assets/LoogaSoft/Generated/Blackboard R3";
        private const string GeneratedAsmdefPath = GeneratedFolder + "/LoogaSoft.Blackboard.R3.asmdef";
        private const string GeneratedSourcePath = GeneratedFolder + "/LoogaBlackboardR3Extensions.cs";

        private const string GeneratedAsmdef = @"{
    ""name"": ""LoogaSoft.Blackboard.R3"",
    ""rootNamespace"": ""LoogaSoft.Blackboard"",
    ""references"": [
        ""LoogaSoft.Blackboard.Runtime"",
        ""R3""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";

        private const string GeneratedSource = @"using System;
using System.Threading;
using R3;

namespace LoogaSoft.Blackboard
{
    public static class LoogaBlackboardR3Extensions
    {
        public static Observable<LoogaBlackboard> ActiveChangedAsObservable(CancellationToken cancellationToken = default)
        {
            return Observable.FromEvent<Action<LoogaBlackboard>, LoogaBlackboard>(
                handler => new Action<LoogaBlackboard>(handler),
                handler => LoogaBlackboardRegistry.ActiveChanged += handler,
                handler => LoogaBlackboardRegistry.ActiveChanged -= handler,
                cancellationToken);
        }

        public static Observable<LoogaBlackboardChange> ValueChangedAsObservable(
            this LoogaBlackboard blackboard,
            CancellationToken cancellationToken = default)
        {
            if (blackboard == null)
                throw new ArgumentNullException(nameof(blackboard));

            return Observable.FromEvent<Action<LoogaBlackboardChange>, LoogaBlackboardChange>(
                handler => new Action<LoogaBlackboardChange>(handler),
                handler => blackboard.ValueChanged += handler,
                handler => blackboard.ValueChanged -= handler,
                cancellationToken);
        }

        public static Observable<LoogaBlackboardKey> ValueRemovedAsObservable(
            this LoogaBlackboard blackboard,
            CancellationToken cancellationToken = default)
        {
            if (blackboard == null)
                throw new ArgumentNullException(nameof(blackboard));

            return Observable.FromEvent<Action<LoogaBlackboardKey>, LoogaBlackboardKey>(
                handler => new Action<LoogaBlackboardKey>(handler),
                handler => blackboard.ValueRemoved += handler,
                handler => blackboard.ValueRemoved -= handler,
                cancellationToken);
        }
    }
}";

        [MenuItem(MenuPath, priority = 202)]
        private static void ToggleR3Support()
        {
            if (IsEnabled())
            {
                Disable();
                return;
            }

            if (!AssemblyIsAvailable("R3"))
            {
                EditorUtility.DisplayDialog("R3 Not Found", "Install R3 before enabling Looga Blackboard R3 support.", "OK");
                return;
            }

            Enable();
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateToggle()
        {
            UnityEditor.Menu.SetChecked(MenuPath, IsEnabled());
            return true;
        }

        private static bool IsEnabled()
        {
            return File.Exists(GeneratedAsmdefPath) && File.Exists(GeneratedSourcePath);
        }

        private static bool AssemblyIsAvailable(string assemblyName)
        {
            if (CompilationPipeline.GetAssemblies().Any(assembly => assembly.name == assemblyName))
                return true;

            if (AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name == assemblyName))
                return true;

            return AssetDatabase.FindAssets($"{assemblyName} t:AssemblyDefinitionAsset").Length > 0;
        }

        private static void Enable()
        {
            Directory.CreateDirectory(GeneratedFolder);
            File.WriteAllText(GeneratedAsmdefPath, GeneratedAsmdef);
            File.WriteAllText(GeneratedSourcePath, GeneratedSource);
            AssetDatabase.Refresh();
            Debug.Log("Looga Blackboard R3 support enabled.");
        }

        private static void Disable()
        {
            DeleteAssetAndMeta(GeneratedSourcePath);
            DeleteAssetAndMeta(GeneratedAsmdefPath);

            if (Directory.Exists(GeneratedFolder) && Directory.GetFiles(GeneratedFolder).Length == 0)
            {
                Directory.Delete(GeneratedFolder);
                DeleteAssetAndMeta(GeneratedFolder + ".meta");
            }

            AssetDatabase.Refresh();
            Debug.Log("Looga Blackboard R3 support disabled.");
        }

        private static void DeleteAssetAndMeta(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            string metaPath = path.EndsWith(".meta") ? path : path + ".meta";
            if (File.Exists(metaPath))
                File.Delete(metaPath);
        }
    }
}