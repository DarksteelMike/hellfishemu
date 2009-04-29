using System;
using System.Windows.Forms;

namespace Sharp8_V3
{
    public class Sharp8_V3
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

        }
    }
}
