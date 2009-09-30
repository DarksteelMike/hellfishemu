using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    public enum Modes { Normal, Digging };
    class GameState : StateBase
    {
        private World.Map CurrentLevel;
        private libtcodWrapper.Console MapCons;

        private World.Creatures.Dwarf Player;

        private Utilities.MessageLog MsgLog;
        private libtcodWrapper.Console MsgCons;

        private libtcodWrapper.Console StatusCons;

        private Modes CurMode;

        private int SkipTurns;

        private int TurnsPassed;

        public GameState() : base()
        {
            CurMode = Modes.Normal;
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_MessageLog"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_MessageLog", new Utilities.MessageLog());
            }

            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_CurrentLevel"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_CurrentLevel", new World.Map());
            }
            World.Creatures.Dwarf tPlayer = new Guardian_Roguelike.World.Creatures.Dwarf();
            tPlayer.Position.X = tPlayer.Position.Y = 1;
            tPlayer.Name = "Urist";
            tPlayer.DrawColor = libtcodWrapper.ColorPresets.ForestGreen;
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_PlayerCreature"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_PlayerCreature", tPlayer);
            }

            StatusCons = libtcodWrapper.RootConsole.GetNewConsole(90, 5);

            MapCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);

            MsgCons = libtcodWrapper.RootConsole.GetNewConsole(90, 5);
            MsgLog = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];

            Player = (World.Creatures.Dwarf)Utilities.InterStateResources.Instance.Resources["Game_PlayerCreature"];

            CurrentLevel = (World.Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            CurrentLevel.Creatures.Add(Player);

            /*
            World.Creatures.Guardian tMonster = new Guardian_Roguelike.World.Creatures.Guardian();
            tMonster.Name = "Chimaira";
            tMonster.Position = CurrentLevel.GetFirstWalkable();
            CurrentLevel.Creatures.Add(tMonster);
            */
            Player.Position = CurrentLevel.GetFirstWalkable();

            MsgLog.AddMsg("Get running, " + Player.Name + "!");

            TurnsPassed = SkipTurns = 0;
        }

        public override void EnterState()
        {

        }

        public override void MainLoop()
        {            
            libtcodWrapper.KeyPress key;
            Render();
            while (true)
            {                
                key = libtcodWrapper.Keyboard.WaitForKeyPress(true);

                if (ProcessInput(key))
                {
                    break;
                }

                do
                {
                    AI();
                    Render();
                    TurnsPassed++;
                } while (SkipTurns-- > 0);
            }
        }

        private void Render()
        {
            Root.Clear();
            MsgLog.RenderRecentToConsole(MsgCons);
            MsgCons.Blit(0, 0, 90, 5, Root, 0, 0);

            CurrentLevel.RenderToConsole(MapCons);

            MapCons.Blit(0, 0, 90, 30, Root, 1, 5);

            StatusCons.PrintLine("HP: " + Player.HP.ToString() + "/" + Player.MaxHP.ToString() + "  Turn: " + TurnsPassed.ToString(),0,0,libtcodWrapper.LineAlignment.Left);
            StatusCons.Blit(0, 0, 90, 5, Root, 0, 35);

            Root.Flush();
        }

        private void AI()
        {
            foreach (World.Creatures.CreatureBase c in CurrentLevel.Creatures)
            {
                if (!c.Equals(Player))
                {
                    c.AI();
                }
            }
        }

        private bool ProcessInput(libtcodWrapper.KeyPress KP)
        {
            if (CurMode == Modes.Digging)
            {
                World.DestroyResults DR = Guardian_Roguelike.World.DestroyResults.Cancelled;
                switch (KP.KeyCode)
                {
                    case (libtcodWrapper.KeyCode.TCODK_HOME):
                        DR = CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y - 1);
                        Player.MoveUpLeft();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_UP):
                        DR = CurrentLevel.DestroyTile(Player.Position.X, Player.Position.Y - 1);
                        Player.MoveUp();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_PAGEUP):
                        DR = CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y - 1);
                        Player.MoveUpRight();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_RIGHT):
                        DR = CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y);
                        Player.MoveRight();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_PAGEDOWN):
                        DR = CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y + 1);
                        Player.MoveDownRight();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_DOWN):
                        DR = CurrentLevel.DestroyTile(Player.Position.X, Player.Position.Y + 1);
                        Player.MoveDown();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_END):
                        DR = CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y + 1);
                        Player.MoveDownLeft();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_LEFT):
                        DR = CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y);
                        Player.MoveLeft();
                        CurMode = Modes.Normal;
                        SkipTurns = 2;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_ESCAPE):
                        CurMode = Modes.Normal;
                        break;
                }

                //Diggin *was* attempted, add appropriate message to log
                if (CurMode == Modes.Normal)
                {
                    switch (DR)
                    {
                        case(World.DestroyResults.AlreadyEmpty):
                            MsgLog.AddMsg("You swing at the air,predictably hitting nothing, and stumble forward.");
                            break;
                        case(World.DestroyResults.Cancelled):
                            MsgLog.AddMsg("You lower your pick again.");
                            break;
                        case(World.DestroyResults.Indestructible):
                            MsgLog.AddMsg("You swing with all your strength, but the pick bounces off the wall without doing damage.");
                            break;
                        case(World.DestroyResults.Success):
                            MsgLog.AddMsg("The rock crumbles under the might of your pick.");
                            break;
                    }
                }
                return false;
            }

            switch (KP.KeyCode)
            {
                case (libtcodWrapper.KeyCode.TCODK_HOME):
                    Player.MoveUpLeft();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_UP):
                    Player.MoveUp();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_PAGEUP):
                    Player.MoveUpRight();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_RIGHT):
                    Player.MoveRight();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_PAGEDOWN):
                    Player.MoveDownRight();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_DOWN):
                    Player.MoveDown();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_END):
                    Player.MoveDownLeft();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_LEFT):
                    Player.MoveLeft();
                    break;
                case (libtcodWrapper.KeyCode.TCODK_ESCAPE):
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

                case('t'): //All-round testing button
                    MsgLog.AddMsg("Dig in which direction?");
                    CurMode = Modes.Digging;
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
