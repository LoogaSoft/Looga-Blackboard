namespace LoogaSoft.Blackboard
{
    /// <summary>
    /// Read-only access to runtime blackboard values.
    /// </summary>
    public interface ILoogaBlackboardReader
    {
        bool TryGetValue(LoogaBlackboardKey key, out LoogaBlackboardValue value);
    }
}
