using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static p4g64.accessibility.Utils;
using Reloaded.Memory;
using static Reloaded.Hooks.Definitions.X64.FunctionAttribute;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.X64;
using System.Drawing;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace p4g64.accessibility.Components
{
    internal unsafe class VibrationNavigation
    {

        Process process;
        static long addr_InteractPromptOpen = 0x11BC7F4;
        static long addr_CanAttackEnemy = 0x1E4382CC;
        static long addr_PlayerData = 0xEC0FE8; //0x2E89E6;//0x140ec0fe8;

        static string closetEntPrompt = "48 89 6c 24 18 56 48 83 ec 50 48 89 5c 24 60 48 8b d9 48 8d 0d 25 72 1c 1e e8 56 91 59 00 48 8b 05 e3 f7 7b 00 33 f6 48 8b 6b 48 48 8b 58 08 48 85 db 0f 84 23 01 00 00 48 8b 5b 18 0f 29 74 24 40 f3 0f 10 35 0b d9 67 00 48 85 db 0f 84 04 01 00 00 48 89 7c 24 68 48 8b 3d fa 86 bd 00 0f 29 7c 24 30 f3 0f 10 3d c9 ca 67 00 44 0f 29 44 24 20 f3 44 0f 10 05 76 cb 67 00 66 0f 1f 44 00 00 8b 43 28 25 02 00 00 10 3d 02 00 00 10 0f 85 a3 00 00 00 48 8b 83 e8 02 00 00 48 85 c0 74 0d 48 8b 40 48 83 38 04 0f 82 8a 00 00 00 48 8b 93 90 01 00 00 48 8d 8f 30 03 00 00 48 81 c2 60 03 00 00 0f 28 d7 e8 97 53 03 00 48 8b 3d 88 86 bd 00 83 f8 01 75 61 48 8b 83 90 01 00 00 f3 0f 10 90 60 03 00 00 f3 0f 10 80 64 03 00 00 f3 0f 5c 87 64 03 00 00 f3 0f 5c 97 60 03 00 00 f3 0f 10 88 68 03 00 00 f3 0f 5c 8f 68 03 00 00 f3 0f 59 c0 f3 0f 59 d2 f3 0f 59 c9 f3 0f 58 c2 f3 0f 58 c1 e8 81 c8 53 00 44 0f 2f c0 76 0b 0f 2f f0 76 06 0f 28 f0 48 8b f3 48 8b 9b 50 01 00 00 48 85 db 0f 85 3a ff ff ff 44 0f 28 44 24 20 0f 28 7c 24 30 48 8b 7c 24 68 0f 28 74 24 40 48 8b 5c 24 60 48 89 75 30 48 85 f6 74 4b c7 45 18 01 00 00 00 83 be 70 02 00 00 00 76 3b f7 05 d1 4e 11 1e 00 20 00 00 75 09 80 3d 24 90 d4 00 00 74 26 f3 0f 10 4d 38 0f 57 c0 0f 2e c8 7a 19 75 17 c7 45 04 00 00 00 00 b8 01 00 00 00 48 8b 6c 24 70 48 83 c4 50 5e c3 48 8b 6c 24 70 33 c0 48 83 c4 50 5e c3";
        bool running = true;
        bool currentPromptState = false;
        long baseAddress;
        // PrepareEntityListDelegate importantThingToCallSoYouCanLoopEntities;



        ILogger logger;
        //private Memory _memory;

       // private IReverseWrapper<getClosestEntityDelegate>? _checkObjectDistanceReverseWrapper;

        IReloadedHooks hooks;
        long entityOffsetListOffset = 0xAA8098;//0x1E4AFACE; //0xAA8098

        uint getControllerSlot()
        {
            for (uint i = 0; i < 4; i++)
            {
                if (XInput.IsControllerConnected(i))
                {
                    return i;
                }
            }
            return 0;
        }

        private List<long> getEntityOffsetsList()
        {
            //importantThingToCallSoYouCanLoopEntities((0x15e4aface)); // pointer to char

            List<long> offsets = new List<long>();

            long baseEntityListPointer = MemoryRead.ReadLong((int)process.Handle, baseAddress + entityOffsetListOffset);

            baseEntityListPointer = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer + 8);

            if (baseEntityListPointer == 0) // If no list, return
            {
                return offsets;
            }

            long currentAddress = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer + 0x18);

            logger.WriteLine("Base entity list address: 0x" + baseEntityListPointer.ToString("X"));
            logger.WriteLine("Current addr: 0x" + currentAddress.ToString("X"));

            while (currentAddress != 0)
            {
                logger.WriteLine("Current entity address: 0x" + currentAddress.ToString("X"));
                offsets.Add(currentAddress);
                currentAddress = MemoryRead.ReadLong((int)process.Handle, currentAddress + 0x150);
            }
            return offsets;
        }
        private float[] getPos(long entOffset)
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
        private long getLocalEntity()
        {
            long baseEntityListPointer = MemoryRead.ReadLong((int)process.Handle, baseAddress + entityOffsetListOffset);
            baseEntityListPointer = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer + 8);
            if (baseEntityListPointer == 0)
            {
                return 0;
            }
            long entPointer = MemoryRead.ReadLong((int)process.Handle, baseEntityListPointer + 0x60);
            if (entPointer == 0)
            {
                return 0;
            }
            return entPointer + 0x160;
        }
        private float[] getPlayerPos()
        {
            long playerDataOffset = getLocalEntity();//baseAddress + addr_PlayerData;
            logger.WriteLine("Player data: 0x" + playerDataOffset.ToString("X"));
            return getPos(playerDataOffset);
           // playerDataOffset = MemoryRead.ReadLong((int)process.Handle, playerDataOffset + 0x400);
            logger.WriteLine("Player data: 0x" + playerDataOffset.ToString("X"));
            float y = MemoryRead.ReadFloat((int)process.Handle, playerDataOffset + 0x364);
            float x = MemoryRead.ReadFloat((int)process.Handle, playerDataOffset + 0x360);
            float z = MemoryRead.ReadFloat((int)process.Handle, playerDataOffset + 0x368);
            return new float[3] { x, y, z };
        }
        

        void fClosets()
        {
            List<long> entOffsets = getEntityOffsetsList();

            float[] pPos = getPlayerPos();
            float smallestDistance = 200f;
            foreach (var o in entOffsets)
            {
                float[] ePos = getPos(o);
                logger.WriteLine($"{o}");
                logger.WriteLine(string.Join(",", ePos));
                float dx = ePos[0] - pPos[0];
                float dy = ePos[1] - pPos[1];
                float dz = ePos[2] - pPos[2];
                float distance = (float)Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                }
            }
            closestDist = smallestDistance;
            logger.WriteLine("Smallest Distance: " + smallestDistance);
            logger.WriteLine(string.Join(",", pPos));
        }
        float closestDist = 200f;
        bool vibrateEnabled = false;
        DateTime nextVibrate = DateTime.Now;
        void Vibrate()
        {
            if (!vibrateEnabled)
            {
                return;
            }
            if (DateTime.Now < nextVibrate)
            {
                return;
            }
            nextVibrate = DateTime.Now.AddSeconds(0.1f + (10f/closestDist));
            XInput.SetVibration(getControllerSlot(), 0.15f, 0.15f);
        }
        void Run()
        {
            logger.WriteLine("Begin run!");
            while (running)
            {
                fClosets();
                Thread.Sleep(10);
                bool newState = MemoryRead.ReadByte((int)process.Handle, baseAddress + addr_InteractPromptOpen) == 1 ? true : false;
                vibrateEnabled = newState;
                Vibrate();
                if ((newState != currentPromptState) && !newState)
                {
                    XInput.SetVibration(getControllerSlot(), 0f, 0f);
                }
                currentPromptState = newState;


            }
        }
        private IReverseWrapper<getClosestEntityDelegate>? _speedChangeReverseWrapper;
        private IAsmHook? _interactPromptUpdateHook;
        internal VibrationNavigation(ILogger _logger, IReloadedHooks _hooks)
        {


            _logger.WriteLine("Hello vibration navigation!");

            process = Process.GetCurrentProcess();
            baseAddress = MemoryRead.GetProcessBaseAddress(process);
            logger = _logger;
            hooks = _hooks;

  
            SigScan(closetEntPrompt, "P4G_InteractableObject_CheckInRange_Hook", (nint address) => {
                logger.WriteLine("Address: 0x" + address.ToString("X"));
                logger.WriteLine("Creating hook!");
                string[] newFunc =
                {
                    "use64",
                    //"push",
                    $"{_hooks.Utilities.GetAbsoluteCallMnemonics(fClosets, out _speedChangeReverseWrapper)}",
                    //"pop"
                };
                //_interactPromptUpdateHook = _hooks.CreateAsmHook(newFunc, address, AsmHookBehaviour.ExecuteAfter).Activate();
                // _hooks.CreateHook<getClosestEntityDelegate>(findClosest, address).Activate();

            });

        

            Task.Run(Run);

        }
        [Function(CallingConventions.Microsoft)]
        private delegate void getClosestEntityDelegate();
        /*public void findClosest(long interactPromptInstanceOffset)
        {
            logger.WriteLine("Find closest ent hook!");
            fClosets();
          
            return;
        }



        [Function(CallingConventions.Microsoft)]
        private delegate void getClosestEntityDelegate(long interactPromptInstanceOffset);*/
    }
}
