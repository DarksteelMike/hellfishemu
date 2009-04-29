using System;

using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace SharpBoySDL
{
    public class SharpBoy
    {
        [STAThread]
        public static void Main()
        {
            SharpBoy app = new SharpBoy();
            app.Go();
        }

        public SharpBoy()
        {
            
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
