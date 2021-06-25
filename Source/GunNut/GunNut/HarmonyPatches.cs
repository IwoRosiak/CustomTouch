using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("com.company.QarsoonMeel.GunNut");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }





    /*[HarmonyPatch(typeof(Verb), "get_WarmupProgress")]
    public static class Patchie
    {
        [HarmonyPostfix]
        public static void CAdjustFullCyclePostfix(Verb __instance, ref float __result)
        {
            if (__instance.EquipmentSource.TryGetComp<GN_ThingComp>() != null)
            {
                float warmupImprove = 1.0f;

                foreach (var slot in __instance.EquipmentSource.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        warmupImprove = warmupImprove - slot.attachment.warmupTimeReduction;

                    }
                }
                Log.Message(__result.ToString() + " " + warmupImprove);

                __result = 1f - __instance.WarmupTicksLeft.TicksToSeconds() / __instance.verbProps.warmupTime * warmupImprove;

                Log.Message(__result.ToString());
            }



        }
    } */



    [HarmonyPatch(typeof(Verb), "TryStartCastOn")]
    [HarmonyPatch(new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool) })]
    public static class FullCycleLOLPatch
    {
        [HarmonyPostfix]
        public static void CAdjustFullCyclePostfix(Verb __instance, LocalTargetInfo castTarg)
        {
            /*if (!__return)
            {
                return;
            }*/

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
                    return;//return false;
                }
                __instance.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, __instance.caster.Position);
                float statValue = __instance.caster.GetStatValue(StatDefOf.AimingDelayFactor, true);
                int ticks = (__instance.verbProps.warmupTime * warmupImprove * statValue).SecondsToTicks();
                __instance.CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, __instance));
            }

        }

    }



    [HarmonyPatch(typeof(VerbProperties), "AdjustedFullCycleTime")]
    public static class FullCyclePatch
    {
        [HarmonyPostfix]
        public static void CAdjustFullCyclePostfix(VerbProperties __instance, Verb ownerVerb, Pawn attacker, ref float __result)
        {
            float warmupImprove = 1.0f;
            if (ownerVerb.EquipmentSource.TryGetComp<GN_ThingComp>() != null)
            {
                foreach (var slot in ownerVerb.EquipmentSource.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        warmupImprove = warmupImprove - slot.attachment.warmupTimeReduction;

                    }
                }

                Log.Message(__result.ToString() + " " + warmupImprove);

                __result = __instance.warmupTime * warmupImprove + __instance.AdjustedCooldown(ownerVerb, attacker) + ((__instance.burstShotCount - 1) * __instance.ticksBetweenBurstShots).TicksToSeconds();

                Log.Message(__result.ToString());
            }
        }
    }


    [HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown")]
    [HarmonyPatch(new Type[] { typeof(Tool), typeof(Pawn), typeof(Thing) })]
    public static class CooldownPatch
    {
        [HarmonyPostfix]
        public static void CooldownPostfix(VerbProperties __instance, Thing equipment, ref float __result)
        {

            if (equipment != null && !__instance.IsMeleeAttack && equipment.TryGetComp<GN_ThingComp>() != null)
            {
                float cooldownImprove = 1.0f;

                foreach (var slot in equipment.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        cooldownImprove = cooldownImprove - slot.attachment.cooldownTimeReduction;

                    }
                }
                __result = __result * cooldownImprove;
                Log.Message(__result.ToString() + cooldownImprove);

            }
        }
    }

    [HarmonyPatch(typeof(VerbProperties), "GetHitChanceFactor")]
    public static class AccuracyPatch
    {
        [HarmonyPostfix]
        public static void AccuracyPostfix(VerbProperties __instance, Thing equipment, float dist, ref float __result)
        {
            if (equipment.TryGetComp<GN_ThingComp>() != null)
            {
                float improveLong = 1.0f;
                float improveMedium = 1.0f;
                float improveShort = 1.0f;
                float improveTouch = 1.0f;
                foreach (var slot in equipment.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        improveTouch += slot.attachment.accuracyImproveTouch;
                        improveShort += slot.attachment.accuracyImproveShort;
                        improveMedium += slot.attachment.accuracyImproveMedium;
                        improveLong += slot.attachment.accuracyImproveLong;
                    }
                    //improveLong = slot.attachment.accuracyImproveLong;

                }

                float value;
                if (dist <= 3f)
                {
                    value = GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Touch, equipment) * improveTouch;
                }
                else if (dist <= 12f)
                {
                    value = Mathf.Lerp(GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Touch, equipment) * improveTouch, GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Short, equipment) * improveShort, (dist - 3f) / 9f);
                }
                else if (dist <= 25f)
                {
                    value = Mathf.Lerp(GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Short, equipment) * improveShort, GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Medium, equipment) * improveMedium, (dist - 12f) / 13f);
                }
                else if (dist <= 40f)
                {
                    value = Mathf.Lerp(GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Medium, equipment) * improveMedium, GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Long, equipment) * improveLong, (dist - 25f) / 15f);
                }
                else
                {
                    value = GN_Verbs.AdjustedAccuracy(GN_Verbs.RangeCategory.Long, equipment) * improveLong;
                }
                __result = Mathf.Clamp(value, 0.01f, 1f);
            }
        }
        /*
        Type theType = Type.GetType("RangeCategory");

        foreach (var slot in equipment.TryGetComp<GN_ThingComp>().Slots)
        {
            ref RimWorld.StatDef ___stat;
            equipment.def.Verbs[0].AdjustedAccuracy
            if (slot.attachment.)
}*/


        /*if (__instance.nam)

            if (__instance.pa.def.IsWeapon)
            {
                var weapon = (GN_ThingDef)__instance.parent.def;

                if (weapon.attachments != null)
                {
                    __instance.parent.def.Verbs[0].GetHitChanceFactor
                }
            }
        */
    }
}



