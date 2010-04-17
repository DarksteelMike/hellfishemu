using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoy2.Emulation
{
    public class Core
    {
        public Memory MyMemory;
        public CPU MyCPU;

        public Core()
        {
            MyMemory = new Memory(this);
            MyCPU = new CPU(this);
        }

        public void LoadROM(byte[] R)
        {
            MyMemory.LoadROM(R);
        }

        public byte MakeInputByte()
        {
            return 0;
        }

        public void Reset()
        {
            MyCPU.Reset();
            MyMemory.Reset();
        }
    }
}