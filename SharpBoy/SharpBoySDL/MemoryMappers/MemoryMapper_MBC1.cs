using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoySDL.MemoryMappers
{
    public class MemoryMapper_MBC1 : MemoryMapperBase
    {
        private enum MaxMemMode { ROM16RAM8, ROM4RAM32 };
        private MaxMemMode MemoryMode;
        private byte SelectedROMBank;
        private byte SelectedRAMBank;
        private bool EnableRAMBank;

        public MemoryMapper_MBC1(MemoryHandler MH)
            : base(MH)
        { }

        public override void TranslatedWrite(int Address, byte Data)
        {
            if (Address <= 0x1FFF) //RAMBank Enable
            {
                if ((Data & 0x0A) == 0x0A)
                {
                    EnableRAMBank = true;
                }
                else
                {
                    EnableRAMBank = false;
                }
                return;
            }
            if (Address <= 0x3FFF) //ROMBank Number
            {
                SelectedROMBank = ((Data & 31) > 0) ? (byte)(Data & 31) : (byte)1;
                return;
            }
            if (Address <= 0x5FFF) //RAMBank Number OR Upper bits of ROMBank Number
            {
                if (MemoryMode == MaxMemMode.ROM16RAM8)
                {
                    SelectedROMBank |= (byte)((Data & 3) << 5);
                }
                else
                {
                    SelectedRAMBank = (byte)(Data & 3);
                }
            }
            if (Address <= 0x7FFF) //ROM/RAM Mode select
            {
                MemoryMode = ((Data & 1) == 1) ? MaxMemMode.ROM4RAM32 : MaxMemMode.ROM16RAM8;
                return;
            }
            if(Address <= 0x9FFF) //?
            {

            }
            if (Address <= 0xBFFF) //Selected RAMBank
            {
                MainMemory.WriteByte((Address - 0xA000) + (SelectedRAMBank * 0x2000), Data);
            }
            
        }

        public override byte TranslatedRead(int Address)
        {
            return 0;
        }
    }
}
