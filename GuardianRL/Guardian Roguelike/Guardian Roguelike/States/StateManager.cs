using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class StateManager
    {
        private static StateBase CurrentState;
        private static StateBase QueuedState;
        private static bool SkipNext;

        public static Dictionary<string, StateBase> PersistentStates = new Dictionary<string, StateBase>();

        public static void ChangeState(StateBase To)
        {
            if (CurrentState != null)
            {
                CurrentState.ExitState();
            }

            CurrentState = To;
            CurrentState.EnterState();
            CurrentState.MainLoop();
            if (!SkipNext)
            {
                ChangeState(QueuedState);
            }
        }

        public static void QueueState(StateBase To)
        {
            QueuedState = To;
        }

        public static void Quit()
        {
            SkipNext = true;
        }
    }
}
