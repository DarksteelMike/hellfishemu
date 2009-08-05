using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class WorldMapMenuState : StateBase
    {
        private libtcodWrapper.Console WMCons;
        private libtcodWrapper.Console MSGCons;

        private World.WorldMap WMap;

        private States.StateBase FollowingState;

        public override void EnterState()
        {
            WMCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);
            MSGCons = libtcodWrapper.RootConsole.GetNewConsole(90, 5);

            Utilities.MessageLog MSGLog = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];
            WMap = (World.WorldMap)Utilities.InterStateResources.Instance.Resources["Game_WorldMap"];

            WMap.RenderToConsole(WMCons);
            MSGLog.RenderRecentToConsole(MSGCons);
        }

        public override void MainLoop()
        {
            while (true)
            {
                Render();

                if (ProcessInput(libtcodWrapper.Keyboard.WaitForKeyPress(true)))
                {
                    break;
                }
            }

            States.StateManager.QueueState(FollowingState);
        }

        private void Render()
        {
            Root.Clear();

            MSGCons.Blit(0, 0, 90, 5, Root, 0, 0);
            WMCons.Blit(0, 0, 90, 30, Root, 0, 5);

            Root.Flush();

        }

        private bool ProcessInput(libtcodWrapper.KeyPress KP)
        {
            switch (KP.KeyCode)
            {
                case(libtcodWrapper.KeyCode.TCODK_ESCAPE):
                    FollowingState = States.StateManager.PersistentStates["GameState"];
                    return true;
                    break;
            }

            return false;
        }

        public override void ExitState()
        {
            
        }
    }
}
