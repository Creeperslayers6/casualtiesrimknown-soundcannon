using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VEF;
using Verse;

namespace CasualtiesRimknown_SoundCannon
{
    [StaticConstructorOnStartup]
    public static class BloodDefHandler
    {
        private static List<GeneDef> customBloodGenes = new List<GeneDef>();

        static BloodDefHandler()
        {
            if (CasualtiesRimknownSoundCannon_Mod.IsVanillaExpandedActive)
            {
                InitalizeCustomDefs();
            }
        }
        public static void InitalizeCustomDefs()
        {
            //Log.Message("Begin Adding Custom Defs!");

            foreach (var def in DefDatabase<GeneDef>.AllDefsListForReading.Where(testGene => testGene.HasModExtension<VEF.Genes.GeneExtension>() && testGene.GetModExtension<VEF.Genes.GeneExtension>().customBloodThingDef != null))
            {
                //Log.Message(def.defName);
                customBloodGenes.Add(def);
            }

            //Log.Message("Completed Addition! " + newDefsList.Count());
        }

        public static bool PawnHasActiveGenes(Pawn pawn, out GeneDef activeGene)
        {
            foreach (GeneDef bloodGene in customBloodGenes)
            {
                if (pawn.genes.HasActiveGene(bloodGene))
                {
                    activeGene = bloodGene;
                    return true;
                }
            }
            activeGene = null;
            return false;
        }

        public static ThingDef AcquireFilthBlood(Pawn targetPawn)
        {
            if (CasualtiesRimknownSoundCannon_Mod.IsVanillaExpandedActive && customBloodGenes.Count >= 1 && PawnHasActiveGenes(targetPawn, out GeneDef activeGene))
            {
                //Log.Message("Found Modded Blood Gene, Returning " + activeGene.defName + " : " + activeGene.GetModExtension<VEF.Genes.GeneExtension>().customBloodThingDef);
                return GetCustomBloodDef(activeGene);
            }
            //Log.Message("Didn't Find Modded Gene, Returning Filth_Blood");
            return ThingDefOf.Filth_Blood;
        }

        private static ThingDef GetCustomBloodDef(GeneDef targetGene)
        {
            return targetGene.GetModExtension<VEF.Genes.GeneExtension>().customBloodThingDef;
        }
    }  
}
