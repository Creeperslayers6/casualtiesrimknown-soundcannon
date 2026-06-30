using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CasualtiesRimknown_SoundCannon
{
    public class Hediff_Hemothorax : Hediff
    {
        float severityIncreaseFactor = 0.05f;
        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (pawn.IsHashIntervalTick(60, delta))
            {
                BodyPartRecord pawnTorso = pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Torso);
                Hediff torsoInternalBleeding = GetFirstHediffFromPart(CRSC_DefOf.CRSC_InternalBleeding, pawnTorso);
                if (torsoInternalBleeding != null)
                {
                    Severity += (torsoInternalBleeding.Severity/100) * severityIncreaseFactor;
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
