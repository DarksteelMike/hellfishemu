using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoySDL.MemoryMappers
{
    class MemoryMapper_None : MemoryMapperBase
    {
        public MemoryMapper_None(MemoryHandler MH)
            : base(MH)
        { }

        public override byte TranslatedRead(int Address)
        {
            return MainMemory.ReadByte(Address);
        }

        public override void TranslatedWrite(int Address, byte Data)
        {
            MainMemory.WriteByte(Address, Data);
        }
    }
}
