using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class CreateCharacterMenuState : StateBase
    {
        private World.Creatures.Guardian Player;
        private string Name;
        private World.Creatures.Deity AttachedDeity;

        private libtcodWrapper.Console DispCons;
       
        public override void EnterState()
        {
            Player = new Guardian_Roguelike.World.Creatures.Guardian();
            DispCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);
        }

        public override void MainLoop()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void ExitState()
        {
            Player.Name = Name;
            Player.AttachedDeity = AttachedDeity;
        }
    }
}
