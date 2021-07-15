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

    [HarmonyPatch(typeof(Graphic), "Print")]
    public static class GN_PrintPatchGraphic
    {
        [HarmonyPostfix]
        public static void GN_PrintPostfix(Graphic __instance, Thing thing, SectionLayer layer, float extraRotation)
        {
            if (__instance.data.texPath == thing.def.graphicData.texPath)
            {

                if (thing.TryGetComp<GN_ThingComp>() != null)
                {
                    Log.Message("I'm in");
                    var weapon = thing.TryGetComp<GN_ThingComp>();

                    foreach (var slot in weapon.Slots)
                    {
                        if (slot.attachment != null)
                        {
                            Log.Message("Displaying attachment");
                            slot.attachment.graphic.Print(layer, thing, extraRotation);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming", new Type[] { typeof(Thing), typeof(Vector3), typeof(float) })]
    public static class PatchPawnRenderer
    {
        [HarmonyPostfix]
        private static void PawnRendererPatch(Thing eq, Vector3 drawLoc, float aimAngle, PawnRenderer __instance)
        {
            if (eq.TryGetComp<GN_ThingComp>() != null)
            {
                var weapon = eq.TryGetComp<GN_ThingComp>();
                foreach (var slot in weapon.Slots)
                {
                    if (slot.attachment != null)
                    {
                        float num = aimAngle - 90f;
                        Mesh mesh;
                        if (aimAngle > 20f && aimAngle < 160f)
                        {
                            mesh = MeshPool.plane10;
                            num += eq.def.equippedAngleOffset;
                        }
                        else if (aimAngle > 200f && aimAngle < 340f)
                        {
                            mesh = MeshPool.plane10Flip;
                            num -= 180f;
                            num -= eq.def.equippedAngleOffset;
                        }
                        else
                        {
                            mesh = MeshPool.plane10;
                            num += eq.def.equippedAngleOffset;
                        }
                        num %= 360f;
                        CompEquippable compEquippable = eq.TryGetComp<CompEquippable>();
                        if (compEquippable != null)
                        {
                            Vector3 b;
                            float num2;
                            EquipmentUtility.Recoil(eq.def, EquipmentUtility.GetRecoilVerb(compEquippable.AllVerbs), out b, out num2, aimAngle);
                            drawLoc += b;
                            num += num2;
                        }
                        Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
                        Material matSingle;
                        if (graphic_StackCount != null)
                        {
                            matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                        }
                        else
                        {
                            matSingle = eq.Graphic.MatSingle;
                        }
                        drawLoc.y += 1;
                        Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), slot.attachment.graphic.MatSingle, 0);
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

    [HarmonyPatch(typeof(ProjectileProperties), "GetDamageAmount")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(StringBuilder) })]
    public static class ExplanationPatch
    {
        private static readonly FieldInfo damageAmountBase = AccessTools.Field(typeof(ProjectileProperties), "damageAmountBase");
        [HarmonyPrefix]
        public static bool GN_GetDamageAmount_PostFix(ref int __result, ProjectileProperties __instance, Thing weapon, StringBuilder explanation)
        {
            float weaponDamageMultiplier = (weapon != null) ? weapon.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier, true) : 1f;
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
    }
}