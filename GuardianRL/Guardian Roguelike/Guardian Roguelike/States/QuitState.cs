using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class QuitState : StateBase
    {
        public override void EnterState()
        {
            //Cleanup goes here
        }

        public override void MainLoop()
        {
            StateManager.Quit();
        }

        public override void ExitState()
        {
            
        }
    }
}
