using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    class DeathData
    {
        public World.Creatures.CreatureBase Player;
        public World.Creatures.CreatureBase Killer;
        public int TurnsSurvived;
        public int LevelsDescended;

        public DeathData(World.Creatures.CreatureBase p, World.Creatures.CreatureBase k, int t, int l)
        {
            Player = p;
            Killer = k;
            TurnsSurvived = t;
            LevelsDescended = l;

        }
    }
}
