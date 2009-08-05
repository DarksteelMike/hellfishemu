using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{    
    abstract class StateBase
    {
        protected libtcodWrapper.RootConsole Root = libtcodWrapper.RootConsole.GetInstance();
        public abstract void EnterState();
        public abstract void MainLoop();
        public abstract void ExitState();
    }
}
