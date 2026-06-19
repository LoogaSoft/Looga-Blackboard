using UnityEngine;

namespace LoogaSoft.Blackboard
{
    public static class LoogaBlackboardBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetActiveBlackboard()
        {
            LoogaBlackboardRegistry.SetActive(null);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterProjectBlackboard()
        {
            if (LoogaBlackboardRegistry.Active != null)
                return;

            LoogaBlackboardProjectSettings settings =
                Resources.Load<LoogaBlackboardProjectSettings>(LoogaBlackboardProjectSettings.ResourcesPath);

            if (settings == null || !settings.AutoRegisterRuntimeBlackboard)
                return;

            LoogaBlackboard blackboard = new();
            LoogaBlackboardRegistry.SetActive(blackboard);

            GameObject hostObject = new("[Looga Blackboard]");
            LoogaBlackboardRuntimeHost host = hostObject.AddComponent<LoogaBlackboardRuntimeHost>();
            host.Initialize(blackboard);
        }
    }
}
