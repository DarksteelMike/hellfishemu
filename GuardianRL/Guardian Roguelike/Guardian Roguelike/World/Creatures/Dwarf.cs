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
            HitAreas = new System.Drawing.Bitmap("HAMaps\\HitArea - Dwarf.bmp");
        }

        public override void Generate()
        {


            BaseAim = BaseEnergy = BaseSpeed = BaseStrength = BaseVigor = 5;
            #region Add variance to stats
            if (RndGen.Next(0, 100) < 10)
            {
                BaseAim--;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseAim++;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseEnergy--;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseEnergy++;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseSpeed--;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseSpeed++;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseStrength--;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseStrength++;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseVigor--;
            }
            if (RndGen.Next(0, 100) < 10)
            {
                BaseVigor++;
            }
            #endregion
            HP = MaxHP = (int)(((double)BaseVigor / (double)10) * (double)500);
            Log = (Utilities.MessageLog)Utilities.InterStateResources.Instance.Resources["Game_MessageLog"];
            Level = (World.Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            InventoryHandler = new Guardian_Roguelike.World.Items.Inventory();

            //TODO: Find out if you can use Dwarf Fortress' language files to generate names
            //Until then, "Gear Inkmoist" it is! :P
            FirstName = "Olon";
            LastName = "Likotidash";
        }
    }
}
