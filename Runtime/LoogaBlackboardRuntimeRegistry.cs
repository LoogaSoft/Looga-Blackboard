using System;

namespace LoogaSoft.Blackboard
{
    [Obsolete("Use LoogaBlackboardRegistry instead.")]
    public static class LoogaBlackboardRuntimeRegistry
    {
        public static LoogaBlackboard Active => LoogaBlackboardRegistry.Active;
        public static ILoogaBlackboardReader ActiveReader => LoogaBlackboardRegistry.ActiveReader;
        public static ILoogaBlackboardWriter ActiveWriter => LoogaBlackboardRegistry.ActiveWriter;

        public static void SetActive(LoogaBlackboard blackboard)
        {
            LoogaBlackboardRegistry.SetActive(blackboard);
        }

        public static void ClearActive(LoogaBlackboard blackboard)
        {
            LoogaBlackboardRegistry.ClearActive(blackboard);
        }

        public static void SetValue(LoogaBlackboardKey key, LoogaBlackboardValue value)
        {
            LoogaBlackboardRegistry.SetValue(key, value);
        }

        public static void SetBool(LoogaBlackboardKey key, bool value)
        {
            LoogaBlackboardRegistry.SetBool(key, value);
        }

        public static void SetInt(LoogaBlackboardKey key, int value)
        {
            LoogaBlackboardRegistry.SetInt(key, value);
        }

        public static void SetFloat(LoogaBlackboardKey key, float value)
        {
            LoogaBlackboardRegistry.SetFloat(key, value);
        }

        public static void SetString(LoogaBlackboardKey key, string value)
        {
            LoogaBlackboardRegistry.SetString(key, value);
        }

        public static bool TryGetValue(LoogaBlackboardKey key, out LoogaBlackboardValue value)
        {
            return LoogaBlackboardRegistry.TryGetValue(key, out value);
        }

        public static bool TryGetBool(LoogaBlackboardKey key, out bool value)
        {
            return LoogaBlackboardRegistry.TryGetBool(key, out value);
        }

        public static bool GetBool(LoogaBlackboardKey key)
        {
            return LoogaBlackboardRegistry.GetBool(key);
        }

        public static bool TryGetInt(LoogaBlackboardKey key, out int value)
        {
            return LoogaBlackboardRegistry.TryGetInt(key, out value);
        }

        public static int GetInt(LoogaBlackboardKey key)
        {
            return LoogaBlackboardRegistry.GetInt(key);
        }

        public static bool TryGetFloat(LoogaBlackboardKey key, out float value)
        {
            return LoogaBlackboardRegistry.TryGetFloat(key, out value);
        }

        public static float GetFloat(LoogaBlackboardKey key)
        {
            return LoogaBlackboardRegistry.GetFloat(key);
        }

        public static bool TryGetString(LoogaBlackboardKey key, out string value)
        {
            return LoogaBlackboardRegistry.TryGetString(key, out value);
        }

        public static string GetString(LoogaBlackboardKey key)
        {
            return LoogaBlackboardRegistry.GetString(key);
        }

        public static void RemoveValue(LoogaBlackboardKey key)
        {
            LoogaBlackboardRegistry.RemoveValue(key);
        }
    }
}
