using UnityEngine;

namespace LoogaSoft.Blackboard
{
    internal sealed class LoogaBlackboardRuntimeHost : MonoBehaviour
    {
        private LoogaBlackboard _blackboard;

        public void Initialize(LoogaBlackboard blackboard)
        {
            _blackboard = blackboard;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            LoogaBlackboardRegistry.ClearActive(_blackboard);
        }
    }
}
