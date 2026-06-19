namespace LoogaSoft.Blackboard
{
    /// <summary>
    /// Write access to runtime blackboard values.
    /// </summary>
    public interface ILoogaBlackboardWriter
    {
        void SetValue(LoogaBlackboardKey key, LoogaBlackboardValue value);
        void SetBool(LoogaBlackboardKey key, bool value);
        void SetInt(LoogaBlackboardKey key, int value);
        void SetFloat(LoogaBlackboardKey key, float value);
        void SetString(LoogaBlackboardKey key, string value);
        void RemoveValue(LoogaBlackboardKey key);
    }
}
