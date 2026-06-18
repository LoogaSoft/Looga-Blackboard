namespace LoogaSoft.Blackboard
{
    public static class LoogaBlackboardRuntimeRegistry
    {
        public static LoogaBlackboard Active { get; private set; }

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
