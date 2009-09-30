using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    public enum MessageLogScrollPossibilities{ None,Up,Down,Both };

    public class MessageLog
    {
        private List<string> Lines;

        public MessageLog()
        {
            Lines = new List<string>();
        }

        public void AddMsg(string Msg)
        {
            Lines.Add(Msg);

            while (Lines.Count > 90)
            {
                Lines.RemoveAt(0);
            }
        }
        public void RenderRecentToConsole(libtcodWrapper.Console Target)
        {
            Target.Clear();

            for (int i = (Lines.Count - 4 > 0 ? Lines.Count-4 : 0),j=0; i < Lines.Count; i++,j++)
            {
                Target.PrintLine(Lines[i] + "\n", 0, j, libtcodWrapper.LineAlignment.Left);
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