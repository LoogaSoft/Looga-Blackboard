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
    }
}
