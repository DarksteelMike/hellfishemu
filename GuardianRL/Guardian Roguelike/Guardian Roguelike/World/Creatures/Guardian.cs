using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures
{
    public enum Deity { Earth, Wind, Fire, Water, Pagan };
    class Guardian : CreatureBase
    {
        public string Name;
        public Deity AttachedDeity;

        public Guardian() : base()
        {
            CharRepresentation = 'G';
        }
    }
}
