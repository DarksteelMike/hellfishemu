using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SharpBoy.Emulation
{
    public class CPU
    {
        //Constants 
        public const int CYCLESPERSECOND = 4194304;
        public const int CYCLESPERFRAME = CYCLESPERSECOND / 60;
        public const byte FLAG_Z = 7;
        public const byte FLAG_N = 6;
        public const byte FLAG_H = 5;
        public const byte FLAG_C = 4;

        private byte[] BIOS;

        public int DIV_Counter;
        public int TIMA_Counter;
        public int HBlank_Start_Counter;
        public int VBlank_Start_Counter;
        public int HBlank_End_Counter;
        public int VBlank_End_Counter;
        public int[] TIMA_Frequencies;
        public int CyclesRunThisFrame;
        public bool InterruptMasterEnable;
        public ushort[] InterruptAddresses;

        public Register AF;
        public Register BC;
        public Register DE;
        public Register HL;
        public Register StackPointer;
        public Register ProgramCounter;

        public Core MyCore;
        public bool EmulateBIOS;

        public CPU(Core C, bool UseBIOS)
        {
            EmulateBIOS = UseBIOS;
            AF = new Register();
            BC = new Register();
            DE = new Register();
            HL = new Register();
            StackPointer = new Register();
            ProgramCounter = new Register();
            MyCore = C;

            using (System.IO.FileStream fs = new System.IO.FileStream(Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\") + 1) + "DMG_ROM.bin", System.IO.FileMode.Open))
            {
                BIOS = new byte[fs.Length];
                fs.Read(BIOS, 0, (int)fs.Length);
                fs.Close();
            }

            TIMA_Frequencies = new int[] { 4096, 262144, 65536, 16384 };
            InterruptAddresses = new ushort[] { 0x40, 0x48, 0x50, 0x58, 0x60 };
            InterruptMasterEnable = true;
        }

        public void Reset()
        {
            AF.High = 0x01;
            AF.Low = 0xB0;
            BC.Word = 0x0013;
            DE.Word = 0x00D8;
            HL.Word = 0x014D;
            StackPointer.Word = 0xFFFE;

            if (EmulateBIOS)
            {
                ProgramCounter.Word = 0;
                Array.Copy(BIOS, 0, MyCore.MyMemory.GameBoyRAM, 0, BIOS.Length);
            }
            else
            {
                ProgramCounter.Word = 0x100;
            }

            DIV_Counter = 256;
            TIMA_Counter = 1024;
            InterruptMasterEnable = true;
        }

        public void DoFrame()
        {
            CyclesRunThisFrame = 0;

            while (CyclesRunThisFrame < CYCLESPERFRAME)
            {
                DoStep();
            }

        }

        public void DoStep()
        {
            int Cycles = DoOpcode();

            UpdateSpecialRegisters(Cycles);
            DoInterrupts(Cycles);

            CyclesRunThisFrame += Cycles;
        }

        public int DoOpcode()
        {
            byte Opcode = MyCore.MyMemory.Read(ProgramCounter.Word);
            sbyte signed8bit;
            int CyclesUsed = 0;
            switch (Opcode)
            {
                case (0x00): //NOP 
                    CyclesUsed = 1;
                    break;
                case (0x01): //LD BC,nn 
                    LD_R16_NN(ref BC, MyCore.MyMemory.ReadWord(++ProgramCounter.Word));
                    CyclesUsed = 3;
                    break;
                case (0x02): //LD (BC),A 
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    MyCore.MyMemory.Write(BC.Word, AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x03): //INC BC 
                    if (BC.Word == 0xFFFF)
                    {
                        BC.Word = 0;
                    }
                    else
                    {
                        BC.Word++;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x04): //INC B 
                    INC_R8(ref BC.High);
                    CyclesUsed = 1;
                    break;
                case (0x05): //DEC B 
                    DEC_R8(ref BC.High);
                    CyclesUsed = 1;
                    break;
                case (0x06): //LD B,n 
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    BC.High = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0x07): //RLC A 
                    RLC_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x08): //LD (nn),SP 
                    AF.Low = 0;
                    MyCore.MyMemory.Write(++ProgramCounter.Word, StackPointer.Low);
                    MyCore.MyMemory.Write(++ProgramCounter.Word, StackPointer.High);
                    CyclesUsed = 5;
                    break;
                case (0x09): //ADD HL,BC 
                    ADD_R16_R16(ref HL, BC);
                    CyclesUsed = 2;
                    break;
                case (0x0A): //LD A,(BC) 
                    AF.Low = 0;
                    AF.High = MyCore.MyMemory.Read(BC.Word);
                    CyclesUsed = 2;
                    break;
                case (0x0B): //DEC BC 
                    if (BC.Word == 0xFFFF)
                    {
                        BC.Word = 0;
                    }
                    else
                    {
                        BC.Word++;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x0C): //INC C 
                    INC_R8(ref BC.Low);
                    CyclesUsed = 1;
                    break;
                case (0x0D): //DEC C 
                    DEC_R8(ref BC.Low);
                    CyclesUsed = 1;
                    break;
                case (0x0E): //LD C,n 
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    BC.Low = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0x0F): //RRC A 
                    RRC_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;

                case (0x10): //STOP 
                    CyclesUsed = 1;
                    break;
                case (0x11): //LD DE,nn 
                    LD_R16_NN(ref DE, MyCore.MyMemory.ReadWord(++ProgramCounter.Word));
                    ++ProgramCounter.Word;
                    CyclesUsed = 3;
                    break;
                case (0x12): //LD (DE),A 
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    MyCore.MyMemory.Write(BC.Word, AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x13): //INC DE 
                    if (BC.Word == 0xFFFF)
                    {
                        BC.Word = 0;
                    }
                    else
                    {
                        BC.Word++;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x14): //INC D 
                    INC_R8(ref DE.High);
                    CyclesUsed = 1;
                    break;
                case (0x15): //DEC D 
                    DEC_R8(ref DE.High);
                    CyclesUsed = 1;
                    break;
                case (0x16): //LD D,n 
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    DE.High = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0x17): //RL A 
                    RL_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x18): //JR n 
                    signed8bit = (sbyte)MyCore.MyMemory.Read(++ProgramCounter.Word);
                    if (signed8bit < 0)
                    {
                        ProgramCounter.Word -= (ushort)Math.Abs(signed8bit);
                    }
                    else
                    {
                        ProgramCounter.Word += (ushort)Math.Abs(signed8bit);
                    }
                    CyclesUsed = 3;
                    break;
                case (0x19): //ADD HL,DE 
                    ADD_R16_R16(ref HL, DE);
                    CyclesUsed = 2;
                    break;
                case (0x1A): //LD A,(DE) 
                    AF.High = MyCore.MyMemory.Read(DE.Word);
                    CyclesUsed = 2;
                    break;
                case (0x1B): //DEC DE 
                    if (DE.Word == 0xFFFF)
                    {
                        DE.Word = 0;
                    }
                    else
                    {
                        DE.Word++;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x1C): //INC E 
                    INC_R8(ref DE.Low);
                    CyclesUsed = 1;
                    break;
                case (0x1D): //DEC E 
                    DEC_R8(ref DE.Low);
                    CyclesUsed = 1;
                    break;
                case (0x1E): //LD E,n 
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    DE.Low = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0x1F): //RR A 
                    RR_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;

                case (0x20): //JR NZ,n 
                    signed8bit = (sbyte)MyCore.MyMemory.Read(++ProgramCounter.Word);
                    if (!Utility.IsBitSet(AF.Low, FLAG_Z))
                    {
                        if (signed8bit < 0)
                        {
                            ProgramCounter.Word -= (ushort)Math.Abs(signed8bit);
                        }
                        else
                        {
                            ProgramCounter.Word += (ushort)Math.Abs(signed8bit);
                        }
                    }
                    CyclesUsed = 3;
                    break;
                case (0x21): //LD HL,nn 
                    LD_R16_NN(ref HL, MyCore.MyMemory.ReadWord(++ProgramCounter.Word));
                    ProgramCounter.Word++;
                    CyclesUsed = 3;
                    break;
                case (0x22): //LDI (HL),A 
                    MyCore.MyMemory.Write(HL.Word, AF.High);
                    INC_R16(ref HL);
                    CyclesUsed = 2;
                    break;
                case (0x23): //INC HL 
                    INC_R16(ref HL);
                    CyclesUsed = 2;
                    break;
                case (0x24): //INC H 
                    INC_R8(ref HL.High);
                    CyclesUsed = 1;
                    break;
                case (0x25): //DEC H 
                    DEC_R8(ref HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0x26): //LD H,n 
                    HL.High = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0x27): //DAA 

                    if (Utility.IsBitSet(AF.Low, FLAG_N))
                    {
                        if ((AF.High & 0x0F) > 0x09 || (AF.Low & 0x20) != 0)
                        {
                            AF.High -= 0x06;
                            if ((AF.High & 0xF0) == 0xF0)
                            {
                                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                            }
                            else
                            {
                                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.Off);
                            }
                        }
                        if ((AF.High & 0xF0) > 0x90 || (AF.High & 0x10) != 0)
                        {
                            AF.High -= 0x60;
                        }
                    }
                    else
                    {
                        if ((AF.High & 0x0F) > 9 || (AF.High & 0x20) != 0)
                        {
                            AF.High += 0x06;
                            if ((AF.High & 0xF0) == 0)
                            {
                                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                            }
                            else
                            {
                                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.Off);
                            }
                        }
                        if ((AF.High & 0xF0) > 0x90 || Utility.IsBitSet(AF.Low, FLAG_C))
                        {
                            AF.High += 0x60;
                        }
                    }
                    if (AF.High == 0)
                    {
                        Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
                    }
                    else
                    {
                        Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    }

                    CyclesUsed = 1;
                    break;
                case (0x28): //JR Z,n 
                    signed8bit = (sbyte)MyCore.MyMemory.Read(++ProgramCounter.Word);
                    if (Utility.IsBitSet(AF.Low, FLAG_Z))
                    {
                        if (signed8bit < 0)
                        {
                            ProgramCounter.Word -= (ushort)Math.Abs(signed8bit);
                        }
                        else
                        {
                            ProgramCounter.Word += (ushort)Math.Abs(signed8bit);
                        }
                    }
                    CyclesUsed = 3;
                    break;
                case (0x29): //ADD HL,HL 
                    ADD_R16_R16(ref HL, HL);
                    CyclesUsed = 2;
                    break;
                case (0x2A): //LDI A,(HL) 
                    AF.High = MyCore.MyMemory.Read(HL.Word);
                    if (HL.Word == 0xFFFF)
                    {
                        HL.Word = 0;
                    }
                    else
                    {
                        HL.Word++;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x2B): //DEC HL 
                    DEC_R16(ref HL);
                    CyclesUsed = 2;
                    break;
                case (0x2C): //INC L 
                    INC_R8(ref HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0x2D): //DEC L 
                    DEC_R8(ref HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0x2E): //LD L,n 
                    HL.Low = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0x2F): //CPL 
                    AF.High = (byte)(~AF.High);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.On);
                    Utility.SetBit(ref AF.Low, FLAG_H, SBMode.On);
                    CyclesUsed = 1;
                    break;

                case (0x30): //JR NC,n 
                    signed8bit = (sbyte)MyCore.MyMemory.Read(++ProgramCounter.Word);
                    if (!Utility.IsBitSet(AF.Low, FLAG_C))
                    {
                        if (signed8bit < 0)
                        {
                            ProgramCounter.Word -= (ushort)Math.Abs(signed8bit);
                        }
                        else
                        {
                            ProgramCounter.Word += (ushort)Math.Abs(signed8bit);
                        }
                    }
                    CyclesUsed = 3;
                    break;
                case (0x31): //LD SP,nn 
                    StackPointer.Word = MyCore.MyMemory.ReadWord(++ProgramCounter.Word);
                    ProgramCounter.Word++;
                    CyclesUsed = 3;
                    break;
                case (0x32): //LDD (HL),A 
                    MyCore.MyMemory.Write(HL.Word, AF.High);
                    if (HL.Word == 0)
                    {
                        HL.Word = 0xFFFF;
                    }
                    else
                    {
                        HL.Word--;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x33): //INC SP 
                    INC_R16(ref StackPointer);
                    CyclesUsed = 2;
                    break;
                case (0x34): //INC (HL) 
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    INC_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 3;
                    break;
                case (0x35): //DEC (HL) 
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.On);
                    DEC_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 3;
                    break;
                case (0x36): //LD (HL),n 
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    MyCore.MyMemory.Write(HL.Word, MyCore.MyMemory.Read(++ProgramCounter.Word));
                    CyclesUsed = 3;
                    break;
                case (0x37): //SCF 
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_H, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                    CyclesUsed = 1;
                    break;
                case (0x38): //JR C,n 
                    signed8bit = (sbyte)MyCore.MyMemory.Read(++ProgramCounter.Word);
                    if (Utility.IsBitSet(AF.Low, FLAG_C))
                    {
                        if (signed8bit < 0)
                        {
                            ProgramCounter.Word -= (ushort)Math.Abs(signed8bit);
                        }
                        else
                        {
                            ProgramCounter.Word += (ushort)Math.Abs(signed8bit);
                        }
                    }
                    CyclesUsed = 3;
                    break;
                case (0x39): //ADD HL,SP 
                    ADD_R16_R16(ref HL, StackPointer);
                    CyclesUsed = 2;
                    break;
                case (0x3A): //LDD A,(HL) 
                    AF.High = MyCore.MyMemory.Read(HL.Word);
                    if (HL.Word == 0)
                    {
                        HL.Word = 0xFFFF;
                    }
                    else
                    {
                        HL.Word--;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x3B): //DEC SP 
                    if (StackPointer.Word == 0) //Yes, I know the Stackpointer shouldn't underflow during regular 
                    {                           //usage, but this way the code is more uniform. Plus, it doesn't hurt. 
                        StackPointer.Word = 0xFFFF;
                    }
                    else
                    {
                        StackPointer.Word--;
                    }
                    CyclesUsed = 2;
                    break;
                case (0x3C): //INC A 
                    INC_R8(ref AF.High);
                    CyclesUsed = 1;
                    break;
                case (0x3D): //DEC A 
                    DEC_R8(ref AF.High);
                    CyclesUsed = 1;
                    break;
                case (0x3E): //LD A,n 
                    AF.High = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0x3F): // CCF 
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_H, SBMode.Off);
                    Utility.SetBit(ref AF.Low, FLAG_C, SBMode.Off);
                    CyclesUsed = 1;
                    break;

                case (0x40): //LD B,B 
                    CyclesUsed = 1;
                    break;
                case (0x41): //LD B,C 
                    BC.High = BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0x42): //LD B,D 
                    BC.High = DE.High;
                    CyclesUsed = 1;
                    break;
                case (0x43): //LD B,E 
                    BC.High = DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0x44): //LD B,H 
                    BC.High = HL.High;
                    CyclesUsed = 1;
                    break;
                case (0x45): //LD B,L 
                    BC.High = HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0x46): //LD B,(HL) 
                    BC.High = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0x47): //LD B,A 
                    BC.High = AF.High;
                    CyclesUsed = 1;
                    break;
                case (0x48): //LD C,B 
                    BC.Low = BC.High;
                    CyclesUsed = 1;
                    break;
                case (0x49): //LD C,C 
                    CyclesUsed = 1;
                    break;
                case (0x4A): //LD C,D 
                    BC.Low = DE.High;
                    CyclesUsed = 1;
                    break;
                case (0x4B): //LD C,E 
                    BC.Low = DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0x4C): //LD C,H 
                    BC.Low = HL.High;
                    CyclesUsed = 1;
                    break;
                case (0x4D): //LD C,L 
                    BC.Low = HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0x4E): //LD C,(HL) 
                    BC.Low = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0x4F): //LD C,A 
                    BC.Low = AF.High;
                    CyclesUsed = 1;
                    break;

                case (0x50): //LD D,B 
                    DE.High = BC.High;
                    CyclesUsed = 1;
                    break;
                case (0x51): //LD D,C 
                    DE.High = BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0x52): //LD D,D 
                    CyclesUsed = 1;
                    break;
                case (0x53): //LD D,E 
                    DE.High = DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0x54): //LD D,H 
                    DE.High = HL.High;
                    CyclesUsed = 1;
                    break;
                case (0x55): //LD D,L 
                    DE.High = HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0x56): //LD D,(HL) 
                    DE.High = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0x57): //LD D,A 
                    DE.High = AF.High;
                    CyclesUsed = 1;
                    break;
                case (0x58): //LD E,B 
                    DE.Low = BC.High;
                    CyclesUsed = 1;
                    break;
                case (0x59): //LD E,C 
                    DE.Low = BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0x5A): //LD E,D 
                    DE.Low = DE.High;
                    CyclesUsed = 1;
                    break;
                case (0x5B): //LD E,E 
                    CyclesUsed = 1;
                    break;
                case (0x5C): //LD E,H 
                    DE.Low = HL.High;
                    CyclesUsed = 1;
                    break;
                case (0x5D): //LD E,L 
                    DE.Low = HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0x5E): //LD E,(HL) 
                    DE.Low = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0x5F): //LD E,A 
                    DE.Low = AF.High;
                    CyclesUsed = 1;
                    break;

                case (0x60): //LD H,B 
                    HL.High = BC.High;
                    CyclesUsed = 1;
                    break;
                case (0x61): //LD H,C 
                    HL.High = BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0x62): //LD H,D 
                    HL.High = DE.High;
                    CyclesUsed = 1;
                    break;
                case (0x63): //LD H,E 
                    HL.High = DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0x64): //LD H,H 
                    CyclesUsed = 1;
                    break;
                case (0x65): //LD H,L 
                    HL.High = HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0x66): //LD H,(HL) 
                    HL.High = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0x67): //LD H,A 
                    HL.High = AF.High;
                    CyclesUsed = 1;
                    break;
                case (0x68): //LD L,B 
                    HL.Low = BC.High;
                    CyclesUsed = 1;
                    break;
                case (0x69): //LD L,C 
                    HL.Low = BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0x6A): //LD L,D 
                    HL.Low = DE.High;
                    CyclesUsed = 1;
                    break;
                case (0x6B): //LD L,E 
                    HL.Low = DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0x6C): //LD L,H 
                    HL.Low = HL.High;
                    CyclesUsed = 1;
                    break;
                case (0x6D): //LD L,L 
                    CyclesUsed = 1;
                    break;
                case (0x6E): //LD L,(HL) 
                    HL.Low = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0x6F): //LD L,A 
                    HL.Low = AF.High;
                    CyclesUsed = 1;
                    break;

                case (0x70): //LD (HL),B 
                    MyCore.MyMemory.Write(HL.Word, BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x71): //LD (HL),C 
                    MyCore.MyMemory.Write(HL.Word, BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x72): //LD (HL),D 
                    MyCore.MyMemory.Write(HL.Word, DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x73): //LD (HL),E 
                    MyCore.MyMemory.Write(HL.Word, DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x74): //LD (HL),H 
                    MyCore.MyMemory.Write(HL.Word, HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x75): //LD (HL),L 
                    MyCore.MyMemory.Write(HL.Word, HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x76): //HALT 
                    CyclesUsed = 1;
                    break;
                case (0x77): //LD (HL),A 
                    MyCore.MyMemory.Write(HL.Word, AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x78): //LD A,B 
                    AF.High = BC.High;
                    CyclesUsed = 1;
                    break;
                case (0x79): //LD A,C 
                    AF.High = BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0x7A): //LD A,D 
                    AF.High = DE.High;
                    CyclesUsed = 1;
                    break;
                case (0x7B): //LD A,E 
                    AF.High = DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0x7C): //LD A,H 
                    AF.High = HL.High;
                    CyclesUsed = 1;
                    break;
                case (0x7D): //LD A,L 
                    AF.High = HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0x7E): //LD A,(HL) 
                    AF.High = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0x7F): //LD A,A 
                    CyclesUsed = 1;
                    break;

                case (0x80): //ADD A,B 
                    ADD_R8_R8(ref AF.High, BC.High);
                    CyclesUsed = 1;
                    break;
                case (0x81): //ADD A,C 
                    ADD_R8_R8(ref AF.High, BC.Low);
                    CyclesUsed = 1;
                    break;
                case (0x82): //ADD A,D 
                    ADD_R8_R8(ref AF.High, DE.High);
                    CyclesUsed = 1;
                    break;
                case (0x83): //ADD A,E 
                    ADD_R8_R8(ref AF.High, DE.Low);
                    CyclesUsed = 1;
                    break;
                case (0x84): //ADD A,H 
                    ADD_R8_R8(ref AF.High, HL.High);
                    CyclesUsed = 1;
                    break;
                case (0x85): //ADD A,L 
                    ADD_R8_R8(ref AF.High, HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0x86): //ADD A,(HL) 
                    ADD_R8_R8(ref AF.High, MyCore.MyMemory.Read(HL.Word));
                    CyclesUsed = 2;
                    break;
                case (0x87): //ADD A,A 
                    ADD_R8_R8(ref AF.High, AF.High);
                    CyclesUsed = 1;
                    break;
                case (0x88): //ADC A,B 
                    ADC_R8_R8(ref AF.High, BC.High);
                    CyclesUsed = 1;
                    break;
                case (0x89): //ADC A,C 
                    ADC_R8_R8(ref AF.High, BC.Low);
                    CyclesUsed = 1;
                    break;
                case (0x8A): //ADC A,D 
                    ADC_R8_R8(ref AF.High, DE.High);
                    CyclesUsed = 1;
                    break;
                case (0x8B): //ADC A,E 
                    ADC_R8_R8(ref AF.High, DE.Low);
                    CyclesUsed = 1;
                    break;
                case (0x8C): //ADC A,H 
                    ADC_R8_R8(ref AF.High, HL.High);
                    CyclesUsed = 1;
                    break;
                case (0x8D): //ADC A,L 
                    ADC_R8_R8(ref AF.High, HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0x8E): //ADC A,(HL) 
                    ADC_R8_R8(ref AF.High, MyCore.MyMemory.Read(HL.Word));
                    CyclesUsed = 2;
                    break;
                case (0x8F): //ADC A,A 
                    ADC_R8_R8(ref AF.High, AF.High);
                    CyclesUsed = 1;
                    break;

                case (0x90): //SUB A,B 
                    SUB_R8_R8(ref AF.High, BC.High);
                    CyclesUsed = 1;
                    break;
                case (0x91): //SUB A,C 
                    SUB_R8_R8(ref AF.High, BC.Low);
                    CyclesUsed = 1;
                    break;
                case (0x92): //SUB A,D 
                    SUB_R8_R8(ref AF.High, DE.High);
                    CyclesUsed = 1;
                    break;
                case (0x93): //SUB A,E 
                    SUB_R8_R8(ref AF.High, DE.Low);
                    CyclesUsed = 1;
                    break;
                case (0x94): //SUB A,H 
                    SUB_R8_R8(ref AF.High, HL.High);
                    CyclesUsed = 1;
                    break;
                case (0x95): //SUB A,L 
                    SUB_R8_R8(ref AF.High, HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0x96): //SUB A,(HL) 
                    SUB_R8_R8(ref AF.High, MyCore.MyMemory.Read(HL.Word));
                    CyclesUsed = 2;
                    break;
                case (0x97): //SUB A,A 
                    SUB_R8_R8(ref AF.High, AF.High);
                    CyclesUsed = 1;
                    break;
                case (0x98): //SBC A,B 
                    SBC_R8_R8(ref AF.High, BC.High);
                    CyclesUsed = 1;
                    break;
                case (0x99): //SBC A,C 
                    SBC_R8_R8(ref AF.High, BC.Low);
                    CyclesUsed = 1;
                    break;
                case (0x9A): //SBC A,D 
                    SBC_R8_R8(ref AF.High, DE.High);
                    CyclesUsed = 1;
                    break;
                case (0x9B): //SBC A,E 
                    SBC_R8_R8(ref AF.High, DE.Low);
                    CyclesUsed = 1;
                    break;
                case (0x9C): //SBC A,H 
                    SBC_R8_R8(ref AF.High, HL.High);
                    CyclesUsed = 1;
                    break;
                case (0x9D): //SBC A,L 
                    SBC_R8_R8(ref AF.High, HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0x9E): //SBC A,(HL) 
                    SBC_R8_R8(ref AF.High, MyCore.MyMemory.Read(HL.Word));
                    CyclesUsed = 2;
                    break;
                case (0x9F): //SBC A,A 
                    SBC_R8_R8(ref AF.High, AF.High);
                    CyclesUsed = 1;
                    break;

                case (0xA0): //AND B 
                    AF.High &= BC.High;
                    CyclesUsed = 1;
                    break;
                case (0xA1): //AND C 
                    AF.High &= BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0xA2): //AND D 
                    AF.High &= DE.High;
                    CyclesUsed = 1;
                    break;
                case (0xA3): //AND E 
                    AF.High &= DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0xA4): //AND H 
                    AF.High &= HL.High;
                    CyclesUsed = 1;
                    break;
                case (0xA5): //AND L 
                    AF.High &= HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0xA6): //AND (HL) 
                    AF.High &= MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0xA7): //AND A 
                    CyclesUsed = 1;
                    break;
                case (0xA8): //XOR B 
                    AF.High ^= BC.High;
                    CyclesUsed = 1;
                    break;
                case (0xA9): //XOR C 
                    AF.High ^= BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0xAA): //XOR D 
                    AF.High ^= DE.High;
                    CyclesUsed = 1;
                    break;
                case (0xAB): //XOR E 
                    AF.High ^= DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0xAC): //XOR H 
                    AF.High ^= HL.High;
                    CyclesUsed = 1;
                    break;
                case (0xAD): //XOR L 
                    AF.High ^= HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0xAE): //XOR (HL) 
                    AF.High ^= MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0xAF): //XOR A 
                    AF.High = 0;
                    CyclesUsed = 1;
                    break;

                case (0xB0): //OR B 
                    AF.High |= BC.High;
                    CyclesUsed = 1;
                    break;
                case (0xB1): //OR C 
                    AF.High |= BC.Low;
                    CyclesUsed = 1;
                    break;
                case (0xB2): //OR D 
                    AF.High |= DE.High;
                    CyclesUsed = 1;
                    break;
                case (0xB3): //OR E 
                    AF.High |= DE.Low;
                    CyclesUsed = 1;
                    break;
                case (0xB4): //OR H 
                    AF.High |= HL.High;
                    CyclesUsed = 1;
                    break;
                case (0xB5): //OR L 
                    AF.High |= HL.Low;
                    CyclesUsed = 1;
                    break;
                case (0xB6): //OR (HL) 
                    AF.High |= MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 2;
                    break;
                case (0xB7): //OR A 
                    CyclesUsed = 1;
                    break;
                case (0xB8): //CP B 
                    CP(AF.High, BC.High);
                    CyclesUsed = 1;
                    break;
                case (0xB9): //CP C 
                    CP(AF.High, BC.Low);
                    CyclesUsed = 1;
                    break;
                case (0xBA): //CP D 
                    CP(AF.High, DE.High);
                    CyclesUsed = 1;
                    break;
                case (0xBB): //CP E 
                    CP(AF.High, DE.Low);
                    CyclesUsed = 1;
                    break;
                case (0xBC): //CP H 
                    CP(AF.High, HL.High);
                    CyclesUsed = 1;
                    break;
                case (0xBD): //CP L 
                    CP(AF.High, HL.Low);
                    CyclesUsed = 1;
                    break;
                case (0xBE): //CP (HL) 
                    CP(AF.High, MyCore.MyMemory.Read(HL.Word));
                    CyclesUsed = 2;
                    break;
                case (0xBF): //CP A 
                    CP(AF.High, AF.High);
                    CyclesUsed = 1;
                    break;

                case (0xC0): //RET NZ 
                    if (!Utility.IsBitSet(AF.High, FLAG_Z))
                    {
                        RET();
                    }
                    CyclesUsed = 5;
                    break;
                case (0xC1): //POP BC 
                    BC.Word = POP();
                    CyclesUsed = 3;
                    break;
                case (0xC2): //JP NZ,nn 
                    if (!Utility.IsBitSet(AF.High, FLAG_Z))
                    {
                        ProgramCounter.Word = MyCore.MyMemory.ReadWord(++ProgramCounter.Word);
                    }
                    CyclesUsed = 3;
                    break;
                case (0xC3): //JP nn 
                    ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(++ProgramCounter.Word)-1);
                    CyclesUsed = 4;
                    break;
                case (0xC4): //CALL NZ,nn 
                    if (!Utility.IsBitSet(AF.High, FLAG_Z))
                    {
                        PUSH((ushort)(ProgramCounter.Word + 2));
                        ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(++ProgramCounter.Word) - 1);
                    }
                    CyclesUsed = 6;
                    break;
                case (0xC5): //PUSH BC 
                    PUSH(BC.Word);
                    CyclesUsed = 4;
                    break;
                case (0xC6): //ADD A,n 
                    ADD_R8_R8(ref AF.High, MyCore.MyMemory.Read(++ProgramCounter.Word));
                    CyclesUsed = 2;
                    break;
                case (0xC7): //RST 0 
                    PUSH(ProgramCounter.Word);
                    ProgramCounter.Word = 0xFFFF;
                    CyclesUsed = 4;
                    break;
                case (0xC8): //RET Z 
                    if (Utility.IsBitSet(AF.Low, FLAG_Z))
                    {
                        ProgramCounter.Word = POP();
                    }
                    CyclesUsed = 5;
                    break;
                case (0xC9): //RET 
                    ProgramCounter.Word = POP();
                    CyclesUsed = 4;
                    break;
                case (0xCA): //JP Z,nn 
                    if (Utility.IsBitSet(AF.Low, FLAG_Z))
                    {
                        ProgramCounter.Word = MyCore.MyMemory.ReadWord(++ProgramCounter.Word);
                    }
                    CyclesUsed = 3;
                    break;
                case (0xCB): //CB-Subset 
                    CyclesUsed = CB_Subset(++ProgramCounter.Word);
                    break;
                case (0xCC): //CALL Z,nn 
                    if (Utility.IsBitSet(AF.Low, FLAG_Z))
                    {
                        PUSH((ushort)(ProgramCounter.Word + 2));
                        ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(++ProgramCounter.Word) - 1);
                    }
                    CyclesUsed = 6;
                    break;
                case (0xCD): //CALL nn 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(++ProgramCounter.Word) - 1);
                    CyclesUsed = 6;
                    break;
                case (0xCE): //ADC A,n 
                    ADC_R8_R8(ref AF.High, MyCore.MyMemory.Read(++ProgramCounter.Word));
                    CyclesUsed = 2;
                    break;
                case (0xCF): //RST 8 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(0x07);
                    CyclesUsed = 4;
                    break;

                case (0xD0): //RET NC 
                    if (!Utility.IsBitSet(AF.Low, FLAG_C))
                    {
                        ProgramCounter.Word = POP();
                    }
                    CyclesUsed = 5;
                    break;
                case (0xD1): //POP DE 
                    DE.Word = POP();
                    CyclesUsed = 3;
                    break;
                case (0xD2): //JP NC,nn 
                    if (!Utility.IsBitSet(AF.Low, FLAG_C))
                    {
                        ProgramCounter.Word = MyCore.MyMemory.ReadWord(++ProgramCounter.Word);
                    }
                    CyclesUsed = 3;
                    break;
                case (0xD3): //Invalid 
                    break;
                case (0xD4): //CALL NC,nn 
                    if (!Utility.IsBitSet(AF.Low, FLAG_C))
                    {
                        PUSH((ushort)(ProgramCounter.Word + 2));
                        ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(++ProgramCounter.Word) - 1);
                    }
                    CyclesUsed = 6;
                    break;
                case (0xD5): //PUSH DE 
                    PUSH(DE.Word);
                    CyclesUsed = 4;
                    break;
                case (0xD6): //SUB A,n 
                    SUB_R8_R8(ref AF.High, MyCore.MyMemory.Read(++ProgramCounter.Word));
                    CyclesUsed = 2;
                    break;
                case (0xD7): //RST 10 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(0x09);
                    CyclesUsed = 4;
                    break;
                case (0xD8): //RET C 
                    if (Utility.IsBitSet(AF.High, FLAG_C))
                    {
                        ProgramCounter.Word = POP();
                    }
                    CyclesUsed = 5;
                    break;
                case (0xD9): //RETI 
                    InterruptMasterEnable = true;
                    ProgramCounter.Word = POP();
                    CyclesUsed = 4;
                    break;
                case (0xDA): //JP C,nn 
                    if (Utility.IsBitSet(AF.High, FLAG_C))
                    {
                        ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(++ProgramCounter.Word) - 1);
                    }
                    CyclesUsed = 4;
                    break;
                case (0xDB): //Invalid 
                    break;
                case (0xDC): //CALL C,nn 
                    if (Utility.IsBitSet(AF.High, FLAG_C))
                    {
                        PUSH((ushort)(ProgramCounter.Word + 2));
                        ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(++ProgramCounter.Word) - 1);
                    }
                    CyclesUsed = 6;
                    break;
                case (0xDD): //Invalid 
                    break;
                case (0xDE): //SBC A,n 
                    SBC_R8_R8(ref AF.High, MyCore.MyMemory.Read(++ProgramCounter.Word));
                    CyclesUsed = 2;
                    break;
                case (0xDF): //RST 18 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(0x17);
                    CyclesUsed = 4;
                    break;

                case (0xE0): //LDH (n),A 
                    MyCore.MyMemory.Write((ushort)(MyCore.MyMemory.Read(++ProgramCounter.Word) + 0xFF00), AF.High);
                    CyclesUsed = 3;
                    break;
                case (0xE1): //POP HL 
                    HL.Word = POP();
                    CyclesUsed = 3;
                    break;
                case (0xE2): //LDH (C),A 
                    MyCore.MyMemory.Write((ushort)(BC.Low + 0xFF00), AF.High);
                    CyclesUsed = 3;
                    break;
                case (0xE3): //Invalid 
                    break;
                case (0xE4): //Invalid 
                    break;
                case (0xE5): //PUSH HL 
                    PUSH(HL.Word);
                    CyclesUsed = 3;
                    break;
                case (0xE6): //AND n 
                    AF.High &= MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0xE7): //RST 20 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(0x19);
                    CyclesUsed = 4;
                    break;
                case (0xE8): //ADD SP,d 
                    signed8bit = (sbyte)MyCore.MyMemory.Read(++ProgramCounter.Word);
                    Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
                    if ((StackPointer.High & 0x0F) + (signed8bit & 0x0F) != 0)
                    {
                        Utility.SetBit(ref AF.Low, FLAG_H, SBMode.On);
                    }
                    else
                    {
                        Utility.SetBit(ref AF.Low, FLAG_H, SBMode.Off);
                    }
                    if ((int)(StackPointer.Word + signed8bit) > 0xFFFF)
                    {
                        Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                        StackPointer.Word = (ushort)((StackPointer.Word + signed8bit) - 0xFFFF);
                    }
                    else
                    {
                        if (signed8bit < 0)
                        {
                            StackPointer.Word -= (ushort)Math.Abs(signed8bit);
                        }
                        else
                        {
                            StackPointer.Word += (ushort)Math.Abs(signed8bit);
                        }
                    }
                    CyclesUsed = 4;
                    break;
                case (0xE9): //JP (HL) 
                    ProgramCounter.Word = MyCore.MyMemory.Read(HL.Word);
                    CyclesUsed = 1;
                    break;
                case (0xEA): //LD (nn),A 
                    MyCore.MyMemory.Write(MyCore.MyMemory.ReadWord(++ProgramCounter.Word), AF.High);
                    ProgramCounter.Word++;
                    CyclesUsed = 4;
                    break;
                case (0xEB): //Invalid 
                    break;
                case (0xEC): //Invalid 
                    break;
                case (0xED): //Invalid 
                    break;
                case (0xEE): //OR n 
                    AF.High |= MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0xEF): //RST 28 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(0x27);
                    CyclesUsed = 4;
                    break;

                case (0xF0): //LDH A,(n) 
                    AF.High = MyCore.MyMemory.Read((ushort)MyCore.MyMemory.Read(++ProgramCounter.Word));
                    CyclesUsed = 3;
                    break;
                case (0xF1): //POP AF 
                    AF.Word = POP();
                    CyclesUsed = 3;
                    break;
                case (0xF2): //Invalid 
                    break;
                case (0xF3): //DI 
                    InterruptMasterEnable = false;
                    CyclesUsed = 1;
                    break;
                case (0xF4): //Invalid 
                    break;
                case (0xF5): //PUSH AF 
                    PUSH(AF.Word);
                    CyclesUsed = 4;
                    break;
                case (0xF6): //OR n 
                    AF.High |= MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 2;
                    break;
                case (0xF7): //RST 30 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(0x29);
                    CyclesUsed = 4;
                    break;
                case (0xF8): //LDHL SP,d 
                    signed8bit = (sbyte)MyCore.MyMemory.Read(++ProgramCounter.Word);
                    if (signed8bit < 0)
                    {
                        HL.Word = (ushort)(StackPointer.Word - (ushort)(Math.Abs(signed8bit)));
                    }
                    else
                    {
                        HL.Word = (ushort)(StackPointer.Word + (ushort)signed8bit);
                    }
                    CyclesUsed = 3;
                    break;
                case (0xF9): //LD SP,HL 
                    StackPointer.Word = HL.Word;
                    CyclesUsed = 2;
                    break;
                case (0xFA): //LD A,(nn) 
                    AF.High = MyCore.MyMemory.Read(++ProgramCounter.Word);
                    CyclesUsed = 4;
                    break;
                case (0xFB): //EI 
                    InterruptMasterEnable = true;
                    CyclesUsed = 1;
                    break;
                case (0xFC): //Invalid 
                    break;
                case (0xFD): //Invalid 
                    break;
                case (0xFE): //CP n 
                    CP(AF.High, MyCore.MyMemory.Read(++ProgramCounter.Word));
                    CyclesUsed = 2;
                    break;
                case (0xFF): //RST 38 
                    PUSH((ushort)(ProgramCounter.Word + 2));
                    ProgramCounter.Word = (ushort)(0x37);
                    CyclesUsed = 3;
                    break;
            }
            ProgramCounter.Word++;
            return CyclesUsed;
        }

        private int CB_Subset(int Opcode)
        {
            int CyclesUsed = 0;
            switch (Opcode)
            {
                case (0x00): //RLC B 
                    RLC_R8(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x01): //RLC C 
                    RLC_R8(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x02): //RLC D 
                    RLC_R8(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x03): //RLC E 
                    RLC_R8(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x04): //RLC H 
                    RLC_R8(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x05): //RLC L 
                    RLC_R8(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x06): //RLC (HL) 
                    RLC_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 2;
                    break;
                case (0x07): //RLC A 
                    RLC_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x08): //RRC B 
                    RRC_R8(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x09): //RRC C 
                    RRC_R8(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x0A): //RRC D 
                    RRC_R8(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x0B): //RRC E 
                    RRC_R8(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x0C): //RRC H 
                    RRC_R8(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x0D): //RRC L 
                    RRC_R8(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x0E): //RRC (HL) 
                    RRC_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 2;
                    break;
                case (0x0F): //RRC A 
                    RRC_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;

                case (0x10): //RL B 
                    RL_R8(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x11): //RL C 
                    RL_R8(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x12): //RL D 
                    RL_R8(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x13): //RL E 
                    RL_R8(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x14): //RL H 
                    RL_R8(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x15): //RL L 
                    RL_R8(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x16): //RL (HL) 
                    RL_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 4;
                    break;
                case (0x17): //RL A 
                    RL_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x18): //RR B 
                    RR_R8(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x19): //RR C 
                    RR_R8(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x1A): //RR D 
                    RR_R8(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x1B): //RR E 
                    RR_R8(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x1C): //RR H 
                    RR_R8(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x1D): //RR L 
                    RR_R8(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x1E): //RR (HL) 
                    RR_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 4;
                    break;
                case (0x1F): //RR A 
                    RR_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;

                case (0x20): //SLA B 
                    SLA_R8(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x21): //SLA C 
                    SLA_R8(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x22): //SLA D 
                    SLA_R8(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x23): //SLA E 
                    SLA_R8(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x24): //SLA H 
                    SLA_R8(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x25): //SLA L 
                    SLA_R8(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x26): //SLA (HL) 
                    SLA_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 4;
                    break;
                case (0x27): //SLA A 
                    SLA_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x28): //SRA B 
                    SRA_R8(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x29): //SRA C 
                    SRA_R8(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x2A): //SRA D 
                    SRA_R8(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x2B): //SRA E 
                    SRA_R8(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x2C): //SRA H 
                    SRA_R8(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x2D): //SRA L 
                    SRA_R8(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x2E): //SRA (HL) 
                    SRA_R8(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 4;
                    break;
                case (0x2F): //SRA A 
                    SRA_R8(ref AF.High);
                    CyclesUsed = 2;
                    break;

                case (0x30): //SWAP B 
                    SWAP(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x31): //SWAP C 
                    SWAP(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x32): //SWAP D 
                    SWAP(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x33): //SWAP E 
                    SWAP(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x34): //SWAP H 
                    SWAP(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x35): //SWAP L 
                    SWAP(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x36): //SWAP (HL) 
                    SWAP(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 4;
                    break;
                case (0x37): //SWAP A 
                    SWAP(ref AF.High);
                    CyclesUsed = 2;
                    break;
                case (0x38): //SRL B 
                    SRL(ref BC.High);
                    CyclesUsed = 2;
                    break;
                case (0x39): //SRL C 
                    SRL(ref BC.Low);
                    CyclesUsed = 2;
                    break;
                case (0x3A): //SRL D 
                    SRL(ref DE.High);
                    CyclesUsed = 2;
                    break;
                case (0x3B): //SRL E 
                    SRL(ref DE.Low);
                    CyclesUsed = 2;
                    break;
                case (0x3C): //SRL H 
                    SRL(ref HL.High);
                    CyclesUsed = 2;
                    break;
                case (0x3D): //SRL L 
                    SRL(ref HL.Low);
                    CyclesUsed = 2;
                    break;
                case (0x3E): //SRL (HL) 
                    SRL(ref MyCore.MyMemory.GameBoyRAM[HL.Word]);
                    CyclesUsed = 4;
                    break;
                case (0x3F): //SRL A 
                    SRL(ref AF.High);
                    CyclesUsed = 2;
                    break;

                case (0x40): //BIT 0,B 
                    BIT(BC.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x41): //BIT 0,C 
                    BIT(BC.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0x42): //BIT 0,D 
                    BIT(DE.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x43): //BIT 0,E 
                    BIT(DE.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0x44): //BIT 0,H 
                    BIT(HL.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x45): //BIT 0,L 
                    BIT(HL.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0x46): //BIT 0,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 0);
                    CyclesUsed = 3;
                    break;
                case (0x47): //BIT 0,A 
                    BIT(AF.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x48): //BIT 1,B 
                    BIT(BC.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0x49): //BIT 1,C 
                    BIT(BC.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0x4A): //BIT 1,D 
                    BIT(DE.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0x4B): //BIT 1,E 
                    BIT(DE.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0x4C): //BIT 1,H 
                    BIT(HL.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0x4D): //BIT 1,L 
                    BIT(HL.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0x4E): //BIT 1,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 1);
                    CyclesUsed = 3;
                    break;
                case (0x4F): //BIT 1,A 
                    BIT(AF.High, 1);
                    CyclesUsed = 2;
                    break;

                case (0x50): //BIT 2,B 
                    BIT(BC.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x51): //BIT 2,C 
                    BIT(BC.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0x52): //BIT 2,D 
                    BIT(DE.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x53): //BIT 2,E 
                    BIT(DE.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0x54): //BIT 2,H 
                    BIT(HL.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x55): //BIT 2,L 
                    BIT(HL.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0x56): //BIT 2,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 2);
                    CyclesUsed = 3;
                    break;
                case (0x57): //BIT 2,A 
                    BIT(AF.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x58): //BIT 3,B 
                    BIT(BC.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0x59): //BIT 3,C 
                    BIT(BC.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0x5A): //BIT 3,D 
                    BIT(DE.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0x5B): //BIT 3,E 
                    BIT(DE.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0x5C): //BIT 3,H 
                    BIT(HL.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0x5D): //BIT 3,L 
                    BIT(HL.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0x5E): //BIT 3,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 3);
                    CyclesUsed = 3;
                    break;
                case (0x5F): //BIT 3,A 
                    BIT(AF.High, 3);
                    CyclesUsed = 2;
                    break;

                case (0x60): //BIT 4,B 
                    BIT(BC.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0x61): //BIT 4,C 
                    BIT(BC.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0x62): //BIT 4,D 
                    BIT(DE.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0x63): //BIT 4,E 
                    BIT(DE.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0x64): //BIT 4,H 
                    BIT(HL.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0x65): //BIT 4,L 
                    BIT(HL.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0x66): //BIT 4,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 4);
                    CyclesUsed = 3;
                    break;
                case (0x67): //BIT 4,A 
                    BIT(AF.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0x68): //BIT 5,B 
                    BIT(BC.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0x69): //BIT 5,C 
                    BIT(BC.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0x6A): //BIT 5,D 
                    BIT(DE.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0x6B): //BIT 5,E 
                    BIT(DE.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0x6C): //BIT 5,H 
                    BIT(HL.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0x6D): //BIT 5,L 
                    BIT(HL.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0x6E): //BIT 5,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 5);
                    CyclesUsed = 3;
                    break;
                case (0x6F): //BIT 5,A 
                    BIT(AF.High, 5);
                    CyclesUsed = 2;
                    break;

                case (0x70): //BIT 6,B 
                    BIT(BC.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0x71): //BIT 6,C 
                    BIT(BC.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0x72): //BIT 6,D 
                    BIT(DE.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0x73): //BIT 6,E 
                    BIT(DE.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0x74): //BIT 6,H 
                    BIT(HL.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0x75): //BIT 6,L 
                    BIT(HL.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0x76): //BIT 6,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 6);
                    CyclesUsed = 3;
                    break;
                case (0x77): //BIT 6,A 
                    BIT(AF.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0x78): //BIT 7,B 
                    BIT(BC.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0x79): //BIT 7,C 
                    BIT(BC.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0x7A): //BIT 7,D 
                    BIT(DE.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0x7B): //BIT 7,E 
                    BIT(DE.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0x7C): //BIT 7,H 
                    BIT(HL.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0x7D): //BIT 7,L 
                    BIT(HL.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0x7E): //BIT 7,(HL) 
                    BIT(MyCore.MyMemory.GameBoyRAM[HL.Word], 7);
                    CyclesUsed = 3;
                    break;
                case (0x7F): //BIT 7,A 
                    BIT(AF.High, 7);
                    CyclesUsed = 2;
                    break;

                case (0x80): //RES 0,B 
                    RES(ref BC.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x81): //RES 0,C 
                    RES(ref BC.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0x82): //RES 0,D 
                    RES(ref DE.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x83): //RES 0,E 
                    RES(ref DE.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0x84): //RES 0,H 
                    RES(ref HL.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x85): //RES 0,L 
                    RES(ref HL.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0x86): //RES 0,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 0);
                    CyclesUsed = 3;
                    break;
                case (0x87): //RES 0,A 
                    RES(ref AF.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0x88): //RES 1,B 
                    RES(ref BC.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0x89): //RES 1,C 
                    RES(ref BC.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0x8A): //RES 1,D 
                    RES(ref DE.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0x8B): //RES 1,E 
                    RES(ref DE.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0x8C): //RES 1,H 
                    RES(ref HL.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0x8D): //RES 1,L 
                    RES(ref HL.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0x8E): //RES 1,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 1);
                    CyclesUsed = 3;
                    break;
                case (0x8F): //RES 1,A 
                    RES(ref AF.High, 1);
                    CyclesUsed = 2;
                    break;

                case (0x90): //RES 2,B 
                    RES(ref BC.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x91): //RES 2,C 
                    RES(ref BC.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0x92): //RES 2,D 
                    RES(ref DE.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x93): //RES 2,E 
                    RES(ref DE.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0x94): //RES 2,H 
                    RES(ref HL.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x95): //RES 2,L 
                    RES(ref HL.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0x96): //RES 2,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 2);
                    CyclesUsed = 3;
                    break;
                case (0x97): //RES 2,A 
                    RES(ref AF.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0x98): //RES 3,B 
                    RES(ref BC.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0x99): //RES 3,C 
                    RES(ref BC.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0x9A): //RES 3,D 
                    RES(ref DE.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0x9B): //RES 3,E 
                    RES(ref DE.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0x9C): //RES 3,H 
                    RES(ref HL.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0x9D): //RES 3,L 
                    RES(ref HL.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0x9E): //RES 3,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 3);
                    CyclesUsed = 3;
                    break;
                case (0x9F): //RES 3,A 
                    RES(ref AF.High, 3);
                    CyclesUsed = 2;
                    break;

                case (0xA0): //RES 4,B 
                    RES(ref BC.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xA1): //RES 4,C 
                    RES(ref BC.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0xA2): //RES 4,D 
                    RES(ref DE.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xA3): //RES 4,E 
                    RES(ref DE.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0xA4): //RES 4,H 
                    RES(ref HL.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xA5): //RES 4,L 
                    RES(ref HL.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0xA6): //RES 4,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 4);
                    CyclesUsed = 3;
                    break;
                case (0xA7): //RES 4,A 
                    RES(ref AF.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xA8): //RES 5,B 
                    RES(ref BC.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0xA9): //RES 5,C 
                    RES(ref BC.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0xAA): //RES 5,D 
                    RES(ref DE.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0xAB): //RES 5,E 
                    RES(ref DE.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0xAC): //RES 5,H 
                    RES(ref HL.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0xAD): //RES 5,L 
                    RES(ref HL.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0xAE): //RES 5,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 5);
                    CyclesUsed = 3;
                    break;
                case (0xAF): //RES 5,A 
                    RES(ref AF.High, 5);
                    CyclesUsed = 2;
                    break;

                case (0xB0): //RES 6,B 
                    RES(ref BC.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xB1): //RES 6,C 
                    RES(ref BC.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0xB2): //RES 6,D 
                    RES(ref DE.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xB3): //RES 6,E 
                    RES(ref DE.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0xB4): //RES 6,H 
                    RES(ref HL.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xB5): //RES 6,L 
                    RES(ref HL.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0xB6): //RES 6,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 6);
                    CyclesUsed = 3;
                    break;
                case (0xB7): //RES 6,A 
                    RES(ref AF.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xB8): //RES 7,B 
                    RES(ref BC.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0xB9): //RES 7,C 
                    RES(ref BC.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0xBA): //RES 7,D 
                    RES(ref DE.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0xBB): //RES 7,E 
                    RES(ref DE.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0xBC): //RES 7,H 
                    RES(ref HL.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0xBD): //RES 7,L 
                    RES(ref HL.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0xBE): //RES 7,(HL) 
                    RES(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 7);
                    CyclesUsed = 3;
                    break;
                case (0xBF): //RES 7,A 
                    RES(ref AF.High, 7);
                    CyclesUsed = 2;
                    break;

                case (0xC0): //SET 0,B 
                    SET(ref BC.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0xC1): //SET 0,C 
                    SET(ref BC.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0xC2): //SET 0,D 
                    SET(ref DE.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0xC3): //SET 0,E 
                    SET(ref DE.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0xC4): //SET 0,H 
                    SET(ref HL.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0xC5): //SET 0,L 
                    SET(ref HL.Low, 0);
                    CyclesUsed = 2;
                    break;
                case (0xC6): //SET 0,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 0);
                    CyclesUsed = 3;
                    break;
                case (0xC7): //SET 0,A 
                    SET(ref AF.High, 0);
                    CyclesUsed = 2;
                    break;
                case (0xC8): //SET 1,B 
                    SET(ref BC.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0xC9): //SET 1,C 
                    SET(ref BC.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0xCA): //SET 1,D 
                    SET(ref DE.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0xCB): //SET 1,E 
                    SET(ref DE.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0xCC): //SET 1,H 
                    SET(ref HL.High, 1);
                    CyclesUsed = 2;
                    break;
                case (0xCD): //SET 1,L 
                    SET(ref HL.Low, 1);
                    CyclesUsed = 2;
                    break;
                case (0xCE): //SET 1,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 1);
                    CyclesUsed = 3;
                    break;
                case (0xCF): //SET 1,A 
                    SET(ref AF.High, 1);
                    CyclesUsed = 2;
                    break;

                case (0xD0): //SET 2,B 
                    SET(ref BC.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0xD1): //SET 2,C 
                    SET(ref BC.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0xD2): //SET 2,D 
                    SET(ref DE.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0xD3): //SET 2,E 
                    SET(ref DE.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0xD4): //SET 2,H 
                    SET(ref HL.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0xD5): //SET 2,L 
                    SET(ref HL.Low, 2);
                    CyclesUsed = 2;
                    break;
                case (0xD6): //SET 2,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 2);
                    CyclesUsed = 3;
                    break;
                case (0xD7): //SET 2,A 
                    SET(ref AF.High, 2);
                    CyclesUsed = 2;
                    break;
                case (0xD8): //SET 3,B 
                    SET(ref BC.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0xD9): //SET 3,C 
                    SET(ref BC.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0xDA): //SET 3,D 
                    SET(ref DE.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0xDB): //SET 3,E 
                    SET(ref DE.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0xDC): //SET 3,H 
                    SET(ref HL.High, 3);
                    CyclesUsed = 2;
                    break;
                case (0xDD): //SET 3,L 
                    SET(ref HL.Low, 3);
                    CyclesUsed = 2;
                    break;
                case (0xDE): //SET 3,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 3);
                    CyclesUsed = 3;
                    break;
                case (0xDF): //SET 3,A 
                    SET(ref AF.High, 3);
                    CyclesUsed = 2;
                    break;

                case (0xE0): //SET 4,B 
                    SET(ref BC.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xE1): //SET 4,C 
                    SET(ref BC.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0xE2): //SET 4,D 
                    SET(ref DE.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xE3): //SET 4,E 
                    SET(ref DE.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0xE4): //SET 4,H 
                    SET(ref HL.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xE5): //SET 4,L 
                    SET(ref HL.Low, 4);
                    CyclesUsed = 2;
                    break;
                case (0xE6): //SET 4,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 4);
                    CyclesUsed = 3;
                    break;
                case (0xE7): //SET 4,A 
                    SET(ref AF.High, 4);
                    CyclesUsed = 2;
                    break;
                case (0xE8): //SET 5,B 
                    SET(ref BC.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0xE9): //SET 5,C 
                    SET(ref BC.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0xEA): //SET 5,D 
                    SET(ref DE.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0xEB): //SET 5,E 
                    SET(ref DE.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0xEC): //SET 5,H 
                    SET(ref HL.High, 5);
                    CyclesUsed = 2;
                    break;
                case (0xED): //SET 5,L 
                    SET(ref HL.Low, 5);
                    CyclesUsed = 2;
                    break;
                case (0xEE): //SET 5,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 5);
                    CyclesUsed = 3;
                    break;
                case (0xEF): //SET 5,A 
                    SET(ref AF.High, 5);
                    CyclesUsed = 2;
                    break;

                case (0xF0): //SET 6,B 
                    SET(ref BC.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xF1): //SET 6,C 
                    SET(ref BC.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0xF2): //SET 6,D 
                    SET(ref DE.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xF3): //SET 6,E 
                    SET(ref DE.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0xF4): //SET 6,H 
                    SET(ref HL.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xF5): //SET 6,L 
                    SET(ref HL.Low, 6);
                    CyclesUsed = 2;
                    break;
                case (0xF6): //SET 6,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 6);
                    CyclesUsed = 3;
                    break;
                case (0xF7): //SET 6,A 
                    SET(ref AF.High, 6);
                    CyclesUsed = 2;
                    break;
                case (0xF8): //SET 7,B 
                    SET(ref BC.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0xF9): //SET 7,C 
                    SET(ref BC.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0xFA): //SET 7,D 
                    SET(ref DE.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0xFB): //SET 7,E 
                    SET(ref DE.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0xFC): //SET 7,H 
                    SET(ref HL.High, 7);
                    CyclesUsed = 2;
                    break;
                case (0xFD): //SET 7,L 
                    SET(ref HL.Low, 7);
                    CyclesUsed = 2;
                    break;
                case (0xFE): //SET 7,(HL) 
                    SET(ref MyCore.MyMemory.GameBoyRAM[HL.Word], 7);
                    CyclesUsed = 3;
                    break;
                case (0xFF): //SET 7,A 
                    SET(ref AF.High, 7);
                    CyclesUsed = 2;
                    break;
            }

            return CyclesUsed;
        }

        #region Generalized functions

        public void RES(ref byte R8, byte bitnum)
        {
            Utility.SetBit(ref R8, bitnum, SBMode.Off);
        }

        public void SET(ref byte R8, byte bitnum)
        {
            Utility.SetBit(ref R8, bitnum, SBMode.On);
        }

        public void BIT(byte R8, byte bitnum)
        {
            Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
            Utility.SetBit(ref AF.Low, FLAG_H, SBMode.On);

            if (Utility.IsBitSet(R8, bitnum))
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
            }
            else
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        public void SRL(ref byte R8)
        {
            AF.Low = 0;
            if (Utility.IsBitSet(R8, 0))
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
            }

            R8 >>= 1;

            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        public void SWAP(ref byte R8)
        {
            R8 = (byte)(((R8 & 0xF0) >> 4) | ((R8 & 0x0F) << 4));
            if (R8 == 0)
            {
                AF.Low = 1 << FLAG_C;
            }
            else
            {
                AF.Low = 0;
            }
        }

        public void ADD_R16_R16(ref Register R16A, Register R16B)
        {
            Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
            if ((R16A.High & 0x0F) + (R16B.High & 0x0F) != 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_H, SBMode.On);
            }
            else
            {
                Utility.SetBit(ref AF.Low, FLAG_H, SBMode.Off);
            }
            if ((int)(R16A.Word + R16B.Word) > 0xFFFF)
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                R16A.Word = (ushort)((R16A.Word + R16B.Word) - 0xFFFF);
            }
            else
            {
                R16A.Word += R16B.Word;
            }
        }

        public void LD_R16_NN(ref Register R16, ushort NN)
        {
            Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
            Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
            R16.Word = NN;
        }

        public void INC_R16(ref Register R16)
        {
            if (R16.Word == 0xFFFF)
            {
                R16.Word = 0;
            }
            else
            {
                R16.Word++;
            }
        }

        public void DEC_R16(ref Register R16)
        {
            if (R16.Word == 0)
            {
                R16.Word = 0xFFFF;
            }
            else
            {
                R16.Word--;
            }
        }

        public void SLA_R8(ref byte R8)
        {
            AF.Low = 0;

            if (Utility.IsBitSet(R8, 7))
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
            }

            R8 <<= 1;

            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        public void SRA_R8(ref byte R8)
        {
            AF.Low = 0;

            bool MSBSet = Utility.IsBitSet(R8, 7);

            if (Utility.IsBitSet(R8, 0))
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
            }

            R8 >>= 1;

            if (MSBSet)
            {
                Utility.SetBit(ref R8, 7, SBMode.On);
            }

            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        public void CP(byte R8A, byte R8B)
        {
            AF.High = 1 << FLAG_N;

            if (R8A - R8B == 0)
            {
                Utility.SetBit(ref AF.High, FLAG_Z, SBMode.On);
            }

            if (R8A < R8B)
            {
                Utility.SetBit(ref AF.High, FLAG_C, SBMode.On);
            }

            short HTest = (short)(R8A & 0xF);
            HTest -= (short)(R8B & 0xF);

            if (HTest < 0)
            {
                Utility.SetBit(ref AF.High, FLAG_H, SBMode.On);
            }
        }

        public void INC_R8(ref byte R8)
        {
            if ((R8 & 0xF) == 0xF)
            {
                Utility.SetBit(ref AF.Low, FLAG_H, SBMode.On);
            }
            else
            {
                Utility.SetBit(ref AF.Low, FLAG_H, SBMode.Off);
            }
            Utility.SetBit(ref AF.Low, FLAG_N, SBMode.Off);
            if (R8 == 0xFF)
            {
                R8 = 0;
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
            else
            {
                R8++;
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
            }
        }

        public void DEC_R8(ref byte R8)
        {
            AF.Low = 0;
            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.Off);
            }
            if ((R8 & 0xF) == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_H, SBMode.On);
            }
            else
            {
                Utility.SetBit(ref AF.Low, FLAG_H, SBMode.Off);
            }
            Utility.SetBit(ref AF.Low, FLAG_N, SBMode.On);
            if (R8 == 0)
            {
                R8 = 0xFF;
            }
            else
            {
                R8--;
                if (R8 == 0)
                {
                    Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
                }
            }
        }

        public void ADD_R8_R8(ref byte R8A, byte R8B)
        {
            if (R8A + R8B > 0xFF)
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                R8A = (byte)((R8A + R8B) - 0xFF);
            }
            else
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.Off);
                R8A += R8B;
            }
        }

        public void SUB_R8_R8(ref byte R8A, byte R8B)
        {
            if (R8A - R8B < 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                R8A = (byte)((R8A - R8B) + 0xFF);
            }
            else
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.Off);
                R8A -= R8B;
            }
        }

        public void ADC_R8_R8(ref byte R8A, byte R8B)
        {
            if (Utility.IsBitSet(AF.Low, FLAG_C))
            {
                ADD_R8_R8(ref R8A, (byte)(R8B + 1));
            }
            else
            {
                ADD_R8_R8(ref R8A, (byte)(R8B + 1));
            }
        }

        public void SBC_R8_R8(ref byte R8A, byte R8B)
        {
            if (Utility.IsBitSet(AF.Low, FLAG_C))
            {
                SUB_R8_R8(ref R8A, (byte)(R8B + 1));
            }
            else
            {
                SUB_R8_R8(ref R8A, (byte)(R8B + 1));
            }
        }

        public void RLC_R8(ref byte R8)
        {
            bool CarrySet = Utility.IsBitSet(AF.Low, FLAG_C);
            bool MSBSet = Utility.IsBitSet(R8, 7);

            AF.Low = 0;
            R8 <<= 1;

            if (MSBSet)
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
            }
            if (CarrySet)
            {
                Utility.SetBit(ref R8, 0, SBMode.On);
            }
            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        public void RRC_R8(ref byte R8)
        {
            bool CarrySet = Utility.IsBitSet(AF.Low, FLAG_C);
            bool LSBSet = Utility.IsBitSet(R8, 0);

            AF.Low = 0;
            R8 >>= 1;

            if (LSBSet)
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
            }
            if (CarrySet)
            {
                Utility.SetBit(ref R8, 7, SBMode.On);
            }
            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        public void RL_R8(ref byte R8)
        {
            AF.Low = 0;
            bool MSBSet = Utility.IsBitSet(R8, 7);
            R8 <<= 1;
            if (MSBSet)
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                Utility.SetBit(ref R8, 0, SBMode.On);
            }

            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        public void RR_R8(ref byte R8)
        {
            AF.Low = 0;
            bool LSBSet = Utility.IsBitSet(R8, 0);
            R8 >>= 1;
            if (LSBSet)
            {
                Utility.SetBit(ref AF.Low, FLAG_C, SBMode.On);
                Utility.SetBit(ref R8, 7, SBMode.On);
            }

            if (R8 == 0)
            {
                Utility.SetBit(ref AF.Low, FLAG_Z, SBMode.On);
            }
        }

        private void RET()
        {
            ProgramCounter.Word = POP();
        }

        private void RETI()
        {
            InterruptMasterEnable = true;
            RET();
        }

        private void PUSH(ushort NN)
        {
            MyCore.MyMemory.Write((ushort)(StackPointer.Word - 1), (byte)((NN & 0xFF00) >> 8));
            MyCore.MyMemory.Write((ushort)(StackPointer.Word - 2), (byte)(NN & 0x00FF));
            StackPointer.Word -= 2;
        }

        private ushort POP()
        {
            ushort ret = (ushort)(MyCore.MyMemory.Read((ushort)(StackPointer.Word + 1)) << 8);
            ret |= MyCore.MyMemory.Read(StackPointer.Word);
            StackPointer.Word += 2;
            return ret;
        }

        #endregion

        public void DoInterrupts(int Cycles)
        {
            if (InterruptMasterEnable)
            {
                for (byte i = 0; i < 8; i++)
                {
                    if (Utility.IsBitSet(MyCore.MyMemory.Read(0xFFFF), i))
                    {
                        Utility.SetBit(ref MyCore.MyMemory.GameBoyRAM[0xFF0F], i, SBMode.Off);
                        InterruptMasterEnable = false;
                        PUSH((ushort)(ProgramCounter.Word));
                        ProgramCounter.Word = (ushort)(MyCore.MyMemory.ReadWord(InterruptAddresses[i]));
                        break;
                    }
                }
            }
        }



        public void UpdateSpecialRegisters(int Cycles)
        {
            //DIV 
            DIV_Counter -= Cycles;
            if (DIV_Counter <= 0)
            {
                if (MyCore.MyMemory.GameBoyRAM[0xFF04] == 0xFF)
                {
                    MyCore.MyMemory.GameBoyRAM[0xFF04] = 0;
                }
                else
                {
                    MyCore.MyMemory.GameBoyRAM[0xFF04]++;
                }
                DIV_Counter = 256;
            }

            //TIMA 
            TIMA_Counter -= Cycles;
            if (TIMA_Counter <= 0)
            {
                if (MyCore.MyMemory.GameBoyRAM[0xFF05] == 0xFF)
                {
                    MyCore.MyMemory.GameBoyRAM[0xFF05] = MyCore.MyMemory.GameBoyRAM[0xFF06];
                    Utility.SetBit(ref MyCore.MyMemory.GameBoyRAM[0xFF0F], 2, SBMode.On);
                }
                else
                {
                    MyCore.MyMemory.GameBoyRAM[0xFF05]++;
                }
                TIMA_Counter = CYCLESPERSECOND / TIMA_Frequencies[MyCore.MyMemory.GameBoyRAM[0xFF07] & 3];
            }
        }
    }

    public struct Register
    {
        public byte High;
        public byte Low;

        public ushort Word
        {
            get { return (ushort)((High << 8) + Low); }
            set { Low = (byte)(value & 0x00FF); High = (byte)((value & 0xFF00) >> 8); }
        }
    }
}