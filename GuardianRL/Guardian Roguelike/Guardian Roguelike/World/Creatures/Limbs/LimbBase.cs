using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures.Limbs
{
    public abstract class LimbBase
    {
        public int HP;
        public CreatureBase Host;
        public Items.Inventory AttachedItems;
        public string Description;
        public int HitChance;
        public int BleedRate;

        public LimbBase(CreatureBase H,string Desc,int HC)
        {
            Host = H;
            AttachedItems = new Guardian_Roguelike.World.Items.Inventory();
            HP = 100;
            BleedRate = 0;
            Description = Desc;
            HitChance = HC;
        }

        public int AttackWith()
        {
            int Damage = Host.BaseStrength / 2;

            if (!AttachedItems.IsEmpty())
            {
                Damage += AttachedItems.GetBag()[0].Damage;
            }

            return Damage;
        }

        public void RecieveDamage(int Damage)
        {
            int EndDamage = Damage;
            if (!AttachedItems.IsEmpty())
            {
                EndDamage -= AttachedItems.GetBag()[0].ArmorClass;
            }

            HP -= EndDamage;
            if (HP <= 0)
            {
                HP = 0;
                Host.EnforceLimbDependencies();
            }
        }

        public abstract bool Equip(Items.ItemBase I);

        public Items.ItemBase Unequip()
        {
            if (AttachedItems.IsEmpty())
            {
                return null;
            }
            Items.ItemBase ret = AttachedItems.GetBag()[0];
            AttachedItems.RemoveItem(ret);
            return ret;
        }
    }

    public struct LimbDependency
    {
        public LimbBase TargetLimb;
        public LimbBase RequiredLimb;

        public LimbDependency(LimbBase T, LimbBase R)
        {
            TargetLimb = T;
            RequiredLimb = R;
        }
    }
}
