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
    }
}
