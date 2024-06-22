using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using XInputDotNetPure;

namespace p4g64.accessibility.Components
{
    internal class VibrationNavigation
    {
        Process process;
        static long addr_InteractPromptOpen = 0x11BC7F4;
        bool running = true;
        bool currentPromptState = false;
        long baseAddress;
        ILogger logger;

        PlayerIndex getControllerSlot()
        {
            PlayerIndex result = PlayerIndex.One;
            for(int i=0; i<4;i++)
            {
                PlayerIndex p = (PlayerIndex)i;
                if (XInputDotNetPure.GamePad.GetState(p).IsConnected)
                {
                    result = p;
                    break;
                }
            }
            return result;
        }
        void Run()
        {
            while (running)
            {
                bool newPromptState = MemoryRead.ReadByte((int)process.Handle, baseAddress + addr_InteractPromptOpen) == 1 ? true : false;
                bool changed = newPromptState != currentPromptState;
                currentPromptState= newPromptState;
                if (changed && !currentPromptState)
                {
                    XInputDotNetPure.GamePad.SetVibration(getControllerSlot(), 0f, 0f);
                    continue;
                }
                if (!currentPromptState)
                {
                    continue;
                }
                //logger.WriteLine("Vibrating!");
                GamePad.SetVibration(getControllerSlot(), 0.25f, 0.25f);
                Thread.Sleep(10);
            }
        }
        internal VibrationNavigation(ILogger _logger) 
        {
            
            process = Process.GetCurrentProcess();
            baseAddress = MemoryRead.GetProcessBaseAddress(process);
            logger = _logger;
            _logger.WriteLine("Hello vibration navigation!");
            Task.Run(Run);
         
      
        }
        
    }
}
