using System;
using System.Collections.Generic;
using System.Text;
using Guardian_Roguelike.World.Creatures.Limbs;

namespace Guardian_Roguelike.World.Creatures
{
    public enum CreatureTypes { Dwarf, Human, Sheep, Goblin, Elf, Giant_Rat };
    public enum Deity { Earth, Wind, Fire, Water, Pagan };
    public enum AIState { Wandering, Tracking, Attacking, Fleeing };
    
    public abstract class CreatureBase
    {
        //Stats
        public string FirstName;
        public string LastName;
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
        public Map Level;
        public CreatureTypes Type;
        public int Faction;
        public Items.Inventory InventoryHandler;
        public System.Drawing.Bitmap HitAreas;
        protected Random RndGen;
        protected libtcodWrapper.TCODFov FOVHandler;

        //Limb-related stuff
        public List<Limbs.LimbBase> Limbs;
        public List<Limbs.LimbDependency> LimbDependencies;
        public List<Limbs.LimbBase> PreferredLimbAttackOrder;
        public int LimbProbabilitySum;

        //AI Stuff
        public AI.AIBase MyAI;

        public CreatureBase()
        {
            BaseVigor = BaseEnergy = BaseSpeed = BaseAim = BaseStrength = 5;

            RndGen = new Random((int)DateTime.Now.Ticks);

            Level = (Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            
            FOVHandler = (libtcodWrapper.TCODFov)Utilities.InterStateResources.Instance.Resources["Game_FOVHandler"];

            Limbs = new List<Guardian_Roguelike.World.Creatures.Limbs.LimbBase>();
            LimbDependencies = new List<Guardian_Roguelike.World.Creatures.Limbs.LimbDependency>();
            PreferredLimbAttackOrder = new List<Guardian_Roguelike.World.Creatures.Limbs.LimbBase>();
        }

        #region Movement Methods
        public void Move(System.Drawing.Point p)
        {
            Move(p.X, p.Y);
        }
        public void Move(int X, int Y)
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

        public void EnforceLimbDependencies()
        {
            foreach (World.Creatures.Limbs.LimbDependency Dep in LimbDependencies)
            {
                if (Dep.TargetLimb.HP >= 0)
                {
                    if (Dep.RequiredLimb.HP <= 0)
                    {
                        Dep.TargetLimb.HP = 0;
                    }
                }
            }
        }

        public bool CanSeeCell(System.Drawing.Point Coords)
        {
            return CanSeeCell(Coords.X, Coords.Y);
        }
        public bool CanSeeCell(int X, int Y)
        {
            FOVHandler.CalculateFOV(Position.X, Position.Y, BaseAim, true, libtcodWrapper.FovAlgorithm.Basic);
            return FOVHandler.CheckTileFOV(X, Y);
        }

        public void AI()
        {
            MyAI.TakeTurn();
        }

        public void Attack(CreatureBase Target)
        {
            Random RndGen = new Random(DateTime.Now.Millisecond);
            double ChanceToHit = (((double)BaseAim - (double)Target.BaseSpeed) + (double)RndGen.Next(1,10)) / (double)10 * (double)100;
            int DamageDone = 0;
            
            if (RndGen.Next(0, 100) > ChanceToHit)//Miss
            {
                Utilities.MessageLog.AddMsg(FirstName + " swings at " + Target.FirstName + " with a precision that leaves something to be desired.");
                return;
            }

            //Pick limb to use for attack
            bool HasPickedALimb = false;
            while(!HasPickedALimb)
            {
                foreach(LimbBase Limb in PreferredLimbAttackOrder)
                {
                    if(RndGen.Next(0,100) < 95)
                    {
                        DamageDone = Limb.AttackWith();
                        HasPickedALimb = true;
                        break;
                    }
                }
            }

            //Pick limb on target to strike
            HasPickedALimb = false;
            while(!HasPickedALimb)
            {
                foreach(LimbBase Limb in Target.Limbs)
                {
                    if(RndGen.Next(0,100) < Limb.HitChance)
                    {
                        if(Limb.HP == 0)
                        {
                            Utilities.MessageLog.AddMsg(FirstName + " swings at the empty spot where " + Target.FirstName + "'s " + Limb.Description + " once was, severely injuring the air");
                            //Miss
                            return;
                        }
                        else
                        {
                            Limb.RecieveDamage(DamageDone);
                            if (Limb.HP < 30)
                            {
                                foreach (LimbDependency Dep in LimbDependencies)
                                {
                                    if (Dep.TargetLimb == Limb)
                                    {
                                        Dep.RequiredLimb.BleedRate++;
                                    }
                                }
                            }
                            Utilities.MessageLog.AddMsg(Target.FirstName + "'s " + Limb.Description + " takes the brunt of the impact!");
                            HasPickedALimb = true;
                            break;
                        }
                    }
                }
            }

            Utilities.DeathData DD = (Utilities.DeathData)Utilities.InterStateResources.Instance.Resources["Game_DeathData"];
            DD.Killer = this;
            Utilities.InterStateResources.Instance.Resources["Game_DeathData"] = DD;
        }

        public void Bleed()
        {
            foreach (LimbBase Limb in Limbs)
            {
                Limb.HP -= Limb.BleedRate;
                if (Limb.HP <= 0)
                {
                    Limb.BleedRate = 0;
                    Limb.HP = 0;
                    EnforceLimbDependencies();
                }
            }
        }


        public bool IsAlive()
        {
            return Limbs[0].HP > 0;
        }

        public abstract void Generate();
    }
}
