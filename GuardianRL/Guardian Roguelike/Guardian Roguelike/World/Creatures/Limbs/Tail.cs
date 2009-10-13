using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures.Limbs
{
    class Tail : LimbBase
    {
        public Tail(CreatureBase C, string s, int d)
            : base(C, s, d)
        {
        }

        public override bool Equip(Guardian_Roguelike.World.Items.ItemBase I)
        {
            return false;
        }
    }
}
