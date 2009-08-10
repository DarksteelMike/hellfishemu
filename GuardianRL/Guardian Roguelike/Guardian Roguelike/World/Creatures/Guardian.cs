using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures
{
    
    class Guardian : CreatureBase
    {
        public string Name;
        public Deity AttachedDeity;

        public Guardian() : base()
        {
            CharRepresentation = 'G';
        }

        public override void AI()
        {
            SortedList<float, CreatureBase> FearLoveList = new SortedList<float,CreatureBase>();
            SortedList<float, CreatureBase> HateList = new SortedList<float, CreatureBase>();
            foreach (CreatureBase C in Level.Creatures)
            {
                int Distance = Math.Max(Math.Abs(Position.X-C.Position.X),Math.Abs(Position.Y-C.Position.Y));
                if (!CanSeeCell(C.Position))
                {
                    //Can't see creature, continue.
                    continue;
                }
                float FearLove;
                float Hate;

                Hate = 1000 / (float)Distance;
                if (C.Faction != Faction)
                {
                    FearLove = (-1000 * (C.PercievedStrength / PercievedStrength) / (float)Distance);
                    
                }
                else
                {
                    FearLove = (1000 * (C.PercievedStrength / PercievedStrength) / (float)Distance);
                }
                FearLoveList.Add(FearLove, C);
                HateList.Add(Hate, C);

            }
        }
    }
}
