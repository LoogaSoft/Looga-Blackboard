using UnityEditor;
using UnityEngine;

namespace LoogaSoft.Blackboard.Editor
{
    internal static class LoogaBlackboardR3SupportMenu
    {
        private const string MenuPath = "LoogaSoft/Blackboard/Enable R3 Support";
        private const string DefineSymbol = "LOOGA_BLACKBOARD_R3_SUPPORT";

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
            return LoogaBlackboardOptionalSupportUtility.DefineIsEnabled(DefineSymbol);
        }

        private static void Enable()
        {
            LoogaBlackboardOptionalSupportUtility.AddDefineSymbol(DefineSymbol);
            AssetDatabase.Refresh();
            Debug.Log("Looga Blackboard R3 support enabled.");
        }

        private static void Disable()
        {
            LoogaBlackboardOptionalSupportUtility.RemoveDefineSymbol(DefineSymbol);
            AssetDatabase.Refresh();
            Debug.Log("Looga Blackboard R3 support disabled.");
        }
    }
}