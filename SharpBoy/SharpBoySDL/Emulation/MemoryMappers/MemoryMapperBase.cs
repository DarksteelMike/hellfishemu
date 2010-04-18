using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoy.Emulation.MemoryMappers
{
    public abstract class MemoryMapperBase
    {
        protected Memory MyMemory;

        public MemoryMapperBase(Memory M)
        {
            MyMemory = M;
        }

        public abstract void Init();
        public abstract bool WriteAttempted(ref ushort Address, ref byte Data);
        public abstract bool ReadAttempted(ref ushort Address);
    }
}