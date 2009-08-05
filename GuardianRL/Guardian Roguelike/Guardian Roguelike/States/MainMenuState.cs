using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.States
{
    class MainMenuState : StateBase
    {
        private libtcodWrapper.Image Logo;
        private List<string> MenuItems;
        private int SelectedMenuItem;
        public const int MENUSTARTX = 40;
        public const int MENUSTARTY = 5;

        public override void EnterState()
        {

            MenuItems = new List<string>();
            MenuItems.Add("New Game");
            MenuItems.Add("Quit");
            SelectedMenuItem = 0;

            Logo = new libtcodWrapper.Image("skullG.png");
            
        }

        public override void MainLoop()
        {
            libtcodWrapper.KeyPress key;

            while (true)
            {
                Render();
                key = libtcodWrapper.Keyboard.WaitForKeyPress(true);
                if (ProcessInput(key))
                {
                    break;
                }
            }
        }

        private void Render()
        {
            Root.Clear();/*
            Root.ForegroundColor = libtcodWrapper.ColorPresets.Gold;
            Root.PutChar(1, 1,'@');
            Root.ForegroundColor = libtcodWrapper.ColorPresets.GhostWhite;
            Root.PrintLine("Hej", 1, 2, libtcodWrapper.LineAlignment.Left);
            Root.Flush();
            return;*/
            Logo.Blit(Root, 20, 20, libtcodWrapper.Background.Set, 1, 1, 0);

            libtcodWrapper.Color txtcol;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                if (i == SelectedMenuItem)
                {
                    txtcol = libtcodWrapper.ColorPresets.Gold;
                }
                else
                {
                    txtcol = libtcodWrapper.ColorPresets.GhostWhite;
                }
                Root.ForegroundColor = txtcol;
                Root.PrintLine(MenuItems[i], MENUSTARTX, MENUSTARTY + i, libtcodWrapper.LineAlignment.Left);
            }

            Root.Flush();
        }

        private bool ProcessInput(libtcodWrapper.KeyPress KP)
        {
            switch (KP.KeyCode)
            {
                case(libtcodWrapper.KeyCode.TCODK_ENTER):
                    switch (SelectedMenuItem)
                    {
                        case(0): //New Game
                            StateManager.QueueState(new GameState());
                            return true;
                            break;
                        case(1): //Quit
                            StateManager.QueueState(new QuitState());
                            return true;
                            break;
                    }
                    break;
                case(libtcodWrapper.KeyCode.TCODK_DOWN):
                    if (SelectedMenuItem == (MenuItems.Count - 1))
                    {
                        SelectedMenuItem = 0;
                    }
                    else
                    {
                        SelectedMenuItem++;
                    }
                    break;
                case(libtcodWrapper.KeyCode.TCODK_UP):
                    if (SelectedMenuItem == 0)
                    {
                        SelectedMenuItem = (MenuItems.Count - 1);
                    }
                    else
                    {
                        SelectedMenuItem--;
                    }
                    break;
            }

            return false;
        }

        public override void ExitState()
        {
            Logo.Dispose();
            
        }
    }
}
