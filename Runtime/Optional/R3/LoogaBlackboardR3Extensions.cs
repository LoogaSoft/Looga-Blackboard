#if LOOGA_BLACKBOARD_R3_SUPPORT
using System;
using System.Threading;
using R3;

namespace LoogaSoft.Blackboard
{
    public static class LoogaBlackboardR3Extensions
    {
        public static Observable<LoogaBlackboard> ActiveChangedAsObservable(CancellationToken cancellationToken = default)
        {
            return Observable.FromEvent<Action<LoogaBlackboard>, LoogaBlackboard>(
                handler => new Action<LoogaBlackboard>(handler),
                handler => LoogaBlackboardRegistry.ActiveChanged += handler,
                handler => LoogaBlackboardRegistry.ActiveChanged -= handler,
                cancellationToken);
        }

        public static Observable<LoogaBlackboardChange> ValueChangedAsObservable(
            this LoogaBlackboard blackboard,
            CancellationToken cancellationToken = default)
        {
            if (blackboard == null)
                throw new ArgumentNullException(nameof(blackboard));

            return Observable.FromEvent<Action<LoogaBlackboardChange>, LoogaBlackboardChange>(
                handler => new Action<LoogaBlackboardChange>(handler),
                handler => blackboard.ValueChanged += handler,
                handler => blackboard.ValueChanged -= handler,
                cancellationToken);
        }

        public static Observable<LoogaBlackboardKey> ValueRemovedAsObservable(
            this LoogaBlackboard blackboard,
            CancellationToken cancellationToken = default)
        {
            if (blackboard == null)
                throw new ArgumentNullException(nameof(blackboard));

            return Observable.FromEvent<Action<LoogaBlackboardKey>, LoogaBlackboardKey>(
                handler => new Action<LoogaBlackboardKey>(handler),
                handler => blackboard.ValueRemoved += handler,
                handler => blackboard.ValueRemoved -= handler,
                cancellationToken);
        }
    }
}
#endif