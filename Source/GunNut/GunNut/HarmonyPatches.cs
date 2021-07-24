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
    internal static class HarmonyPatches
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
                    var weapon = thing.TryGetComp<GN_ThingComp>();

                    foreach (var slot in weapon.Slots)
                    {
                        if (slot.attachment != null)
                        {
                            slot.attachment.onWeaponGraphic.Graphic.Print(layer, thing, extraRotation);
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
                        Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), slot.attachment.onWeaponGraphic.Graphic.MatSingle, 0);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Widgets), "ThingIcon")]
    [HarmonyPatch(new Type[] { typeof(Rect), typeof(Thing), typeof(float), typeof(Rot4) })]
    public static class ThingIconPatch
    {
        [HarmonyPostfix]
        public static void GN_AdjustFullCyclePostfix(Thing thing, Rect rect)
        {
            if (thing.TryGetComp<GN_ThingComp>() != null)
            {
                Log.Message("ThingIcon Patch");

                var weapon = thing.TryGetComp<GN_ThingComp>();

                foreach (var slot in weapon.Slots)
                {
                    if (slot.attachment != null)
                    {
                        Log.Message("Displaying attachment");

                        Texture texture = slot.attachment.onWeaponGraphic.Graphic.MatSingle.mainTexture;

                        Vector2 texProportions = new Vector2((float)texture.width, (float)slot.attachment.uiIcon.height);
                        Rect texCoords = new Rect(0f, 0f, 1f, 1f);
                        if (slot.attachment.onWeaponGraphic != null)
                        {
                            texProportions = slot.attachment.onWeaponGraphic.drawSize.RotatedBy(slot.attachment.defaultPlacingRot);
                            if (slot.attachment.uiIconPath.NullOrEmpty() && slot.attachment.onWeaponGraphic.linkFlags != LinkFlags.None)
                            {
                                texCoords = new Rect(0f, 0.5f, 0.25f, 0.25f);
                            }
                        }
                        Widgets.DrawTextureFitted(rect, texture, GenUI.IconDrawScale(slot.attachment) * 1, texProportions, texCoords, slot.attachment.uiIconAngle, null);
                    }
                }
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

    [HarmonyPatch(typeof(VerbTracker), "CreateVerbTargetCommand")]
    public static class TestPatch1
    {
        [HarmonyPrefix]
        public static bool GN_GetDamageAmount_PostFix(VerbTracker __instance, Thing ownerThing, Verb verb, ref Command_VerbTarget __result)
        {
            GN_Command_VerbTarget command_VerbTarget = new GN_Command_VerbTarget();
            ThingStyleDef styleDef = ownerThing.StyleDef;
            command_VerbTarget.defaultDesc = ownerThing.LabelCap + ": " + ownerThing.def.description.CapitalizeFirst();
            command_VerbTarget.icon = ((styleDef != null && styleDef.UIIcon != null) ? styleDef.UIIcon : ownerThing.def.uiIcon);
            command_VerbTarget.iconAngle = ownerThing.def.uiIconAngle;
            command_VerbTarget.iconOffset = ownerThing.def.uiIconOffset;
            command_VerbTarget.tutorTag = "VerbTarget";
            command_VerbTarget.verb = verb;
            if (verb.caster.Faction != Faction.OfPlayer)
            {
                command_VerbTarget.Disable("CannotOrderNonControlled".Translate());
            }
            else if (verb.CasterIsPawn)
            {
                string reason;
                if (verb.CasterPawn.WorkTagIsDisabled(WorkTags.Violent))
                {
                    command_VerbTarget.Disable("IsIncapableOfViolence".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
                else if (!verb.CasterPawn.drafter.Drafted)
                {
                    command_VerbTarget.Disable("IsNotDrafted".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
                else if (verb is Verb_LaunchProjectile)
                {
                    Apparel apparel = verb.FirstApparelPreventingShooting();
                    if (apparel != null)
                    {
                        command_VerbTarget.Disable("ApparelPreventsShooting".Translate(verb.CasterPawn.Named("PAWN"), apparel.Named("APPAREL")).CapitalizeFirst());
                    }
                }
                else if (EquipmentUtility.RolePreventsFromUsing(verb.CasterPawn, verb.EquipmentSource, out reason))
                {
                    command_VerbTarget.Disable(reason);
                }
            }
            __result = command_VerbTarget;
            return false;
        }
    }
}