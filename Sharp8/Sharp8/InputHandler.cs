using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Sharp8_V3
{
    public class InputHandler
    {
        private bool[] keyStates;
        public bool[] KeyStates
        {
            get { return keyStates; }
        }

        private Keys[] keyMappings;
        public Keys[] KeyMappings
        {
            get { return keyMappings; }
            set { keyMappings = value; }
        }

        private string CfgPath;

        public InputHandler()
        {
            keyStates = new bool[16];
            keyMappings = new Keys[16];
            CfgPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\') + 1) + "Key-mapping.cfg";
            LoadMappings();
        }

        public void ClearStates()
        {
            for (int i = 0; i < keyStates.Length; i++)
            {
                keyStates[i] = false;
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < keyMappings.Length; i++)
            {
                if (e.KeyCode == keyMappings[i])
                {
                    keyStates[i] = true;
                }
            }
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < keyMappings.Length; i++)
            {
                if (e.KeyCode == keyMappings[i])
                {
                    keyStates[i] = false;
                }
            }
        }

        public void LoadMappings()
        {
            if (!System.IO.File.Exists(CfgPath))
            {
                SetDefaultMappings();
                SaveMappings();
                return;
            }

            System.IO.StreamReader SR = System.IO.File.OpenText(CfgPath);
            for (int i = 0; i < 0x10; i++)
            {
                if (SR.EndOfStream)
                {
                    SetDefaultMappings();
                    SaveMappings();
                    break;
                }
                try
                {
                    keyMappings[i] = (Keys)Enum.Parse(typeof(Keys), SR.ReadLine());
                }
                catch (ArgumentException AE)
                {
                    SetDefaultMappings();
                    SaveMappings();
                    break;
                }
            }
            SR.Close();
        }

        public void SaveMappings()
        {
            System.IO.File.Delete(CfgPath);
            System.IO.StreamWriter SW = System.IO.File.CreateText(CfgPath);
            for (int i = 0; i < 0x10; i++)
            {
                SW.WriteLine(keyMappings[i].ToString());
            }
            SW.Close();
        }

        public void SetDefaultMappings()
        {
            keyMappings[0] = Keys.X;
            keyMappings[1] = Keys.D1;
            keyMappings[2] = Keys.D2;
            keyMappings[3] = Keys.D3;
            keyMappings[4] = Keys.Q;
            keyMappings[5] = Keys.W;
            keyMappings[6] = Keys.E;
            keyMappings[7] = Keys.A;
            keyMappings[8] = Keys.S;
            keyMappings[9] = Keys.D;
            keyMappings[0xA] = Keys.Z;
            keyMappings[0xB] = Keys.C;
            keyMappings[0xC] = Keys.D4;
            keyMappings[0xD] = Keys.R;
            keyMappings[0xE] = Keys.F;
            keyMappings[0xF] = Keys.V;
        }
    }
}
