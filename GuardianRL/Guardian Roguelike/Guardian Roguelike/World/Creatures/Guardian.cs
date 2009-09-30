using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures
{
    
    class Dwarf : CreatureBase
    {
        public Dwarf() : base()
        {
            CharRepresentation = '@';
            DrawColor = libtcodWrapper.ColorPresets.Aqua;
        }
    }
}
