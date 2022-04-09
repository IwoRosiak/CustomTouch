using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace GunNut
{
    [HarmonyPatch(typeof(Verb), "TryStartCastOn")]
    [HarmonyPatch(new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool), typeof(bool) })]
    public static class IR_TryStartCastOnPatch
    {
        //TO-DO Make prefix?
        [HarmonyPostfix]
        public static void AccuracyPostfix(Verb __instance, LocalTargetInfo castTarg)
        {
            if (__instance.CasterIsPawn && __instance.verbProps.warmupTime > 0f && __instance.EquipmentSource.TryGetComp<GN_AttachmentComp>() != null)
            {
                float warmupImprove = 1.0f;
                var weapon = __instance.EquipmentSource.TryGetComp<GN_AttachmentComp>();
                foreach (var attachment in weapon.ActiveAttachments)
                {
                    warmupImprove += attachment.warmupMult;
                }

                ShootLine newShootLine;
                if (!__instance.TryFindShootLineFromTo(__instance.caster.Position, castTarg, out newShootLine))
                {
                    return;
                }
                __instance.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, __instance.caster.Position);
                float statValue = __instance.caster.GetStatValue(StatDefOf.AimingDelayFactor, true);
                int ticks = (__instance.verbProps.warmupTime * warmupImprove * statValue).SecondsToTicks();
                __instance.CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, __instance));
            }
        }
    }
}