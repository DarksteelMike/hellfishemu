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

        public frmDebugger(SharpBoy2.Emulation.Core C)
        {
            MyCore = C;
            InitializeComponent();
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
        }

        private void bStep_Click(object sender, EventArgs e)
        {
            MyCore.MyCPU.DoStep();
            RefreshData();
        }

        private void vlbROM_DrawItem(object sender, DrawItemEventArgs e)
        {
            string output = Convert.ToString(MyCore.MyMemory.ROM[e.Index+1], 16);
            vlbROM.DefaultDrawItem(e, output);
        }

        private void vlbRAM_DrawItem(object sender, DrawItemEventArgs e)
        {
            string output = Convert.ToString(e.Index,16).PadLeft(3,'0') + ": " + Convert.ToString(MyCore.MyMemory.GameBoyRAM[e.Index], 16);
            vlbRAM.DefaultDrawItem(e, output);
        }
    }
}