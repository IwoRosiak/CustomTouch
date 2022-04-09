using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunNut.Functionality.Attachment_Effects.Fire_Rate_Effect
{
    /*
    [HarmonyPatch(typeof(Verb_Shoot), "get_ShotsPerBurst")]
    //[HarmonyPatch(new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool), typeof(bool) })]
    public static class IR_FireRate_Patch
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
    */
}
