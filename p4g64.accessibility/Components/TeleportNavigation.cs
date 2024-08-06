using p4g64.accessibility.Configuration;
using p4g64.accessibility.Utility;
using Reloaded.Hooks.Definitions;
using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p4g64.accessibility.Components
{
    internal class TeleportNavigation
    {
        Process process;
        Config config;
        P4Entities p4Ents;
        ILogger logger;
        IReloadedHooks hooks;
        long baseAddress;

        int selectedMenu = 0;
        bool menuOpen = false;

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
            while (true)
            { 
                menuOpen = XInput.GetButton(getControllerSlot(), XInputButton.LeftThumb) ? !menuOpen : menuOpen;
                if (!menuOpen)
                {
                    selectedMenu = 0;
                    continue;
                }

                List<long> entities = p4Ents.getNPCEntitiesOffsets();

                selectedMenu = XInput.GetButton(getControllerSlot(), XInputButton.DPadUp) ? selectedMenu + 1 : selectedMenu;
                selectedMenu = XInput.GetButton(getControllerSlot(), XInputButton.DPadDown) ? selectedMenu - 1 : selectedMenu;

        
                selectedMenu = Math.Min(selectedMenu, entities.Count-1);
                selectedMenu = Math.Max(selectedMenu, 0);

                XInput.SetVibration(getControllerSlot(), 0.1f * selectedMenu, 0.1f * selectedMenu);

                if (XInput.GetButton(getControllerSlot(), XInputButton.A))
                {
                    logger.WriteLine("Teleporting to entity " + selectedMenu);
                    menuOpen = false;
                    XInput.SetVibration(getControllerSlot(), 0.25f, 0.25f);
                    p4Ents.setPlayerPos(p4Ents.getEntPos(entities[selectedMenu]));
                }
            }
        }
        public TeleportNavigation(ILogger _logger, IReloadedHooks _hooks, Config _config, P4Entities _p4Ents)
        {
            _logger.WriteLine("Hello Teleportation navigation!");

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
