using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class GameState : StateBase
    {
        private World.WorldMap CurrentWorld;
        private World.Map CurrentLocalMap;
        private libtcodWrapper.Console MapCons;

        private World.Creatures.Guardian Player;

        private Utilities.MessageLog MsgLog;
        private libtcodWrapper.Console MsgCons;

        private States.StateBase FollowingState;

        public override void EnterState()
        {
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_MessageLog"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_MessageLog", new Utilities.MessageLog());
            }
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_WorldMap"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_WorldMap", new World.WorldMap());
            }
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_PlayerCreature"))
            {
                World.Creatures.Guardian tPlayer = new Guardian_Roguelike.World.Creatures.Guardian();
                tPlayer.HP = 100;
                tPlayer.LocalPos.X = tPlayer.LocalPos.Y = 1;
                tPlayer.GlobalPos.X = tPlayer.GlobalPos.Y = 0;
                tPlayer.Name = "Spartacus";
                tPlayer.DrawColor = libtcodWrapper.ColorPresets.ForestGreen;
                Utilities.InterStateResources.Instance.Resources.Add("Game_PlayerCreature", tPlayer);
            }
            MapCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);
            CurrentWorld = (World.WorldMap)Utilities.InterStateResources.Instance.Resources["Game_WorldMap"];

            MsgCons = libtcodWrapper.RootConsole.GetNewConsole(90, 5);
            MsgLog = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];

            Player = (World.Creatures.Guardian)Utilities.InterStateResources.Instance.Resources["Game_PlayerCreature"];

            CurrentLocalMap = CurrentWorld.LocalMaps[Player.GlobalPos.X, Player.GlobalPos.Y];
            CurrentLocalMap.Creatures.Add(Player);

            MsgLog.AddMsg("Welcome to Guardian, " + Player.Name + "!");
        }

        public override void MainLoop()
        {            
            libtcodWrapper.KeyPress key;
            while (true)
            {
                Render();

                key = libtcodWrapper.Keyboard.WaitForKeyPress(true);

                if (ProcessInput(key))
                {
                    break;
                }
            }

            StateManager.QueueState(FollowingState);
        }

        private void Render()
        {
            Root.Clear();
            MsgLog.RenderRecentToConsole(MsgCons);
            MsgCons.Blit(0, 0, 90, 5, Root, 0, 0);

            CurrentLocalMap = CurrentWorld.LocalMaps[Player.GlobalPos.X, Player.GlobalPos.Y];

            CurrentLocalMap.RenderToConsole(MapCons);

            MapCons.Blit(0, 0, 90, 30, Root, 1, 5);

            Root.Flush();
        }

        private bool ProcessInput(libtcodWrapper.KeyPress KP)
        {
            switch (KP.KeyCode)
            {
                case(libtcodWrapper.KeyCode.TCODK_UP):
                    Player.MoveUp();
                    /*
                    if (Player.LocalPos.Y == 0)
                    {
                        Player.LocalPos.Y = 30;
                        if (Player.GlobalPos.Y == 0)
                        {
                            Player.GlobalPos.Y = 29;
                        }
                        else
                        {
                            Player.GlobalPos.Y--;
                        }
                        MsgLog.AddMsg("Moved in world: " + Player.GlobalPos);
                    }
                    if (CurrentLocalMap.GetWalkable(Player.LocalPos.X, Player.LocalPos.Y - 1))
                    {
                        Player.LocalPos.Y--;
                        MsgLog.AddMsg("Moved in local: " + Player.LocalPos);
                    }
                     * */
                    break;
                case(libtcodWrapper.KeyCode.TCODK_DOWN):
                    Player.MoveDown();
                    /*
                    if (Player.LocalPos.Y == 29)
                    {
                        Player.LocalPos.Y = -1;
                        if (Player.GlobalPos.Y == 29)
                        {
                            Player.GlobalPos.Y = 0;
                        }
                        else
                        {
                            Player.GlobalPos.Y++;
                        }
                        MsgLog.AddMsg("Moved in world: " + Player.GlobalPos);
                    }
                    if (CurrentLocalMap.GetWalkable(Player.LocalPos.X, Player.LocalPos.Y + 1))
                    {
                        Player.LocalPos.Y++;
                        MsgLog.AddMsg("Moved in local: " + Player.LocalPos);
                    }
                     * */
                    break;
                case(libtcodWrapper.KeyCode.TCODK_LEFT):
                    Player.MoveLeft();    
                    /*
                    if (Player.LocalPos.X == 0)
                    {
                        Player.LocalPos.X = 89;
                        if (Player.GlobalPos.X == 0)
                        {
                            Player.GlobalPos.X = 89;
                        }
                        else
                        {
                            Player.GlobalPos.X--;
                        }
                        MsgLog.AddMsg("Moved in world: " + Player.GlobalPos);
                    }
                    if (CurrentLocalMap.GetWalkable(Player.LocalPos.X - 1, Player.LocalPos.Y))
                    {
                        Player.LocalPos.X--;
                        MsgLog.AddMsg("Moved in local: " + Player.LocalPos);
                    }
                    */
                    break;
                case(libtcodWrapper.KeyCode.TCODK_RIGHT):
                    Player.MoveRight();
                    /*
                    if (Player.LocalPos.X == 89)
                    {
                        Player.LocalPos.X = 0;
                        if (Player.GlobalPos.X == 89)
                        {
                            Player.GlobalPos.X = 0;
                        }
                        else
                        {
                            Player.GlobalPos.X++;
                        }
                        MsgLog.AddMsg("Moved in world: " + Player.GlobalPos);
                    }
                    if (CurrentLocalMap.GetWalkable(Player.LocalPos.X + 1, Player.LocalPos.Y))
                    {
                        Player.LocalPos.X++;
                        MsgLog.AddMsg("Moved in local: " + Player.LocalPos);
                    }
                     * */
                    break;                    
                case(libtcodWrapper.KeyCode.TCODK_ESCAPE):
                    FollowingState = States.StateManager.PersistentStates["MainMenuState"];
                    return true;
                    break;
            }

            switch ((char)KP.Character)
            {
                case('d'):
                    MsgLog.AddMsg("Debug!");
                    break;
                case('m'):
                    FollowingState = StateManager.PersistentStates["MessageLogMenuState"];
                    return true;
                    break;
                case('w'):
                    FollowingState = StateManager.PersistentStates["WorldMapMenuState"];
                    return true;
                    break;
                case('s'):
                    CurrentWorld.DEBUG_SaveMap(Player.GlobalPos);
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
