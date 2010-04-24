using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SharpBoy
{
    public partial class frmDebugger : Form
    {
        private Emulation.Core MyCore;
        private System.Drawing.Graphics SpecDrawing;

        public frmDebugger(Emulation.Core C)
        {
            SpecDrawing = System.Drawing.Graphics.FromHwnd(this.Handle);           

            MyCore = C;
            InitializeComponent();

            vlbRAM.Count = 0x10000;
            vlbROM.Count = MyCore.MyMemory.ROM.Length-1;
            RefreshData();
        }

        public void RefreshData()
        {
            tbA.Text = "0x" + Convert.ToString(MyCore.MyCPU.AF.High,16).PadLeft(2,'0').ToUpper();
            tbB.Text = "0x" + Convert.ToString(MyCore.MyCPU.BC.High, 16).PadLeft(2, '0').ToUpper();
            tbC.Text = "0x" + Convert.ToString(MyCore.MyCPU.BC.Low, 16).PadLeft(2, '0').ToUpper();
            tbD.Text = "0x" + Convert.ToString(MyCore.MyCPU.DE.High, 16).PadLeft(2, '0').ToUpper();
            tbE.Text = "0x" + Convert.ToString(MyCore.MyCPU.DE.Low, 16).PadLeft(2, '0').ToUpper();
            tbF.Text = "0x" + Convert.ToString(MyCore.MyCPU.AF.Low, 16).PadLeft(2, '0').ToUpper();
            tbHL.Text = "0x" + Convert.ToString(MyCore.MyCPU.HL.Word, 16).PadLeft(2, '0').ToUpper(); ;
            tbPC.Text = "0x" + Convert.ToString(MyCore.MyCPU.ProgramCounter.Word, 16).PadLeft(4, '0').ToUpper();
            tbSP.Text = "0x" + Convert.ToString(MyCore.MyCPU.StackPointer.Word, 16).PadLeft(4, '0').ToUpper();

            lblTIMAUpdate.Text = MyCore.MyCPU.TIMA_Counter.ToString();
            lblDIVUpdate.Text = MyCore.MyCPU.DIV_Counter.ToString();
            lblCyclesRun.Text = MyCore.MyCPU.CyclesRunThisFrame.ToString();

            cbC.Checked = Utility.IsBitSet(MyCore.MyCPU.AF.Low, Emulation.CPU.FLAG_C);
            cbH.Checked = Utility.IsBitSet(MyCore.MyCPU.AF.Low, Emulation.CPU.FLAG_H);
            cbN.Checked = Utility.IsBitSet(MyCore.MyCPU.AF.Low, Emulation.CPU.FLAG_N);
            cbZ.Checked = Utility.IsBitSet(MyCore.MyCPU.AF.Low, Emulation.CPU.FLAG_Z);

            vlbRAM.Refresh();
            vlbROM.Refresh();
            vlbROM.Update();
            vlbRAM.Update();
        }

        private void bStep_Click(object sender, EventArgs e)
        {
            MyCore.MyCPU.DoStep();
            RefreshData();
        }

        private void vlbROM_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                //Construct output
                //Address
                string output = "$" + Convert.ToString(e.Index, 16).PadLeft(4, '0').ToUpper() + ": ";

                //Content
                output += Convert.ToString(MyCore.MyMemory.GameBoyRAM[e.Index], 16).PadLeft(2, '0').ToUpper();

                //Disassembly
                if (e.Index != 0)
                {
                    if (e.Index != 1) //2 params
                    {
                        if (TakesParameters(MyCore.MyMemory.GameBoyRAM[e.Index - 1]) == 0 && TakesParameters(MyCore.MyMemory.GameBoyRAM[e.Index - 2]) < 2)
                        {
                            output += "(" + TranslateOpcode(MyCore.MyMemory.GameBoyRAM[e.Index], (e.Index < 0xFFFF) ? MyCore.MyMemory.GameBoyRAM[e.Index + 1] : MyCore.MyMemory.GameBoyRAM[e.Index]) + ")";
                        }
                    }
                    else//1 param
                    {
                        if (TakesParameters(MyCore.MyMemory.GameBoyRAM[e.Index - 1]) == 0)
                        {
                            output += "(" + TranslateOpcode(MyCore.MyMemory.GameBoyRAM[e.Index], (e.Index < 0xFFFF) ? MyCore.MyMemory.GameBoyRAM[e.Index + 1] : MyCore.MyMemory.GameBoyRAM[e.Index]) + ")";
                        }
                    }
                }
                else //0 params
                {
                    output += "(" + TranslateOpcode(MyCore.MyMemory.GameBoyRAM[e.Index], (e.Index < 0xFFFF) ? MyCore.MyMemory.GameBoyRAM[e.Index + 1] : MyCore.MyMemory.GameBoyRAM[e.Index]) + ")";
                }
                vlbRAM.DefaultDrawItem(e, output);
            }
        }

        private void vlbRAM_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                //Construct output
                //Address
                string output = "$" + Convert.ToString(e.Index, 16).PadLeft(4, '0').ToUpper() + ": "; 
                
                //Content
                output += Convert.ToString(MyCore.MyMemory.GameBoyRAM[e.Index], 16).PadLeft(2, '0').ToUpper();
                
                //Disassembly
                if(e.Index != 0) 
                {
                    if (e.Index != 1) //2 params
                    {
                        if (TakesParameters(MyCore.MyMemory.GameBoyRAM[e.Index - 1]) == 0 && TakesParameters(MyCore.MyMemory.GameBoyRAM[e.Index - 2]) < 2)
                        {
                            output += "(" + TranslateOpcode(MyCore.MyMemory.GameBoyRAM[e.Index], (e.Index < 0xFFFF) ? MyCore.MyMemory.GameBoyRAM[e.Index + 1] : MyCore.MyMemory.GameBoyRAM[e.Index]) + ")";
                        }
                    }
                    else//1 param
                    {
                        if (TakesParameters(MyCore.MyMemory.GameBoyRAM[e.Index - 1]) == 0)
                        {
                            output += "(" + TranslateOpcode(MyCore.MyMemory.GameBoyRAM[e.Index], (e.Index < 0xFFFF) ? MyCore.MyMemory.GameBoyRAM[e.Index + 1] : MyCore.MyMemory.GameBoyRAM[e.Index]) + ")";
                        }
                    }
                }
                else //0 params
                {
                    output += "(" + TranslateOpcode(MyCore.MyMemory.GameBoyRAM[e.Index], (e.Index < 0xFFFF) ? MyCore.MyMemory.GameBoyRAM[e.Index + 1] : MyCore.MyMemory.GameBoyRAM[e.Index]) + ")";
                }
                 
                if (e.Index == MyCore.MyCPU.ProgramCounter.Word)
                {
                    e.DrawBackground();
                    e.DrawFocusRectangle();
                    e.Graphics.DrawString(output, new Font(FontFamily.GenericMonospace, (float)(8.25), FontStyle.Bold), new SolidBrush(Color.Red), e.Bounds);
                }
                else
                {
                    vlbRAM.DefaultDrawItem(e, output);
                }
            }
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            MyCore.Reset();
        }

        private int TakesParameters(byte Opcode)
        {
            int ret = 0;
            switch (Opcode)
            {
                case (0x01): ret = 2; break;
                case (0x06): ret = 1; break;
                case (0x08): ret = 2; break;
                case (0x0E): ret = 1; break;

                case (0x11): ret = 2; break;
                case (0x16): ret = 1; break;
                case (0x18): ret = 1; break;
                case (0x1E): ret = 1; break;

                case (0x20): ret = 1; break;
                case (0x21): ret = 2; break;
                case (0x26): ret = 1; break;
                case (0x28): ret = 1; break;
                case (0x2E): ret = 1; break;

                case (0x30): ret = 1; break;
                case (0x31): ret = 2; break;
                case (0x36): ret = 1; break;
                case (0x38): ret = 1; break;
                case (0x3E): ret = 1; break;

                case (0xC2): ret = 2; break;
                case (0xC3): ret = 2; break;
                case (0xC4): ret = 2; break;
                case (0xC6): ret = 1; break;
                case (0xCA): ret = 2; break;
                case (0xCB): ret = 1; break;
                case (0xCC): ret = 2; break;
                case (0xCD): ret = 2; break;
                case (0xCE): ret = 1; break;

                case (0xD2): ret = 2; break;
                case (0xD4): ret = 2; break;
                case (0xD6): ret = 1; break;
                case (0xDA): ret = 2; break;
                case (0xDC): ret = 2; break;
                case (0xDE): ret = 1; break;

                case (0xE0): ret = 1; break;
                case (0xE6): ret = 1; break;
                case (0xEA): ret = 2; break;
                case (0xEE): ret = 1; break;

                case (0xF0): ret = 1; break;
                case (0xF6): ret = 1; break;
                case (0xFA): ret = 2; break;
                case (0xFE): ret = 1; break;
            }

            return ret;
        }

        private string TranslateOpcode(byte Opcode1,byte Opcode2)
        {
            string ret = "";
            switch (Opcode1)
            {
                case (0x00): ret = "NOP"; break;
                case (0x01): ret = "LD BC,nn"; break;
                case (0x02): ret = "LD (BC),A"; break;
                case (0x03): ret = "INC BC"; break;
                case (0x04): ret = "INC B"; break;
                case (0x05): ret = "DEC B"; break;
                case (0x06): ret = "LD B,n"; break;
                case (0x07): ret = "RLC A"; break;
                case (0x08): ret = "LD (nn),SP"; break;
                case (0x09): ret = "ADD HL,BC"; break;
                case (0x0A): ret = "LD A,(BC)"; break;
                case (0x0B): ret = "DEC BC"; break;
                case (0x0C): ret = "INC C"; break;
                case (0x0D): ret = "DEC C"; break;
                case (0x0E): ret = "LD C,n"; break;
                case (0x0F): ret = "RRC A"; break;

                case (0x10): ret = "STOP"; break;
                case (0x11): ret = "LD DE,nn"; break;
                case (0x12): ret = "LD (DE),A"; break;
                case (0x13): ret = "INC DE"; break;
                case (0x14): ret = "INC D"; break;
                case (0x15): ret = "DEC D"; break;
                case (0x16): ret = "LD D,n"; break;
                case (0x17): ret = "RL A"; break;
                case (0x18): ret = "JR n"; break;
                case (0x19): ret = "ADD HL,DE"; break;
                case (0x1A): ret = "LD A,(DE)"; break;
                case (0x1B): ret = "DEC DE"; break;
                case (0x1C): ret = "INC E"; break;
                case (0x1D): ret = "DEC E"; break;
                case (0x1E): ret = "LD E,n"; break;
                case (0x1F): ret = "RR A"; break;

                case (0x20): ret = "JR NZ,n"; break;
                case (0x21): ret = "LD HL,nn"; break;
                case (0x22): ret = "LDI (HL),A"; break;
                case (0x23): ret = "INC HL"; break;
                case (0x24): ret = "INC H"; break;
                case (0x25): ret = "DEC H"; break;
                case (0x26): ret = "LD H,n"; break;
                case (0x27): ret = "DAA"; break;
                case (0x28): ret = "JR Z,n"; break;
                case (0x29): ret = "ADD HL,HL"; break;
                case (0x2A): ret = "LDI A,(HL)"; break;
                case (0x2B): ret = "DEC HL"; break;
                case (0x2C): ret = "INC L"; break;
                case (0x2D): ret = "DEC L"; break;
                case (0x2E): ret = "LD L,n"; break;
                case (0x2F): ret = "CPL"; break;

                case (0x30): ret = "JR NC,n"; break;
                case (0x31): ret = "LD SP,nn"; break;
                case (0x32): ret = "LDD (HL),A"; break;
                case (0x33): ret = "INC SP"; break;
                case (0x34): ret = "INC (HL)"; break;
                case (0x35): ret = "DEC (HL)"; break;
                case (0x36): ret = "LD (HL),n"; break;
                case (0x37): ret = "SCF"; break;
                case (0x38): ret = "JR C,n"; break;
                case (0x39): ret = "ADD HL,SP"; break;
                case (0x3A): ret = "LDD A,(HL)"; break;
                case (0x3B): ret = "DEC SP"; break;
                case (0x3C): ret = "INC A"; break;
                case (0x3D): ret = "DEC A"; break;
                case (0x3E): ret = "LD A,n"; break;
                case (0x3F): ret = "CCF"; break;

                case(0x40): ret = "LD B,B"; break;
                case(0x41): ret = "LD B,C"; break;
                case(0x42): ret = "LD B,D"; break;
                case(0x43): ret = "LD B,E"; break;
                case(0x44): ret = "LD B,H"; break;
                case(0x45): ret = "LD B,L"; break;
                case(0x46): ret = "LD B,(HL)"; break;
                case(0x47): ret = "LD B,A"; break;
                case(0x48): ret = "LD C,B"; break;
                case(0x49): ret = "LD C,C"; break;
                case(0x4A): ret = "LD C,D"; break;
                case(0x4B): ret = "LD C,E"; break;
                case(0x4C): ret = "LD C,H"; break;
                case(0x4D): ret = "LD C,L"; break;
                case(0x4E): ret = "LD C,(HL)"; break;
                case(0x4F): ret = "LD C,A"; break;

                case (0x50): ret = "LD D,B"; break;
                case (0x51): ret = "LD D,C"; break;
                case (0x52): ret = "LD D,D"; break;
                case (0x53): ret = "LD D,E"; break;
                case (0x54): ret = "LD D,H"; break;
                case (0x55): ret = "LD D,L"; break;
                case (0x56): ret = "LD D,(HL)"; break;
                case (0x57): ret = "LD D,A"; break;
                case (0x58): ret = "LD E,B"; break;
                case (0x59): ret = "LD E,C"; break;
                case (0x5A): ret = "LD E,D"; break;
                case (0x5B): ret = "LD E,E"; break;
                case (0x5C): ret = "LD E,H"; break;
                case (0x5D): ret = "LD E,L"; break;
                case (0x5E): ret = "LD E,(HL)"; break;
                case (0x5F): ret = "LD E,A"; break;

                case (0x60): ret = "LD H,B"; break;
                case (0x61): ret = "LD H,C"; break;
                case (0x62): ret = "LD H,D"; break;
                case (0x63): ret = "LD H,E"; break;
                case (0x64): ret = "LD H,H"; break;
                case (0x65): ret = "LD H,L"; break;
                case (0x66): ret = "LD H,(HL)"; break;
                case (0x67): ret = "LD H,A"; break;
                case (0x68): ret = "LD L,B"; break;
                case (0x69): ret = "LD L,C"; break;
                case (0x6A): ret = "LD L,D"; break;
                case (0x6B): ret = "LD L,E"; break;
                case (0x6C): ret = "LD L,H"; break;
                case (0x6D): ret = "LD L,L"; break;
                case (0x6E): ret = "LD L,(HL)"; break;
                case (0x6F): ret = "LD L,A"; break;

                case (0x70): ret = "LD (HL),B"; break;
                case (0x71): ret = "LD (HL),C"; break;
                case (0x72): ret = "LD (HL),D"; break;
                case (0x73): ret = "LD (HL),E"; break;
                case (0x74): ret = "LD (HL),H"; break;
                case (0x75): ret = "LD (HL),L"; break;
                case (0x76): ret = "HALT"; break;
                case (0x77): ret = "LD (HL),A"; break;
                case (0x78): ret = "LD A,B"; break;
                case (0x79): ret = "LD A,C"; break;
                case (0x7A): ret = "LD A,D"; break;
                case (0x7B): ret = "LD A,E"; break;
                case (0x7C): ret = "LD A,H"; break;
                case (0x7D): ret = "LD A,L"; break;
                case (0x7E): ret = "LD A,(HL)"; break;
                case (0x7F): ret = "LD A,A"; break;

                case (0x80): ret = "ADD A,B"; break;
                case (0x81): ret = "ADD A,C"; break;
                case (0x82): ret = "ADD A,D"; break;
                case (0x83): ret = "ADD A,E"; break;
                case (0x84): ret = "ADD A.H"; break;
                case (0x85): ret = "ADD A,L"; break;
                case (0x86): ret = "ADD A,(HL)"; break;
                case (0x87): ret = "ADD A,A"; break;
                case (0x88): ret = "ADC A,B"; break;
                case (0x89): ret = "ADC A,C"; break;
                case (0x8A): ret = "ADC A,D"; break;
                case (0x8B): ret = "ADC A,E"; break;
                case (0x8C): ret = "ADC A,H"; break;
                case (0x8D): ret = "ADC A,L"; break;
                case (0x8E): ret = "ADC A,(HL)"; break;
                case (0x8F): ret = "ADC A,A"; break;

                case (0x90): ret = "SUB A,B"; break;
                case (0x91): ret = "SUB A,C"; break;
                case (0x92): ret = "SUB A,D"; break;
                case (0x93): ret = "SUB A,E"; break;
                case (0x94): ret = "SUB A,H"; break;
                case (0x95): ret = "SUB A,L"; break;
                case (0x96): ret = "SUB A,(HL)"; break;
                case (0x97): ret = "SUB A,A"; break;
                case (0x98): ret = "SBC A,B"; break;
                case (0x99): ret = "SBC A,C"; break;
                case (0x9A): ret = "SBC A,D"; break;
                case (0x9B): ret = "SBC A,E"; break;
                case (0x9C): ret = "SBC A,H"; break;
                case (0x9D): ret = "SBC A,L"; break;
                case (0x9E): ret = "SBC A,(HL)"; break;
                case (0x9F): ret = "SBC A,A"; break;

                case (0xA0): ret = "AND B"; break;
                case (0xA1): ret = "AND C"; break;
                case (0xA2): ret = "AND D"; break;
                case (0xA3): ret = "AND E"; break;
                case (0xA4): ret = "AND H"; break;
                case (0xA5): ret = "AND L"; break;
                case (0xA6): ret = "AND (HL)"; break;
                case (0xA7): ret = "AND A"; break;
                case (0xA8): ret = "XOR B"; break;
                case (0xA9): ret = "XOR C"; break;
                case (0xAA): ret = "XOR D"; break;
                case (0xAB): ret = "XOR E"; break;
                case (0xAC): ret = "XOR H"; break;
                case (0xAD): ret = "XOR L"; break;
                case (0xAE): ret = "XOR (HL)"; break;
                case (0xAF): ret = "XOR A"; break;

                case (0xB0): ret = "OR B"; break;
                case (0xB1): ret = "OR C"; break;
                case (0xB2): ret = "OR D"; break;
                case (0xB3): ret = "OR E"; break;
                case (0xB4): ret = "OR H"; break;
                case (0xB5): ret = "OR L"; break;
                case (0xB6): ret = "OR (HL)"; break;
                case (0xB7): ret = "OR A"; break;
                case (0xB8): ret = "CP B"; break;
                case (0xB9): ret = "CP C"; break;
                case (0xBA): ret = "CP D"; break;
                case (0xBB): ret = "CP E"; break;
                case (0xBC): ret = "CP H"; break;
                case (0xBD): ret = "CP L"; break;
                case (0xBE): ret = "CP (HL)"; break;
                case (0xBF): ret = "CP A"; break;

                case (0xC0): ret = "RET NZ"; break;
                case (0xC1): ret = "POP BC"; break;
                case (0xC2): ret = "JP NZ,nn"; break;
                case (0xC3): ret = "JP nn"; break;
                case (0xC4): ret = "CALL NZ,nn"; break;
                case (0xC5): ret = "PUSH BC"; break;
                case (0xC6): ret = "ADD A,n"; break;
                case (0xC7): ret = "RST 0"; break;
                case (0xC8): ret = "RET Z"; break;
                case (0xC9): ret = "RET"; break;
                case (0xCA): ret = "JP Z,nn"; break;
                case (0xCB): ret = CB_Subset(Opcode2); break;
                case (0xCC): ret = "CALL Z,nn"; break;
                case (0xCD): ret = "CALL nn"; break;
                case (0xCE): ret = "ADC A,n"; break;
                case (0xCF): ret = "RST 8"; break;

                case (0xD0): ret = "RET NC"; break;
                case (0xD1): ret = "POP DE"; break;
                case (0xD2): ret = "JP NC,nn"; break;
                case (0xD3): ret = "Invalid"; break;
                case (0xD4): ret = "CALL NC,nn"; break;
                case (0xD5): ret = "PUSH DE"; break;
                case (0xD6): ret = "SUB A,n"; break;
                case (0xD7): ret = "RST 10"; break;
                case (0xD8): ret = "RET C"; break;
                case (0xD9): ret = "RETI"; break;
                case (0xDA): ret = "JP C,nn"; break;
                case (0xDB): ret = "Invalid"; break;
                case (0xDC): ret = "CALL C,nn"; break;
                case (0xDD): ret = "Invalid"; break;
                case (0xDE): ret = "SBC A,n"; break;
                case (0xDF): ret = "RST 18"; break;

                case (0xE0): ret = "LDH (n),A"; break;
                case (0xE1): ret = "POP HL"; break;
                case (0xE2): ret = "LDH (C),A"; break;
                case (0xE3): ret = "Invalid"; break;
                case (0xE4): ret = "Invalid"; break;
                case (0xE5): ret = "PUSH HL"; break;
                case (0xE6): ret = "AND n"; break;
                case (0xE7): ret = "RST 20"; break;
                case (0xE8): ret = "ADD SP,d"; break;
                case (0xE9): ret = "JP (HL)"; break;
                case (0xEA): ret = "LD (nn),A"; break;
                case (0xEB): ret = "Invalid"; break;
                case (0xEC): ret = "Invalid"; break;
                case (0xED): ret = "Invalid"; break;
                case (0xEE): ret = "OR n"; break;
                case (0xEF): ret = "RST 28"; break;

                case (0xF0): ret = "LDH A,(n)"; break;
                case (0xF1): ret = "POP AF"; break;
                case (0xF2): ret = "Invalid"; break;
                case (0xF3): ret = "DI"; break;
                case (0xF4): ret = "Invalid"; break;
                case (0xF5): ret = "PUSH AF"; break;
                case (0xF6): ret = "OR n"; break;
                case (0xF7): ret = "RST 30"; break;
                case (0xF8): ret = "LDHL SP,d"; break;
                case (0xF9): ret = "LD SP,HL"; break;
                case (0xFA): ret = "LD A,(nn)"; break;
                case (0xFB): ret = "EI"; break;
                case (0xFC): ret = "Invalid"; break;
                case (0xFD): ret = "Invalid"; break;
                case (0xFE): ret = "CP n"; break;
                case (0xFF): ret = "RST 38"; break;
            }

            return ret;
        }

        private string CB_Subset(byte Opcode)
        {
            string ret = "";
            switch (Opcode)
            {
                case (0x00): ret = "RLC B"; break;
                case (0x01): ret = "RLC C"; break;
                case (0x02): ret = "RLC D"; break;
                case (0x03): ret = "RLC E"; break;
                case (0x04): ret = "RLC H"; break;
                case (0x05): ret = "RLC L"; break;
                case (0x06): ret = "RLC (HL)"; break;
                case (0x07): ret = "RLC A"; break;
                case (0x08): ret = "RRC B"; break;
                case (0x09): ret = "RRC C"; break;
                case (0x0A): ret = "RRC D"; break;
                case (0x0B): ret = "RRC E"; break;
                case (0x0C): ret = "RRC H"; break;
                case (0x0D): ret = "RRC L"; break;
                case (0x0E): ret = "RRC (HL)"; break;
                case (0x0F): ret = "RRC A"; break;

                case (0x10): ret = "RL B"; break;
                case (0x11): ret = "RL C"; break;
                case (0x12): ret = "RL D"; break;
                case (0x13): ret = "RL E"; break;
                case (0x14): ret = "RL H"; break;
                case (0x15): ret = "RL L"; break;
                case (0x16): ret = "RL (HL)"; break;
                case (0x17): ret = "RL A"; break;
                case (0x18): ret = "RR B"; break;
                case (0x19): ret = "RR C"; break;
                case (0x1A): ret = "RR D"; break;
                case (0x1B): ret = "RR E"; break;
                case (0x1C): ret = "RR H"; break;
                case (0x1D): ret = "RR L"; break;
                case (0x1E): ret = "RR (HL)"; break;
                case (0x1F): ret = "RR A"; break;

                case (0x20): ret = "SLA B"; break;
                case (0x21): ret = "SLA C"; break;
                case (0x22): ret = "SLA D"; break;
                case (0x23): ret = "SLA E"; break;
                case (0x24): ret = "SLA H"; break;
                case (0x25): ret = "SLA L"; break;
                case (0x26): ret = "SLA (HL)"; break;
                case (0x27): ret = "SLA A"; break;
                case (0x28): ret = "SRA B"; break;
                case (0x29): ret = "SRA C"; break;
                case (0x2A): ret = "SRA D"; break;
                case (0x2B): ret = "SRA E"; break;
                case (0x2C): ret = "SRA H"; break;
                case (0x2D): ret = "SRA L"; break;
                case (0x2E): ret = "SRA (HL)"; break;
                case (0x2F): ret = "SRA A"; break;

                case (0x30): ret = "SWAP B"; break;
                case (0x31): ret = "SWAP C"; break;
                case (0x32): ret = "SWAP D"; break;
                case (0x33): ret = "SWAP E"; break;
                case (0x34): ret = "SWAP H"; break;
                case (0x35): ret = "SWAP L"; break;
                case (0x36): ret = "SWAP (HL)"; break;
                case (0x37): ret = "SWAP A"; break;
                case (0x38): ret = "SRL B"; break;
                case (0x39): ret = "SRL C"; break;
                case (0x3A): ret = "SRL D"; break;
                case (0x3B): ret = "SRL E"; break;
                case (0x3C): ret = "SRL H"; break;
                case (0x3D): ret = "SRL L"; break;
                case (0x3E): ret = "SRL (HL)"; break;
                case (0x3F): ret = "SRL A"; break;

                case (0x40): ret = "BIT 0,B"; break;
                case (0x41): ret = "BIT 0,C"; break;
                case (0x42): ret = "BIT 0,D"; break;
                case (0x43): ret = "BIT 0,E"; break;
                case (0x44): ret = "BIT 0,H"; break;
                case (0x45): ret = "BIT 0,L"; break;
                case (0x46): ret = "BIT 0,(HL)"; break;
                case (0x47): ret = "BIT 0,A"; break;
                case (0x48): ret = "BIT 1,B"; break;
                case (0x49): ret = "BIT 1,C"; break;
                case (0x4A): ret = "BIT 1,D"; break;
                case (0x4B): ret = "BIT 1,E"; break;
                case (0x4C): ret = "BIT 1,H"; break;
                case (0x4D): ret = "BIT 1,L"; break;
                case (0x4E): ret = "BIT 1,(HL)"; break;
                case (0x4F): ret = "BIT 1,A"; break;

                case (0x50): ret = "BIT 2,B"; break;
                case (0x51): ret = "BIT 2,C"; break;
                case (0x52): ret = "BIT 2,D"; break;
                case (0x53): ret = "BIT 2,E"; break;
                case (0x54): ret = "BIT 2,H"; break;
                case (0x55): ret = "BIT 2,L"; break;
                case (0x56): ret = "BIT 2,(HL)"; break;
                case (0x57): ret = "BIT 2,A"; break;
                case (0x58): ret = "BIT 3,B"; break;
                case (0x59): ret = "BIT 3,C"; break;
                case (0x5A): ret = "BIT 3,D"; break;
                case (0x5B): ret = "BIT 3,E"; break;
                case (0x5C): ret = "BIT 3,H"; break;
                case (0x5D): ret = "BIT 3,L"; break;
                case (0x5E): ret = "BIT 3,(HL)"; break;
                case (0x5F): ret = "BIT 3,A"; break;

                case (0x60): ret = "BIT 4,B"; break;
                case (0x61): ret = "BIT 4,C"; break;
                case (0x62): ret = "BIT 4,D"; break;
                case (0x63): ret = "BIT 4,E"; break;
                case (0x64): ret = "BIT 4,H"; break;
                case (0x65): ret = "BIT 4,L"; break;
                case (0x66): ret = "BIT 4,(HL)"; break;
                case (0x67): ret = "BIT 4,A"; break;
                case (0x68): ret = "BIT 5,B"; break;
                case (0x69): ret = "BIT 5,C"; break;
                case (0x6A): ret = "BIT 5,D"; break;
                case (0x6B): ret = "BIT 5,E"; break;
                case (0x6C): ret = "BIT 5,H"; break;
                case (0x6D): ret = "BIT 5,L"; break;
                case (0x6E): ret = "BIT 5,(HL)"; break;
                case (0x6F): ret = "BIT 5,A"; break;

                case (0x70): ret = "BIT 6,B"; break;
                case (0x71): ret = "BIT 6,C"; break;
                case (0x72): ret = "BIT 6,D"; break;
                case (0x73): ret = "BIT 6,E"; break;
                case (0x74): ret = "BIT 6,H"; break;
                case (0x75): ret = "BIT 6,L"; break;
                case (0x76): ret = "BIT 6,(HL)"; break;
                case (0x77): ret = "BIT 6,A"; break;
                case (0x78): ret = "BIT 7,B"; break;
                case (0x79): ret = "BIT 7,C"; break;
                case (0x7A): ret = "BIT 7,D"; break;
                case (0x7B): ret = "BIT 7,E"; break;
                case (0x7C): ret = "BIT 7,H"; break;
                case (0x7D): ret = "BIT 7,L"; break;
                case (0x7E): ret = "BIT 7,(HL)"; break;
                case (0x7F): ret = "BIT 7,A"; break;

                case (0X80): ret = "RES 0,B"; break;
                case (0X81): ret = "RES 0,C"; break;
                case (0X82): ret = "RES 0,D"; break;
                case (0X83): ret = "RES 0,E"; break;
                case (0X84): ret = "RES 0,H"; break;
                case (0X85): ret = "RES 0,L"; break;
                case (0X86): ret = "RES 0,(HL)"; break;
                case (0X87): ret = "RES 0,A"; break;
                case (0X88): ret = "RES 1,B"; break;
                case (0X89): ret = "RES 1,C"; break;
                case (0X8A): ret = "RES 1,D"; break;
                case (0X8B): ret = "RES 1,E"; break;
                case (0X8C): ret = "RES 1,H"; break;
                case (0X8D): ret = "RES 1,L"; break;
                case (0X8E): ret = "RES 1,(HL)"; break;
                case (0X8F): ret = "RES 1,A"; break;

                case (0x90): ret = "RES 2,B"; break;
                case (0x91): ret = "RES 2,C"; break;
                case (0x92): ret = "RES 2,D"; break;
                case (0x93): ret = "RES 2,E"; break;
                case (0x94): ret = "RES 2,H"; break;
                case (0x95): ret = "RES 2,L"; break;
                case (0x96): ret = "RES 2,(HL)"; break;
                case (0x97): ret = "RES 2,A"; break;
                case (0x98): ret = "RES 3,B"; break;
                case (0x99): ret = "RES 3,C"; break;
                case (0x9A): ret = "RES 3,D"; break;
                case (0x9B): ret = "RES 3,E"; break;
                case (0x9C): ret = "RES 3,H"; break;
                case (0x9D): ret = "RES 3,L"; break;
                case (0x9E): ret = "RES 3,(HL)"; break;
                case (0x9F): ret = "RES 3,A"; break;

                case (0xA0): ret = "RES 4,B"; break;
                case (0xA1): ret = "RES 4,C"; break;
                case (0xA2): ret = "RES 4,D"; break;
                case (0xA3): ret = "RES 4,E"; break;
                case (0xA4): ret = "RES 4,H"; break;
                case (0xA5): ret = "RES 4,L"; break;
                case (0xA6): ret = "RES 4,(HL)"; break;
                case (0xA7): ret = "RES 4,A"; break;
                case (0xA8): ret = "RES 5,B"; break;
                case (0xA9): ret = "RES 5,C"; break;
                case (0xAA): ret = "RES 5,D"; break;
                case (0xAB): ret = "RES 5,E"; break;
                case (0xAC): ret = "RES 5,H"; break;
                case (0xAD): ret = "RES 5,L"; break;
                case (0xAE): ret = "RES 5,(HL)"; break;
                case (0xAF): ret = "RES 5,A"; break;

                case (0xB0): ret = "RES 6,B"; break;
                case (0xB1): ret = "RES 6,C"; break;
                case (0xB2): ret = "RES 6,D"; break;
                case (0xB3): ret = "RES 6,E"; break;
                case (0xB4): ret = "RES 6,H"; break;
                case (0xB5): ret = "RES 6,L"; break;
                case (0xB6): ret = "RES 6,(HL)"; break;
                case (0xB7): ret = "RES 6,A"; break;
                case (0xB8): ret = "RES 7,B"; break;
                case (0xB9): ret = "RES 7,C"; break;
                case (0xBA): ret = "RES 7,D"; break;
                case (0xBB): ret = "RES 7,E"; break;
                case (0xBC): ret = "RES 7,H"; break;
                case (0xBD): ret = "RES 7,L"; break;
                case (0xBE): ret = "RES 7,(HL)"; break;
                case (0xBF): ret = "RES 7,A"; break;

                case (0xC0): ret = "SET 0,B"; break;
                case (0xC1): ret = "SET 0,C"; break;
                case (0xC2): ret = "SET 0,D"; break;
                case (0xC3): ret = "SET 0,E"; break;
                case (0xC4): ret = "SET 0,H"; break;
                case (0xC5): ret = "SET 0,L"; break;
                case (0xC6): ret = "SET 0,(HL)"; break;
                case (0xC7): ret = "SET 0,A"; break;
                case (0xC8): ret = "SET 1,B"; break;
                case (0xC9): ret = "SET 1,C"; break;
                case (0xCA): ret = "SET 1,D"; break;
                case (0xCB): ret = "SET 1,E"; break;
                case (0xCC): ret = "SET 1,H"; break;
                case (0xCD): ret = "SET 1,L"; break;
                case (0xCE): ret = "SET 1,(HL)"; break;
                case (0xCF): ret = "SET 1,A"; break;

                case (0xD0): ret = "SET 2,B"; break;
                case (0xD1): ret = "SET 2,C"; break;
                case (0xD2): ret = "SET 2,D"; break;
                case (0xD3): ret = "SET 2,E"; break;
                case (0xD4): ret = "SET 2,H"; break;
                case (0xD5): ret = "SET 2,L"; break;
                case (0xD6): ret = "SET 2,(HL)"; break;
                case (0xD7): ret = "SET 2,A"; break;
                case (0xD8): ret = "SET 3,B"; break;
                case (0xD9): ret = "SET 3,C"; break;
                case (0xDA): ret = "SET 3,D"; break;
                case (0xDB): ret = "SET 3,E"; break;
                case (0xDC): ret = "SET 3,H"; break;
                case (0xDD): ret = "SET 3,L"; break;
                case (0xDE): ret = "SET 3,(HL)"; break;
                case (0xDF): ret = "SET 3,A"; break;

                case (0xE0): ret = "SET 4,B"; break;
                case (0xE1): ret = "SET 4,C"; break;
                case (0xE2): ret = "SET 4,D"; break;
                case (0xE3): ret = "SET 4,E"; break;
                case (0xE4): ret = "SET 4,H"; break;
                case (0xE5): ret = "SET 4,L"; break;
                case (0xE6): ret = "SET 4,(HL)"; break;
                case (0xE7): ret = "SET 4,A"; break;
                case (0xE8): ret = "SET 5,B"; break;
                case (0xE9): ret = "SET 5,C"; break;
                case (0xEA): ret = "SET 5,D"; break;
                case (0xEB): ret = "SET 5,E"; break;
                case (0xEC): ret = "SET 5,H"; break;
                case (0xED): ret = "SET 5,L"; break;
                case (0xEE): ret = "SET 5,(HL)"; break;
                case (0xEF): ret = "SET 5,A"; break;

                case (0xF0): ret = "SET 6,B"; break;
                case (0xF1): ret = "SET 6,C"; break;
                case (0xF2): ret = "SET 6,D"; break;
                case (0xF3): ret = "SET 6,E"; break;
                case (0xF4): ret = "SET 6,H"; break;
                case (0xF5): ret = "SET 6,L"; break;
                case (0xF6): ret = "SET 6,(HL)"; break;
                case (0xF7): ret = "SET 6,A"; break;
                case (0xF8): ret = "SET 7,B"; break;
                case (0xF9): ret = "SET 7,C"; break;
                case (0xFA): ret = "SET 7,D"; break;
                case (0xFB): ret = "SET 7,E"; break;
                case (0xFC): ret = "SET 7,H"; break;
                case (0xFD): ret = "SET 7,L"; break;
                case (0xFE): ret = "SET 7,(HL)"; break;
                case (0xFF): ret = "SET 7,A"; break;
            }

            return ret;
        }
    }
}