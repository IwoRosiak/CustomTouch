using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using System.Text;
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

    /*[HarmonyPatch(typeof(Thing), "DrawGUIOverlay")]
    //[HarmonyPatch(new Type[] { typeof(SectionLayer), typeof(Thing), typeof(float) })]
    public static class GN_PrintPatch
    {
        [HarmonyPostfix]
        public static void GN_PrintPostfix(Thing __instance)
        {
            if (__instance.TryGetComp<GN_ThingComp>() != null)
            {
                var weapon = __instance.TryGetComp<GN_ThingComp>();
                weapon.Draw();
                //Log.Message("Hi!");



                /*foreach (var slot in __instance.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        Log.Message("hi");
                        //slot.attachment.graphic.Print(__instance, __instance, 0f);
                        //slot.attachment.graphic.draw
                        //Printer_Plane.PrintPlane(layer, center, size, mat);
                        //slot.attachment.graphic.Draw(drawPos, Rot4.North, __instance);
                    }
                }*/
    // }
    // }
    // }


    [HarmonyPatch(typeof(Thing), "Print")]
    //[HarmonyPatch(new Type[] { typeof(SectionLayer) })]
    public static class GN_PrintPatch
    {
        [HarmonyPostfix]
        public static void GN_PrintPostfix(Thing __instance, SectionLayer layer)
        {
            if (__instance.TryGetComp<GN_ThingComp>() != null)
            {
                var weapon = __instance.TryGetComp<GN_ThingComp>();

                foreach (var slot in weapon.Slots)
                {
                    if (slot.attachment != null)
                    {
                        slot.attachment.graphic.Print(layer, __instance, __instance.Graphic.DrawRotatedExtraAngleOffset);
                        //Printer_Plane.PrintPlane(layer, center, size, mat);
                        //slot.attachment.graphic.DrawFromDef(thing.DrawPos, Rot4.North, null);
                    }
                }
            }
        }
    }


    [HarmonyPatch(typeof(Verb), "TryStartCastOn")]
    [HarmonyPatch(new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool), typeof(bool) })]
    public static class FullCycleLOLPatch
    {
        [HarmonyPostfix]
        public static void GN_AdjustFullCyclePostfix(Verb __instance, LocalTargetInfo castTarg)
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

    [HarmonyPatch(typeof(ProjectileProperties), "GetDamageAmount")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(StringBuilder) })]
    public static class ExplanationPatch
    {
        private static readonly FieldInfo damageAmountBase = AccessTools.Field(typeof(ProjectileProperties), "damageAmountBase");
        [HarmonyPrefix]
        public static bool GN_GetDamageAmount_PostFix(ref int __result, ProjectileProperties __instance, Thing weapon, StringBuilder explanation)
        {
            //Log.Message("Hi!");
            float weaponDamageMultiplier = (weapon != null) ? weapon.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier, true) : 1f;
            //return __instance.GetDamageAmount(weaponDamageMultiplier, explanation);

            //explanation.Clear();
            int num;
            if ((int)damageAmountBase.GetValue(__instance) != -1)
            {
                num = (int)damageAmountBase.GetValue(__instance);
            }
            else
            {
                if (__instance.damageDef == null)
                {
                    Log.ErrorOnce("Failed to find sane damage amount", 91094882);
                    return true;
                }
                num = __instance.damageDef.defaultDamage;
            }



            if (weapon.TryGetComp<GN_ThingComp>() != null && weapon.TryGetComp<GN_ThingComp>().Slots != null)
            {

                float improvement = 1;
                //string defName = this.parentStat.defName;

                foreach (var slot in weapon.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        //if (defName == "RangedWeapon_DamageMultiplier")
                        //{
                        improvement += slot.attachment.damageIncrease;
                        //}
                    }
                }

                if (explanation != null)
                {
                    explanation.AppendLine("StatsReport_BaseValue".Translate() + ": " + num);



                    explanation.Append("StatsReport_QualityMultiplier".Translate() + ": " + (weaponDamageMultiplier / improvement).ToStringPercent());
                }
                explanation.Append("\nAttachment multiplier" + ": " + improvement.ToStringPercent());
            }
            num = Mathf.RoundToInt((float)num * weaponDamageMultiplier);
            if (explanation != null)
            {
                explanation.AppendLine();
                explanation.AppendLine();
                explanation.Append("StatsReport_FinalValue".Translate() + ": " + num);
            }
            __result = num;
            return false;
        }

        /*[HarmonyPostfix]
        public static void GN_PreFix(ref string __result, ProjectileProperties __instance, float weaponDamageMultiplier, StringBuilder explanation)
        {

        }

                    explanation.Clear();
            if (explanation != null)
            {
                explanation.AppendLine("StatsReport_BaseValue".Translate() + ": " + 1);
                explanation.Append("StatsReport_QualityMultiplier".Translate() + ": " + weaponDamageMultiplier.ToStringPercent());

                explanation.Append("Attachment multiplier " + ": " + __instance.damageDef.);
            }
            num = Mathf.RoundToInt((float)num * weaponDamageMultiplier);
            if (explanation != null)
            {
                explanation.AppendLine();
                explanation.AppendLine();
                explanation.Append("StatsReport_FinalValue".Translate() + ": " + num);
            }
            return num;*/

    }

}

/*[HarmonyPatch(typeof(VerbProperties), "AdjustedFullCycleTime")]
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

*/
/*[HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown")]
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
}*/
/*
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
//}




