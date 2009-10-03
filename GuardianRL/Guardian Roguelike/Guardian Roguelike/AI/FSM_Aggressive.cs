using System;
using System.Collections.Generic;
using System.Text;
using Guardian_Roguelike.World.Creatures;
using System.Drawing;

namespace Guardian_Roguelike.AI
{
    public enum AIState { Wander, Follow, Flee, Attack };
    class FSM_Aggressive : AIBase
    {
        public AIState CurState; //TEMPORARILY PUBLIC! FOR DEBUG ONLY!
        private Point CurWanderDirection;
        private Point PlayerLastSeenAt;
        
        private CreatureBase Player;

        private List<Point> CurFollowPath;
        private int CurFollowPathIndex;

        private Utilities.MessageLog MsgLog;

        public FSM_Aggressive(CreatureBase C)
            : base(C)
        {
            CurState = AIState.Wander;
            CurWanderDirection = new Point();
            do
            {
                CurWanderDirection.X = RndGen.Next(-1, 1);
                CurWanderDirection.Y = RndGen.Next(-1, 1);
            } while (CurWanderDirection.X == 0 && CurWanderDirection.Y == 0);

            PlayerLastSeenAt = new Point();

            Player = (CreatureBase)Utilities.InterStateResources.Instance.Resources["Game_PlayerCreature"];

            MsgLog = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];
            
        }
        public override void TakeTurn()
        {
            switch (CurState)
            {
                case(AIState.Wander):
                    //If the current direction is walkable, do it!
                    if(LinkedCreature.Level.CheckWalkable(Utilities.GeneralMethods.AddPoints(LinkedCreature.Position,CurWanderDirection)))
                    {
                        LinkedCreature.Move(CurWanderDirection);
                        if (RndGen.Next(0, 100) < 25) //Maybe decide on a new direction
                        {
                            do
                            {
                                CurWanderDirection.X = RndGen.Next(-1, 1);
                                CurWanderDirection.Y = RndGen.Next(-1, 1);
                            } while (CurWanderDirection.X == 0 && CurWanderDirection.Y == 0);

                        }
                    }
                    else //Not walkable, decide on a new one.
                    {
                        do
                        {
                            CurWanderDirection.X = RndGen.Next(-1, 1);
                            CurWanderDirection.Y = RndGen.Next(-1, 1);
                        } while (CurWanderDirection.X == 0 && CurWanderDirection.Y == 0);
                    }                    

                    FOVHandler.CalculateFOV(LinkedCreature.Position.X, LinkedCreature.Position.Y, 0, false, libtcodWrapper.FovAlgorithm.Basic);
                    if (FOVHandler.CheckTileFOV(Player.Position.X, Player.Position.Y)) //If can see player
                    {
                        PlayerLastSeenAt = Player.Position;//Remember where
                        CurState = AIState.Follow; //Follow him!
                        CurFollowPath = ComputePath(LinkedCreature.Position, PlayerLastSeenAt); //How do we get there?
                        CurFollowPathIndex = 0; //Start from the beginning
                    }
                    if (LinkedCreature.HP <= LinkedCreature.MaxHP / 3) //OH NO!
                    {
                        CurState = AIState.Flee;
                    }
                    break;

                case(AIState.Follow):
                    FOVHandler.CalculateFOV(LinkedCreature.Position.X,LinkedCreature.Position.Y,0,false,libtcodWrapper.FovAlgorithm.Basic);
                    if(PlayerLastSeenAt != Player.Position) //If player isn't in the same place still...
                    {
                        if(FOVHandler.CheckTileFOV(Player.Position.X,Player.Position.Y)) //And we can see player
                        {
                            PlayerLastSeenAt = Player.Position; //Update our memory
                            CurFollowPath = ComputePath(LinkedCreature.Position,PlayerLastSeenAt); //And figure out a new path.
                            CurFollowPathIndex = 0; //And start from the beginning of it!
                        }
                    }
                    if (CurFollowPathIndex >= 0 && CurFollowPathIndex < CurFollowPath.Count-1)//We're on the way to the player (Or where we saw him last)
                    {
                        LinkedCreature.Move(CurFollowPath[CurFollowPathIndex].X - LinkedCreature.Position.X, CurFollowPath[CurFollowPathIndex].Y - LinkedCreature.Position.Y);
                    }
                    CurFollowPathIndex++;
                    if (Utilities.GeneralMethods.Distance(LinkedCreature.Position,Player.Position) == 1) //We're in stabby range!
                    {
                        CurState = AIState.Attack; //ATTACK!!!!
                    }
                    if (CurFollowPathIndex == CurFollowPath.Count-1 && !FOVHandler.CheckTileFOV(Player.Position.X, Player.Position.Y)) //We're there but he's gone! We can't see him!
                    {
                        CurState = AIState.Wander;
                    }
                    if (LinkedCreature.HP <= LinkedCreature.MaxHP / 3) //Ack! We've been wounded!
                    {
                        CurState = AIState.Flee;
                    }
                    break;

                case(AIState.Attack):
                    MsgLog.AddMsg(LinkedCreature.Name + " swings at you!");
                    LinkedCreature.Attack(Player);
                    if (Utilities.GeneralMethods.Distance(LinkedCreature.Position, Player.Position) != 1)
                    {
                        CurState = AIState.Follow;
                    }
                    if (LinkedCreature.HP <= LinkedCreature.MaxHP / 3)
                    {
                        CurState = AIState.Flee;
                    }
                    break;

                case(AIState.Flee):
                    Point Diff = Utilities.GeneralMethods.SubtractPoints(LinkedCreature.Position, Player.Position);
                    Point FleeDir = new Point();
                    if(Diff.X > 0)
                    {
                        FleeDir.X = 1;
                    }
                    else if(Diff.X < 0)
                    {
                        FleeDir.X = -1;
                    }
                    else
                    {
                        FleeDir.X = 0;
                    }
                    if (Diff.Y > 0)
                    {
                        FleeDir.Y = 1;
                    }
                    else if (Diff.Y < 0)
                    {
                        FleeDir.Y = -1;
                    }
                    else
                    {
                        FleeDir.Y = 0;
                    }
                    LinkedCreature.Move(FleeDir);
                    break;
                    
            }
        }
    }
}
