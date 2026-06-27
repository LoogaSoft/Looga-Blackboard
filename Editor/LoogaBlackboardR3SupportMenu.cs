using UnityEditor;
using UnityEngine;

namespace LoogaSoft.Blackboard.Editor
{
    internal static class LoogaBlackboardR3SupportMenu
    {
        private const string MenuPath = "LoogaSoft/Blackboard/Enable R3 Support";
        private const string DefineSymbol = "LOOGA_BLACKBOARD_R3_SUPPORT";
        private const string RuntimeAsmdef = "LoogaSoft.Blackboard.Runtime";

        private static readonly string[] RequiredAssemblies =
        {
            "R3"
        };

        [MenuItem(MenuPath, priority = 202)]
        private static void ToggleR3Support()
        {
            if (IsEnabled())
            {
                Disable();
                return;
            }

            if (!LoogaBlackboardOptionalSupportUtility.AllAssembliesAreAvailable(RequiredAssemblies, out string missingAssemblies))
            {
                EditorUtility.DisplayDialog("R3 Not Found", "Install R3 before enabling Looga Blackboard R3 support.\n\nMissing: " + missingAssemblies, "OK");
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
            return LoogaBlackboardOptionalSupportUtility.DefineIsEnabled(DefineSymbol)
                && LoogaBlackboardOptionalSupportUtility.AsmdefReferences(RuntimeAsmdef, "R3");
        }

        private static void Enable()
        {
            LoogaBlackboardOptionalSupportUtility.AddDefineSymbol(DefineSymbol);
            if (!LoogaBlackboardOptionalSupportUtility.SetAsmdefReferences(RuntimeAsmdef, RequiredAssemblies, include: true, out string error))
                EditorUtility.DisplayDialog("Unable To Update Blackboard", error, "OK");

            AssetDatabase.Refresh();
            Debug.Log("Looga Blackboard R3 support enabled.");
        }

        private static void Disable()
        {
            LoogaBlackboardOptionalSupportUtility.RemoveDefineSymbol(DefineSymbol);
            if (!LoogaBlackboardOptionalSupportUtility.SetAsmdefReferences(RuntimeAsmdef, RequiredAssemblies, include: false, out string error))
                EditorUtility.DisplayDialog("Unable To Update Blackboard", error, "OK");

            AssetDatabase.Refresh();
            Debug.Log("Looga Blackboard R3 support disabled.");
        }
    }
}