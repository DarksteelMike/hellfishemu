using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    public enum MessageLogScrollPossibilities{ None,Up,Down,Both };

    public struct Message
    {
        public string Text;
        public libtcodWrapper.Color TextColor;

        public Message(string t, libtcodWrapper.Color c)
        {
            Text = t;
            TextColor = c;
        }
    }

    public abstract class MessageLog
    {
        private static List<Message> Lines = new List<Message>();

        public static void AddMsg(string Msg)
        {
            AddMsg(Msg, libtcodWrapper.Color.FromRGB(255, 255, 255));
        }

        public static void Clear()
        {
            Lines.Clear();
        }

        public static void AddMsg(string Msg, libtcodWrapper.Color Col)
        {
            Lines.Add(new Message(Msg,Col));

            while (Lines.Count > 90)
            {
                Lines.RemoveAt(0);
            }
        }
        public static void RenderRecentToConsole(libtcodWrapper.Console Target)
        {
            Target.Clear();

            for (int i = (Lines.Count - 4 > 0 ? Lines.Count-4 : 0),j=0; i < Lines.Count; i++,j++)
            {
                Target.ForegroundColor = Lines[i].TextColor;
                Target.PrintLine(Lines[i].Text + "\n", 0, j, libtcodWrapper.LineAlignment.Left);
            }
        }

        /// <summary>
        /// Renders the full message log to a console.
        /// </summary>
        /// <param name="Target">Target console.</param>
        /// <param name="Scroll">How much to scroll down.</param>
        /// <returns>Which way can be scrolled.</returns>
        public static MessageLogScrollPossibilities RenderFullToConsole(libtcodWrapper.Console Target, int Scroll)
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
                Target.ForegroundColor = Lines[i].TextColor;
                Target.PrintLine(Lines[i].Text, 0, j, libtcodWrapper.LineAlignment.Left);
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