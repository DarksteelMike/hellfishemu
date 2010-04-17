using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoy2.Emulation.MemoryMappers
{
    class MemoryMapper_None : MemoryMapperBase
    {
        public MemoryMapper_None(Memory M)
            : base(M)
        { }

        public override void Init()
        {
            return;
        }

        public override bool ReadAttempted(ref ushort Address)
        {
            return true;
        }

        public override bool WriteAttempted(ref ushort Address, ref byte Data)
        {
            return true;
        }
    }
}