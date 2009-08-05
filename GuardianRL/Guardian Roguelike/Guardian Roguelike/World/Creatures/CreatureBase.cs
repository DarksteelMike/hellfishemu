using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures
{
    class CreatureBase
    {
        //Stats
        public int HP;

        //Drawing stuff
        public System.Drawing.Point LocalPos;
        public System.Drawing.Point GlobalPos;
        public libtcodWrapper.Color DrawColor;
        public char CharRepresentation;

        //Internal Stuffis
        private WorldMap World;
        private Utilities.MessageLog Log;

        public CreatureBase()
        {
            World = (WorldMap)Utilities.InterStateResources.Instance.Resources["Game_WorldMap"];
            Log = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];
        }

        public void MoveUp()
        {
            if (LocalPos.Y == 0)
            {
                if (GlobalPos.Y == 0)
                {
                    if (World.CheckWalkable(new System.Drawing.Point(GlobalPos.X, 29), new System.Drawing.Point(LocalPos.X, 29)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[GlobalPos.X, 29]);
                        GlobalPos.Y = 29;
                        LocalPos.Y = 29;
                    }
                }
                else
                {
                    if (World.CheckWalkable(new System.Drawing.Point(GlobalPos.X, GlobalPos.Y - 1), new System.Drawing.Point(LocalPos.X, 29)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[GlobalPos.X, GlobalPos.Y - 1]);
                        GlobalPos.Y--;
                        LocalPos.Y = 29;
                    }
                }
                Log.AddMsg("Moved Globally to: " + GlobalPos.ToString());
            }
            else
            {
                if (World.CheckWalkable(GlobalPos, new System.Drawing.Point(LocalPos.X, LocalPos.Y - 1)))
                {
                    LocalPos.Y--;
                    Log.AddMsg("Moved Locally to: " + LocalPos.ToString());
                }
            }
        }

        public void MoveDown()
        {
            if (LocalPos.Y == 29)
            {
                if (GlobalPos.Y == 29)
                {
                    if (World.CheckWalkable(new System.Drawing.Point(GlobalPos.X, 0), new System.Drawing.Point(LocalPos.X, 0)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[GlobalPos.X, 0]);
                        GlobalPos.Y = 0;
                        LocalPos.Y = 0;
                    }
                }
                else
                {
                    if (World.CheckWalkable(new System.Drawing.Point(GlobalPos.X, GlobalPos.Y + 1), new System.Drawing.Point(LocalPos.X, 0)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[GlobalPos.X, GlobalPos.Y + 1]);
                        GlobalPos.Y++;
                        LocalPos.Y = 0;
                    }
                }
                Log.AddMsg("Moved Globally to: " + GlobalPos.ToString());
            }
            else
            {
                if (World.CheckWalkable(GlobalPos, new System.Drawing.Point(LocalPos.X, LocalPos.Y + 1)))
                {
                    LocalPos.Y++;
                    Log.AddMsg("Moved Locally to: " + LocalPos.ToString());
                }
            }
        }

        public void MoveLeft()
        {
            if (LocalPos.X == 0)
            {
                if (GlobalPos.X == 0)
                {
                    if (World.CheckWalkable(new System.Drawing.Point(89, GlobalPos.Y), new System.Drawing.Point(89, LocalPos.Y)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[89, GlobalPos.Y]);
                        GlobalPos.X = 89;
                        LocalPos.X = 89;
                        
                    }
                }
                else
                {
                    if (World.CheckWalkable(new System.Drawing.Point(GlobalPos.X - 1, GlobalPos.Y), new System.Drawing.Point(89, LocalPos.Y)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[GlobalPos.X - 1, GlobalPos.Y]);
                        GlobalPos.X--;
                        LocalPos.X = 89;
                    }
                }
                Log.AddMsg("Moved Globally to: " + GlobalPos.ToString());
            }
            else
            {
                if (World.CheckWalkable(GlobalPos, new System.Drawing.Point(LocalPos.X - 1, LocalPos.Y)))
                {
                    LocalPos.X--;
                    Log.AddMsg("Moved Locally to: " + LocalPos.ToString());
                }
            }
        }

        public void MoveRight()
        {
            if (LocalPos.X == 89)
            {
                if (GlobalPos.X == 89)
                {
                    if (World.CheckWalkable(new System.Drawing.Point(0, GlobalPos.Y), new System.Drawing.Point(0, LocalPos.Y)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[0, GlobalPos.Y]);
                        GlobalPos.X = 0;
                        LocalPos.X = 0;

                    }
                }
                else
                {
                    if (World.CheckWalkable(new System.Drawing.Point(GlobalPos.X + 1, GlobalPos.Y), new System.Drawing.Point(0, LocalPos.Y)))
                    {
                        World.MoveCreature(this, World.LocalMaps[GlobalPos.X, GlobalPos.Y], World.LocalMaps[GlobalPos.X + 1, GlobalPos.Y]);
                        GlobalPos.X++;
                        LocalPos.X = 0;
                    }
                }
                Log.AddMsg("Moved Globally to: " + GlobalPos.ToString());
            }
            else
            {
                if (World.CheckWalkable(GlobalPos, new System.Drawing.Point(LocalPos.X + 1, LocalPos.Y)))
                {
                    LocalPos.X++;
                    Log.AddMsg("Moved Locally to: " + LocalPos.ToString());
                }
            }
        }
    }
}
