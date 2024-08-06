using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static p4g64.accessibility.Utils;
namespace p4g64.accessibility.Utility
{
    internal class P4Entities
    {
        static long addr_InteractNpcList = 0xAA8098;
        static long addr_InteractObjectList = 0x1E4382C0;
        static long addr_PlayerData = 0xEC0FE8;

        string npcEntityListSignature = "40 a3 ec 40 01 00 00 00";

        Process process;
        long baseAddress;
        public List<long> getNPCEntitiesOffsets()
        {
            List<long> offsets = new List<long>();

            long baseEntityListPointer = MemoryRead.ReadLong((int)process.Handle, baseAddress + addr_InteractNpcList);

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
        public List<long> getInteractableNonNPCEntitiesOffsets()
        {
            List<long> offsets = new List<long>();

            long baseEntityListPointer = baseAddress + addr_InteractObjectList;

            long entStart = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer);
            long entOffset = MemoryRead.ReadLong((int)process.Handle, entStart + 0x58);

            if (entOffset == 0) // If no list, return
            {
                return offsets;
            }

            for (int i=0; i < 8; i++)
            {
                entStart = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer);
                entOffset = MemoryRead.ReadLong((int)process.Handle, entStart + 0x58);
                offsets.Add(entOffset);
                baseEntityListPointer += 1; // ????
               // currentAddress = MemoryRead.ReadLong((int)process.Handle, currentAddress + 0x150);
            }
            return offsets;
        }
        public float[] getEntPosRaw(long entOffset)
        {
            try
            {
                float y = MemoryRead.ReadFloat((int)process.Handle, entOffset + 0x364);
                float x = MemoryRead.ReadFloat((int)process.Handle, entOffset + 0x360);
                float z = MemoryRead.ReadFloat((int)process.Handle, entOffset + 0x368);
                return new float[3] { x, y, z };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new float[3] { 0, 0, 0 };

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
        public void setPlayerPos(float x, float y, float z)
        {
            long playerDataOffset = baseAddress + addr_PlayerData;
            playerDataOffset = MemoryRead.ReadLong((int)process.Handle, playerDataOffset);
            MemoryRead.WriteFloat((int)process.Handle, playerDataOffset + 0x360, x);
            MemoryRead.WriteFloat((int)process.Handle, playerDataOffset + 0x364, y);
            MemoryRead.WriteFloat((int)process.Handle, playerDataOffset + 0x368, z);
        }
        public void setPlayerPos(float[] pos)
        {
            setPlayerPos(pos[0], pos[1], pos[2]);
        }
        public P4Entities (ILogger _logger) 
        {
            _logger.WriteLine("Initialising P4Entities library...");
            process = Process.GetCurrentProcess();
            baseAddress = MemoryRead.GetProcessBaseAddress(process);

            SigScan(npcEntityListSignature, "npcEntityList", (offset) =>
            {
                _logger.WriteLine($"npcEntList offset: 0x{offset.ToString("X")}");
                addr_InteractNpcList = (offset - baseAddress);
                _logger.WriteLine($"npcEntList offset: 0x{addr_InteractNpcList.ToString("X")}");
            });

            _logger.WriteLine("Finished sig scan?");
        }
    }
}
