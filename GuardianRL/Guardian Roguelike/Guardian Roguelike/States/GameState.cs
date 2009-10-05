using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    public enum Modes { Normal, Swinging, HasDug };
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

        private bool SkipAI;

        private int TurnsPassed;

        private int LevelNumber;

        private DateTime DEBUG_PerformanceTime;

        private int DEBUG_PassedMilliseconds;

        public GameState() : base()
        {
            DEBUG_PassedMilliseconds = 0;
            SkipAI = false;
            CurMode = Modes.Normal;
            if (Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_MessageLog"))
            {
                Utilities.InterStateResources.Instance.Resources.Remove("Game_MessageLog");
            }
            if (Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_FOVHandler"))
            {
                Utilities.InterStateResources.Instance.Resources.Remove("Game_FOVHandler");
            }
            if (Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_PathFinder"))
            {
                Utilities.InterStateResources.Instance.Resources.Remove("Game_PathFinder");
            }
            if (Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_CurrentLevel"))
            {
                Utilities.InterStateResources.Instance.Resources.Remove("Game_CurrentLevel");
            }
            if (Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_NotableEventsLog"))
            {
                Utilities.InterStateResources.Instance.Resources.Remove("Game_NotableEventsLog");
            }

            Utilities.InterStateResources.Instance.Resources.Add("Game_MessageLog", new Utilities.MessageLog());
            Utilities.InterStateResources.Instance.Resources.Add("Game_FOVHandler", new libtcodWrapper.TCODFov(90, 30));
            Utilities.InterStateResources.Instance.Resources.Add("Game_PathFinder", new libtcodWrapper.TCODPathFinding((libtcodWrapper.TCODFov)Utilities.InterStateResources.Instance.Resources["Game_FOVHandler"], 1));
            Utilities.InterStateResources.Instance.Resources.Add("Game_CurrentLevel", new World.Map());
            Utilities.InterStateResources.Instance.Resources.Add("Game_NotableEventsLog", new Utilities.NotableEventsLog());
            
            /*
            World.Creatures.Dwarf tPlayer = new Guardian_Roguelike.World.Creatures.Dwarf();
            tPlayer.Position.X = tPlayer.Position.Y = 1;
            tPlayer.FirstName = "Urist";
            tPlayer.LastName = "Litasterar";
            tPlayer.DrawColor = libtcodWrapper.ColorPresets.ForestGreen;*/
            if (Utilities.InterStateResources.Instance.Resources.ContainsKey("Game_PlayerCreature"))
            {
                Utilities.InterStateResources.Instance.Resources.Remove("Game_PlayerCreature");
            }
            World.Creatures.CreatureBase tPlayer = new World.Creatures.Dwarf();
            tPlayer.Generate();
            tPlayer.DrawColor = libtcodWrapper.ColorPresets.ForestGreen;

            Utilities.InterStateResources.Instance.Resources.Add("Game_PlayerCreature", tPlayer);

            StatusCons = libtcodWrapper.RootConsole.GetNewConsole(90, 5);

            MapCons = libtcodWrapper.RootConsole.GetNewConsole(90, 30);

            MsgCons = libtcodWrapper.RootConsole.GetNewConsole(90, 5);
            MsgLog = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];

            Player = (World.Creatures.Dwarf)Utilities.InterStateResources.Instance.Resources["Game_PlayerCreature"];
            Player.BaseAim += 2;
            Player.BaseEnergy += 2;
            Player.BaseSpeed += 2;
            Player.BaseStrength += 2;
            Player.BaseVigor += 2;
            Player.FirstName = "Urist";
            Player.LastName = "Litasterar";

            CurrentLevel = (World.Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            CurrentLevel.Creatures.Add(Player);

            /*
            World.Creatures.Guardian tMonster = new Guardian_Roguelike.World.Creatures.Guardian();
            tMonster.Name = "Chimaira";
            tMonster.Position = CurrentLevel.GetFirstWalkable();
            CurrentLevel.Creatures.Add(tMonster);
            */
            Player.Position = CurrentLevel.GetFirstWalkable();

            MsgLog.AddMsg("Get running, " + Player.FirstName + "!");

            TurnsPassed = SkipTurns = 0;

            LevelNumber = 1;
        }

        public override void EnterState()
        {

        }

        public override void MainLoop()
        {            
            libtcodWrapper.KeyPress key;
            CurrentLevel.CalculateVisible(Player.Position.X, Player.Position.Y,Player.BaseAim);
            Render();
            while (true)
            {                
                key = libtcodWrapper.Keyboard.WaitForKeyPress(true);

                if (ProcessInput(key))
                {
                    break;
                }

                DEBUG_PerformanceTime = DateTime.Now;
                CurrentLevel.CalculateVisible(Player.Position.X, Player.Position.Y,Player.BaseAim);
                if (!SkipAI)
                {
                    do
                    {
                        
                        AI();
                        Render();
                        TurnsPassed++;
                    } while (SkipTurns-- > 0);
                }
                DEBUG_PassedMilliseconds = DateTime.Now.Millisecond - DEBUG_PerformanceTime.Millisecond;
                libtcodWrapper.RootConsole.WindowTitle = "Run, Urist, Run! Time: " + DEBUG_PassedMilliseconds.ToString() + "ms";

                SkipAI = false;
                if (Player.HP <= 0)
                {
                    States.DeathData DD = (States.DeathData)Utilities.InterStateResources.Instance.Resources["Game_DeathData"];
                    DD.LevelsDescended = LevelNumber;
                    DD.TurnsSurvived = TurnsPassed;
                    StateManager.QueueState(StateManager.PersistentStates["GameOverState"]);
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

            StatusCons.PrintLine("HP: " + Player.HP.ToString() + "/" + Player.MaxHP.ToString() + "  Turn: " + TurnsPassed.ToString(),0,0,libtcodWrapper.LineAlignment.Left);
            StatusCons.PrintLine("Level " + LevelNumber.ToString(), 0, 1, libtcodWrapper.LineAlignment.Left);
            StatusCons.PrintLine("V:" + Player.BaseVigor.ToString() + " E:" + Player.BaseEnergy.ToString() + " Sp:" + Player.BaseSpeed.ToString() + " St:" + Player.BaseStrength.ToString() + " A:" + Player.BaseAim.ToString(), 0, 2, libtcodWrapper.LineAlignment.Left);
            StatusCons.Blit(0, 0, 90, 5, Root, 0, 35);

            Root.Flush();
        }

        private void AI()
        {
            foreach (World.Creatures.CreatureBase c in CurrentLevel.Creatures)
            {
                if (!c.Equals(Player) && c.IsAlive())
                {
                    c.AI();
                }
            }
        }

        private bool ProcessInput(libtcodWrapper.KeyPress KP)
        {
            if (CurMode == Modes.Swinging)
            {
                
                World.DestroyResults DR = Guardian_Roguelike.World.DestroyResults.Cancelled;
                switch (KP.KeyCode)
                {
                    case (libtcodWrapper.KeyCode.TCODK_HOME):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X - 1, Player.Position.Y - 1))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X - 1, Player.Position.Y - 1));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y - 1);
                            Player.MoveUpLeft();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_UP):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X, Player.Position.Y - 1))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X, Player.Position.Y - 1));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X, Player.Position.Y - 1);
                            Player.MoveUp();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_PAGEUP):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X + 1, Player.Position.Y - 1))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X + 1, Player.Position.Y - 1));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y - 1);
                            Player.MoveUpRight();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_RIGHT):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X + 1, Player.Position.Y))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X + 1, Player.Position.Y));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y);
                            Player.MoveRight();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_PAGEDOWN):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X + 1, Player.Position.Y + 1))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X + 1, Player.Position.Y + 1));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X + 1, Player.Position.Y + 1);
                            Player.MoveDownRight();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_DOWN):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X, Player.Position.Y + 1))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X, Player.Position.Y + 1));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X, Player.Position.Y + 1);
                            Player.MoveDown();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_END):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X - 1, Player.Position.Y + 1))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X - 1, Player.Position.Y + 1));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y + 1);
                            Player.MoveDownLeft();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_LEFT):
                        if (CurrentLevel.CheckContainsCreature(Player.Position.X - 1, Player.Position.Y))
                        {
                            Player.Attack(CurrentLevel.GetCreatureAt(Player.Position.X - 1, Player.Position.Y));
                            CurMode = Modes.Normal;
                        }
                        else
                        {
                            DR = CurrentLevel.DestroyTile(Player.Position.X - 1, Player.Position.Y);
                            Player.MoveLeft();
                            CurMode = Modes.HasDug;
                            SkipTurns = 2;
                        }
                        break;
                    case (libtcodWrapper.KeyCode.TCODK_ESCAPE):
                        CurMode = Modes.Normal;
                        break;
                }

                //Digging was attempted, add appropriate message to log
                if (CurMode == Modes.HasDug)
                {
                    switch (DR)
                    {
                        case (World.DestroyResults.AlreadyEmpty):
                            MsgLog.AddMsg("You swing at the air,predictably hitting nothing, and stumble forward.");
                            break;
                        case (World.DestroyResults.Cancelled):
                            MsgLog.AddMsg("You lower your pick again.");
                            break;
                        case (World.DestroyResults.Indestructible):
                            MsgLog.AddMsg("You swing with all your strength, but the pick bounces off the wall without doing damage.", libtcodWrapper.Color.FromRGB(255, 0, 0));
                            break;
                        case (World.DestroyResults.Success):
                            MsgLog.AddMsg("The rock crumbles under the might of your pick.");
                            break;
                    }
                    CurMode = Modes.Normal;
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
                case('m'):
                    //View Messagelog
                    StateManager.QueueState(StateManager.PersistentStates["MessageLogMenuState"]);
                    return true;
                    break;
                case('d')://Debug key
                    MsgLog.AddMsg(CurrentLevel.Creatures[1].FirstName + " is in " + ((global::Guardian_Roguelike.AI.FSM_Aggressive)CurrentLevel.Creatures[1].MyAI).CurState.ToString() + " mode " + CurrentLevel.Creatures[1].Position.ToString());
                    break;

                case('t'): //All-round testing button
                    //Enter Portal
                    if (CurrentLevel.CheckType(Player.Position) == Guardian_Roguelike.World.TerrainTypes.ExitPortal)
                    {
                        MakeNewMap();
                        MsgLog.AddMsg("You descend deeper into the pit...");
                    }
                    else
                    {
                        MsgLog.AddMsg("There are no stairs here!");
                    }
                    break;

                case('s'):
                    //Swing
                    //TODO: Check what is wielded.Assumes pick for now
                    MsgLog.AddMsg("Swing in which direction?");
                    CurMode = Modes.Swinging;
                    SkipAI = true;
                    break;
                case('l'):
                    //Look
                    CurrentLevel.CalculateVisible(Player.Position, Player.BaseAim);
                    CurrentLevel.PostLookMessages(MsgLog);
                    break;
            }

            return false;
        }

        private void MakeNewMap()
        {
            CurrentLevel = new Guardian_Roguelike.World.Map();
            Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"] = CurrentLevel;
            MapCons.Clear();
            CurrentLevel.Creatures.Add(Player);
            Player.Level = CurrentLevel;
            CurrentLevel.DestroyTile(Player.Position);
            LevelNumber++;

            //Place a dwarf randomly
            Random RandGen = new Random();
            while (true)
            {
                int dx = RandGen.Next(0, 90);
                int dy = RandGen.Next(0, 30);

                if (CurrentLevel.CheckWalkable(dx, dy))
                {
                    World.Creatures.Dwarf Olon = new World.Creatures.Dwarf();
                    Olon.Generate();
                    Olon.Position = new System.Drawing.Point(dx, dy);
                    Olon.MyAI = new AI.FSM_Aggressive(Olon);
                    CurrentLevel.Creatures.Add(Olon);
                    break;
                }
            }
        }

        public override void ExitState()
        {
        }
    }
}
