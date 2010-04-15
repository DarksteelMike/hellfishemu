using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoySDL.MemoryMappers
{
    public abstract class MemoryMapperBase
    {
        public byte[] OnboardRAM;
        public MemoryHandler MainMemory;

        public abstract void TranslatedWrite(int Address, byte Data);
        public abstract byte TranslatedRead(int Address);


        public MemoryMapperBase(MemoryHandler MH)
        {
            MainMemory = MH;
        }
    }
}
