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

        protected override void ExplosionDamageThing(Explosion explosion, Thing t, List<Thing> damagedThings, List<Thing> ignoredThings, IntVec3 cell)
        {
            if (t.def.category == ThingCategory.Mote || t.def.category == ThingCategory.Ethereal || damagedThings.Contains(t))
            {
                return;
            }
            damagedThings.Add(t);
            if (ignoredThings != null && ignoredThings.Contains(t))
            {
                return;
            }
            if (def == DamageDefOf.Bomb && t.def == ThingDefOf.Fire && !t.Destroyed)
            {
                t.Destroy();
                return;
            }
            DamageInfo dinfo = new DamageInfo(angle: (!(t.Position == explosion.Position)) ? (t.Position - explosion.Position).AngleFlat : ((float)Rand.RangeInclusive(0, 359)), def: def, amount: explosion.GetDamageAmountAt(cell), armorPenetration: explosion.GetArmorPenetrationAt(cell), instigator: explosion.instigator, hitPart: null, weapon: explosion.weapon, category: DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget: explosion.intendedTarget);
            if (def.explosionAffectOutsidePartsOnly)
            {
                dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
            }
            BattleLogEntry_ExplosionImpact battleLogEntry_ExplosionImpact = null;
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                battleLogEntry_ExplosionImpact = new BattleLogEntry_ExplosionImpact(explosion.instigator, t, explosion.weapon, explosion.projectile, def);
                Find.BattleLog.Add(battleLogEntry_ExplosionImpact);
            }
            DamageResult damageResult = t.TakeDamage(dinfo);
            damageResult.AssociateWithLog(battleLogEntry_ExplosionImpact);
            if (pawn != null && damageResult.wounded && pawn.stances != null)
            {
                pawn.stances.stagger.StaggerFor(6 * 60);
            }
        }
    }
}
