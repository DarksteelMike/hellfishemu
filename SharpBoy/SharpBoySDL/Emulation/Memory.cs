using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoy2.Emulation
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
            if (Address == 0xFF00)
            {
                return MyCore.MakeInputByte();
            }
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
            return (ushort)((Read(Address) << 8) | Read((ushort)(Address + 1)));
        }

        public void Write(ushort Address, byte Data)
        {
	        bool MasterShouldWrite = true;

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