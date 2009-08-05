using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World
{
    class Map
    {
        private static Random RandGen;
        public const int WIDTH = 90;
        public const int HEIGHT = 30;
        private char[,] FieldData;

        public List<Creatures.CreatureBase> Creatures;

        //Only generates a blank map so far.
        public Map()
        {
            Creatures = new List<Guardian_Roguelike.World.Creatures.CreatureBase>();
            if (RandGen == null)
            {
                RandGen = new Random(System.DateTime.Now.Millisecond);
            }
            FieldData = new char[WIDTH, HEIGHT];
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    FieldData[x, y] = '.';
                    if (RandGen.Next(0, 100) > 88)
                    {
                        FieldData[x, y] = '?';
                    }
                }
            }
        }

        public void RenderToConsole(libtcodWrapper.Console Target)
        {
            Target.ForegroundColor = libtcodWrapper.ColorPresets.GhostWhite;
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Target.PutChar(x, y, FieldData[x, y]);
                }
            }

            foreach (World.Creatures.CreatureBase C in Creatures)
            {
                Target.ForegroundColor = C.DrawColor;
                Target.PutChar(C.LocalPos.X, C.LocalPos.Y, C.CharRepresentation);
            }
        }

        /// <summary>
        /// Returns wether the square is walkable or not.
        /// </summary>
        /// <param name="x">X part of the LocalPosition</param>
        /// <param name="y">Y part of the LocalPosition</param>
        /// <returns>True if walkable.</returns>
        public bool GetWalkable(int x, int y)
        {
            return (FieldData[x, y] != '=');
        }

        /// <summary>
        /// Returns wether the square is walkable or not.
        /// </summary>
        /// <param name="p">The LocalPosition</param>
        /// <returns>True if walkable.</returns>
        public bool GetWalkable(System.Drawing.Point p)
        {
            return GetWalkable(p.X, p.Y);
        }

        /// <summary>
        /// Returns the most common tile on the map, for use in the world map.
        /// </summary>
        /// <returns>A char, the most common tile.</returns>
        public char GetWorldMapRepresentation()
        {
            Dictionary<char, int> Fields = new Dictionary<char,int>();
            foreach (char c in FieldData)
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
                    sw.Write(FieldData[x, y]);
                }
                sw.Write('\n');
            }
            sw.Close();
        }
    }
}
