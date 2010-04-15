using System;

using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace SharpBoySDL
{
    public class SharpBoy
    {
        private CPUHandler Emulator;

        [STAThread]
        public static void Main()
        {
            SharpBoy app = new SharpBoy();
            app.Go();
        }

        public SharpBoy()
        {
            string path = "D:\\emu\\GB+GBC+SGB+GBA\\ROMS\\GB+GBC+SGB\\Tetris\\Tetris (JUE) (V1.1) [!].gb";

            Emulator = new CPUHandler();
            /*
            try
            {
                using (System.IO.BinaryReader BR = new System.IO.BinaryReader(new System.IO.FileStream(path, System.IO.FileMode.Open)))
                {
                    Emulator.LoadROM(BR.ReadBytes((int)BR.BaseStream.Length));
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Couldn't load!");
            }

            System.Windows.Forms.MessageBox.Show("Done!");
            */
            byte[] b = new byte[] { 0x00,0x01,0x00,0x05};
            Emulator.LoadROM(b);
            Emulator.RunOneFrame();
            
        }

        public void Go()
        {
            Events.Quit += new EventHandler<QuitEventArgs>(this.Quit);
            Events.Tick += new EventHandler<TickEventArgs>(this.Tick);

            Events.Run();
        }

        private void Quit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }

        private void Tick(object sender, TickEventArgs e)
        {            
            if (Timer.TicksElapsed - LastFPSWrite >= 1000)
            {
                LastFPSWrite = Timer.TicksElapsed;
                FPSDisplay = FramesDone;
                FramesDone = 0;
                Video.WindowCaption = "SharpBoy - FPS:" + FPSDisplay.ToString();
            }
            else
            {
                FramesDone++;
            }
        }

        private int FramesDone;
        private int LastFPSWrite;
        private int FPSDisplay;


    }
}
