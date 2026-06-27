using System;
using System.Collections.Generic;

namespace LoogaSoft.Blackboard
{
    /// <summary>
    /// Describes a runtime blackboard value mutation.
    /// </summary>
    public readonly struct LoogaBlackboardChange
    {
        public LoogaBlackboardChange(
            LoogaBlackboardKey key,
            LoogaBlackboardValue value,
            bool hadPreviousValue,
            LoogaBlackboardValue previousValue)
        {
            Key = key;
            Value = value;
            HadPreviousValue = hadPreviousValue;
            PreviousValue = previousValue;
        }

        public LoogaBlackboardKey Key { get; }
        public LoogaBlackboardValue Value { get; }
        public bool HadPreviousValue { get; }
        public LoogaBlackboardValue PreviousValue { get; }
    }

    public sealed class LoogaBlackboard : ILoogaBlackboardReader, ILoogaBlackboardWriter
    {
        private readonly Dictionary<LoogaBlackboardKey, LoogaBlackboardValue> _values = new();

        public event Action<LoogaBlackboardChange> ValueChanged;
        public event Action<LoogaBlackboardKey> ValueRemoved;
        public event Action Cleared;

        public void SetValue(LoogaBlackboardKey key, LoogaBlackboardValue value)
        {
            if (key == null || key.ValueType != value.type)
                return;

            bool hadPreviousValue = _values.TryGetValue(key, out LoogaBlackboardValue previousValue);
            if (hadPreviousValue && ValuesMatch(previousValue, value))
                return;

            _values[key] = value;
            ValueChanged?.Invoke(new LoogaBlackboardChange(key, value, hadPreviousValue, previousValue));
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
            if (key != null && _values.Remove(key))
            {
                ValueRemoved?.Invoke(key);
            }
        }

        public void Clear()
        {
            if (_values.Count == 0)
                return;

            _values.Clear();
            Cleared?.Invoke();
        }

        private static bool ValuesMatch(LoogaBlackboardValue a, LoogaBlackboardValue b)
        {
            if (a.type != b.type)
                return false;

            return a.type switch
            {
                LoogaBlackboardValueType.Bool => a.boolValue == b.boolValue,
                LoogaBlackboardValueType.Int => a.intValue == b.intValue,
                LoogaBlackboardValueType.Float => a.floatValue.Equals(b.floatValue),
                LoogaBlackboardValueType.String => string.Equals(a.stringValue, b.stringValue, StringComparison.Ordinal),
                _ => false
            };
        }
    }
}