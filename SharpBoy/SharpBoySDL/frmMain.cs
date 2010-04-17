using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SharpBoy2
{
    public partial class frmMain : Form
    {
        private frmDebugger Debugger;
        private Emulation.Core MyCore;

        public frmMain()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SdlDotNet.Core.Events.QuitApplication();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            SdlDotNet.Core.Events.QuitApplication();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdGetROM.ShowDialog() == DialogResult.OK)
            {
                byte[] tmpbuf;
                using (System.IO.FileStream fs = new System.IO.FileStream(ofdGetROM.FileName,System.IO.FileMode.Open))
                {
                    tmpbuf = new byte[fs.Length];
                    fs.Read(tmpbuf, 0, (int)fs.Length);
                    fs.Close();
                }
                MyCore = new SharpBoy2.Emulation.Core();
                MyCore.LoadROM(tmpbuf);
                MyCore.Reset();
                debugToolStripMenuItem.Enabled = true;
                resetAndDebugToolStripMenuItem.Enabled = true;
                Debugger = new frmDebugger(MyCore);
                Debugger.Disposed += new EventHandler(Debugger_Disposed);
            }
        }

        void Debugger_Disposed(object sender, EventArgs e)
        {
            Debugger = new frmDebugger(MyCore);
        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debugger.Show();
        }

        private void resetAndDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCore.Reset();
            Debugger.Show();
        }
    }
}