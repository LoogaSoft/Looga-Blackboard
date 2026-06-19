using UnityEngine;

namespace LoogaSoft.Blackboard
{
    internal sealed class LoogaBlackboardRuntimeHost : MonoBehaviour
    {
        private LoogaBlackboard _blackboard;

        public void Initialize(LoogaBlackboard blackboard, bool persistAcrossScenes)
        {
            _blackboard = blackboard;

            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            LoogaBlackboardRegistry.ClearActive(_blackboard);
        }
    }
}
