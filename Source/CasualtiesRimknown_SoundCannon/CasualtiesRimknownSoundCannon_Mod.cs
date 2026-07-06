using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace CasualtiesRimknown_SoundCannon
{
    public class CasualtiesRimknownSoundCannon_Mod : Mod
    {
        Mod_Settings settings;

        public CasualtiesRimknownSoundCannon_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<Mod_Settings>();
        }

        public override string SettingsCategory()
        {
            return "Casualties: Rimknown - Sound Cannon".Translate();
        }
    }

    public class Mod_Settings : ModSettings
    {
        // Save to File
        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
