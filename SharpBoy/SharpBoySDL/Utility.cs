using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoy2
{
    public enum SBMode { On, Off, Toggle };
    class Utility
    {
        public static void SetBit(ref byte Target,byte bitnum, SBMode Mode)
        {
            if (Mode == SBMode.On)
            {
                Target |= (byte)(1 << bitnum);
            }
            else if (Mode == SBMode.Off)
            {
                Target &= (byte)~(1 << bitnum);
            }
            else
            {
                Target ^= (byte)(1 << bitnum);
            }
        }

        public static bool IsBitSet(byte Target, byte bitnum)
        {
            return ((Target & (1 << bitnum)) == (1 << bitnum));
        }
    }
}
