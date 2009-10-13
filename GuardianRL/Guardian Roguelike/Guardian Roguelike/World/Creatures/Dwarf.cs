using System;
using System.Collections.Generic;
using System.Text;
using Guardian_Roguelike.World.Creatures.Limbs;

namespace Guardian_Roguelike.World.Creatures
{   
    class Dwarf : CreatureBase
    {
        public Dwarf() : base()
        {
            CharRepresentation = 'D';
            DrawColor = libtcodWrapper.ColorPresets.Aqua;
            this.Type = CreatureTypes.Dwarf;

            Head MyHead = new Head(this,"Head",40);
            Foot LeftFoot = new Foot(this,"Left Foot",20);
            Foot RightFoot = new Foot(this, "Right Foot", 25);
            Leg LeftLeg = new Leg(this, "Left Leg", 75);
            Leg RightLeg = new Leg(this, "Right Leg", 75);
            Torso MyTorso = new Torso(this, "Torso",70);
            Arm LeftArm = new Arm(this, "Left Arm",75);
            Arm RightArm = new Arm(this, "Right Arm", 75);
            Hand LeftHand = new Hand(this, "Left Hand", 20);
            Hand RightHand = new Hand(this, "Right Hand", 25);

            LimbDependencies.Add(new LimbDependency(LeftHand, LeftArm));
            LimbDependencies.Add(new LimbDependency(RightHand, RightArm));
            LimbDependencies.Add(new LimbDependency(LeftFoot, LeftLeg));
            LimbDependencies.Add(new LimbDependency(RightFoot, RightLeg));
            LimbDependencies.Add(new LimbDependency(MyHead, MyTorso));
            LimbDependencies.Add(new LimbDependency(LeftLeg, MyTorso));
            LimbDependencies.Add(new LimbDependency(RightLeg, MyTorso));
            LimbDependencies.Add(new LimbDependency(LeftArm, MyTorso));
            LimbDependencies.Add(new LimbDependency(RightArm, MyTorso));
            LimbDependencies.Add(new LimbDependency(MyTorso, MyHead));

            //TODO: Enforce dependencies!

            Limbs.Add(MyTorso);
            Limbs.Add(MyHead);            
            Limbs.Add(LeftArm);
            Limbs.Add(RightArm);
            Limbs.Add(LeftHand);
            Limbs.Add(RightHand);
            Limbs.Add(LeftLeg);
            Limbs.Add(RightLeg);
            Limbs.Add(LeftFoot);
            Limbs.Add(RightFoot);

            PreferredLimbAttackOrder.Add(RightHand);
            PreferredLimbAttackOrder.Add(LeftHand);
            PreferredLimbAttackOrder.Add(MyHead);
            PreferredLimbAttackOrder.Add(RightArm);
            PreferredLimbAttackOrder.Add(LeftArm);
            PreferredLimbAttackOrder.Add(RightFoot);
            PreferredLimbAttackOrder.Add(LeftFoot);
            PreferredLimbAttackOrder.Add(RightLeg);
            PreferredLimbAttackOrder.Add(LeftLeg);
            PreferredLimbAttackOrder.Add(MyTorso);

        }

        public override void Generate()
        {
            BaseAim = BaseEnergy = BaseSpeed = BaseStrength = BaseVigor = 2;
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
            
            Level = (World.Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            InventoryHandler = new Guardian_Roguelike.World.Items.Inventory();

            //TODO: Find out if you can use Dwarf Fortress' language files to generate names
            //Until then, "Gear Inkmoist" it is! :P
            FirstName = "Olon";
            LastName = "Likotidash";
        }
    }
}
