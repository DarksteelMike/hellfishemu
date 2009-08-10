using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures
{
    public enum CreatureTypes { Guardian, Human, Sheep };
    public enum Deity { Earth, Wind, Fire, Water, Pagan };
    public enum AIState { Wandering, Tracking, Attacking, Fleeing };

    public abstract class CreatureBase
    {
        //Stats
        public int HP;
        public int MaxHP;
        public int BaseVigor;
        public int BaseEnergy;
        public int BaseSpeed;
        public int BaseAim;
        public int BaseStrength;

        //Drawing stuff
        public System.Drawing.Point Position;
        public libtcodWrapper.Color DrawColor;
        public char CharRepresentation;

        //Internal Stuffis
        protected Map Level;
        protected Utilities.MessageLog Log;
        public CreatureTypes Type;
        public int Faction;
        protected AIState AIState;

        public CreatureBase()
        {
            Level = (Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            AIState = AIState.Wandering;
            Log = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];
        }

        #region Movement Methods
        private void Move(int X, int Y)
        {

            System.Drawing.Point LocalPosToCheck = new System.Drawing.Point();

            LocalPosToCheck.X = Position.X + X;
            LocalPosToCheck.Y = Position.Y + Y;

            if (Level.CheckWalkable(LocalPosToCheck))
            {
 
                Position.X = LocalPosToCheck.X;
                Position.Y = LocalPosToCheck.Y;
            }
        }

        public void MoveUpLeft()
        {
            Move(-1, -1);
        }
        public void MoveUp()
        {
            Move(0, -1);
        }
        public void MoveUpRight()
        {
            Move(1, -1);
        }
        public void MoveRight()
        {
            Move(1, 0);
        }
        public void MoveDownRight()
        {
            Move(1, 1);
        }
        public void MoveDown()
        {
            Move(0, 1);
        }
        public void MoveDownLeft()
        {
            Move(-1, 1);
        }
        public void MoveLeft()
        {
            Move(-1, 0);
        }
        #endregion

        public abstract void AI();

        #region AI Helper Methods
        public float PhysicalCondition
        {
            get
            {
                return (float)(HP / MaxHP);
            }
        }

        public float PercievedStrength
        {
            get
            {
                return (float)(PhysicalCondition * BaseStrength);
            }
        }

        public float PercievedDanger
        {
            get
            {
                return PercievedStrength;
            }
        }
        #endregion

        public bool CanSeeCell(System.Drawing.Point Coords)
        {
            //Is cell inside circleofsight?
            if ((Math.Pow((double)(Coords.X - Position.X), (double)2) + Math.Pow((double)(Coords.Y - Position.Y), (double)2)) < (Math.Pow(Math.Round((double)(BaseAim / 2) + 1, 0), (double)2)))
            {
                foreach (System.Drawing.Point P in Utilities.GeneralMethods.CalcBresenhamLine(Position, Coords))
                {
                    if (!Level.CheckSeeThrough(P))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public struct AILoveHateFearCreatureSet
    {
        public float Love_Fear;
        public float Hate;
        public CreatureBase SubjectCreature;
    }
}
