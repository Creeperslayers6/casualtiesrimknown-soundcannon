using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualtiesRimknown_SoundCannon
{
    public class IncidentWorker_SoundCannonDrop : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            
            return CasualtiesRimknownSoundCannon_Mod.settings.storytellerSoundCannonDrop;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TryFindSoundCannonDropCell(map.Center, map, 999999, out var pos))
            {
                return false;
            }
            if (Find.FactionManager.FirstFactionOfDef(FactionDefOf.Mechanoid) == null)
            {
                return false;
            }
            if (TryFindSoundCannonDropCell(pos, map, 5, out var pos2))
            {
                Thing newSoundCannon = ThingMaker.MakeThing(CRSC_DefOf.CRSC_SoundCannon_Basic, ThingDefOf.Uranium);
                if (newSoundCannon.def.CanHaveFaction)
                {
                    newSoundCannon.SetFactionDirect(Faction.OfMechanoids);
                }
                SkyfallerMaker.SpawnSkyfaller(CRSC_DefOf.CRSC_SoundCannonIncoming, newSoundCannon, pos, map);
                
                if (CasualtiesRimknownSoundCannon_Mod.settings.incidentDefSoundCannonDropMessage)
                {
                    Messages.Message("CRSC_SoundCannonDrop_IncidDefMessage".Translate(), new TargetInfo(pos, map), MessageTypeDefOf.NeutralEvent);
                }
            }
            return true;
        }

        private bool TryFindSoundCannonDropCell(IntVec3 nearloc, Map map, int maxDist, out IntVec3 pos)
        {
            return CellFinderLoose.TryFindSkyfallerCell(CRSC_DefOf.CRSC_SoundCannonIncoming, map, CRSC_DefOf.CRSC_SoundCannon_Basic.terrainAffordanceNeeded, out pos, 10, nearloc, maxDist);
        }
    }
}