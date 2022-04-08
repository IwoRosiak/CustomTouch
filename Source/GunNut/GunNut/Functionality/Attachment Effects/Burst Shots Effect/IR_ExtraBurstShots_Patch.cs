using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GunNut.Functionality.Attachment_Effects.Burst_Shots_Effect
{
    [HarmonyPatch(typeof(VerbProperties), "AdjustedFullCycleTime")]
    public static class IR_ExtraBurstShots_Patch
    {

        [HarmonyPrefix]
        public static bool GN_GetDamageAmount_PostFix(VerbProperties __instance, Verb ownerVerb, Pawn attacker, ref float __result, ref float ___burstShotCount)
        {
            //___burstShotCount = 1;
            //__result =__instance.warmupTime + __instance.AdjustedCooldown(ownerVerb, attacker) + ((__instance.burstShotCount - 1) * __instance.ticksBetweenBurstShots).TicksToSeconds();

            return true;
        }
    }

    [HarmonyPatch(typeof(Verb), "WarmupComplete")]
    //[HarmonyPatch(new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool), typeof(bool) })]
    public static class IR_WarmupCompletePatch
    {
        //TO-DO Make prefix?
        [HarmonyPostfix]
        public static void AccuracyPostfix(Verb __instance, ref float ___burstShotsLeft)
        {
            //___burstShotsLeft = 5;
            
        }
    }

    [HarmonyPatch(typeof(Verb_Shoot), "get_ShotsPerBurst")]
    //[HarmonyPatch(new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool), typeof(bool) })]
    public static class IR_WarmupCompletePatch1
    {
        [HarmonyPostfix]
        public static void AccuracyPostfix(Verb __instance, ref int __result)
        {
            int shotsPerBurst = __result;

            GN_AttachmentComp comp = null;

            if (__instance.CasterIsPawn)
            {
                comp = ((Pawn)__instance.Caster).equipment.Primary.TryGetComp<GN_AttachmentComp>();
            }
            
            float extraShots;

            if (comp != null)
            {
                foreach (var attachment in comp.ActiveAttachments)
                {
                    shotsPerBurst += attachment.burstShotsOffset;
                }
            }

            __result = shotsPerBurst;
        }
    }
}
