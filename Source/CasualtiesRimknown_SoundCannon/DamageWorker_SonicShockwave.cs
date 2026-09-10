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
        private const float ExplosionCamShakeMultiplier = 4f;
        float stunDuration = 6f;

        // Intercept DamageWorker_AddInjury.Apply() to apply Stun effect on Pawns
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            DamageResult damageResult = base.Apply(dinfo, thing);

            if (thing is Pawn pawn)
            {
                stunPawn(pawn, dinfo);
            }

            return damageResult;
        }

        private void stunPawn(Pawn pawn, DamageInfo dinfo)
        {
            // Reduce Stun on Mechanoids
            int stunDurationDenominator = !pawn.RaceProps.IsMechanoid ? 1 : 2;
            pawn?.stances?.stunner?.StunFor(stunDuration.SecondsToTicks() / stunDurationDenominator, dinfo.Instigator, addBattleLog: false, showMote: true);
        }

        protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
        {
            return GetSpecificRandomNotMissingPart(pawn, dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
        }

        // Return BodyPartRecords that can bleed.
        private BodyPartRecord GetSpecificRandomNotMissingPart(Pawn pawn, DamageDef damDef, BodyPartHeight height = BodyPartHeight.Undefined, BodyPartDepth depth = BodyPartDepth.Undefined, BodyPartRecord partParent = null)
        {
            IEnumerable<BodyPartRecord> enumerable = null;
            if (pawn.health.hediffSet.GetNotMissingParts(height, depth, null, partParent).Any((BodyPartRecord p) => p.coverageAbs > 0f && p.def.bleedRate > 0))
            {
                enumerable = pawn.health.hediffSet.GetNotMissingParts(height, depth, null, partParent).Where((BodyPartRecord p) => p.def.bleedRate > 0);
            }
            else
            {
                if (!pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, depth, null, partParent).Any((BodyPartRecord p) => p.coverageAbs > 0f && p.def.bleedRate > 0))
                {
                    return null;
                }
                enumerable = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, depth, null, partParent).Where((BodyPartRecord p) => p.def.bleedRate > 0);
            }
            if (enumerable.TryRandomElementByWeight((BodyPartRecord x) => x.coverageAbs * x.def.GetHitChanceFactorFor(damDef), out var result))
            {
                return result;
            }
            if (enumerable.TryRandomElementByWeight((BodyPartRecord x) => x.coverageAbs, out result))
            {
                return result;
            }
            return null;
        }

        // Removed ExplosionVisualEffectCenter(explosion); from ExplosionStart
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
                    Find.CameraDriver.shaker.DoShake(ExplosionCamShakeMultiplier * explosion.radius * explosion.screenShakeFactor / magnitude);
                }
            }
        }
    }
}
