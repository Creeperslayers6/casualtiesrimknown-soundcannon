using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace CasualtiesRimknown_SoundCannon
{
    public class Hediff_InternalBleeding : Hediff_Injury
    {
        private float hemothoraxSeverityIncreaseValue = 0.1f;
        private Hediff hediffHemothorax = null;
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            base.destroysBodyParts = CasualtiesRimknownSoundCannon_Mod.settings.shockwaveDestroysLimbs;
        }

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (pawn.IsHashIntervalTick(60, delta) && !this.IsTended())
            {
                if (Part == pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Torso))
                {
                    if (hediffHemothorax == null)
                    {
                        hediffHemothorax = GetFirstHediffFromPart(CRSC_DefOf.CRSC_Hemothorax, Part);
                    }
                    if (hediffHemothorax == null)
                    {
                        hediffHemothorax = HediffMaker.MakeHediff(CRSC_DefOf.CRSC_Hemothorax, pawn);
                        pawn.health.AddHediff(hediffHemothorax);
                    }
                    else
                    {
                        hediffHemothorax.Severity += (this.BleedRate / 100) * hemothoraxSeverityIncreaseValue * pawn.RaceProps.bleedRateFactor;
                    }
                }
            }
        }

        Hediff GetFirstHediffFromPart(HediffDef hediffDef, BodyPartRecord bodyPart, bool mustBeVisible = false)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i].def == hediffDef && hediffs[i].Part == bodyPart && (!mustBeVisible || hediffs[i].Visible))
                {
                    return hediffs[i];
                }
            }
            return null;
        }
    }
}
