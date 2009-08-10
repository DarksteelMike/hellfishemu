using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class MessageLogMenuState : StateBase
    {
        private libtcodWrapper.Console MsgCons;
        private Utilities.MessageLog MsgLog;
        private Utilities.MessageLogScrollPossibilities Scrollability;
        private int ScrollValue;

        public override void EnterState()
        {
            MsgLog = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];
            MsgCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);
            ScrollValue = 0;
            Scrollability = Guardian_Roguelike.Utilities.MessageLogScrollPossibilities.None;
        }

        public override void MainLoop()
        {
            while (true)
            {
                Render();

                if(ProcessInput(libtcodWrapper.Keyboard.WaitForKeyPress(true)))
                {
                    break;
                }
            }
        }

        private void Render()
        {
            Root.Clear();
            Scrollability = MsgLog.RenderFullToConsole(MsgCons, ScrollValue);

            MsgCons.Blit(0, 0, 90, 30, Root, 0, 5);
            Root.PrintLine("Press <Enter> or <Escape> to return.", 1, 31, libtcodWrapper.LineAlignment.Left);

            Root.Flush();
        }

        public bool ProcessInput(libtcodWrapper.KeyPress KP)
        {
            switch (KP.KeyCode)
            {
                case(libtcodWrapper.KeyCode.TCODK_UP):
                    if (Scrollability == Guardian_Roguelike.Utilities.MessageLogScrollPossibilities.Both || Scrollability == Guardian_Roguelike.Utilities.MessageLogScrollPossibilities.Up)
                    {
                        ScrollValue--;
                    }
                    break;
                case(libtcodWrapper.KeyCode.TCODK_DOWN):
                    if (Scrollability == Guardian_Roguelike.Utilities.MessageLogScrollPossibilities.Both || Scrollability == Guardian_Roguelike.Utilities.MessageLogScrollPossibilities.Down)
                    {
                        ScrollValue++;
                    }
                    break;
                case(libtcodWrapper.KeyCode.TCODK_ESCAPE):
                    StateManager.QueueState(StateManager.PersistentStates["GameState"]);
                    return true;
                    break;
                case(libtcodWrapper.KeyCode.TCODK_ENTER):
                    StateManager.QueueState(StateManager.PersistentStates["GameState"]);
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
