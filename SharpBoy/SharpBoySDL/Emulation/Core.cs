using System;
using System.Collections.Generic;
using System.Text;
using SdlDotNet.Input;

namespace SharpBoy.Emulation
{
    public class Core
    {
        public Memory MyMemory;
        public CPU MyCPU;
        public Display MyDisplay;

        private Key[] DesignatedKeys;

        public Core(SdlDotNet.Windows.SurfaceControl SC)
        {
            MyMemory = new Memory(this);
            MyCPU = new CPU(this,false);
            MyDisplay = new Display(this,SC);
            SdlDotNet.Core.Events.KeyboardDown += new EventHandler<KeyboardEventArgs>(Events_KeyboardDown);
            DesignatedKeys = new Key[] { Key.RightArrow, Key.LeftArrow, Key.UpArrow, Key.DownArrow, Key.A, Key.S, Key.R, Key.T };
        }

        void Events_KeyboardDown(object sender, KeyboardEventArgs e)
        {
            foreach (Key k in DesignatedKeys)
            {
                if (k == e.Key)
                {
                    System.Windows.Forms.MessageBox.Show("AM I INTERRUPTING YOU??");
                    Utility.SetBit(ref MyMemory.GameBoyRAM[0xFF0F], 4, SBMode.On); //Request joypad interrupt
                    return;
                }
            }            
        }

        public void LoadROM(byte[] R)
        {
            MyMemory.LoadROM(R);
        }

        public byte MakeInputByte()
        {
            int res = 0;
            if (Utility.IsBitSet(MyMemory.Read(0xFF00), 4)) //Directions
            {
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[0]) ? 0 : 1; //Right
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[1]) ? 0 : 2; //Left
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[2]) ? 0 : 4; //Up
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[3]) ? 0 : 8; //Down
            }
            else if (Utility.IsBitSet(MyMemory.Read(0xFF00), 5)) //Buttons
            {
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[4]) ? 0 : 1; //A
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[5]) ? 0 : 2; //B
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[6]) ? 0 : 4; //Select
                res |= SdlDotNet.Input.Keyboard.IsKeyPressed(DesignatedKeys[7]) ? 0 : 8; //Start
            }
            return (byte)res;
        }

        public void Reset()
        {
            MyCPU.Reset();
            MyMemory.Reset();
        }
    }
}