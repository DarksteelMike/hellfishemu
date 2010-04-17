using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SharpBoy2
{
    public partial class frmDebugger : Form
    {
        private Emulation.Core MyCore;
        private System.Drawing.Graphics SpecDrawing;

        public frmDebugger(SharpBoy2.Emulation.Core C)
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
            tbA.Text = MyCore.MyCPU.AF.High.ToString();
            tbB.Text = MyCore.MyCPU.BC.High.ToString();
            tbC.Text = MyCore.MyCPU.BC.Low.ToString();
            tbD.Text = MyCore.MyCPU.DE.High.ToString();
            tbE.Text = MyCore.MyCPU.DE.Low.ToString();
            tbF.Text = MyCore.MyCPU.AF.Low.ToString();
            tbHL.Text = MyCore.MyCPU.HL.Word.ToString();
            tbPC.Text = MyCore.MyCPU.ProgramCounter.Word.ToString();
            tbSP.Text = MyCore.MyCPU.StackPointer.Word.ToString();

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
                string output = "$" + Convert.ToString(MyCore.MyMemory.ROM[e.Index + 1], 16).PadLeft(2,'0');
                vlbROM.DefaultDrawItem(e, output);
            }
        }

        private void vlbRAM_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                string output = "$" + Convert.ToString(e.Index, 16).PadLeft(4, '0') + ": " + Convert.ToString(MyCore.MyMemory.GameBoyRAM[e.Index], 16).PadLeft(2,'0');
                if (e.Index == MyCore.MyCPU.ProgramCounter.Word)
                {
                    SpecDrawing.Clip = e.Graphics.Clip; 
                    //SpecDrawing.ClipBounds = e.Graphics.ClipBounds
                    //System.Drawing.Rectangle R = new Rectangle((int)e.Graphics.ClipBounds.Y, (int)e.Graphics.ClipBounds.Y, (int)e.Graphics.ClipBounds.Width, (int)e.Graphics.ClipBounds.Height);
                    DrawItemEventArgs e2 = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State, Color.Red, Color.Red);
                    vlbRAM.DefaultDrawItem(e2, output);
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
    }
}