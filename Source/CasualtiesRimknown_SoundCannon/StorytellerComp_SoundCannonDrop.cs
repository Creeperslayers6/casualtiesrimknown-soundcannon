using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualtiesRimknown_SoundCannon
{
    public class StorytellerComp_SoundCannonDrop : StorytellerComp
    {
        public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
        {
            if (Rand.MTBEventOccurs())
            {
                IncidentDef soundCannonDrop = CRSC_DefOf.CRSC_SoundCannonDrop;
                IncidentParms parms = GenerateParms(soundCannonDrop.category, target);
                if (soundCannonDrop.Worker.CanFireNow())
                {
                    yield return new FiringIncident(soundCannonDrop, this, parms);
                }
            }
        }
    }
}
