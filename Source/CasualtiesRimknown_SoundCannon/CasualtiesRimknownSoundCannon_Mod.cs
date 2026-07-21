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
        public static Mod_Settings settings;

        private static bool isVanillaExpandedActive;

        public static bool IsVanillaExpandedActive => isVanillaExpandedActive;

        public CasualtiesRimknownSoundCannon_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Mod_Settings>();
            //
            isVanillaExpandedActive = ModLister.AnyModActiveNoSuffix(["OskarPotocki.VanillaFactionsExpanded.Core"]);
        }

        public override string SettingsCategory()
        {
            return "CasualtiesRimknown_SoundCannon".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard ListA = new Listing_Standard();
            ListA.Begin(inRect);
            //
            ListA.Label("CRSC_SettingLabel_SoundCannon".Translate());
            ListA.CheckboxLabeled("CRSC_Setting_UseFireLoud".Translate(), ref settings.useLoudFireSFX, "CRSC_Setting_UserFireLoud_TT".Translate());
            ListA.CheckboxLabeled("CRSC_Setting_EnableShockwaveLimbDamage".Translate(), ref settings.shockwaveDestroysLimbs, "CRSC_Setting_EnableShockwaveLimbDamage_TT".Translate());
            //
            ListA.GapLine();
            ListA.Label("CRSC_SettingLabel_Audio".Translate());
            settings.shockwaveNormalVolumeCoef = ListA.SliderLabeled("CRSC_Setting_ShockwaveNormalVolume".Translate(settings.shockwaveNormalVolumeCoef.ToString("F2")), settings.shockwaveNormalVolumeCoef, 0f, 5f, 0.5f, "CRSC_Setting_ShockwaveNormalVolume_TT".Translate());
            settings.shockwaveLoudVolumeCoef = ListA.SliderLabeled("CRSC_Setting_ShockwaveLoudVolume".Translate(settings.shockwaveLoudVolumeCoef.ToString("F2")), settings.shockwaveLoudVolumeCoef, 0f, 5f, 0.5f, "CRSC_Setting_ShockwaveLoudVolume_TT".Translate());
            //
            ListA.GapLine();
            ListA.Label("CRSC_SettingLabel_Storyteller".Translate());
            ListA.CheckboxLabeled("CRSC_Setting_StorytellerSoundCannonDrop".Translate(), ref settings.storytellerSoundCannonDrop, "CRSC_Setting_StorytellerSoundCannonDrop_TT".Translate());
            ListA.CheckboxLabeled("CRSC_Setting_IncidentDef_SoundCannonDropMessage".Translate(), ref settings.incidentDefSoundCannonDropMessage, "CRSC_Setting_IncidentDef_SoundCannonDropMessage_TT".Translate());
            //    
            ListA.End();
            //
            base.DoSettingsWindowContents(inRect);
        }
    }

    public class Mod_Settings : ModSettings
    {
        // Save to File
        public bool useLoudFireSFX = false;
        public bool shockwaveDestroysLimbs = false;

        public float shockwaveNormalVolumeCoef = 1f;
        public float shockwaveLoudVolumeCoef = 1f;

        public bool storytellerSoundCannonDrop = true;
        public bool incidentDefSoundCannonDropMessage = false;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref useLoudFireSFX, "use_loud_fire_sfx", false);
            Scribe_Values.Look(ref shockwaveDestroysLimbs, "shockwave_destroy_limbs", false);
            //
            Scribe_Values.Look(ref shockwaveNormalVolumeCoef, "shockwave_normal_volume_coef", 1f);
            Scribe_Values.Look(ref shockwaveLoudVolumeCoef, "shockwave_loud_volume_coef", 1f);
            //
            Scribe_Values.Look(ref storytellerSoundCannonDrop, "storyteller_sound_cannon_drop", true);
            Scribe_Values.Look(ref incidentDefSoundCannonDropMessage, "incidentdef_sound_cannon_drop_message", false);
            //
            base.ExposeData();
        }
    }
}
