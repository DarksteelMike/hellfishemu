using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Guardian_Roguelike
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            libtcodWrapper.RootConsole.Fullscreen = false;
            libtcodWrapper.RootConsole.Width = 92;
            libtcodWrapper.RootConsole.Height = 40;
            libtcodWrapper.RootConsole.WindowTitle = "Run Urist, Run!";

            States.StateManager.PersistentStates.Add("MainMenuState", new States.MainMenuState());
            States.StateManager.PersistentStates.Add("GameState", new States.GameState());
            States.StateManager.PersistentStates.Add("QuitState", new States.QuitState());
            States.StateManager.PersistentStates.Add("MessageLogMenuState",new States.MessageLogMenuState());
            
            States.StateManager.ChangeState(States.StateManager.PersistentStates["MainMenuState"]);
        }
    }
}