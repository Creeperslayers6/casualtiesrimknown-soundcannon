using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CasualtiesRimknown_SoundCannon
{
    public class Hediff_InternalBleeding : Hediff_Injury
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            BodyPartRecord pawnTorso = pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Torso);
            if (pawnTorso != null && base.Part == pawnTorso && !pawn.health.hediffSet.HasHediff(CRSC_DefOf.CRSC_Hemothorax))
            {
                pawn.health.AddHediff(CRSC_DefOf.CRSC_Hemothorax, pawnTorso, dinfo);
            }
        }
    }
}
