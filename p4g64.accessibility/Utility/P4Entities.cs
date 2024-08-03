using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p4g64.accessibility.Utility
{
    internal class P4Entities
    {
        static long entityOffsetListOffset = 0xAA8098;
        static long addr_PlayerData = 0xEC0FE8;
        Process process;
        long baseAddress;
        public List<long> getEntityOffsetsList()
        {
            List<long> offsets = new List<long>();

            long baseEntityListPointer = MemoryRead.ReadLong((int)process.Handle, baseAddress + entityOffsetListOffset);

            baseEntityListPointer = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer + 8);

            if (baseEntityListPointer == 0) // If no list, return
            {
                return offsets;
            }

            long currentAddress = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer + 0x18);

            while (currentAddress != 0)
            {
                offsets.Add(currentAddress);
                currentAddress = MemoryRead.ReadLong((int)process.Handle, currentAddress + 0x150);
            }
            return offsets;
        }
        public float[] getEntPos(long entOffset)
        {
            try
            {
                long newOffset = MemoryRead.ReadLong((int)process.Handle, (entOffset + 400));
                float y = MemoryRead.ReadFloat((int)process.Handle, newOffset + 0x364);
                float x = MemoryRead.ReadFloat((int)process.Handle, newOffset + 0x360);
                float z = MemoryRead.ReadFloat((int)process.Handle, newOffset + 0x368);
                return new float[3] { x, y, z };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new float[3] { 0, 0, 0 };

        }
        public float[] getPlayerPos()
        {
            long playerDataOffset = baseAddress + addr_PlayerData;
            playerDataOffset = MemoryRead.ReadLong((int)process.Handle, playerDataOffset);

            float y = MemoryRead.ReadFloat((int)process.Handle, playerDataOffset + 0x364);
            float x = MemoryRead.ReadFloat((int)process.Handle, playerDataOffset + 0x360);
            float z = MemoryRead.ReadFloat((int)process.Handle, playerDataOffset + 0x368);
            return new float[3] { x, y, z };
        }

        public P4Entities () 
        {
            process = Process.GetCurrentProcess();
            baseAddress = MemoryRead.GetProcessBaseAddress(process);
        }
    }
}
