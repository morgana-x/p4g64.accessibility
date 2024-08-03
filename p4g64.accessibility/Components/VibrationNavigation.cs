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
using p4g64.accessibility.Configuration;
using DavyKager;
using p4g64.accessibility.Utility;

namespace p4g64.accessibility.Components
{
    internal unsafe class VibrationNavigation
    {

        Process process;
        Config config;
        P4Entities p4Ents;

        DateTime nextVibrate = DateTime.Now;

        IReloadedHooks hooks;
        ILogger logger;

        static long addr_InteractPromptOpen = 0x11BC7F4;
        static long addr_CanAttackEnemy = 0x1E4382CC;
        static long addr_IsInDialogue = 0x51FE97C;
        static long addr_IsInPauseMenu = 0x11A63FC;

        bool running = true;
        bool currentPromptState = false;
        bool interactPromptOpen = false;
        bool noEntities = false;
   


        long baseAddress;

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

        float getClosestDistance(float[] offset = null)
        {
            float smallestDistance = 2000f;

            List<long> entOffsets = p4Ents.getEntityOffsetsList();
            noEntities = entOffsets.Count < 1 ? true : false;

            if (noEntities)
            {
                return smallestDistance;
            }

            float[] pPos = p4Ents.getPlayerPos();
            if (offset != null)
            {
                pPos[0] += offset[0];
                pPos[1] += offset[1];
                pPos[2] += offset[2];
            }
            foreach (var o in entOffsets)
            {
                float[] ePos = p4Ents.getEntPos(o);
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
            logger.WriteLine("Smallest Distance: " + smallestDistance);
            logger.WriteLine(string.Join(",", pPos));
            return smallestDistance;
        }
        float getIntensity(float dist)
        {
            if (interactPromptOpen)
            {
                return 0.3f;
            }

            return (float)Math.Min(100 / dist, 0.15f);
        }
        float getNextVibrate(float dist)
        {
            float frequency = (dist + 1) / 1000f * 0.5f;
            frequency = Math.Min(frequency, 2f);
            frequency = Math.Max(frequency, 0.2f);
            return frequency;
        }

        void Vibrate(bool _interactPrompOpen)
        {

            interactPromptOpen = _interactPrompOpen;

            if (!interactPromptOpen)
            {
                XInput.SetVibration(getControllerSlot(), 0, 0);
            }
            if (DateTime.Now < nextVibrate)
            {
                return;
            }

            float closestDistance = getClosestDistance();
            float closestDistanceLeft = getClosestDistance(new float[] { -10f, 0, 0 });
            float closestDistanceRight = getClosestDistance(new float[] { 10f, 0, 0 });
            nextVibrate = DateTime.Now.AddSeconds(getNextVibrate(closestDistance));

            float intensityLeft = getIntensity(closestDistanceLeft);
            float intensityRight = getIntensity(closestDistanceRight);
            XInput.SetVibration(getControllerSlot(), intensityLeft, intensityLeft);
        }
        bool canMove()
        {
            bool inMenu = MemoryRead.ReadByte((int)process.Handle, baseAddress + addr_IsInPauseMenu) == 1 ? true : false;
            bool inDialouge = MemoryRead.ReadByte((int)process.Handle, baseAddress + addr_IsInDialogue) == 1 ? true : false;
            return !(inMenu || inDialouge || noEntities);
        }
        void Run()
        {
            logger.WriteLine("Begin run!");
            while (running)
            {
                Thread.Sleep(10);
                getClosestDistance();
                if (!canMove())
                {
                    XInput.SetVibration(getControllerSlot(), 0, 0);
                    continue;
                }

                bool interactPromptOpen = MemoryRead.ReadByte((int)process.Handle, baseAddress + addr_InteractPromptOpen) == 1 ? true : false;

     
                Vibrate(interactPromptOpen);

                if ( (interactPromptOpen != currentPromptState) && interactPromptOpen && config.TextToSpeechInteractPrompt)
                {
                    Tolk.Speak("Interact Prompt");
                }

                currentPromptState = interactPromptOpen;
            }
        }

        internal VibrationNavigation(ILogger _logger, IReloadedHooks _hooks, Config _config, P4Entities _p4Ents)
        {
            _logger.WriteLine("Hello vibration navigation!");

            process = Process.GetCurrentProcess();
            baseAddress = MemoryRead.GetProcessBaseAddress(process);

            logger = _logger;
            hooks = _hooks;
            config = _config;
            p4Ents = _p4Ents;

            Task.Run(Run);
        }
    }
}
