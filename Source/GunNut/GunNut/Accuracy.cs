using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace GunNut
{
    public static class GN_Verbs
    {
        public static float AdjustedAccuracy(RangeCategory category, Thing equipment)
        {
            StatDef stat = null;
            switch (category)
            {
                case RangeCategory.Touch:
                    stat = StatDefOf.AccuracyTouch;
                    break;

                case RangeCategory.Short:
                    stat = StatDefOf.AccuracyShort;
                    break;

                case RangeCategory.Medium:
                    stat = StatDefOf.AccuracyMedium;
                    break;

                case RangeCategory.Long:
                    stat = StatDefOf.AccuracyLong;
                    break;
            }
            return equipment.GetStatValue(stat, true);
        }

        public enum RangeCategory : byte
        {
            Touch,
            Short,
            Medium,
            Long
        }
    }

    [HarmonyPatch(typeof(Verb), "TryStartCastOn")]
    [HarmonyPatch(new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool), typeof(bool) })]
    public static class AccuracyPatch
    {
        //TO-DO Make prefix?
        [HarmonyPostfix]
        public static void AccuracyPostfix(Verb __instance, LocalTargetInfo castTarg)
        {
            if (__instance.CasterIsPawn && __instance.verbProps.warmupTime > 0f && __instance.EquipmentSource.TryGetComp<GN_ThingComp>() != null)
            {
                float warmupImprove = 1.0f;

                foreach (var slot in __instance.EquipmentSource.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        warmupImprove = warmupImprove - slot.attachment.warmupTimeReduction;
                    }
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