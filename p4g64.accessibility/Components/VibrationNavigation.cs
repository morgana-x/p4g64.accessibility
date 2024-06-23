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

namespace p4g64.accessibility.Components
{
    internal class VibrationNavigation
    {
        //P4G.exe+2EB900 - 40 53                 - push rbx
        // Compare distance for particular object function?
        /*
         * 40 53 41 56 41 57 48 83 EC 40 48 89 6C 24 60 48 8B E9 48 8D 0D B5 41 1C 1E 4C 89 64 24 78 E8 E1 60 59 00 48 8B 5D 48 45 33 F6 0F 57 D2 F3 0F 10 4B 38 0F 2F CA 44 89 73 18 76 22 F3 0F 10 05 CD F9 83 00 F3 0F 59 05 ED 92 67 00 F3 0F 5C C8 0F 2F D1 F3 0F 11 4B 38 76 04 44 89 73 38 E8 1E 02 03 00 41 BF 01 00 00 00 44 39 B3 E4 01 00 00 75 3F 48 8B 05 20 C7 7B 00 41 8B CF 48 8B 40 08 48 85 C0 74 26 48 8B 40 18 48 85 C0 74 1D 0F 1F 00 F7 40 28 00 00 00 10 74 0E 48 8B 80 50 01 00 00 48 85 C0 75 EB EB 03 41 8B CE 89 8B E4 01 00 00 8B 0B 4C 8D 25 E7 16 82 00 48 89 7C 24 70 44 89 B3 F0 01 00 00 85 C9 74 0E 41 3B CF 0F 84 DB 00 00 00 E9 19 01 00 00 48 89 74 24 68 41 8B FE 49 8B F4 48 8B CD FF 16 41 3B C7 74 1C FF C7 48 83 C6 10 44 39 73 08 B8 10 00 00 00 41 0F 45 C7 3B F8 7C DF 8B 7B 0C EB 03 89 7B 0C 48 8B 74 24 68 83 FF FF 0F 8E C8 00 00 00 48 8B 05 C8 55 BD 00 48 8B 48 08 48 85 C9 74 14 48 8B 81 C8 01 00 00 48 85 C0 74 08 8B 88 BC 02 00 00 EB 03 41 8B CE 0F BF F9 41 8B CF E8 25 30 01 00 3B F8 74 0C 41 8B CF E8 F9 30 01 00 3B F8 75 30 41 8B CF BF 04 00 00 00 E8 38 2F 01 00 48 8B 0D 79 55 BD 00 44 8B C0 33 D2 66 44 89 7C 24 20 44 0F B7 CF E8 FD A3 20 00 41 8B CF E8 35 FF FD FF 48 8B 05 36 F8 EB 00 48 85 C0 74 08 48 8B 40 48 83 48 04 02 44 89 B3 B8 01 00 00 44 89 73 18 44 89 3B 48 63 43 0C 48 8B CD 48 03 C0 41 FF 54 C4 08 41 3B C7 74 2F 48 8B 05 00 F8 EB 00 48 85 C0 74 08 48 8B 40 48 83 60 04 FD C7 43 0C FF FF FF FF 44 89 33 EB 0F 48 8B CD E8 C7 05 00 00 44 89 BB F0 01 00 00 4C 8B 64 24 78 48 8B 7C 24 70 48 8B 6C 24 60 48 8B 05 7A 56 BD 00 44 39 7B 18 0F 85 FB 00 00 00 48 85 C0 0F 84 FF 00 00 00 4C 8B 05 C8 54 BD 00 48 8D 54 24 30 48 8B 0D 94 CC F5 05 49 81 C0 60 03 00 00 48 8B 58 48 E8 F4 0D 22 00 F3 0F 10 05 70 9A 67 00 F3 0F 10 4C 24 30 0F 2F C1 72 1E 4C 8D 0D D6 41 62 00 48 8D 15 BF 42 62 00 4C 8D 15 C4 41 62 00 48 8D 0D AD 42 62 00 EB 47 F3 0F 10 05 C3 A2 67 00 0F 2F C1 76 1E 4C 8D 0D B3 41 62 00 48 8D 15 9C 42 62 00 4C 8D 15 A1 41 62 00 48 8D 0D 8A 42 62 00 EB 1C 4C 8D 0D 9D 41 62 00 48 8D 15 86 42 62 00 4C 8D 15 8B 41 62 00 48 8D 0D 74 42 62 00 4C 8B 1D DD C4 7B 00 45 0F B7 03 41 8D 40 D8 66 83 F8 13 77 07 66 45 39 73 04 74 12 66 41 83 E8 3C 66 41 83 F8 13 76 06 49 8B D1 49 8B CA 0F B7 09 0F B7 02 66 89 4B 14 66 89 43 16 48 8B 05 89 55 BD 00 48 8B 48 48 33 C0 44 89 79 04 48 83 C4 40 41 5F 41 5E 5B C3 48 85 C0 74 08 48 8B 40 48 44 89 70 04 33 C0 48 83 C4 40 41 5F 41 5E 5B C3
         */
        Process process;
        static long addr_InteractPromptOpen = 0x11BC7F4;

