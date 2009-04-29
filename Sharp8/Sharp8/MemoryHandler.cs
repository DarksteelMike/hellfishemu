using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp8_V3
{
    public class MemoryHandler
    {
        private byte[] MemorySpace;
        private byte[] FontData;

        public MemoryHandler()
        {
            MemorySpace = new byte[0x1000];

            FontData = new byte[] { 0xF0, 0x90, 0x90, 0x90, 0xF0, 0x20, 0x60, 0x20, 0x20, 0x70, 0x60, 0x90, 0x20, 0x40, 0xF0, 0xF0, 0x10, 0xF0, 0x10, 0xF0, 0x90, 0x90, 0xF0, 0x10, 0x10, 0xF0, 0x80, 0xF0, 0x10, 0xF0, 0xF0, 0x80, 0xF0, 0x90, 0xF0, 0xF0, 0x10, 0x20, 0x40, 0x80, 0xF0, 0x90, 0xF0, 0x90, 0xF0, 0xF0, 0x90, 0xF0, 0x10, 0x10, 0xF0, 0x90, 0xF0, 0x90, 0x90, 0xE0, 0x90, 0xE0, 0x90, 0xE0, 0xF0, 0x80, 0x80, 0x80, 0xF0, 0xE0, 0x90, 0x90, 0x90, 0xE0, 0xF0, 0x80, 0xF0, 0x80, 0xF0, 0xF0, 0x80, 0xF0, 0x80, 0x80 };

            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < 0x1000; i++)
            {
                MemorySpace[i] = 0;
            }
            Write(0, FontData);
        }

        public void Write(ushort Address, byte Data)
        {
            MemorySpace[Address] = Data;
        }

        public void Write(ushort Address, byte[] Data)
        {
            if (Address + Data.Length > 0xFFF)
            {
                throw new Exception("Tried to write beyond memory!");
            }

            for (int i = 0; i < Data.Length; i++)
            {
                MemorySpace[Address + i] = Data[i];
            }

            return;
        }

        public byte ReadByte(ushort Address)
        {
            return MemorySpace[Address];
        }

        public byte[] ReadBytes(ushort Address, ushort Amount)
        {
            if (Address + Amount > 0xFFF)
            {
                throw new Exception("Tried to read beyond memory!");
            }

            byte[] Result = new byte[Amount];
            Array.Copy(MemorySpace, Address, Result, 0, Amount);
            return Result;
        }

        public ushort ReadWord(ushort Address)
        {
            if (Address >= 0xFFF)
            {
                throw new Exception("Tried to read beyond memory!");
            }

            return (ushort)((MemorySpace[Address] << 8) | (MemorySpace[Address + 1]));
        }
    }
}
