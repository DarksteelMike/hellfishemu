using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sharp8_V3
{
    public partial class DebuggerForm : Form
    {
        private Emulator AttachedEmulator;
        private DrawItemEventHandler MemoryDraw;
        private VoidNoParams ForceDisplayUpdate;

        public DebuggerForm(Emulator AttachedEmu,VoidNoParams FDU)
        {
            InitializeComponent();
            AttachedEmulator = AttachedEmu;
            UpdateInfo();
            ForceDisplayUpdate = FDU;
            ForceDisplayUpdate();

            MemoryDraw = new DrawItemEventHandler(vlbMemory_DrawItem);
            vlbMemory.Count = 0x1000;
            vlbMemory.DrawItem += MemoryDraw;            
        }

        void vlbMemory_DrawItem(object sender, DrawItemEventArgs e)
        {
            string text = Convert.ToString(e.Index, 16).PadLeft(3, '0').ToUpper() + ": " + Convert.ToString((int)AttachedEmulator.Memory.ReadByte((ushort)e.Index), 16).PadLeft(2, '0').ToUpper();

            vlbMemory.DefaultDrawItem(e, text);
        }

        public void UpdateInfo()
        {
            tbRegister0.Text = Convert.ToString((int)AttachedEmulator.Registers[0], 16).PadLeft(2, '0').ToUpper();
            tbRegister1.Text = Convert.ToString((int)AttachedEmulator.Registers[1], 16).PadLeft(2, '0').ToUpper();
            tbRegister2.Text = Convert.ToString((int)AttachedEmulator.Registers[2], 16).PadLeft(2, '0').ToUpper();
            tbRegister3.Text = Convert.ToString((int)AttachedEmulator.Registers[3], 16).PadLeft(2, '0').ToUpper();
            tbRegister4.Text = Convert.ToString((int)AttachedEmulator.Registers[4], 16).PadLeft(2, '0').ToUpper();
            tbRegister5.Text = Convert.ToString((int)AttachedEmulator.Registers[5], 16).PadLeft(2, '0').ToUpper();
            tbRegister6.Text = Convert.ToString((int)AttachedEmulator.Registers[6], 16).PadLeft(2, '0').ToUpper();
            tbRegister7.Text = Convert.ToString((int)AttachedEmulator.Registers[7], 16).PadLeft(2, '0').ToUpper();
            tbRegister8.Text = Convert.ToString((int)AttachedEmulator.Registers[8], 16).PadLeft(2, '0').ToUpper();
            tbRegister9.Text = Convert.ToString((int)AttachedEmulator.Registers[9], 16).PadLeft(2, '0').ToUpper();
            tbRegisterA.Text = Convert.ToString((int)AttachedEmulator.Registers[0xA], 16).PadLeft(2, '0').ToUpper();
            tbRegisterB.Text = Convert.ToString((int)AttachedEmulator.Registers[0xB], 16).PadLeft(2, '0').ToUpper();
            tbRegisterC.Text = Convert.ToString((int)AttachedEmulator.Registers[0xC], 16).PadLeft(2, '0').ToUpper();
            tbRegisterD.Text = Convert.ToString((int)AttachedEmulator.Registers[0xD], 16).PadLeft(2, '0').ToUpper();
            tbRegisterE.Text = Convert.ToString((int)AttachedEmulator.Registers[0xE], 16).PadLeft(2, '0').ToUpper();
            tbRegisterF.Text = Convert.ToString((int)AttachedEmulator.Registers[0xF], 16).PadLeft(2, '0').ToUpper();

            tbProgramCounter.Text = Convert.ToString((int)AttachedEmulator.ProgramCounter, 16).PadLeft(4, '0').ToUpper();
            tbAddressRegister.Text = Convert.ToString((int)AttachedEmulator.AddressRegister, 16).PadLeft(4, '0').ToUpper();
            tbDelayTimer.Text = Convert.ToString((int)AttachedEmulator.DelayTimer, 16).PadLeft(2, '0').ToUpper();
            tbSoundTimer.Text = Convert.ToString((int)AttachedEmulator.SoundTimer, 16).PadLeft(2, '0').ToUpper();
            ushort PC = AttachedEmulator.Memory.ReadWord(AttachedEmulator.ProgramCounter);
            tbCurrentOpcode.Text = Convert.ToString((int)PC, 16).PadLeft(4, '0').ToUpper();
            lblOpcodeDescription.Text = OpcodeMeaning(PC);
            lblMode.Text = AttachedEmulator.IsInSCHIPMode ? "SCHIP" : "CHIP8";

            lbProgramStack.Items.Clear();
            foreach (ushort entry in AttachedEmulator.ProgramStack)
            {
                lbProgramStack.Items.Add(Convert.ToString((int)entry, 16));
            }
        }

        private void bStepOne_Click(object sender, EventArgs e)
        {
            if (AttachedEmulator.DoNextOpcode())
            {
                ForceDisplayUpdate();
            }
            UpdateInfo();
            vlbMemory.Invalidate();
        }

        private void bRunToggle_Click(object sender, EventArgs e)
        {
            if (AttachedEmulator.IsPaused)
            {
                vlbMemory.DrawItem -= MemoryDraw;
                AttachedEmulator.IsPaused = false;
                bRunToggle.Text = "Stop";
            }
            else
            {
                vlbMemory.DrawItem += MemoryDraw;
                AttachedEmulator.IsPaused = true;
                UpdateInfo();
                bRunToggle.Text = "Run";
            }
        }

        public string OpcodeMeaning(ushort FullOpCode)
        {
            string text = "";

            ushort LastThreeNibbles;
            byte FirstByte, SecondByte;
            byte FirstNibble, SecondNibble, ThirdNibble, FourthNibble;
            LastThreeNibbles = (ushort)(FullOpCode & 0x0FFF);
            FirstByte = (byte)((FullOpCode & 0xFF00) >> 8);
            SecondByte = (byte)(FullOpCode & 0x00FF);
            FirstNibble = (byte)((FullOpCode & 0xF000) >> 12);
            SecondNibble = (byte)((FullOpCode & 0x0F00) >> 8);
            ThirdNibble = (byte)((FullOpCode & 0x00F0) >> 4);
            FourthNibble = (byte)(FullOpCode & 0x000F);
            //Return description of the correct opcode depending on the bits set
            switch (FirstNibble)
            {
                case (0x0): //Subset
                    switch (ThirdNibble)
                    {
                        case (0xC): //
                            text = "00CN: Scroll down N lines";
                            break;
                        case (0xE): //Subset
                            switch (FourthNibble)
                            {
                                case (0x0):
                                    text = "00E0: Erase the screen";
                                    break;

                                case (0xE):
                                    text = "00EE: Return from a subroutine";
                                    break;
                            }
                            break;

                        case (0xF): //Subset
                            switch (FourthNibble)
                            {
                                case (0xB):
                                    text = "00FB: Scroll 4 or 2 pixels right (4 for SCHIP, 2 otherwise)";
                                    break;
                                case (0xC):
                                    text = "00FC: Scroll 4 or 2 pixels left (4 for SCHIP, 2 otherwise)";
                                    break;

                                case (0xD):
                                    text = "00FD: Quit the emulator";
                                    break;

                                case (0xE):
                                    text = "00FE: Set CHIP8 Graphics Mode";
                                    break;

                                case (0xF):
                                    text = "00FF: Set SCHIP Graphics Mode";
                                    break;
                            }
                            break;

                    }
                    break;

                case (0x1):
                    text = "1NNN: Unconditional jump to NNN";
                    break;

                case (0x2):
                    text = "2NNN: Subroutine call to NNN";
                    break;

                case (0x3):
                    text = "3XKK: Skip next instruction if Register X == KK";
                    break;

                case (0x4):
                    text = "4XKK: Skip next instruction if Register X != KK";
                    break;

                case (0x5):
                    text = "5XY0: Skip next instruction if Register X == Register Y";
                    break;

                case (0x6):
                    text = "6XKK: Register X = KK";
                    break;

                case (0x7):
                    text = "7XKK: Register X += KK, Carry not affected";
                    break;

                case (0x8): //Subset
                    switch (FourthNibble)
                    {
                        case (0x0):
                            text = "8XY0: Register X = Register Y";
                            break;

                        case (0x1):
                            text = "8XY1: Register X |= Register Y";
                            break;

                        case (0x2):
                            text = "8XY2: Register X &= Register Y";
                            break;

                        case (0x3):
                            text = "8XY3: Register X ^= Register Y";
                            break;

                        case (0x4):
                            text = "8XY4: Register X += Register Y, Register 0xF = Carry";
                            break;

                        case (0x5):
                            text = "8XY5: Register X -= Register Y, Register 0xF = Not borrow";
                            break;

                        case (0x6):
                            text = "8XY6: Register X >>= 1. Register 0xF = Carry";
                            break;

                        case (0x7):
                            text = "8XY7: Register X = Register Y - Register X. Register 0xF = Not Borrow";
                            break;

                        case (0xE):
                            text = "8XYE: Register X <<= 1. Register 0xF = Carry";
                            break;
                    }
                    break;

                case (0x9):
                    text = "9XY0: Skip next instruction if Register X != Register Y";
                    break;

                case (0xA): 
                    text = "ANNN: Address register = NNN";
                    break;

                case (0xB):
                    text = "BNNN: Unconditional jump to NNN + Register 0";
                    break;

                case (0xC):
                    text = "CXKK: Register X = Random Number & KK";
                    break;

                case (0xD):
                    text = "DXYN: Draw sprite (located at Address Register) at position Register X,Register Y. Sprite is N high";
                    break;

                case (0xE): //Subset
                    switch (SecondByte)
                    {
                        case (0x9E):
                            text = "EX9E: Skip next instruction if key Register X is down";
                            break;

                        case (0xA1):
                            text = "EXA1: Skip next instruction if key Register X is up";
                            break;
                    }
                    break;

                case (0xF): //Subset
                    switch (SecondByte)
                    {
                        case (0x07):
                            text = "FX07: Register X = Delay Timer";
                            break;

                        case (0x0A):
                            text = "FX0A: Wait for a keypress and store it in Register X";
                            break;

                        case (0x15):
                            text = "FX15: Delay Timer = Register X";
                            break;

                        case (0x18):
                            text = "FX18: Sound Timer = Register X";
                            break;

                        case (0x1E):
                            text = "FX1E: Address Register += Register X";
                            break;

                        case (0x29):
                            text = "FX29: Address Register = Address of font sprite for character in Register X";
                            break;

                        case (0x33):
                            text = "FX33: Store BCD representation of Register X in memory starting at Address Register";
                            break;

                        case (0x55):
                            text = "FX55: Store Registers 0 through X in memory starting at Address Register";
                            break;

                        case (0x65):
                            text = "FX65: Load Registers 0 through X from memory starting at Address Register";
                            break;
                    }
                    break;
            }
            return text;
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            AttachedEmulator.Reset();
            UpdateInfo();
        }

        private void DebuggerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AttachedEmulator.IsPaused = false;
        }
    }
}