        static long addr_MinDistPromptOpen = 0x9655B4; //??? Where 4 byte float for min dist for a prompt to appear is stored?
        // F3 0F 10 05 70 9A 67 00
        static string hookSignature_CheckDistanceSuccess = "F3 0F 10 05 70 9A 67 00"; // xmm0 is the MinDist, xmm1 is the distance between player and object
        static string hookSignature_CheckDistance = "0F 2F CA 44 89 73 18 76 22 F3 0F 10 05 CD F9 83 00 F3 0F 59 05 ED 92 67 00 F3 0F 5C C8 0F 2F D1 F3 0F";
        static string hookSignature_CheckIntObjDistanceMethod = "40 53 41 56 41 57 48 83 EC 40 48 89 6C 24 60 48 8B E9 48 8D 0D B5 41 1C 1E 4C 89 64 24 78 E8 E1 60 59 00 48 8B 5D 48 45 33 F6 0F 57 D2 F3 0F 10 4B 38 0F 2F CA 44 89 73 18 76 22 F3 0F 10 05";
        private IAsmHook _interactableObjDistanceCompareHook;

        float closestObjectDist = -1f;
        private IntPtr _objectDistTemp;
        bool running = true;
        bool currentPromptState = false;
        long baseAddress;

        ILogger logger;

        private IReverseWrapper<CheckDistFuncDelegate>? _checkObjectDistanceReverseWrapper;

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
        void Run()
        {
            while (running)
            {
                Thread.Sleep(10);
                closestObjectDist = -1f;
                bool newPromptState = MemoryRead.ReadByte((int)process.Handle, baseAddress + addr_InteractPromptOpen) == 1 ? true : false;
                bool changed = newPromptState != currentPromptState;
                currentPromptState = newPromptState;
                if (changed && !currentPromptState)
                {
                    XInput.SetVibration(getControllerSlot(), 0f, 0f);
                    continue;
                }
                if (!currentPromptState)
                {
                    continue;
                }
                //logger.WriteLine("Vibrating!");
                XInput.SetVibration(getControllerSlot(), 0.25f, 0.25f);

            }
        }

        // P4G.exe+2EB93B - F3 0F10 05 CDF98300   - movss xmm0,[P4G.exe+B2B310]
        private void CheckDistFunc(float dist)
        {
            Color oldColor = logger.TextColor;
            logger.WriteLine("ObjectDistCheck was called with params: ");
            logger.TextColor = logger.ColorPink;
            logger.WriteLine(dist.ToString());
            logger.TextColor = oldColor;
        }
        internal VibrationNavigation(ILogger _logger, IReloadedHooks _hooks)
        {

            process = Process.GetCurrentProcess();
            baseAddress = MemoryRead.GetProcessBaseAddress(process);
            logger = _logger;
            _logger.WriteLine("Hello vibration navigation!");
            Task.Run(Run);

            //SigScan(hookSignature_CheckIntObjDistanceMethod, "P4G_InteractableObject_CheckInRange_Hook", InteractableObjectCheckInRange);
            SigScan(hookSignature_CheckIntObjDistanceMethod, "P4G_InteractableObject_CheckInRange_Hook", (nint address) => {

                string[] newFunction =
                {
                    "use64",
                    "push rcx\npush rax\npush r8\npush r9\npush r10\npush r11",
                    "mov rcx, rax",
                    "sub rsp, 32",
                    $"{_hooks.Utilities.GetAbsoluteCallMnemonics(CheckDistFunc, out _checkObjectDistanceReverseWrapper)}",
                    "add rsp, 32",
                    "pop r11\npop r10\npop r9\npop r8\npop rax\npop rcx",
                };
                _interactableObjDistanceCompareHook = _hooks.CreateAsmHook(newFunction, address, AsmHookBehaviour.ExecuteAfter).Activate();

            });
        }
        [Function(CallingConventions.Microsoft)]
        private delegate nuint CheckDistFuncDelegate(float dist);
    }
}
