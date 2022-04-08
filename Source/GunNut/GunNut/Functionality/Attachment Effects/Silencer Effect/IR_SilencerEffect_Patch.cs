using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut.HarmonyPatches.Stats_Patches.Silencer_Effect
{
    [HarmonyPatch(typeof(PawnUtility), "GetManhunterChanceFactorForInstigator")]
    [HarmonyPatch(new Type[] { typeof(Pawn) })]
    public static class IR_SilencerEffect_Patch
    {
        /*
         * This patch is responsible for modifying the method which returns chance for the pawn to trigger manhunter rage on an animal.
        */
        
        [HarmonyPostfix]
        public static void GetManhunterOnDamageChance(Pawn instigator, ref float __result)
        {
            if (instigator != null)
            {
                GN_AttachmentComp attachmentComp;
                float silencer = 0;

                if ((attachmentComp = instigator.equipment?.Primary?.TryGetComp<GN_AttachmentComp>()) != null) 
                {
                    foreach (GN_AttachmentDef attachment in attachmentComp.ActiveAttachments)
                    {
                        silencer += attachment.silencerEffect;
                    }
                    silencer = Mathf.Clamp(silencer, -0.8f, 0.8f);
                }

                float silencerOffset = __result * silencer;

                __result -= silencerOffset;
            }
        }
    }
}
