using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    public enum MessageLogScrollPossibilities{ None,Up,Down,Both };

    class MessageLog
    {
        private List<string> Lines;

        public MessageLog()
        {
            Lines = new List<string>();
        }

        public void AddMsg(string Msg)
        {
            if (Lines.Count == 0)
            {
                Lines.Add(Msg);
            }
            else
            {
                Lines[Lines.Count - 1] += Msg;
            }
            if (Lines[Lines.Count - 1].Length > 90)
            {
                string NewLine = Lines[Lines.Count - 1].Substring(89);
                Lines[Lines.Count - 1] = Lines[Lines.Count - 1].Substring(0, 89);
                Lines.Add(NewLine);

            }
        }
        public void RenderRecentToConsole(libtcodWrapper.Console Target)
        {
            Target.Clear();

            for (int i = (Lines.Count - 4 > 0 ? Lines.Count-4 : 0),j=0; i < Lines.Count; i++,j++)
            {
                Target.PrintLine(Lines[i], 0, j, libtcodWrapper.LineAlignment.Left);
            }
        }

        /// <summary>
        /// Renders the full message log to a console.
        /// </summary>
        /// <param name="Target">Target console.</param>
        /// <param name="Scroll">How much to scroll down.</param>
        /// <returns>Which way can be scrolled.</returns>
        public MessageLogScrollPossibilities RenderFullToConsole(libtcodWrapper.Console Target, int Scroll)
        {
            Target.Clear();

            bool canscrollup, canscrolldown;
            canscrollup = canscrolldown = false;
            if (Scroll > 0)
            {
                canscrollup = true;
            }
            for (int i = Scroll,j=0; i < Lines.Count; i++,j++)
            {
                Target.PrintLine(Lines[i], 0, j, libtcodWrapper.LineAlignment.Left);
                if (j > 30)
                {
                    canscrolldown = true;
                    break;
                }
            }
            if (canscrolldown && canscrollup)
            {
                return MessageLogScrollPossibilities.Both;
            }
            else if (canscrolldown)
            {
                return MessageLogScrollPossibilities.Down;
            }
            else if (canscrollup)
            {
                return MessageLogScrollPossibilities.Up;
            }
            else
            {
                return MessageLogScrollPossibilities.None;
            }
        }
    }
}