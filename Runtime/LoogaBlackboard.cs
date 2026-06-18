using System.Collections.Generic;

namespace LoogaSoft.Blackboard
{
    public sealed class LoogaBlackboard
    {
        private readonly Dictionary<LoogaBlackboardKey, LoogaBlackboardValue> _values = new();

        public void SetValue(LoogaBlackboardKey key, LoogaBlackboardValue value)
        {
            if (key == null || key.ValueType != value.type)
                return;

            _values[key] = value;
        }

        public void SetBool(LoogaBlackboardKey key, bool value)
        {
            SetValue(key, LoogaBlackboardValue.Bool(value));
        }

        public void SetInt(LoogaBlackboardKey key, int value)
        {
            SetValue(key, LoogaBlackboardValue.Int(value));
        }

        public void SetFloat(LoogaBlackboardKey key, float value)
        {
            SetValue(key, LoogaBlackboardValue.Float(value));
        }

        public void SetString(LoogaBlackboardKey key, string value)
        {
            SetValue(key, LoogaBlackboardValue.String(value));
        }

        public bool TryGetValue(LoogaBlackboardKey key, out LoogaBlackboardValue value)
        {
            if (key != null && _values.TryGetValue(key, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        public void RemoveValue(LoogaBlackboardKey key)
        {
            if (key != null)
            {
                _values.Remove(key);
            }
        }

        public void Clear()
        {
            _values.Clear();
        }
    }
}
