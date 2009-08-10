using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    abstract class GeneralMethods
    {
        private static Random RndGen = new Random();

        public static void Copy2DArray<TArrType>(TArrType[,] From, ref TArrType[,] To,int W,int H)
        {
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    To[x, y] = From[x, y];
                }
            }
        }

        public static List<System.Drawing.Point> CalcBresenhamLine(System.Drawing.Point Start, System.Drawing.Point End)
        {
            List<System.Drawing.Point> LinePoints = new List<System.Drawing.Point>();
            int deltaX, deltaY, error, deltaerror, xStep, yStep, x, y,tmpx,tmpy;
            System.Drawing.Point WStart, WEnd;
            bool Steep;

            if (Math.Abs(End.Y - Start.Y) > Math.Abs(End.X - Start.X))
            {
                WStart = End;
                WEnd = Start;
                Steep = true;
            }
            else
            {
                WStart = Start;
                WEnd = End;
                Steep = false;
            }
            
            deltaX = Math.Abs(WEnd.X - WStart.X);
            deltaY = Math.Abs(WEnd.Y - WStart.Y);
            error = 0;
            deltaerror = deltaY;
            x = WStart.X;
            y = WStart.Y;

            if (WStart.Y < WEnd.Y)
            {
                yStep = 1;
            }
            else
            {
                yStep = -1;
            }

            if (WStart.X < WEnd.X)
            {
                xStep = 1;
            }
            else
            {
                xStep = -1;
            }

            while (x != WEnd.X)
            {
                x += xStep;
                error += deltaerror;

                if ((2 * error) > deltaX)
                {
                    y += yStep;
                    error -= deltaX;
                }

                if (Steep)
                {
                    tmpx = y;
                    tmpy = x;
                }
                else
                {
                    tmpx = x;
                    tmpy = y;
                }

                LinePoints.Add(new System.Drawing.Point(tmpx, tmpy));
            }

            return LinePoints;
        }

        public int RollDiceSum(int sides, int number)
        {
            int Res = 0;
            for (int i = 0; i < number; i++)
            {
                Res += RollSingleDice(sides);
            }

            return Res;
        }

        public List<int> RollDiceIndiv(int sides, int number)
        {
            List<int> Res = new List<int>();

            for (int i = 0; i < number; i++)
            {
                Res.Add(RollSingleDice(sides));
            }

            return Res;
        }

        public int RollSingleDice(int Sides)
        {
            return RndGen.Next(1, Sides+1);
        }
    }
}
