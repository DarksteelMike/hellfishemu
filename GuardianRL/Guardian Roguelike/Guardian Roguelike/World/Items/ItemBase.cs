using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Items
{
    abstract class ItemBase
    {
        public abstract bool Equippable
        {
            get;
        }
        public abstract bool Useable
        {
            get;
        }
        public abstract bool Evokeable
        {
            get;
        }

        public abstract void OnEquip();

        public abstract void OnUnequip();

        public abstract void OnUse();

        public abstract void OnEvoke();

        public abstract void AlterStats(Creatures.CreatureBase Creature);
    }
}
