namespace LoogaSoft.Blackboard
{
    public static class LoogaBlackboardReaderExtensions
    {
        public static bool TryGetBool(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, out bool value)
        {
            value = false;
            if (!TryGetTypedValue(reader, key, LoogaBlackboardValueType.Bool, out LoogaBlackboardValue rawValue))
                return false;

            value = rawValue.boolValue;
            return true;
        }

        public static bool GetBool(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, bool fallback = false)
        {
            return reader.TryGetBool(key, out bool value) ? value : fallback;
        }

        public static bool TryGetInt(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, out int value)
        {
            value = 0;
            if (!TryGetTypedValue(reader, key, LoogaBlackboardValueType.Int, out LoogaBlackboardValue rawValue))
                return false;

            value = rawValue.intValue;
            return true;
        }

        public static int GetInt(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, int fallback = 0)
        {
            return reader.TryGetInt(key, out int value) ? value : fallback;
        }

        public static bool TryGetFloat(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, out float value)
        {
            value = 0f;
            if (!TryGetTypedValue(reader, key, LoogaBlackboardValueType.Float, out LoogaBlackboardValue rawValue))
                return false;

            value = rawValue.floatValue;
            return true;
        }

        public static float GetFloat(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, float fallback = 0f)
        {
            return reader.TryGetFloat(key, out float value) ? value : fallback;
        }

        public static bool TryGetString(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, out string value)
        {
            value = string.Empty;
            if (!TryGetTypedValue(reader, key, LoogaBlackboardValueType.String, out LoogaBlackboardValue rawValue))
                return false;

            value = rawValue.stringValue;
            return true;
        }

        public static string GetString(this ILoogaBlackboardReader reader, LoogaBlackboardKey key, string fallback = "")
        {
            return reader.TryGetString(key, out string value) ? value : fallback;
        }

        private static bool TryGetTypedValue(
            ILoogaBlackboardReader reader,
            LoogaBlackboardKey key,
            LoogaBlackboardValueType expectedType,
            out LoogaBlackboardValue value)
        {
            if (reader == null || key == null || key.ValueType != expectedType)
            {
                value = default;
                return false;
            }

            if (!reader.TryGetValue(key, out value))
                return false;

            return value.type == expectedType;
        }
    }
}
