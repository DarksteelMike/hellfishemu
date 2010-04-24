using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoy.Emulation
{
    public class Memory
    {
        public byte[] ROM;
        public byte[] GameBoyRAM;
        public MemoryMappers.MemoryMapperBase Mapper;

        public Core MyCore;

        public Memory(Core C)
        {
            MyCore = C;
            GameBoyRAM = new byte[0x10000];
        }

        public void Reset()
        {
            GameBoyRAM[0xFF05] = 0;
            GameBoyRAM[0xFF06] = 0;
            GameBoyRAM[0xFF07] = 0;
            GameBoyRAM[0xFF10] = 0x80;
            GameBoyRAM[0xFF11] = 0xBF;
            GameBoyRAM[0xFF12] = 0xF3;
            GameBoyRAM[0xFF14] = 0xBF;
            GameBoyRAM[0xFF16] = 0x3F;
            GameBoyRAM[0xFF17] = 0;
            GameBoyRAM[0xFF19] = 0xBF;
            GameBoyRAM[0xFF1A] = 0x7F;
            GameBoyRAM[0xFF1B] = 0xFF;
            GameBoyRAM[0xFF1C] = 0x9F;
            GameBoyRAM[0xFF1E] = 0xBF;
            GameBoyRAM[0xFF20] = 0xFF;
            GameBoyRAM[0xFF21] = 0;
            GameBoyRAM[0xFF22] = 0;
            GameBoyRAM[0xFF23] = 0xBF;
            GameBoyRAM[0xFF24] = 0x77;
            GameBoyRAM[0xFF25] = 0xF3;
            GameBoyRAM[0xFF26] = 0xF1;
            GameBoyRAM[0xFF40] = 0x91;
            GameBoyRAM[0xFF42] = 0;
            GameBoyRAM[0xFF43] = 0;
            GameBoyRAM[0xFF45] = 0;
            GameBoyRAM[0xFF47] = 0xFC;
            GameBoyRAM[0xFF48] = 0xFF;
            GameBoyRAM[0xFF49] = 0xFF;
            GameBoyRAM[0xFF4A] = 0;
            GameBoyRAM[0xFF4B] = 0;
            GameBoyRAM[0xFFFF] = 0;
        }

        public void LoadROM(byte[] R)
        {
            ROM = new byte[R.Length];
            Array.Copy(R, ROM, R.Length);

            if (ROM.Length <= 0x4000)
            {
                Array.Copy(ROM, GameBoyRAM, ROM.Length);
            }
            else
            {
                Array.Copy(ROM, GameBoyRAM, 0x4000);
            }

            switch (R[0x147])
            {
                case (1): Mapper = new MemoryMappers.MemoryMapper_MBC1(this); break;
                case (2): Mapper = new MemoryMappers.MemoryMapper_MBC1(this); break;
                case (3): Mapper = new MemoryMappers.MemoryMapper_MBC1(this); break;
                default: Mapper = new MemoryMappers.MemoryMapper_None(this); break;
            }
            Mapper.Init();
        }

        public byte Read(ushort Address)
        {
            if (Mapper.ReadAttempted(ref Address))
            {
                return GameBoyRAM[Address];
            }
            else
            {
                return 0;
            }
        }
        public ushort ReadWord(ushort Address)
        {
            return (ushort)(Read(Address) | Read((ushort)(Address + 1)) << 8);
        }

        public void Write(ushort Address, byte Data)
        {
	        bool MasterShouldWrite = true;

            //ECHO RAM
            if(Address >= 0xC000 && Address <= 0xDDFF)
            {
                GameBoyRAM[Address + 0x2000] = Data;
            }
            if (Address >= 0xE000 && Address <= 0xFDFF)
            {
                GameBoyRAM[Address - 0x2000] = Data;
            }

	        if(Address == 0xFF0F)
	        {
		        GameBoyRAM[0xFF0F] = 0; //Reset DIV when anything is written to it.
	        }
	        if(Address == 0xFF00)
	        {
		        if(Utility.IsBitSet(Data,5))
		        {
                    Utility.SetBit(ref GameBoyRAM[0xFF00], 5, SBMode.On);
		        }
		        if(Utility.IsBitSet(Data,4))
		        {
			        Utility.SetBit(ref GameBoyRAM[0xFF00],4,SBMode.On);
		        }
	        }
            if (Address == 0xFF50 && (Data & 1) == 1) //"Disable" boot ROM
            {
                Array.Copy(ROM, GameBoyRAM, 0x100);
            }
            if(Mapper.WriteAttempted(ref Address,ref Data) && MasterShouldWrite)
	        {
                GameBoyRAM[Address] = Data;
	        }
        }
    }
}