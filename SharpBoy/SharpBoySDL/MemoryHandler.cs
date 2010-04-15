using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBoySDL
{
    public class MemoryHandler
    {
        #region Member Fields
        private byte[] RAMSpace;
        private byte[] ROMSpace;
        private byte[] RAMBanks;
        private byte[] BIOS;
        private byte CurRAMBank;
        private byte CurROMBank;
        private enum Mapper { Unknown = -1, None, MBC1, MBC2 };
        private Mapper RAMMapper;
        private bool EnableRAM;
        #endregion

        #region Properties
        private int stackPointer;
        public int StackPointer
        {
            get { return stackPointer; }
            set { stackPointer = value; }
        }
        #endregion

        public MemoryHandler()
        {
            using (System.IO.FileStream fs = new System.IO.FileStream("DMG_ROM.bin", System.IO.FileMode.Open))
            {
                BIOS = new byte[fs.Length];
                fs.Read(BIOS, 0, (int)fs.Length);
                fs.Close();
            }
            
            RAMSpace = new byte[0x10000 + BIOS.Length];
            Array.Copy(BIOS, 0, RAMSpace, 0x10000, BIOS.Length);
            RAMBanks = new byte[0x8000];
            CurRAMBank = 0;
            CurROMBank = 1;
            RAMMapper = Mapper.None;
            EnableRAM = false;
        }

        public byte ReadByte(int Address)
        {
            int FinalAddress = Address;
            if(Address >= 0x4000 && Address <= 0x7FFF) //Translate reads to the switchable ROM Bank
            {
                FinalAddress -= 0x4000;
                FinalAddress += 0x4000 * (int)CurROMBank;
                return ROMSpace[FinalAddress];
            }

            return RAMSpace[Address];
        }

        public byte[] ReadBytes(int Address, int Amount)
        {
            if ((Address + Amount) > 0xFFFF)
            {
                throw new InvalidOperationException("Tried to read beyond the end of memory!");
            }

            byte[] ret = new byte[Amount];
            for (int i = 0; i < Amount; i++)
            {
                ret[i] = RAMSpace[Address + i];
            }

            return ret;

        }

        public ushort ReadWord(int Address)
        {
            if ((Address + 1) > 0xFFFF)
            {
                throw new InvalidOperationException("Tried to read beyond the end of memory!");
            }

            return (ushort)((ReadByte(Address+1) << 8) | ReadByte(Address));
        }

        public void WriteWord(int Address, ushort Data)
        {
            if ((Address + 1) > 0xFFFF)
            {
                throw new InvalidOperationException("Tried to read beyond the end of memory!");
            }

            RAMSpace[Address] = (byte)(Data & 0x00FF);
            RAMSpace[Address + 1] = (byte)((Data & 0xFF00) >> 8);
        }

        public void WriteByte(int Address, byte Data)
        {
            //Manage bank switching
            if(Address < 0x8000)
            {
                HandleBanks(Address, Data);
                return;
            }

            //Translate to correct RAM Bank
            if (Address >= 0xA000 && Address < 0xC000)
            {
                RAMBanks[(Address - 0xA000) + (CurRAMBank * 0x2000)] = Data;
                return;
            }

            //Disallowed region
            if ((Address >= 0xFEA0 && Address <= 0xFEFF))
            {
                return;
            }

            //Mirror region
            if (Address > 0xE000 && Address < 0xFE00)
            {
                WriteByte(Address - 0x2000, Data);
            }

            RAMSpace[Address] = Data;
        }

        public void WriteBytes(int Address, byte[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                WriteByte(Address + i, Data[i]);
            }
        }

        public void HandleBanks(int Address, byte Data)
        {
            //Handle RAM Bank switch
            if (Address < 0x2000)
            {
                if (RAMMapper == Mapper.MBC1 || RAMMapper == Mapper.MBC2)
                {
                    EnableRAMBank(Address, Data);
                }
                return;
            }

            //Handle ROM Bank switch
            if (Address >= 0x2000 && Address < 0x4000)
            {
                if (RAMMapper == Mapper.MBC1 || RAMMapper == Mapper.MBC2)
                {
                    SwitchLoROMBank(Data);
                }

                return;
            }
        }

        public void EnableRAMBank(int Address, byte Data)
        {
            if (RAMMapper == Mapper.MBC2)
            {
                if ((Address & 8) == 8) //Bit 4 must be unset, only for MBC2
                {
                    return;
                }
            }

            //RAM Enabling
            if ((Data & 0x0F) == 0x0A)
            {
                EnableRAM = true;
            }
            else if ((Data & 0x0F) == 0x00)
            {
                EnableRAM = false;
            }
        }

        public void SwitchLoROMBank(byte Data)
        {
            //Codeslinger, damnit!
        }

        /// <summary>
        /// Loads a byte array ROM into it's proper place in memory.
        /// </summary>
        /// <param name="ROM"></param>
        public void LoadROM(byte[] ROM)
        {
            ROMSpace = new byte[ROM.Length];
            Array.Copy(ROM, ROMSpace, ROM.Length);
            if (ROM.Length < 0x4000)
            {
                Array.Copy(ROM, RAMSpace, ROM.Length);
            }
            else
            {
                Array.Copy(ROM, RAMSpace, 0x4000);//Fixed Bank 0
            }

            switch (RAMSpace[0x147])
            {
               case (1): RAMMapper = Mapper.MBC1; break;
               case (2): RAMMapper = Mapper.MBC1; break;
               case (3): RAMMapper = Mapper.MBC1; break;
               case (5): RAMMapper = Mapper.MBC2; break;
               case (6): RAMMapper = Mapper.MBC2; break;
               default: RAMMapper = Mapper.None; break;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < 0x10000; i++)
            {
                RAMSpace[i] = (byte)0;
            }
            RAMSpace[0xFF10] = 0x80;
            RAMSpace[0xFF11] = 0xBF;
            RAMSpace[0xFF12] = 0xF3;
            RAMSpace[0xFF14] = 0xBF;
            RAMSpace[0xFF16] = 0x3F;
            RAMSpace[0xFF19] = 0xBF;
            RAMSpace[0xFF1A] = 0x7F;
            RAMSpace[0xFF1B] = 0xFF;
            RAMSpace[0xFF1C] = 0x9F;
            RAMSpace[0xFF1E] = 0xBF;
            RAMSpace[0xFF20] = 0xFF;
            RAMSpace[0xFF23] = 0xBF;
            RAMSpace[0xFF24] = 0x77;
            RAMSpace[0xFF25] = 0xF3;
            RAMSpace[0xFF26] = 0xF1;
            RAMSpace[0xFF40] = 0x91;
            RAMSpace[0xFF47] = 0xFC;
            RAMSpace[0xFF48] = 0xFF;
            RAMSpace[0xFF49] = 0xFF;
        }
    }
}
