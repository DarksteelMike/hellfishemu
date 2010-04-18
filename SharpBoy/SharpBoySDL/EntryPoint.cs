using System;

using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace SharpBoy
{
    public class SharpBoy
    {
        //private CPUHandler Emulator;

        [STAThread]
        public static void Main()
        {
            SharpBoy app = new SharpBoy();
            app.Go();
        }

        public SharpBoy()
        {
            
            frmMain MainBoyo = new frmMain();
            MainBoyo.Show();
            
            //Video.SetVideoMode(320, 288,32,false,false,false,false,true);
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

            //Video.Screen.Fill(System.Drawing.Color.Black);
            //Video.Update();
        }

        private int FramesDone;
        private int LastFPSWrite;
        private int FPSDisplay;


    }
}
