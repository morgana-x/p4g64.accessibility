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

        //private IHook<CheckDistFuncDelegate> _interactableObjDistanceCompareHook;
       // private IAsmHook _interactableAsmObjDistanceCompareHook;
        float closestObjectDist = -1f;
       // private IntPtr _objectDistTemp;
        bool running = true;
        bool currentPromptState = false;
        long baseAddress;

        ILogger logger;

        //private IReverseWrapper<CheckDistFuncAsmHookDelegate>? _checkObjectDistanceReverseWrapper;

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
                Thread.Sleep(50);
                XInput.SetVibration(getControllerSlot(), 0.15f, 0.15f);

            }
        }

        // P4G.exe+2EB93B - F3 0F10 05 CDF98300   - movss xmm0,[P4G.exe+B2B310]
        /*private void CheckDistFunc(int entOffset, nint arg2, nint arg3)
        {
            //Color oldColor = logger.TextColor;
            logger.WriteLine("ObjectDistCheck was called with params: " + entOffset + ", " + arg2 + ", " + arg3);
           /* int newOffset = MemoryRead.ReadInt((int)process.Handle, entOffset + 48);
            logger.WriteLine("Entity offset: " + newOffset);
            float dist = MemoryRead.ReadFloat((int)process.Handle, newOffset + 38 ); // 0x38


            logger.TextColor = logger.ColorPink;
            logger.WriteLine("Dist: " + dist.ToString());
            logger.TextColor = oldColor;
        }*/
        
        /*private void CheckDistFunc(int entOffset, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9, int param10, int param12)
        {
            _interactableObjDistanceCompareHook.OriginalFunction(entOffset,  param2,  param3,  param4,  param5, param6, param7, param8, param9, param10, param12);
            logger.WriteLine("ObjectDistCheck was called with params: " + entOffset + ", " + param2 + ", " + param3 +", " + param4 + ", " + param5 + ", " + param6 + ", " + param7 + ", " + param8 + ", " + param9 + ", " + param10 + ", " + param12);
        }
        private float CheckDistFuncAsm(float dist) // entOffset should be RCX
        {
            logger.WriteLine("ObjectDistCheck was called with params: " + dist);
            return dist;
        }
        // INTERNAL CLR Error*/
        internal VibrationNavigation(ILogger _logger, IReloadedHooks _hooks)
        {
            _logger.WriteLine("Hello vibration navigation!");

            process = Process.GetCurrentProcess();
            baseAddress = MemoryRead.GetProcessBaseAddress(process);
            logger = _logger;
 

            Task.Run(Run);

            /*SigScan(hookSignature_CheckDistance2, "P4G_InteractableObject_CheckInRange_Hook", (nint address) => {
                logger.WriteLine("Address: " + address);
               // _interactableObjDistanceCompareHook = _hooks.CreateHook<CheckDistFuncDelegate>(CheckDistFunc, address).Activate();
                
                string[] newFunction =
                {
                    "use64",
                    "push r10",
                    $"{_hooks.Utilities.GetAbsoluteCallMnemonics(CheckDistFuncAsm, out _checkObjectDistanceReverseWrapper)}",
                    //"pop r10",
                };
                _interactableAsmObjDistanceCompareHook = _hooks.CreateAsmHook(newFunction, address, AsmHookBehaviour.ExecuteFirst ).Activate();
            });*/
        }
       
        /*
        [Function(CallingConventions.Microsoft)]
        private delegate void CheckDistFuncDelegate(int entOffset, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9, int param10, int param12);

        [Function(Register.r10, Register.r10, true)]
        private delegate float CheckDistFuncAsmHookDelegate(float dist);*/
    }
}

