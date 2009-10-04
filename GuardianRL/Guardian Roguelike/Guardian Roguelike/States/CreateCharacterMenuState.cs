using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class CreateCharacterMenuState : StateBase
    {
        private World.Creatures.Dwarf Player;
        private string Name;
        private libtcodWrapper.Console DispCons;
        private int CreationStep;
       
        public override void EnterState()
        {
            Player = new Guardian_Roguelike.World.Creatures.Dwarf();
            DispCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);
            CreationStep = 0;
        }

        public override void MainLoop()
        {
            while (true)
            {

            }
        }

        public override void ExitState()
        {
            Player.FirstName = Name;
        }
    }
}
