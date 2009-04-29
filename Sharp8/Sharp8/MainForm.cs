using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using SdlDotNet.Graphics;
using SdlDotNet.Core;

namespace Sharp8_V3
{
    public delegate void VoidNoParams();

    public partial class MainForm : Form
    {
        #region Member variables
        private Emulator Emu;
        private int LastFPSCalc;
        private int FramesDone;
        private DebuggerForm Debugger;
        private InputMapperForm InputMapper;
        private InputHandler InpHand;
        private NumericInputFrom NumInp;
        private int TargetFPS;
        
        private VoidNoParams UpdateDisplayDel;
        #endregion

        public MainForm()
        {
            InitializeComponent();

            Emu = new Emulator();
            SdlDotNet.Core.Events.Tick += new EventHandler<TickEventArgs>(Events_Tick);
            TargetFPS = 60;
            SdlDotNet.Core.Events.Fps = 60;
            UpdateDisplayDel = new VoidNoParams(UpdateDisplay);
            InpHand = new InputHandler();
            this.KeyDown += new KeyEventHandler(InpHand.KeyDown);
            this.KeyUp += new KeyEventHandler(InpHand.KeyUp);
            Emu.AttachInputHandler(InpHand);
            InputMapper = new InputMapperForm(InpHand);
            Debugger = new DebuggerForm(Emu,UpdateDisplayDel);
            NumInp = new NumericInputFrom();
            NumInp.Title = "Target FPS";
            
        }
        
        #region Eventhandlers
        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdGetROM.ShowDialog() == DialogResult.OK) //If the user actually *chose* a file...
            {
                //Load the file
                FileStream FSROM = File.OpenRead(ofdGetROM.FileName);
                byte[] OpenedROM = new byte[FSROM.Length];
                FSROM.Read(OpenedROM, 0, (int)FSROM.Length);
                Emu.LoadROM(OpenedROM);

                //Enable FPS display and debugger
                LastFPSCalc = SdlDotNet.Core.Timer.TicksElapsed;
                resetWithDebuggerToolStripMenuItem.Enabled = true;
                pauseAndOpenDebuggerToolStripMenuItem.Enabled = true;

                //Off we go!
                SdlDotNet.Core.Events.Run();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SdlDotNet.Core.Events.Close();
            SdlDotNet.Core.Events.QuitApplication();
            Application.Exit();
        }

        /// <summary>
        /// This event is called every frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Events_Tick(object sender, TickEventArgs e)
        {
            //Only update the FPS display once a second, no more is necessary.
            if (SdlDotNet.Core.Timer.TicksElapsed - LastFPSCalc >= 1000)
            {
                this.Text = "Sharp8 - FPS: " + SdlDotNet.Core.Events.Fps;
                LastFPSCalc = SdlDotNet.Core.Timer.TicksElapsed;
            }
            else
            {
                FramesDone++;
            }

            if (!Emu.IsPaused) //If the debugger hasn't paused the emulator...
            {
                if (Emu.DoOneFrame()) //Emulate one frame and if the output has been altered...
                {
                    UpdateDisplay(); //Draw the output.
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SdlDotNet.Core.Events.Close();
            SdlDotNet.Core.Events.QuitApplication();
            Application.Exit();
        }

        private void resetWithDebuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Debugger == null || Debugger.IsDisposed) //Just in case the debugger was dismissed with the close button.
            {
                Debugger = new DebuggerForm(Emu,UpdateDisplayDel);
            }
            Emu.Reset();
            Emu.IsPaused = true;
            Debugger.Show();
            UpdateDisplay();
        }

        private void pauseAndOpenDebuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Debugger == null || Debugger.IsDisposed) //Just in case the debugger was dismissed with the close button.
            {
                Debugger = new DebuggerForm(Emu, UpdateDisplayDel);
            }
            Emu.IsPaused = true;
            Debugger.UpdateInfo();
            Debugger.Show();
        }
        private void mapinputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InputMapper == null  || InputMapper.IsDisposed) //Just in case the input mapper was dismissed with the close button.
            {
                InputMapper = new InputMapperForm(Emu.Input);
            }

            Emu.IsPaused = true;
            InputMapper.ShowDialog();
            Emu.IsPaused = false;

        }

        private void targetFPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NumInp.Value = TargetFPS;
            Emu.IsPaused = true;
            if (NumInp.ShowDialog() == DialogResult.OK)
            {
                TargetFPS = NumInp.Value;
                SdlDotNet.Core.Events.Fps = TargetFPS;
                targetFPSToolStripMenuItem.Text = "Target FPS: " + TargetFPS.ToString();
            }
            Emu.IsPaused = false;
        }

        private void playSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Emu.UseSound = playSoundToolStripMenuItem.Checked;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Emu.Reset();
        }
        #endregion

        private void UpdateDisplay()
        {
            scDisplay.Blit(Emu.SurfaceOut);
            scDisplay.Update();
        }

        
    }
}