using System;
using System.Collections.Generic;
using System.Text;
using SdlDotNet.Graphics;
using SdlDotNet.Windows;
using System.Drawing;

namespace SharpBoy.Emulation
{
    public class Display
    {
        private Core MyCore;
        private SurfaceControl OutputControl;
        private Surface InternalSurface;

        public Display(Core C,SurfaceControl SC)
        {
            InternalSurface = new Surface(320, 288, Video.BestBitsPerPixel(320, 288, false));
            MyCore = C;
            OutputControl = SC;
            //Video.SetVideoMode(320, 288, Video.BestBitsPerPixel(320, 288, false), false, false);
        }

        public void DrawAll()
        {
            Point curp = new Point();
            InternalSurface.Lock();
            //Draw background
            for (int x = 0; x < 160; x++)
            {
                for (int y = 0; y < 144; y++)
                {
                    curp.X = x;curp.Y = y;
                    //InternalSurface
                }
            }

            InternalSurface.Unlock();
            OutputControl.Blit(InternalSurface);
            OutputControl.Update();
        }

        public Surface GetTile(int tilenum)
        {
            Surface ret = new Surface(32, 32);
            tilenum *= 16;//16 bytes per tile
            ushort ActualAddress = (ushort)(0x8000 + tilenum);
            byte Lowerbits, Upperbits;

            for (int i = 0; i < 16; i++)
            {
                Lowerbits = MyCore.MyMemory.GameBoyRAM[ActualAddress + i];
                Upperbits = MyCore.MyMemory.GameBoyRAM[ActualAddress + (++i)];
            }

            return ret;
        }
    }
}
