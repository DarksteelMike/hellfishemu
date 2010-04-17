using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoy2.Emulation.MemoryMappers
{
    class MemoryMapper_MBC1 : MemoryMapperBase
    {
        public MemoryMapper_MBC1(Memory M)
            : base(M)
        { }

        public override void Init()
        {

        }
        public override bool ReadAttempted(ref ushort Address)
        {
            return false;
        }
        public override bool WriteAttempted(ref ushort Address, ref byte Data)
        {
            return false;
        }
    }
}