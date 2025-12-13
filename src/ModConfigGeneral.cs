using MGSC;
using ModConfigMenu.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalCurrency
{
    // Token: 0x02000006 RID: 6
    public class ModConfigGeneral
    {
        

        public ModConfigGeneral(string ModName, string ConfigPath)
        {
            this.ModName = ModName;
            this.ModData = new ModConfigData(ConfigPath);
            this.ModData.AddConfigHeader("STRING:General Settings", "general");
            this.ModData.AddConfigValue("general", "about_final", "STRING:<color=#f51b1b>The game must be restarted after setting then saving this config to take effect.</color>\n");

            this.ModData.AddConfigValue("general", "Disable_Terrorist_GlobalCurrency_On", false, "STRING:Disable Global Currency for PE", "STRING:Disable Global Currency mechanics for Public Enemy factions - which are: Quasimorphic factions, and Civil Resistance.");

            //this.ModData.AddConfigValue("general", "Debug_Log_On", false, "STRING:Debug Log", "STRING:For personal debugging. DO NOT TURN IT ON if you don't intend to.");

            this.ModData.RegisterModConfigData(ModName);
        }

        // Token: 0x04000011 RID: 17
        private string ModName;

        // Token: 0x04000012 RID: 18
        public ModConfigData ModData;

    }
}
