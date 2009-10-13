using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures.Limbs
{
    class Leg : LimbBase
    {
        public Leg(CreatureBase C, string s, int d)
            : base(C, s, d)
        {
        }

        public override bool Equip(Guardian_Roguelike.World.Items.ItemBase I)
        {
            if (AttachedItems.IsEmpty())
            {
                if (I.Wearable && I.Type == Guardian_Roguelike.World.Items.ItemType.Legging)
                {
                    AttachedItems.AddItem(I);
                    return true;
                }
                //Log msg in else?
            }
            return false;
        }
    }
}
