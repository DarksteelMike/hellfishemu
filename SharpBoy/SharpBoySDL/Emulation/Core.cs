using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace SharpBoy.Emulation
{
    public class Core
    {
        public Memory MyMemory;
        public CPU MyCPU;
        public Display MyDisplay;

        private Keys[] DesignatedKeys;

        public Core(SdlDotNet.Windows.SurfaceControl SC)
        {
            MyMemory = new Memory(this);
            MyCPU = new CPU(this,false);
            MyDisplay = new Display(this,SC);
            DesignatedKeys = new Keys[] { Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.A, Keys.S, Keys.R, Keys.T };
        }

        public void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            for (byte i = 0; i < DesignatedKeys.Length; i++)
            {
                if (DesignatedKeys[i] == e.KeyCode)
                {
                    if (i < 4 && Utility.IsBitSet(MyMemory.Read(0xFF00), 5)) //Directions
                    {
                        Utility.SetBit(ref MyMemory.GameBoyRAM[0xFF00], i, SBMode.On);
                        return;
                    }
                    else if (i >= 4 && Utility.IsBitSet(MyMemory.Read(0xFF00), 4)) //Buttons
                    {
                        Utility.SetBit(ref MyMemory.GameBoyRAM[0xFF00], (byte)(i-4), SBMode.On);
                        return;
                    }
                }
            }
        }

        public void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            for (byte i = 0; i < DesignatedKeys.Length; i++)
            {
                if (DesignatedKeys[i] == e.KeyCode)
                {
                    if (i < 4 && Utility.IsBitSet(MyMemory.Read(0xFF00), 5)) //Directions
                    {
                        Utility.SetBit(ref MyMemory.GameBoyRAM[0xFF00], i, SBMode.Off);
                        return;
                    }
                    else if (i >= 4 && Utility.IsBitSet(MyMemory.Read(0xFF00), 4)) //Buttons
                    {
                        Utility.SetBit(ref MyMemory.GameBoyRAM[0xFF00], (byte)(i - 4), SBMode.Off);
                        return;
                    }
                }
            }
        }

        public void LoadROM(byte[] R)
        {
            MyMemory.LoadROM(R);
        }


        public void Reset()
        {
            MyCPU.Reset();
            MyMemory.Reset();
        }
    }
}