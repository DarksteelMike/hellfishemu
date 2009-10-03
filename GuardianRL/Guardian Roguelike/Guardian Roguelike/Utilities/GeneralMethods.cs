using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace Guardian_Roguelike.Utilities
{
    abstract class GeneralMethods
    {
        private static Random RndGen = new Random();

        public static System.Drawing.Point AddPoints(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            return new System.Drawing.Point(p1.X + p2.X, p1.Y + p2.Y);
        }       
        public static System.Drawing.Point SubtractPoints(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            return new System.Drawing.Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static int Distance(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            return (int)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static Bitmap LoadBitmapFromAssembly(string ImageName)
        {
            Assembly ExecAssembly = Assembly.GetExecutingAssembly();

            Stream ImgStream = ExecAssembly.GetManifestResourceStream("Guardian_Roguelike." + ImageName);

            return new Bitmap(ImgStream);
        }

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

        public static bool CompareColors(System.Drawing.Color C1, System.Drawing.Color C2)
        {
            return (C1.A == C2.A && C1.R == C2.R && C1.G == C2.G && C1.B == C2.B);
        }

        public static string ColorToBodypart(System.Drawing.Color C)
        {
            if(CompareColors(C,System.Drawing.Color.FromArgb(255, 0, 0)))
            {
                return "Torso";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(0, 255, 0)))
            {
                return "Head";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(0, 0, 255)))
            {
                return "Left Arm";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(255, 255, 0)))
            {
                return "Right Arm";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(255, 0, 255)))
            {
                return "Left Leg";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(0, 255, 255)))
            {
                return "Right Leg";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(255, 255, 255)))
            {
                return "Left Hand";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(0, 0, 0)))
            {
                return "Right Hand";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(128, 0, 0)))
            {
                return "Left Foot";
            }
            if (CompareColors(C, System.Drawing.Color.FromArgb(0, 128, 0)))
            {
                return "Right Foot";
            }

            return "ummm... epidermis? {" + C.A + "," + C.R + "," + C.G + "," + C.B + "}";
        }

        public static List<System.Drawing.Point> CalcBresenhamLine(System.Drawing.Point Start, System.Drawing.Point End)
        {
            List<System.Drawing.Point> LinePoints = new List<System.Drawing.Point>();
            int deltaX, deltaY, error, deltaerror, xStep, yStep, x, y,tmpx,tmpy;
            System.Drawing.Point WStart, WEnd;
            bool Steep;

            //Specialcase vertical and horizontal, the real algorithm should handle this. Dunno why this implementation doesn't.
            if (Start.X == End.X) //Vertical
            {
                if (Start.Y > End.Y) //Up
                {
                    for (int y1 = Start.Y; y1 >= End.Y; y1--)
                    {
                        LinePoints.Add(new System.Drawing.Point(Start.X, y1));
                    }
                }
                else if (Start.Y < End.Y) //Down
                {
                    for (int y1 = Start.Y; y1 <= End.Y; y1++)
                    {
                        LinePoints.Add(new System.Drawing.Point(Start.X, y1));
                    }
                }
                else //Start == End!
                {
                    LinePoints.Add(Start);
                }

                return LinePoints;
            }
            if (Start.Y == End.Y) //Horizontal
            {
                if (Start.X > End.X) //Left
                {
                    for (int x1 = Start.X; x1 >= End.X; x1--)
                    {
                        LinePoints.Add(new System.Drawing.Point(x1, Start.Y));
                    }
                }
                else //Right
                {
                    for (int x1 = Start.X; x1 <= End.X; x1++)
                    {
                        LinePoints.Add(new System.Drawing.Point(x1, Start.Y));
                    }
                }

                return LinePoints;
            }

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
