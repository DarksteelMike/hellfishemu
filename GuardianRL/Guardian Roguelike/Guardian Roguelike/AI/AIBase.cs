using System;
using System.Collections.Generic;
using System.Text;
using Guardian_Roguelike.World.Creatures;

namespace Guardian_Roguelike.AI
{
    public abstract class AIBase
    {
        protected CreatureBase LinkedCreature;
        protected Random RndGen;
        protected libtcodWrapper.TCODFov FOVHandler;
        protected libtcodWrapper.TCODPathFinding PathFinder;

        public AIBase(CreatureBase C)
        {
            LinkedCreature = C;
            RndGen = new Random();
            FOVHandler = (libtcodWrapper.TCODFov)Utilities.InterStateResources.Instance.Resources["Game_FOVHandler"];
            PathFinder = (libtcodWrapper.TCODPathFinding)Utilities.InterStateResources.Instance.Resources["Game_PathFinder"];
        }

        public abstract void TakeTurn();

        protected List<System.Drawing.Point> ComputePath(System.Drawing.Point From, System.Drawing.Point To)
        {
            return ComputePath(From.X, From.Y, To.X, To.Y);
        }

        protected List<System.Drawing.Point> ComputePath(int FromX, int FromY, int ToX, int ToY)
        {
            List<System.Drawing.Point> Ret = new List<System.Drawing.Point>();
            PathFinder.ComputePath(FromX, FromY, ToX, ToY);
            for (int i = 0; i < PathFinder.GetPathSize(); i++)
            {
                int x, y;
                PathFinder.GetPointOnPath(i, out x, out y);
                Ret.Add(new System.Drawing.Point(x, y));
            }

            return Ret;

        }
    }
}
