namespace LoogaSoft.Blackboard
{
    public static class LoogaBlackboardRegistry
    {
        public static LoogaBlackboard Active { get; private set; }
        public static ILoogaBlackboardReader ActiveReader => Active;
        public static ILoogaBlackboardWriter ActiveWriter => Active;

        public static void SetActive(LoogaBlackboard blackboard)
        {
            Active = blackboard;
        }

        public static void ClearActive(LoogaBlackboard blackboard)
        {
            if (ReferenceEquals(Active, blackboard))
            {
                Active = null;
            }
        }

        public static void SetValue(LoogaBlackboardKey key, LoogaBlackboardValue value)
        {
            ActiveWriter?.SetValue(key, value);
        }

        public static void SetBool(LoogaBlackboardKey key, bool value)
        {
            ActiveWriter?.SetBool(key, value);
        }

        public static void SetInt(LoogaBlackboardKey key, int value)
        {
            ActiveWriter?.SetInt(key, value);
        }

        public static void SetFloat(LoogaBlackboardKey key, float value)
        {
            ActiveWriter?.SetFloat(key, value);
        }

        public static void SetString(LoogaBlackboardKey key, string value)
        {
            ActiveWriter?.SetString(key, value);
        }

        public static bool TryGetValue(LoogaBlackboardKey key, out LoogaBlackboardValue value)
        {
            if (ActiveReader != null)
            {
                return ActiveReader.TryGetValue(key, out value);
            }

            value = default;
            return false;
        }

        public static bool TryGetBool(LoogaBlackboardKey key, out bool value)
        {
            return ActiveReader.TryGetBool(key, out value);
        }

        public static bool GetBool(LoogaBlackboardKey key)
        {
            return ActiveReader.GetBool(key);
        }

        public static bool TryGetInt(LoogaBlackboardKey key, out int value)
        {
            return ActiveReader.TryGetInt(key, out value);
        }

        public static int GetInt(LoogaBlackboardKey key)
        {
            return ActiveReader.GetInt(key);
        }

        public static bool TryGetFloat(LoogaBlackboardKey key, out float value)
        {
            return ActiveReader.TryGetFloat(key, out value);
        }

        public static float GetFloat(LoogaBlackboardKey key)
        {
            return ActiveReader.GetFloat(key);
        }

        public static bool TryGetString(LoogaBlackboardKey key, out string value)
        {
            return ActiveReader.TryGetString(key, out value);
        }

        public static string GetString(LoogaBlackboardKey key)
        {
            return ActiveReader.GetString(key);
        }

        public static void RemoveValue(LoogaBlackboardKey key)
        {
            ActiveWriter?.RemoveValue(key);
        }
    }
}
