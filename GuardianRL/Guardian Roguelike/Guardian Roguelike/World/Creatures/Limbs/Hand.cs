using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures.Limbs
{
    public class Hand : LimbBase
    {

        public Hand(CreatureBase C, string s, int d)
            : base(C, s, d)
        {
        }

        public override bool Equip(Guardian_Roguelike.World.Items.ItemBase I)
        {
            if (AttachedItems.IsEmpty())
            {
                if (I.Wieldable)
                {
                    AttachedItems.AddItem(I);
                    return true;
                }
                //Log message in else?
            }
            return false;
        }
    }
}
