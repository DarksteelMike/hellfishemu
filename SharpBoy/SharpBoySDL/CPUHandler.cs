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
        public const int FLAG_Z = 128;
        public const int FLAG_N = 64;
        public const int FLAG_H = 32;
        public const int FLAG_C = 16;

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
            ProgramCounter = 0x00; //0x100;
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
            byte Opcode = Memory.ReadByte(ProgramCounter);
            int CyclesUsed = 0;

            switch (Opcode)
            {
                case(0x00): //NOP                    
                    CyclesUsed = 1;
                    break;
                case(0x01): //LD BC,nn
                    BC.Word = (ushort)((Memory.ReadByte(++ProgramCounter) << 8) | Memory.ReadByte(++ProgramCounter));
                    CyclesUsed = 3;
                    break;
                case(0x02): //LD (BC),nn
                    ushort tmp = Memory.ReadWord(++ProgramCounter);
                    ProgramCounter++;
                    Memory.WriteWord(BC.Word, tmp);
                    CyclesUsed = 2;
                    break;
                case(0x03): //INC BC
                    if (BC.Word == ushort.MaxValue)
                    {
                        AF.Low |= FLAG_Z;
                        AF.Low |= FLAG_C;
                        BC.Word = 0;
                    }
                    else
                    {
                        BC.Word++;
                    }
                    CyclesUsed = 2;
                    break;
                case(0x04): //INC B
                    if (BC.High == byte.MaxValue)
                    {
                        AF.Low |= FLAG_Z;
                        AF.Low |= FLAG_C;
                        BC.High = 0;
                    }
                    else
                    {
                        BC.High++;
                    }
                    CyclesUsed = 1;
                    break;
                case(0x05): //DEC B
                    AF.Low |= FLAG_N;
                    if (BC.High == 0)
                    {
                        AF.Low |= FLAG_C;
                        BC.High = byte.MaxValue;
                    }
                    else
                    {
                        BC.High--;
                    }
                    CyclesUsed = 1;
                    break;
                case(0x06): //LD B,n
                    BC.High = Memory.ReadByte(++ProgramCounter);
                    if (BC.High == 0)
                    {
                        AF.Low |= FLAG_Z;
                    }
                    CyclesUsed = 2;
                    break;
                case(0x07): //RLC A

                    break;
                    CyclesUsed = 2;
            }
            ProgramCounter++;
            return CyclesUsed;
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