/* Entity arg stored in rcx
P4G.exe+2EB900 - 40 53                 - push rbx                       
P4G.exe+2EB902 - 41 56                 - push r14
P4G.exe+2EB904 - 41 57                 - push r15
P4G.exe+2EB906 - 48 83 EC 40           - sub rsp,40
P4G.exe+2EB90A - 48 89 6C 24 60        - mov [rsp+60],rbp
P4G.exe+2EB90F - 48 8B E9              - mov rbp,rcx
P4G.exe+2EB912 - 48 8D 0D B5411C1E     - lea rcx,[P4G.exe+1E4AFACE]
P4G.exe+2EB919 - 4C 89 64 24 78        - mov [rsp+78],r12
P4G.exe+2EB91E - E8 E1605900           - call P4G.exe+881A04
P4G.exe+2EB923 - 48 8B 5D 48           - mov rbx,[rbp+48]
P4G.exe+2EB927 - 45 33 F6              - xor r14d,r14d
P4G.exe+2EB92A - 0F57 D2               - xorps xmm2,xmm2
P4G.exe+2EB92D - F3 0F10 4B 38         - movss xmm1,[rbx+38]
P4G.exe+2EB932 - 0F2F CA               - comiss xmm1,xmm2
P4G.exe+2EB935 - 44 89 73 18           - mov [rbx+18],r14d
P4G.exe+2EB939 - 76 22                 - jna P4G.exe+2EB95D
P4G.exe+2EB93B - F3 0F10 05 CDF98300   - movss xmm0,[P4G.exe+B2B310]
P4G.exe+2EB943 - F3 0F59 05 ED926700   - mulss xmm0,[P4G.exe+964C38]
P4G.exe+2EB94B - F3 0F5C C8            - subss xmm1,xmm0
P4G.exe+2EB94F - 0F2F D1               - comiss xmm2,xmm1
P4G.exe+2EB952 - F3 0F11 4B 38         - movss [rbx+38],xmm1
P4G.exe+2EB957 - 76 04                 - jna P4G.exe+2EB95D
P4G.exe+2EB959 - 44 89 73 38           - mov [rbx+38],r14d
P4G.exe+2EB95D - E8 1E020300           - call P4G.exe+31BB80
P4G.exe+2EB962 - 41 BF 01000000        - mov r15d,00000001
P4G.exe+2EB968 - 44 39 B3 E4010000     - cmp [rbx+000001E4],r14d
P4G.exe+2EB96F - 75 3F                 - jne P4G.exe+2EB9B0
P4G.exe+2EB971 - 48 8B 05 20C77B00     - mov rax,[P4G.exe+AA8098]
P4G.exe+2EB978 - 41 8B CF              - mov ecx,r15d
P4G.exe+2EB97B - 48 8B 40 08           - mov rax,[rax+08]
P4G.exe+2EB97F - 48 85 C0              - test rax,rax
P4G.exe+2EB982 - 74 26                 - je P4G.exe+2EB9AA
P4G.exe+2EB984 - 48 8B 40 18           - mov rax,[rax+18]
P4G.exe+2EB988 - 48 85 C0              - test rax,rax
P4G.exe+2EB98B - 74 1D                 - je P4G.exe+2EB9AA
P4G.exe+2EB98D - 0F1F 00               - nop [rax]
P4G.exe+2EB990 - F7 40 28 00000010     - test [rax+28],10000000 : [00905A4D]
P4G.exe+2EB997 - 74 0E                 - je P4G.exe+2EB9A7
P4G.exe+2EB999 - 48 8B 80 50010000     - mov rax,[rax+00000150]
P4G.exe+2EB9A0 - 48 85 C0              - test rax,rax
P4G.exe+2EB9A3 - 75 EB                 - jne P4G.exe+2EB990
P4G.exe+2EB9A5 - EB 03                 - jmp P4G.exe+2EB9AA
P4G.exe+2EB9A7 - 41 8B CE              - mov ecx,r14d
P4G.exe+2EB9AA - 89 8B E4010000        - mov [rbx+000001E4],ecx
P4G.exe+2EB9B0 - 8B 0B                 - mov ecx,[rbx]
P4G.exe+2EB9B2 - 4C 8D 25 E7168200     - lea r12,[P4G.exe+B0D0A0]
P4G.exe+2EB9B9 - 48 89 7C 24 70        - mov [rsp+70],rdi
P4G.exe+2EB9BE - 44 89 B3 F0010000     - mov [rbx+000001F0],r14d
P4G.exe+2EB9C5 - 85 C9                 - test ecx,ecx
P4G.exe+2EB9C7 - 74 0E                 - je P4G.exe+2EB9D7
P4G.exe+2EB9C9 - 41 3B CF              - cmp ecx,r15d
P4G.exe+2EB9CC - 0F84 DB000000         - je P4G.exe+2EBAAD
P4G.exe+2EB9D2 - E9 19010000           - jmp P4G.exe+2EBAF0
P4G.exe+2EB9D7 - 48 89 74 24 68        - mov [rsp+68],rsi
P4G.exe+2EB9DC - 41 8B FE              - mov edi,r14d
P4G.exe+2EB9DF - 49 8B F4              - mov rsi,rsp
P4G.exe+2EB9E2 - 48 8B CD              - mov rcx,rbp
P4G.exe+2EB9E5 - FF 16                 - call qword ptr [rsi]
P4G.exe+2EB9E7 - 41 3B C7              - cmp eax,r15d
P4G.exe+2EB9EA - 74 1C                 - je P4G.exe+2EBA08
P4G.exe+2EB9EC - FF C7                 - inc edi
P4G.exe+2EB9EE - 48 83 C6 10           - add rsi,10
P4G.exe+2EB9F2 - 44 39 73 08           - cmp [rbx+08],r14d
P4G.exe+2EB9F6 - B8 10000000           - mov eax,00000010
P4G.exe+2EB9FB - 41 0F45 C7            - cmovne eax,r15d
P4G.exe+2EB9FF - 3B F8                 - cmp edi,eax
P4G.exe+2EBA01 - 7C DF                 - jnge P4G.exe+2EB9E2
P4G.exe+2EBA03 - 8B 7B 0C              - mov edi,[rbx+0C]
P4G.exe+2EBA06 - EB 03                 - jmp P4G.exe+2EBA0B
P4G.exe+2EBA08 - 89 7B 0C              - mov [rbx+0C],edi
P4G.exe+2EBA0B - 48 8B 74 24 68        - mov rsi,[rsp+68]
P4G.exe+2EBA10 - 83 FF FF              - cmp edi,-01
P4G.exe+2EBA13 - 0F8E C8000000         - jng P4G.exe+2EBAE1
P4G.exe+2EBA19 - 48 8B 05 C855BD00     - mov rax,[P4G.exe+EC0FE8]
P4G.exe+2EBA20 - 48 8B 48 08           - mov rcx,[rax+08]
P4G.exe+2EBA24 - 48 85 C9              - test rcx,rcx
P4G.exe+2EBA27 - 74 14                 - je P4G.exe+2EBA3D
P4G.exe+2EBA29 - 48 8B 81 C8010000     - mov rax,[rcx+000001C8]
P4G.exe+2EBA30 - 48 85 C0              - test rax,rax
P4G.exe+2EBA33 - 74 08                 - je P4G.exe+2EBA3D
P4G.exe+2EBA35 - 8B 88 BC020000        - mov ecx,[rax+000002BC]
P4G.exe+2EBA3B - EB 03                 - jmp P4G.exe+2EBA40
P4G.exe+2EBA3D - 41 8B CE              - mov ecx,r14d
P4G.exe+2EBA40 - 0FBF F9               - movsx edi,cx
P4G.exe+2EBA43 - 41 8B CF              - mov ecx,r15d
P4G.exe+2EBA46 - E8 25300100           - call P4G.exe+2FEA70
P4G.exe+2EBA4B - 3B F8                 - cmp edi,eax
P4G.exe+2EBA4D - 74 0C                 - je P4G.exe+2EBA5B
P4G.exe+2EBA4F - 41 8B CF              - mov ecx,r15d
P4G.exe+2EBA52 - E8 F9300100           - call P4G.exe+2FEB50
P4G.exe+2EBA57 - 3B F8                 - cmp edi,eax
P4G.exe+2EBA59 - 75 30                 - jne P4G.exe+2EBA8B
P4G.exe+2EBA5B - 41 8B CF              - mov ecx,r15d
P4G.exe+2EBA5E - BF 04000000           - mov edi,00000004
P4G.exe+2EBA63 - E8 382F0100           - call P4G.exe+2FE9A0
P4G.exe+2EBA68 - 48 8B 0D 7955BD00     - mov rcx,[P4G.exe+EC0FE8]
P4G.exe+2EBA6F - 44 8B C0              - mov r8d,eax
P4G.exe+2EBA72 - 33 D2                 - xor edx,edx
P4G.exe+2EBA74 - 66 44 89 7C 24 20     - mov [rsp+20],r15w
P4G.exe+2EBA7A - 44 0FB7 CF            - movzx r9d,di
P4G.exe+2EBA7E - E8 FDA32000           - call P4G.exe+4F5E80
P4G.exe+2EBA83 - 41 8B CF              - mov ecx,r15d
P4G.exe+2EBA86 - E8 35FFFDFF           - call P4G.exe+2CB9C0
P4G.exe+2EBA8B - 48 8B 05 36F8EB00     - mov rax,[P4G.exe+11AB2C8]
P4G.exe+2EBA92 - 48 85 C0              - test rax,rax
P4G.exe+2EBA95 - 74 08                 - je P4G.exe+2EBA9F
P4G.exe+2EBA97 - 48 8B 40 48           - mov rax,[rax+48]
P4G.exe+2EBA9B - 83 48 04 02           - or dword ptr [rax+04],02
P4G.exe+2EBA9F - 44 89 B3 B8010000     - mov [rbx+000001B8],r14d
P4G.exe+2EBAA6 - 44 89 73 18           - mov [rbx+18],r14d
P4G.exe+2EBAAA - 44 89 3B              - mov [rbx],r15d
P4G.exe+2EBAAD - 48 63 43 0C           - movsxd  rax,dword ptr [rbx+0C]
P4G.exe+2EBAB1 - 48 8B CD              - mov rcx,rbp
P4G.exe+2EBAB4 - 48 03 C0              - add rax,rax
P4G.exe+2EBAB7 - 41 FF 54 C4 08        - call qword ptr [r12+rax*8+08]
P4G.exe+2EBABC - 41 3B C7              - cmp eax,r15d
P4G.exe+2EBABF - 74 2F                 - je P4G.exe+2EBAF0
P4G.exe+2EBAC1 - 48 8B 05 00F8EB00     - mov rax,[P4G.exe+11AB2C8]
P4G.exe+2EBAC8 - 48 85 C0              - test rax,rax
P4G.exe+2EBACB - 74 08                 - je P4G.exe+2EBAD5
P4G.exe+2EBACD - 48 8B 40 48           - mov rax,[rax+48]
P4G.exe+2EBAD1 - 83 60 04 FD           - and dword ptr [rax+04],-03
P4G.exe+2EBAD5 - C7 43 0C FFFFFFFF     - mov [rbx+0C],FFFFFFFF
P4G.exe+2EBADC - 44 89 33              - mov [rbx],r14d
P4G.exe+2EBADF - EB 0F                 - jmp P4G.exe+2EBAF0
P4G.exe+2EBAE1 - 48 8B CD              - mov rcx,rbp
P4G.exe+2EBAE4 - E8 C7050000           - call P4G.exe+2EC0B0
P4G.exe+2EBAE9 - 44 89 BB F0010000     - mov [rbx+000001F0],r15d
P4G.exe+2EBAF0 - 4C 8B 64 24 78        - mov r12,[rsp+78]
P4G.exe+2EBAF5 - 48 8B 7C 24 70        - mov rdi,[rsp+70]
P4G.exe+2EBAFA - 48 8B 6C 24 60        - mov rbp,[rsp+60]
P4G.exe+2EBAFF - 48 8B 05 7A56BD00     - mov rax,[P4G.exe+EC1180]
P4G.exe+2EBB06 - 44 39 7B 18           - cmp [rbx+18],r15d
P4G.exe+2EBB0A - 0F85 FB000000         - jne P4G.exe+2EBC0B
P4G.exe+2EBB10 - 48 85 C0              - test rax,rax
P4G.exe+2EBB13 - 0F84 FF000000         - je P4G.exe+2EBC18
P4G.exe+2EBB19 - 4C 8B 05 C854BD00     - mov r8,[P4G.exe+EC0FE8]
P4G.exe+2EBB20 - 48 8D 54 24 30        - lea rdx,[rsp+30]
P4G.exe+2EBB25 - 48 8B 0D 94CCF505     - mov rcx,[P4G.exe+62487C0]
P4G.exe+2EBB2C - 49 81 C0 60030000     - add r8,00000360
P4G.exe+2EBB33 - 48 8B 58 48           - mov rbx,[rax+48]
P4G.exe+2EBB37 - E8 F40D2200           - call P4G.exe+50C930
P4G.exe+2EBB3C - F3 0F10 05 709A6700   - movss xmm0,[P4G.exe+9655B4]
P4G.exe+2EBB44 - F3 0F10 4C 24 30      - movss xmm1,[rsp+30]
P4G.exe+2EBB4A - 0F2F C1               - comiss xmm0,xmm1 /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// distance stored in r10 value I think at here
P4G.exe+2EBB4D - 72 1E                 - jb P4G.exe+2EBB6D
P4G.exe+2EBB4F - 4C 8D 0D D6416200     - lea r9,[P4G.exe+90FD2C]
P4G.exe+2EBB56 - 48 8D 15 BF426200     - lea rdx,[P4G.exe+90FE1C]
P4G.exe+2EBB5D - 4C 8D 15 C4416200     - lea r10,[P4G.exe+90FD28]
P4G.exe+2EBB64 - 48 8D 0D AD426200     - lea rcx,[P4G.exe+90FE18]
P4G.exe+2EBB6B - EB 47                 - jmp P4G.exe+2EBBB4
P4G.exe+2EBB6D - F3 0F10 05 C3A26700   - movss xmm0,[P4G.exe+965E38]
P4G.exe+2EBB75 - 0F2F C1               - comiss xmm0,xmm1
P4G.exe+2EBB78 - 76 1E                 - jna P4G.exe+2EBB98
P4G.exe+2EBB7A - 4C 8D 0D B3416200     - lea r9,[P4G.exe+90FD34]
P4G.exe+2EBB81 - 48 8D 15 9C426200     - lea rdx,[P4G.exe+90FE24]
P4G.exe+2EBB88 - 4C 8D 15 A1416200     - lea r10,[P4G.exe+90FD30]
P4G.exe+2EBB8F - 48 8D 0D 8A426200     - lea rcx,[P4G.exe+90FE20]
P4G.exe+2EBB96 - EB 1C                 - jmp P4G.exe+2EBBB4
P4G.exe+2EBB98 - 4C 8D 0D 9D416200     - lea r9,[P4G.exe+90FD3C]
P4G.exe+2EBB9F - 48 8D 15 86426200     - lea rdx,[P4G.exe+90FE2C]
P4G.exe+2EBBA6 - 4C 8D 15 8B416200     - lea r10,[P4G.exe+90FD38]
P4G.exe+2EBBAD - 48 8D 0D 74426200     - lea rcx,[P4G.exe+90FE28]
P4G.exe+2EBBB4 - 4C 8B 1D DDC47B00     - mov r11,[P4G.exe+AA8098]
P4G.exe+2EBBBB - 45 0FB7 03            - movzx r8d,word ptr [r11]
P4G.exe+2EBBBF - 41 8D 40 D8           - lea eax,[r8-28]
P4G.exe+2EBBC3 - 66 83 F8 13           - cmp ax,13
P4G.exe+2EBBC7 - 77 07                 - ja P4G.exe+2EBBD0
P4G.exe+2EBBC9 - 66 45 39 73 04        - cmp [r11+04],r14w
P4G.exe+2EBBCE - 74 12                 - je P4G.exe+2EBBE2
P4G.exe+2EBBD0 - 66 41 83 E8 3C        - sub r8w,3C
P4G.exe+2EBBD5 - 66 41 83 F8 13        - cmp r8w,13
P4G.exe+2EBBDA - 76 06                 - jna P4G.exe+2EBBE2
P4G.exe+2EBBDC - 49 8B D1              - mov rdx,r9
P4G.exe+2EBBDF - 49 8B CA              - mov rcx,r10
P4G.exe+2EBBE2 - 0FB7 09               - movzx ecx,word ptr [rcx]
P4G.exe+2EBBE5 - 0FB7 02               - movzx eax,word ptr [rdx]
P4G.exe+2EBBE8 - 66 89 4B 14           - mov [rbx+14],cx
P4G.exe+2EBBEC - 66 89 43 16           - mov [rbx+16],ax
P4G.exe+2EBBF0 - 48 8B 05 8955BD00     - mov rax,[P4G.exe+EC1180]
P4G.exe+2EBBF7 - 48 8B 48 48           - mov rcx,[rax+48]
P4G.exe+2EBBFB - 33 C0                 - xor eax,eax
P4G.exe+2EBBFD - 44 89 79 04           - mov [rcx+04],r15d
P4G.exe+2EBC01 - 48 83 C4 40           - add rsp,40
P4G.exe+2EBC05 - 41 5F                 - pop r15
P4G.exe+2EBC07 - 41 5E                 - pop r14
P4G.exe+2EBC09 - 5B                    - pop rbx
P4G.exe+2EBC0A - C3                    - ret 
P4G.exe+2EBC0B - 48 85 C0              - test rax,rax
P4G.exe+2EBC0E - 74 08                 - je P4G.exe+2EBC18
P4G.exe+2EBC10 - 48 8B 40 48           - mov rax,[rax+48]
P4G.exe+2EBC14 - 44 89 70 04           - mov [rax+04],r14d
P4G.exe+2EBC18 - 33 C0                 - xor eax,eax
P4G.exe+2EBC1A - 48 83 C4 40           - add rsp,40
P4G.exe+2EBC1E - 41 5F                 - pop r15
P4G.exe+2EBC20 - 41 5E                 - pop r14
P4G.exe+2EBC22 - 5B                    - pop rbx
P4G.exe+2EBC23 - C3                    - ret 

 */
