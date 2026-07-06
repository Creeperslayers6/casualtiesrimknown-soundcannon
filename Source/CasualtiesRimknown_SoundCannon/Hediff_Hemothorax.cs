using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace CasualtiesRimknown_SoundCannon
{
    public class Hediff_Hemothorax : Hediff
    {
        private bool recordedTale;
        private bool visible;
        float severityIncreaseFactor = 0.05f;
        public override void TickInterval(int delta)
        {
            ageTicks += delta;
            if (def.hediffGivers != null && pawn.IsHashIntervalTick(60, delta))
            {
                for (int i = 0; i < def.hediffGivers.Count; i++)
                {
                    def.hediffGivers[i].OnIntervalPassed(pawn, this);
                }
            }
            if (Visible && !visible)
            {
                visible = true;
                if (def.taleOnVisible != null)
                {
                    TaleRecorder.RecordTale(def.taleOnVisible, pawn, def);
                }
            }
            HediffStage curStage = CurStage;
            if (curStage == null)
            {
                return;
            }
            if (curStage.hediffGivers != null && pawn.IsHashIntervalTick(60, delta))
            {
                for (int j = 0; j < curStage.hediffGivers.Count; j++)
                {
                    curStage.hediffGivers[j].OnIntervalPassed(pawn, this);
                }
            }
            if (curStage.mentalStateGivers != null && pawn.IsHashIntervalTick(60, delta) && !pawn.InMentalState)
            {
                for (int k = 0; k < curStage.mentalStateGivers.Count; k++)
                {
                    MentalStateGiver mentalStateGiver = curStage.mentalStateGivers[k];
                    if (Rand.MTBEventOccurs(mentalStateGiver.mtbDays, 60000f, 60f))
                    {
                        pawn.mindState.mentalStateHandler.TryStartMentalState(mentalStateGiver.mentalState, "MentalStateReason_Hediff".Translate(Label));
                    }
                }
            }
            if (curStage.mentalBreakMtbDays > 0f && pawn.IsHashIntervalTick(60, delta) && !pawn.InMentalState && !pawn.Downed && Rand.MTBEventOccurs(curStage.mentalBreakMtbDays, 60000f, 60f))
            {
                TryDoRandomMentalBreak();
            }
            if (curStage.vomitMtbDays > 0f && pawn.IsHashIntervalTick(600, delta) && Rand.MTBEventOccurs(curStage.vomitMtbDays, 60000f, 600f) && pawn.Spawned && pawn.Awake() && pawn.RaceProps.IsFlesh)
            {
                pawn.jobs.StartJob(JobMaker.MakeJob(CRSC_DefOf.CRSC_VomitBlood), JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
            }
            if (curStage.forgetMemoryThoughtMtbDays > 0f && pawn.needs?.mood != null && pawn.IsHashIntervalTick(400, delta) && Rand.MTBEventOccurs(curStage.forgetMemoryThoughtMtbDays, 60000f, 400f) && pawn.needs.mood.thoughts.memories.Memories.TryRandomElement(out var result))
            {
                pawn.needs.mood.thoughts.memories.RemoveMemory(result);
            }
            if (!recordedTale && curStage.tale != null)
            {
                TaleRecorder.RecordTale(curStage.tale, pawn);
                recordedTale = true;
            }
            if (curStage.destroyPart && Part != null && Part != pawn.RaceProps.body.corePart)
            {
                pawn.health.AddHediff(HediffDefOf.MissingBodyPart, Part);
            }
            if (curStage.deathMtbDays > 0f && pawn.IsHashIntervalTick(200, delta) && Rand.MTBEventOccurs(curStage.deathMtbDays, 60000f, 200f))
            {
                DoMTBDeath();
            }
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
        private void TryDoRandomMentalBreak()
        {
            HediffStage curStage = CurStage;
            if (curStage != null)
            {
                TaggedString taggedString = "MentalStateReason_Hediff".Translate(Label);
                if (!curStage.mentalBreakExplanation.NullOrEmpty())
                {
                    taggedString += "\n\n" + curStage.mentalBreakExplanation.Formatted(pawn.Named("PAWN"));
                }
                MentalBreakDef result;
                if (pawn.NonHumanlikeOrWildMan())
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, taggedString);
                }
                else if (DefDatabase<MentalBreakDef>.AllDefsListForReading.Where((MentalBreakDef x) => x.Worker.BreakCanOccur(pawn) && (curStage.allowedMentalBreakIntensities == null || curStage.allowedMentalBreakIntensities.Contains(x.intensity))).TryRandomElementByWeight((MentalBreakDef x) => x.Worker.CommonalityFor(pawn), out result))
                {
                    result.Worker.TryStart(pawn, taggedString.Resolve(), causedByMood: false);
                }
            }
        }
        private void DoMTBDeath()
        {
            HediffStage curStage = CurStage;
            if (!curStage.mtbDeathDestroysBrain && ModsConfig.BiotechActive)
            {
                Pawn_GeneTracker genes = pawn.genes;
                if (genes != null && genes.HasActiveGene(GeneDefOf.Deathless))
                {
                    return;
                }
            }
            pawn.Kill(null, this);
            if (curStage.mtbDeathDestroysBrain)
            {
                BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
                if (brain != null)
                {
                    Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, brain);
                    pawn.health.AddHediff(hediff, brain);
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
