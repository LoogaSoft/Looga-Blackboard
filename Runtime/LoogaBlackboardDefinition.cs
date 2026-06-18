using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoogaSoft.Blackboard
{
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "LoogaSoft/Blackboard/Definition")]
    public sealed class LoogaBlackboardDefinition : ScriptableObject
    {
        [SerializeField] private LoogaBlackboardKey[] _boolKeys = Array.Empty<LoogaBlackboardKey>();
        [SerializeField] private LoogaBlackboardKey[] _intKeys = Array.Empty<LoogaBlackboardKey>();
        [SerializeField] private LoogaBlackboardKey[] _floatKeys = Array.Empty<LoogaBlackboardKey>();
        [SerializeField] private LoogaBlackboardKey[] _stringKeys = Array.Empty<LoogaBlackboardKey>();

        public LoogaBlackboardKey[] BoolKeys => _boolKeys;
        public LoogaBlackboardKey[] IntKeys => _intKeys;
        public LoogaBlackboardKey[] FloatKeys => _floatKeys;
        public LoogaBlackboardKey[] StringKeys => _stringKeys;

        public IEnumerable<LoogaBlackboardKey> Keys
        {
            get
            {
                foreach (LoogaBlackboardKey key in _boolKeys)
                    yield return key;
                foreach (LoogaBlackboardKey key in _intKeys)
                    yield return key;
                foreach (LoogaBlackboardKey key in _floatKeys)
                    yield return key;
                foreach (LoogaBlackboardKey key in _stringKeys)
                    yield return key;
            }
        }
    }
}
