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

        private Modes CurMode;

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
            tPlayer.Name = "Bellerofon";
            tPlayer.DrawColor = libtcodWrapper.ColorPresets.ForestGreen;
            if (!Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_PlayerCreature"))
            {
                Utilities.InterStateResources.Instance.Resources.Add("Game_PlayerCreature", tPlayer);
            }

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

            MsgLog.AddMsg("Welcome to Guardian, " + Player.Name + "!");
        }

        public override void EnterState()
        {

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
                switch (KP.KeyCode)
                {
                    case (libtcodWrapper.KeyCode.TCODK_HOME):
                        CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y - 1);
                        Player.MoveUpLeft();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_UP):
                        CurrentLevel.DestroyTile(Player.Position.X, Player.Position.Y - 1);
                        Player.MoveUp();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_PAGEUP):
                        CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y - 1);
                        Player.MoveUpRight();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_RIGHT):
                        CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y);
                        Player.MoveRight();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_PAGEDOWN):
                        CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y + 1);
                        Player.MoveDownRight();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_DOWN):
                        CurrentLevel.DestroyTile(Player.Position.X, Player.Position.Y + 1);
                        Player.MoveDown();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_END):
                        CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y + 1);
                        Player.MoveDownLeft();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_LEFT):
                        CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y);
                        Player.MoveLeft();
                        CurMode = Modes.Normal;
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_ESCAPE):
                        CurMode = Modes.Normal;
                        break;
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
