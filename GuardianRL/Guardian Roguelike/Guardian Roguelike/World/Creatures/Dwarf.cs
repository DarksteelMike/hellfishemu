using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.World.Creatures
{   
    class Dwarf : CreatureBase
    {
        public Dwarf() : base()
        {
            CharRepresentation = 'D';
            DrawColor = libtcodWrapper.ColorPresets.Aqua;
            this.Type = CreatureTypes.Dwarf;
            HitAreas = Utilities.GeneralMethods.LoadBitmapFromAssembly("HAMaps.HitArea - Dwarf.bmp");
        }
    }
}
