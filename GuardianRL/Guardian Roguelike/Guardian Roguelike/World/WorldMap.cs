using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World
{
    class WorldMap
    {
        public const int WORLDWIDTH = 90;
        public const int WORLDHEIGHT = 30;

        public Map[,] LocalMaps;

        public WorldMap()
        {
            LocalMaps = new Map[WORLDWIDTH, WORLDHEIGHT];
            for (int x = 0; x < WORLDWIDTH; x++)
            {
                for (int y = 0; y < WORLDHEIGHT; y++)
                {
                    LocalMaps[x, y] = new Map();
                }
            }
        }

        public void RenderToConsole(libtcodWrapper.Console Target)
        {
            for (int x = 0; x < WORLDWIDTH; x++)
            {
                for (int y = 0; y < WORLDHEIGHT; y++)
                {
                    Target.PutChar(x, y, LocalMaps[x,y].GetWorldMapRepresentation());
                }
            }
        }

        public bool CheckWalkable(System.Drawing.Point GlobalPos, System.Drawing.Point LocalPos)
        {
            return LocalMaps[GlobalPos.X, GlobalPos.Y].GetWalkable(LocalPos);
        }

        public void MoveCreature(World.Creatures.CreatureBase Subject,World.Map From, World.Map To)
        {
            From.Creatures.Remove(Subject);
            To.Creatures.Add(Subject);
        }

        public void DEBUG_SaveMap(System.Drawing.Point GlobalPos)
        {
            LocalMaps[GlobalPos.X, GlobalPos.Y].DEBUG_Savemap(GlobalPos.ToString()+".txt");
        }
    }
}
