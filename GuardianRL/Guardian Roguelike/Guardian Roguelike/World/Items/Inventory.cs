using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Items
{
    public class Inventory
    {
        private List<ItemBase> Bag;

        private HeadwearBase WornHelmet;

        public Inventory()
        {
            WornHelmet = null;

            Bag = new List<ItemBase>();
        }

        public bool IsEmpty()
        {
            return Bag.Count == 0 && WornHelmet == null; //Will probably change if limbs are implemented.
        }

        public List<ItemBase> GetBag()
        {
            return Bag;
        }

        public void AddItem(ItemBase Item)
        {
            Bag.Add(Item);
        }

        public void RemoveItem(ItemBase Item)
        {
            Bag.Remove(Item);
        }

    }
}
