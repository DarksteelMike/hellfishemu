using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class GameState : StateBase
    {
        private World.Map CurrentLevel;
        private libtcodWrapper.Console MapCons;

        private World.Creatures.Guardian Player;

        private Utilities.MessageLog MsgLog;
        private libtcodWrapper.Console MsgCons;

        public override void EnterState()
        {
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_MessageLog"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_MessageLog", new Utilities.MessageLog());
            }
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_CurrentLevel"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_CurrentLevel", new World.Map());
            }
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_PlayerCreature"))
            {
                World.Creatures.Guardian tPlayer = new Guardian_Roguelike.World.Creatures.Guardian();
                tPlayer.HP = 100;
                tPlayer.Position.X = tPlayer.Position.Y = 1;
                tPlayer.Name = "Spartacus";
                tPlayer.DrawColor = libtcodWrapper.ColorPresets.ForestGreen;
                Utilities.InterStateResources.Instance.Resources.Add("Game_PlayerCreature", tPlayer);
            }
            
            MapCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);

            MsgCons = libtcodWrapper.RootConsole.GetNewConsole(90, 5);
            MsgLog = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];

            Player = (World.Creatures.Guardian)Utilities.InterStateResources.Instance.Resources["Game_PlayerCreature"];

            CurrentLevel = (World.Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            CurrentLevel.Creatures.Add(Player);

            Player.Position = CurrentLevel.GetFirstWalkable();

            MsgLog.AddMsg("Welcome to Guardian, " + Player.Name + "!");
        }

        public override void MainLoop()
        {            
            libtcodWrapper.KeyPress key;
            while (true)
            {
                AI();
                Render();
                
                key = libtcodWrapper.Keyboard.WaitForKeyPress(true);

                if (ProcessInput(key))
                {
                    break;
                }
            }
        }

        private void Render()
        {
            Root.Clear();
            MsgLog.RenderRecentToConsole(MsgCons);
            MsgCons.Blit(0, 0, 90, 5, Root, 0, 0);

            CurrentLevel.RenderToConsole(MapCons);

            MapCons.Blit(0, 0, 90, 30, Root, 1, 5);

            Root.Flush();
        }

        private void AI()
        {
            foreach (World.Creatures.CreatureBase c in CurrentLevel.Creatures)
            {
                if (c != Player)
                {
                    c.AI();
                }
            }
        }

        private bool ProcessInput(libtcodWrapper.KeyPress KP)
        {
            switch (KP.KeyCode)
            {
                case(libtcodWrapper.KeyCode.TCODK_HOME):
                    Player.MoveUpLeft();
                    break;
                case(libtcodWrapper.KeyCode.TCODK_UP):
                    Player.MoveUp();
                    break;
                case(libtcodWrapper.KeyCode.TCODK_PAGEUP):
                    Player.MoveUpRight();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_RIGHT):
                    Player.MoveRight();
                    break; 
                case(libtcodWrapper.KeyCode.TCODK_PAGEDOWN):
                    Player.MoveDownRight();
                    break;
                case(libtcodWrapper.KeyCode.TCODK_DOWN):
                    Player.MoveDown();
                    break;
                case(libtcodWrapper.KeyCode.TCODK_END):
                    Player.MoveDownLeft();
                    break;
                case(libtcodWrapper.KeyCode.TCODK_LEFT):
                    Player.MoveLeft();    
                    break;                                   
                case(libtcodWrapper.KeyCode.TCODK_ESCAPE):
                    StateManager.QueueState(StateManager.PersistentStates["MainMenuState"]);
                    return true;
                    break;
            }

            switch ((char)KP.Character)
            {
                case('d'):
                    MsgLog.AddMsg("Debug!");
                    break;
                case('m'):
                    StateManager.QueueState(StateManager.PersistentStates["MessageLogMenuState"]);
                    return true;
                    break;
                case('w'):
                    StateManager.QueueState(StateManager.PersistentStates["WorldMapMenuState"]);
                    return true;
                    break;
                case('s'):
                    CurrentLevel.DEBUG_Savemap("dbg.txt");
                    break;
            }

            return false;
        }

        public override void ExitState()
        {
            MapCons.Dispose();
            
        }
    }
}
