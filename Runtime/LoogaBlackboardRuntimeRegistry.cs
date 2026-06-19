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
    }
}
