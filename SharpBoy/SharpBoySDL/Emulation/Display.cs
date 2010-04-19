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
        private Color[] Palette;

        public Display(Core C, SurfaceControl SC)
        {
            InternalSurface = new Surface(320, 288, Video.BestBitsPerPixel(320, 288, false));
            MyCore = C;
            OutputControl = SC;
            Palette = new Color[] { Color.White, Color.LightGray, Color.DarkGray, Color.Black };
            //Video.SetVideoMode(320, 288, Video.BestBitsPerPixel(320, 288, false), false, false); 
        }

        public void DrawAll()
        {
            Point curp = new Point();
            Color ColorToUse;

            if (!Utility.IsBitSet(MyCore.MyMemory.GameBoyRAM[0xFF40], 7)) //LCD Disabled
            {
                InternalSurface.Fill(Color.White);
            }
            else
            {
                //Draw background if applicable
                if (Utility.IsBitSet(MyCore.MyMemory.GameBoyRAM[0xFF40], 0)) //BG Display
                {
                    for (int x = 0; x < 160; x++)
                    {
                        for (int y = 0; y < 144; y++)
                        {
                            curp.X = x; curp.Y = y;
                            //InternalSurface 
                        }
                    }
                }
                else
                {
                    InternalSurface.Fill(Color.White);
                }
            }
            InternalSurface.Update();
            OutputControl.Blit(InternalSurface);
            OutputControl.Update();
        }

        public Surface GetTile(int tilenum) 
         { 
	        Color ColorToUse;
            Surface ret = new Surface(32, 32); 
            tilenum *= 16;//16 bytes per tile 
            ushort ActualAddress = (ushort)(0x8000 + tilenum); 
            byte LowerBits, UpperBits, finalpalnum; 
  
            for (byte y = 0; y < 16; y++) //Each loop = 1 line
            { 
                LowerBits = MyCore.MyMemory.GameBoyRAM[ActualAddress + y]; 
                UpperBits = MyCore.MyMemory.GameBoyRAM[ActualAddress + (++y)];

	            for(byte x = 7;x >= 0;x--)
	            {
			        finalpalnum = 0;
	        		if(Utility.IsBitSet(LowerBits,x))
			        {
				        finalpalnum |= 1;
			        }
			        if(Utility.IsBitSet(UpperBits,x))
			        {
			        	finalpalnum |= 2;
			        }
			        ColorToUse = Palette[GetPaletteNum(MyCore.MyMemory.GameBoyRAM[0xFF47],finalpalnum)];
		        	//Howeverthefuckyoudrawarectangle(X x,Y y,Width 2,Height 2,Color ColorToUse);
	            }
             } 
  
             return ret; 
         }

        public byte GetPaletteNum(byte ioport, byte num)
        {
            byte[] Getpalval = new byte[] { 3, 12, 48, 192 };
            return (byte)((ioport & Getpalval[num]) >> num * 2);
        }
    }
}