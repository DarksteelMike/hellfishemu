using System;
using System.Collections.Generic;
using System.Text;
using Guardian_Roguelike.World.Creatures.Limbs;

namespace Guardian_Roguelike.World.Creatures
{
    class GiantRat : CreatureBase
    {

        public GiantRat()
            : base()
        {
            CharRepresentation = 'r';
            DrawColor = libtcodWrapper.ColorPresets.Brown;
            Type = CreatureTypes.Giant_Rat;

            Head MyHead = new Head(this, "Head", 60);
            Torso MyTorso = new Torso(this, "Torso", 70);
            Foot FrontLeftPaw = new Foot(this, "Front Left Paw", 50);
            Foot FrontRightPaw = new Foot(this, "Front Right Paw", 50);
            Foot RearLeftPaw = new Foot(this, "Rear Left Paw", 35);
            Foot RearRightPaw = new Foot(this, "Rear Right Paw", 35);
            Tail MyTail = new Tail(this, "Tail", 20);

            LimbDependencies.Add(new LimbDependency(MyHead, MyTorso));
            LimbDependencies.Add(new LimbDependency(MyTorso, MyHead));
            LimbDependencies.Add(new LimbDependency(FrontLeftPaw, MyTorso));
            LimbDependencies.Add(new LimbDependency(FrontRightPaw, MyTorso));
            LimbDependencies.Add(new LimbDependency(RearLeftPaw, MyTorso));
            LimbDependencies.Add(new LimbDependency(RearRightPaw, MyTorso));
            LimbDependencies.Add(new LimbDependency(MyTail, MyTorso));

            PreferredLimbAttackOrder.Add(MyHead);
            PreferredLimbAttackOrder.Add(FrontRightPaw);
            PreferredLimbAttackOrder.Add(FrontLeftPaw);
            PreferredLimbAttackOrder.Add(MyTail);

            Limbs.Add(MyTorso);
            Limbs.Add(MyHead);
            Limbs.Add(FrontLeftPaw);
            Limbs.Add(FrontRightPaw);
            Limbs.Add(RearLeftPaw);
            Limbs.Add(RearRightPaw);
            Limbs.Add(MyTail);
        }

        public override void Generate()
        {
            FirstName = LastName = "";
            BaseAim = BaseEnergy = BaseSpeed = BaseStrength = BaseVigor = 1;

            Level = (World.Map)Utilities.InterStateResources.Instance.Resources["Game_CurrentLevel"];
            InventoryHandler = new Guardian_Roguelike.World.Items.Inventory();
        }
    }
}
