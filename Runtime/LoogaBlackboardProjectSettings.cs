using UnityEngine;

namespace LoogaSoft.Blackboard
{
    [CreateAssetMenu(fileName = AssetName, menuName = "LoogaSoft/Blackboard/Project Settings")]
    public sealed class LoogaBlackboardProjectSettings : ScriptableObject
    {
        public const string AssetName = "LoogaBlackboardProjectSettings";
        public const string ResourcesPath = AssetName;

        [SerializeField] private LoogaBlackboardDefinition _blackboardDefinition;
        [SerializeField] private bool _autoRegisterRuntimeBlackboard = true;
        [SerializeField] private bool _persistAcrossScenes = true;

        public LoogaBlackboardDefinition BlackboardDefinition => _blackboardDefinition;
        public bool AutoRegisterRuntimeBlackboard => _autoRegisterRuntimeBlackboard;
        public bool PersistAcrossScenes => _persistAcrossScenes;
    }
}
