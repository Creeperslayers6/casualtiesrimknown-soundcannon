using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CasualtiesRimknown_SoundCannon
{
    public class DamageWorker_SonicShockwave : DamageWorker_AddInjury
    {
        public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (def.explosionHeatEnergyPerCell > float.Epsilon)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map, def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
            }
            if (explosion.doVisualEffects)
            {
                FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
                if (explosion.Map == Find.CurrentMap)
                {
                    float magnitude = (explosion.Position.ToVector3Shifted() - Find.Camera.transform.position).magnitude;
                    Find.CameraDriver.shaker.DoShake(4f * explosion.radius * explosion.screenShakeFactor / magnitude);
                }
            }
        } 
    }
}
