using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World
{
    public class Map
    {
        private static Random RandGen;
        public const int WIDTH = 90;
        public const int HEIGHT = 30;
        public TerrainTile[,] DisplayData;
        //private bool[,] WalkabilityData;
        //private bool[,] SeeThroughData;

        public List<Creatures.CreatureBase> Creatures;

        public Map()
        {            
            Creatures = new List<Guardian_Roguelike.World.Creatures.CreatureBase>();
            if (RandGen == null)
            {
                RandGen = new Random(System.DateTime.Now.Millisecond);
            }
            DisplayData = new TerrainTile[WIDTH, HEIGHT];
            //WalkabilityData = new bool[WIDTH, HEIGHT];
            //SeeThroughData = new bool[WIDTH, HEIGHT];            

            Generate();

        }

        private void Generate()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    DisplayData[x, y] = TerrainTile.Create(TerrainTypes.EmptyFloor);
                    if (RandGen.Next(0, 100) > 51)
                    {
                        DisplayData[x, y] = TerrainTile.Create(TerrainTypes.DestructibleWall);
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                char[,] DisplayDataRet = new char[WIDTH, HEIGHT];
                TerrainTile[,] NewDisplayDataRet = new TerrainTile[WIDTH, HEIGHT];

                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        if (DisplayData[x,y].Type == TerrainTypes.DestructibleWall)
                        {
                            if (CountNeighbouringWalls(DisplayData, x, y) >= 4)
                            {
                                NewDisplayDataRet[x, y] = TerrainTile.Create(TerrainTypes.DestructibleWall);
                            }
                            else
                            {
                                NewDisplayDataRet[x, y] = TerrainTile.Create(TerrainTypes.EmptyFloor);
                            }
                        }
                        else
                        {
                            if (CountNeighbouringWalls(DisplayData, x, y) >= 5)
                            {
                                NewDisplayDataRet[x, y] = TerrainTile.Create(TerrainTypes.DestructibleWall);
                            }
                            else
                            {
                                NewDisplayDataRet[x, y] = TerrainTile.Create(TerrainTypes.EmptyFloor);
                            }
                        }
                    }
                }

                Utilities.GeneralMethods.Copy2DArray<TerrainTile>(NewDisplayDataRet, ref DisplayData, WIDTH, HEIGHT);
            }

            //Close off boundaries
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (x == 0 || y == 0 || x == (WIDTH-1) || y == (HEIGHT-1))
                    {
                        DisplayData[x, y] = TerrainTile.Create(TerrainTypes.IndestructibleWall);
                    }
                }
            }

            //Place the stairs to the next level
            int ExitX, ExitY;

            ExitX = RandGen.Next(1, WIDTH-1);
            ExitY = RandGen.Next(1, HEIGHT-1);

            DisplayData[ExitX, ExitY] = TerrainTile.Create(TerrainTypes.ExitPortal);
        }

        public System.Drawing.Point GetFirstWalkable()
        {
            for (int x = 0; x < 90; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    if (CheckWalkable(x, y))
                    {
                        bool occupied = false;
                        foreach (World.Creatures.CreatureBase C in Creatures)
                        {
                            if (C.Position.X == x && C.Position.Y == y)
                            {
                                occupied = true;
                                break;
                            }
                        }
                        if (occupied)
                        {
                            continue;
                        }
                        return new System.Drawing.Point(x, y);
                    }
                }
            }

            throw new Exception("No spaces in map were walkable!");
        }

        private int CountNeighbouringWalls(TerrainTile[,] FieldData, int x, int y)
        {
            int realx, realy,nwcount = 0;

            //Up left
            realx = x - 1;
            realy = y - 1;
            if (realx < 0)
            {
                realx = 89;
            }
            if (realy < 0)
            {
                realy = 29;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            //Up
            realx = x;
            realy = y - 1;
            if (realy < 0)
            {
                realy = 29;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            //Up right
            realx = x + 1;
            realy = y - 1;
            if (realx > 89)
            {
                realx = 0;
            }
            if (realy < 0)
            {
                realy = 29;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            //Left
            realx = x - 1;
            realy = y;
            if (realx < 0)
            {
                realx = 89;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            //Right
            realx = x + 1;
            realy = y;
            if (realx > 89)
            {
                realx = 0;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            //Down Left
            realx = x - 1;
            realy = y + 1;
            if (realx < 0)
            {
                realx = 89;
            }
            if (realy > 29)
            {
                realy = 0;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            //Down
            realx = x;
            realy = y + 1;
            if (realy > 29)
            {
                realy = 0;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            //Down right
            realx = x + 1;
            realy = y + 1;
            if (realx > 89)
            {
                realx = 0;
            }
            if (realy > 29)
            {
                realy = 0;
            }
            if (FieldData[realx, realy].Type == TerrainTypes.DestructibleWall)
            {
                nwcount++;
            }

            return nwcount;

        }

        public void RenderToConsole(libtcodWrapper.Console Target)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Target.ForegroundColor = DisplayData[x, y].DrawColor;
                    Target.PutChar(x, y, DisplayData[x, y].CharRepresentation);
                }
            }

            foreach (World.Creatures.CreatureBase C in Creatures)
            {
                Target.ForegroundColor = C.DrawColor;
                Target.PutChar(C.Position.X, C.Position.Y, C.CharRepresentation);
            }
        }

        /// <summary>
        /// Returns wether the square is walkable or not.
        /// </summary>
        /// <param name="x">X part of the LocalPosition</param>
        /// <param name="y">Y part of the LocalPosition</param>
        /// <returns>True if walkable.</returns>
        public bool CheckWalkable(int x, int y)
        {
            return DisplayData[x,y].Walkable;
        }

        /// <summary>
        /// Returns wether the square is walkable or not.
        /// </summary>
        /// <param name="p">The LocalPosition</param>
        /// <returns>True if walkable.</returns>
        public bool CheckWalkable(System.Drawing.Point p)
        {
            return CheckWalkable(p.X, p.Y);
        }

        public bool CheckSeeThrough(int x, int y)
        {
            return DisplayData[x,y].Seethrough;
        }

        public bool CheckSeeThrough(System.Drawing.Point p)
        {
            return CheckSeeThrough(p.X, p.Y);
        }

        public void DEBUG_Savemap(string Filename)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(Filename);
            for (int y = 0; y < 30; y++)
            {
                for (int x = 0; x < 90; x++)
                {
                    sw.Write(DisplayData[x, y].CharRepresentation);
                }
                sw.Write('\n');
            }
            sw.Close();
        }

        public void DestroyTile(int x, int y)
        {
            if (DisplayData[x, y].Destructible)
            {
                DisplayData[x, y] = TerrainTile.Create(TerrainTypes.EmptyFloor);
            }
            else
            {
                Utilities.MessageLog ml = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];

                ml.AddMsg("You swing with all your might, but the pick bounces off the wall without doing damage.");
                //TODO: Do damage to the pick.
            }
        }
    }

    public enum TerrainTypes {IndestructibleWall, DestructibleWall,ExitPortal,EmptyFloor,Water,Lava,Fog};
    public struct TerrainTile
    {
        public TerrainTypes Type;
        public char CharRepresentation;
        public libtcodWrapper.Color DrawColor;
        public bool Destructible;
        public bool Walkable;
        public bool Seethrough;

        public TerrainTile(char c, libtcodWrapper.Color col, bool d,bool w,bool s,TerrainTypes t)
        {
            CharRepresentation = c;
            DrawColor = col;
            Destructible = d;
            Walkable = w;
            Seethrough = s;
            Type = t;
        }

        public static TerrainTile Create(TerrainTypes Type)
        {
            switch (Type)
            {
                case(TerrainTypes.IndestructibleWall):
                    return new TerrainTile('#', libtcodWrapper.ColorPresets.Gray, false, false, false,Type);
                    break;
                case(TerrainTypes.DestructibleWall):
                    return new TerrainTile('#', libtcodWrapper.ColorPresets.GhostWhite, true, false, false, Type);
                    break;
                case(TerrainTypes.ExitPortal):
                    return new TerrainTile('>', libtcodWrapper.ColorPresets.GhostWhite, false, true, true, Type);
                    break;
                case(TerrainTypes.EmptyFloor):
                    return new TerrainTile('.', libtcodWrapper.ColorPresets.GhostWhite, false, true, true, Type);
                    break;
                case(TerrainTypes.Water):
                    return new TerrainTile('=', libtcodWrapper.ColorPresets.Blue, false, true, true, Type);
                    break;
                case(TerrainTypes.Lava):
                    return new TerrainTile('=', libtcodWrapper.ColorPresets.Red, false, true, true, Type);
                    break;
                case(TerrainTypes.Fog):
                    return new TerrainTile('~', libtcodWrapper.ColorPresets.Gray, false, true, false, Type);
                    break;
                default:
                    return new TerrainTile('?', libtcodWrapper.ColorPresets.Pink, false, true, true, Type);
                    break;
            }
        }

    }
}
