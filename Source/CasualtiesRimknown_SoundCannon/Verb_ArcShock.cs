using RimWorld;
using Verse;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualtiesRimknown_SoundCannon
{
    public class Verb_ArcShock : Verb
    {
        protected override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }

            if (base.EquipmentSource != null)
            {
                base.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
                base.EquipmentSource.GetComp<CompApparelReloadable>()?.UsedOnce();
            }

            IntVec3 position = caster.Position;
            float targetAngle = Mathf.Atan2(-(currentTarget.Cell.z - position.z), currentTarget.Cell.x - position.x) * 57.29578f;
            FloatRange value = new FloatRange(targetAngle - 35f, targetAngle + 35f);
            IntVec3 center = position;
            Map mapHeld = caster.MapHeld;
            float effectiveRange = EffectiveRange;
            DamageDef shock = CRSC_DefOf.CRSC_SonicShock;
            Thing instigator = caster;
            ThingDef weapon = base.EquipmentSource.def;
            FloatRange? affectedAngle = value;

            if (CasualtiesRimknownSoundCannon_Mod.settings.useLoudFireSFX)
            {
                shock.soundExplosion = CRSC_DefOf.CRSC_SoundCannon_BlastLoud;
            }
            else
            {
                shock.soundExplosion = CRSC_DefOf.CRSC_SoundCannon_Blast;
            }

            GenExplosion.DoExplosion(center, mapHeld, effectiveRange, shock, instigator, shock.defaultDamage, shock.defaultArmorPenetration, shock.soundExplosion, weapon, null, currentTarget.Thing, null, 0f, 1, null, null, 255, applyDamageToExplosionCellsNeighbors: false, null, 0f, 1, 0f, damageFalloff: true, null, null, affectedAngle, doVisualEffects: true, shock.expolosionPropagationSpeed, 0f, doSoundEffects: true, null, 2f);
            return true;
        }
    }
}
