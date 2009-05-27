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
        private DebuggerForm Debugger;
        private InputMapperForm InputMapper;
        private InputHandler InpHand;
        private NumericInputFrom NumInp;
        private int TargetRenderFPS;
        
        private VoidNoParams UpdateDisplayDel;
        #endregion

        public MainForm()
        {
            InitializeComponent();

            //Emulator component.
            Emu = new Emulator();

            //Register Tick Handler. This event is raised every frame by SDL.
            SdlDotNet.Core.Events.Tick += new EventHandler<TickEventArgs>(Events_Tick);

            //Set the FPS that SDL will try to match.
            TargetRenderFPS = 60;
            SdlDotNet.Core.Events.Fps = TargetRenderFPS;

            //Set the FPS at which the Tick event must be raised.
            SdlDotNet.Core.Events.TargetFps = 60;

            //This delegate allows the debugger to redraw the output window after every step. (Normally it is just done once a frame)
            UpdateDisplayDel = new VoidNoParams(UpdateDisplay);

            //Ready the input handler and attach it to the emulator
            InpHand = new InputHandler();
            Emu.AttachInputHandler(InpHand);
            this.KeyDown += new KeyEventHandler(InpHand.KeyDown);
            this.KeyUp += new KeyEventHandler(InpHand.KeyUp);
            
            //Attach the input handler to the input mapper form
            InputMapper = new InputMapperForm(InpHand);

            //Attach the redraw delegate to the debugger
            Debugger = new DebuggerForm(Emu,UpdateDisplayDel);

            //Create a "NumericInput" form. This is used to get the Target FPS when the user wants to change it.
            NumInp = new NumericInputFrom();
            NumInp.Title = "Target FPS";
            
        }
        
        #region Eventhandlers
        /// <summary>
        /// This event is called when clicking the Open ROM menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdGetROM.ShowDialog() == DialogResult.OK) //If the user actually *chose* a file...
            {
                //Load the file
                FileStream FSROM = File.OpenRead(ofdGetROM.FileName);
                byte[] OpenedROM = new byte[FSROM.Length];
                FSROM.Read(OpenedROM, 0, (int)FSROM.Length);
                Emu.LoadROM(OpenedROM);
                Emu.Reset();

                //Enable FPS display and debugger
                LastFPSCalc = SdlDotNet.Core.Timer.TicksElapsed;
                resetWithDebuggerToolStripMenuItem.Enabled = true;
                pauseAndOpenDebuggerToolStripMenuItem.Enabled = true;

                //Off we go!
                SdlDotNet.Core.Events.Run();
            }
        }

        /// <summary>
        /// This event is called when clicking the Exit menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close down SDLDotNet
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

            if (!Emu.IsPaused) //If the debugger hasn't paused the emulator...
            {
                if (Emu.DoOneFrame()) //Emulate one frame and if the output has been altered...
                {
                    UpdateDisplay(); //Draw the output.
                }
            }
        }

        /// <summary>
        /// This event handler is called if the main windw is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SdlDotNet.Core.Events.Close();
            SdlDotNet.Core.Events.QuitApplication();
            Application.Exit();
        }

        /// <summary>
        /// This event handler is called when clicking the Reset With Debugger menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// This event handler is called when clicking the Pause And Open Debugger menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// This event handler is called when clicking the Map Input menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// This event handler is called when clicking the Target FPS menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void targetFPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NumInp.Value = TargetRenderFPS;
            Emu.IsPaused = true;
            if (NumInp.ShowDialog() == DialogResult.OK)
            {
                TargetRenderFPS = NumInp.Value;
                SdlDotNet.Core.Events.Fps = TargetRenderFPS;
                targetFPSToolStripMenuItem.Text = "Target FPS: " + TargetRenderFPS.ToString();
            }
            Emu.IsPaused = false;
        }

        /// <summary>
        /// This event handler is called when clicking the Play Sound menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Emu.UseSound = playSoundToolStripMenuItem.Checked;
        }

        /// <summary>
        /// This event handler is called when clicking the Reset menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Emu.Reset();
        }
        #endregion

        /// <summary>
        /// Force the redraw of the Display SDLSurface control.
        /// </summary>
        private void UpdateDisplay()
        {
            scDisplay.Blit(Emu.SurfaceOut);
            scDisplay.Update();
        }

        
    }
}