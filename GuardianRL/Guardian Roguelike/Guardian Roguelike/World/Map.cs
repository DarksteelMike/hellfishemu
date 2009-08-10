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
        public char[,] DisplayData;
        public TerrainTile[,] NewDisplayData;
        private bool[,] WalkabilityData;
        private bool[,] SeeThroughData;

        public List<Creatures.CreatureBase> Creatures;

        public Map()
        {            
            Creatures = new List<Guardian_Roguelike.World.Creatures.CreatureBase>();
            if (RandGen == null)
            {
                RandGen = new Random(System.DateTime.Now.Millisecond);
            }
            DisplayData = new char[WIDTH, HEIGHT];
            NewDisplayData = new TerrainTile[WIDTH, HEIGHT];
            WalkabilityData = new bool[WIDTH, HEIGHT];
            SeeThroughData = new bool[WIDTH, HEIGHT];
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    DisplayData[x, y] = '.';
                    WalkabilityData[x, y] = true;
                    SeeThroughData[x, y] = true;
                    if (RandGen.Next(0, 100) > 50)
                    {
                        DisplayData[x, y] = '=';
                        WalkabilityData[x, y] = false;
                        SeeThroughData[x, y] = false;
                    }
                }
            }

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    NewDisplayData[x, y] = TerrainTile.Create(TerrainTypes.EmptyFloor);
                    if (RandGen.Next(0, 100) > 50)
                    {
                        NewDisplayData[x, y] = TerrainTile.Create(TerrainTypes.DestructibleWall);
                    }
                }
            }

            Generate(5);

        }

        private void Generate(int iters)
        {
            for (int i = 0; i < iters; i++)
            {
                char[,] DisplayDataRet = new char[WIDTH, HEIGHT];
                TerrainTile[,] NewDisplayDataRet = new TerrainTile[WIDTH, HEIGHT];

                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        if (NewDisplayData[x,y].Type == TerrainTypes.DestructibleWall)
                        {
                            if (CountNeighbouringWalls(NewDisplayData, x, y) >= 4)
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
                            if (CountNeighbouringWalls(NewDisplayData, x, y) >= 5)
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

                Utilities.GeneralMethods.Copy2DArray<TerrainTile>(NewDisplayDataRet, ref NewDisplayData, WIDTH, HEIGHT);
            }

            //Close off boundaries
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (x == 0 || y == 0 || x == (WIDTH-1) || y == (HEIGHT-1))
                    {
                        NewDisplayData[x, y] = TerrainTile.Create(TerrainTypes.IndestructibleWall);
                    }
                }
            }
        }

        public System.Drawing.Point GetFirstWalkable()
        {
            for (int x = 0; x < 90; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    if (CheckWalkable(x, y))
                    {
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
                    Target.ForegroundColor = NewDisplayData[x, y].DrawColor;
                    Target.PutChar(x, y, NewDisplayData[x, y].CharRepresentation);
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
            return NewDisplayData[x,y].Walkable;
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
            return NewDisplayData[x,y].Seethrough;
        }

        public bool CheckSeeThrough(System.Drawing.Point p)
        {
            return CheckSeeThrough(p.X, p.Y);
        }

        /// <summary>
        /// Returns the most common tile on the map, for use in the world map.
        /// </summary>
        /// <returns>A char, the most common tile.</returns>
        public char GetWorldMapRepresentation()
        {
            Dictionary<char, int> Fields = new Dictionary<char,int>();
            foreach (char c in DisplayData)
            {
                if (Fields.ContainsKey(c))
                {
                    Fields[c]++;
                }
                else
                {
                    Fields[c] = 1;
                }
            }
            
            int imax;
            char cmax;
            imax = 0; cmax = ' ';
            foreach (char c in Fields.Keys)
            {
                if (Fields[c] > imax)
                {
                    imax = Fields[c];
                    cmax = c;
                }
            }

            return cmax;
        }

        public void DEBUG_Savemap(string Filename)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(Filename);
            for (int y = 0; y < 30; y++)
            {
                for (int x = 0; x < 90; x++)
                {
                    sw.Write(DisplayData[x, y]);
                }
                sw.Write('\n');
            }
            sw.Close();
        }
    }

    public enum TerrainTypes {IndestructibleWall, DestructibleWall,EmptyFloor,Water,Lava};
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
                    return new TerrainTile('=', libtcodWrapper.ColorPresets.Gray, false,false,false,Type);
                    break;
                case(TerrainTypes.DestructibleWall):
                    return new TerrainTile('=', libtcodWrapper.ColorPresets.GhostWhite, true, false, false, Type);
                    break;
                case(TerrainTypes.EmptyFloor):
                    return new TerrainTile('.', libtcodWrapper.ColorPresets.GhostWhite, false, true, true, Type);
                    break;
                case(TerrainTypes.Water):
                    return new TerrainTile('#', libtcodWrapper.ColorPresets.Blue, false, true, true, Type);
                    break;
                case(TerrainTypes.Lava):
                    return new TerrainTile('#', libtcodWrapper.ColorPresets.Red, false, true, true, Type);
                    break;
                default:
                    return new TerrainTile('?', libtcodWrapper.ColorPresets.Pink, false, true, true, Type);
                    break;
            }
        }

    }
}
