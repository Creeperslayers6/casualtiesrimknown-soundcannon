using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using VEF;
using UnityEngine;

namespace CasualtiesRimknown_SoundCannon
{
    public class JobDriver_VomitBlood : JobDriver_Vomit
    {
        private int ticksLeft;

        public ThingDef filthBlood;
        public EffecterDef effectVomitBlood = CRSC_DefOf.CRSC_VomitBloodEffect;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            filthBlood = BloodDefHandler.AcquireFilthBlood(pawn);
            Color filthBloodColor = filthBlood.graphicData.color;

            //Log.Message(filthBlood.defName + " : " + filthBloodColor);

            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate
            {
                ticksLeft = Rand.Range(300, 900);
                int num = 0;
                IntVec3 intVec;
                do
                {
                    intVec = pawn.Position + GenAdj.AdjacentCellsAndInside[Rand.Range(0, 9)];
                    num++;
                    if (num > 12)
                    {
                        intVec = pawn.Position;
                        break;
                    }
                }
                while (!intVec.InBounds(pawn.Map) || !intVec.Standable(pawn.Map));
                job.targetA = intVec;
                pawn.pather.StopDead();
            };
            toil.tickIntervalAction = delegate (int delta)
            {
                if (pawn.IsHashIntervalTick(150, delta))
                {
                    FilthMaker.TryMakeFilth(job.targetA.Cell, base.Map, filthBlood, pawn.LabelIndefinite());
                    if (pawn.needs != null && pawn.needs.TryGetNeed(out Need_Food need) && need.CurLevelPercentage > 0.1f)
                    {
                        need.CurLevel -= need.MaxLevel * 0.04f;
                    }
                }
                ticksLeft -= delta;
                if (ticksLeft <= 0)
                {
                    ReadyForNextToil();
                    TaleRecorder.RecordTale(CRSC_DefOf.CRSC_VomitedBlood, pawn);
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect(effectVomitBlood, TargetIndex.A, filthBloodColor);
            toil.PlaySustainerOrSound(() => SoundDefOf.Vomit);
            yield return toil;
        }
    }
}
