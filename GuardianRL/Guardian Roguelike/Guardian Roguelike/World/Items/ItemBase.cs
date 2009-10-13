using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Items
{
    public enum ItemType { Headwear,BodyArmor,Glove,ShoulderPad,Legging,Boot,Weapon,Shield };
    public abstract class ItemBase
    {
        public abstract bool Wearable
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
        public abstract bool Wieldable
        {
            get;
        }

        public abstract ItemType Type
        {
            get;
        }

        public int Damage;
        public int ArmorClass;

        public abstract void OnEquip();

        public abstract void OnUnequip();

        public abstract void OnUse();

        public abstract void OnEvoke();

        public abstract void AlterStats(Creatures.CreatureBase Creature);
    }
}
