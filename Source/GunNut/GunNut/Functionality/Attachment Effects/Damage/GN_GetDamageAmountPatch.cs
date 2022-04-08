using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace GunNut.HarmonyPatches
{
    [HarmonyPatch(typeof(ProjectileProperties), "GetDamageAmount")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(StringBuilder) })]
    public static class GN_GetDamageAmountPatch
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

            if (weapon.TryGetComp<GN_AttachmentComp>() != null && weapon.TryGetComp<GN_AttachmentComp>().ActiveSlots != null)
            {
                float improvement = 1;

                foreach (var attachment in weapon.TryGetComp<GN_AttachmentComp>().ActiveAttachments)
                {
                    if (attachment != null)
                    {
                        improvement += attachment.damageMult;
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