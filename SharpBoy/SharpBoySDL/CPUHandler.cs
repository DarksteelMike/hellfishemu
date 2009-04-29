using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoySDL
{
    class CPUHandler
    {
        //Constants
        public const int CYCLESPERSECOND = 419430;
        public const int CYCLESPERFRAME = CYCLESPERSECOND / 60;

        //Members
        private Register AF, BC, DE, HL;
        private MemoryHandler Memory;
        private Register StackPointer;
        private ushort ProgramCounter;

        public CPUHandler()
        {
            AF = new Register();
            BC = new Register();
            DE = new Register();
            HL = new Register();
            StackPointer = new Register();

            Memory = new MemoryHandler();
            Reset();
        }

        public void Reset()
        {
            AF.Word = 0x01B0;
            BC.Word = 0x0013;
            DE.Word = 0x00D8;
            HL.Word = 0x014D;
            ProgramCounter = 0x100;
            StackPointer.Word = 0xFFFE;
            Memory.Reset();
        }

        public void LoadROM(byte[] NewROM)
        {
            Memory.LoadROM(NewROM);
        }

        public void RunOneFrame()
        {
            int CyclesRun = 0;

            while (CyclesRun < CYCLESPERFRAME)
            {
                int Cycles = DoOpcode();
                CyclesRun += Cycles;
                UpdateTimers(Cycles);
                UpdateGraphics(Cycles);
            }
        }

        public int DoOpcode()
        {
            return 0;
        }

        public void UpdateTimers(int Cycles)
        {
        }

        public void UpdateGraphics(int Cycles)
        {
        }

        public void DoInterrupts()
        {
        }
    }

    public struct Register
    {
        public byte Low;
        public byte High;

        public ushort Word
        {
            get { return (ushort)((High << 8) | Low); }
            set { Low = (byte)(value & 0x00FF); High = (byte)((value & 0xFF00)>>8); }
        }
    }

}
