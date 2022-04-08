using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut.HarmonyPatches.Stats_Patches.Silencer_Effect
{
    //ShotCalculationTipString
    [HarmonyPatch(typeof(TooltipUtility), "ShotCalculationTipString")]
    public static class IR_SilencerEffect_Readout_Patch
    {
        /*
         * This patch is responsible for modifying the method which returns string for tooltip which displays information about hit chances
        */

        [HarmonyPostfix]
        public static void GetManhunterOnDamageChance(Thing target, ref string __result)
        {
            if (!__result.Contains("ManhunterPerHit".Translate())) return;

            Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
            Pawn pawn = singleSelectedThing as Pawn;

            GN_AttachmentComp attachmentComp;
            float silencer = 0;

            if ((attachmentComp = pawn.equipment?.Primary?.TryGetComp<GN_AttachmentComp>()) != null)
            {
                foreach (GN_AttachmentDef attachment in attachmentComp.ActiveAttachments)
                {
                    silencer += attachment.silencerEffect;
                }
                silencer = Mathf.Clamp(silencer, -0.8f, 0.8f);
            } else return;


            if (silencer == 0) return;

            int index = __result.IndexOf("ManhunterPerHit".Translate());
            index += "ManhunterPerHit".Translate().Length +2;


            string value = __result.Substring(index, 3);


            if (value.Last() == '%')
            {
                value = __result.Substring(index, 2);
            }

            if (value.Last() == '%')
            {
                value = __result.Substring(index, 1);
            }

            float parsedValue = float.Parse(value);

            silencer = 1-silencer;

            float attachmentOffset = (parsedValue / silencer) - parsedValue;
            attachmentOffset = (int)Math.Ceiling(attachmentOffset);

            string replaceWith = string.Format("{0}: {1}", "ManhunterPerHit".Translate(), value + "%") + "\n  Silenced -" + attachmentOffset.ToString() + "%";

            __result = __result.ReplaceFirst(string.Format("{0}: {1}", "ManhunterPerHit".Translate(), value + "%"), replaceWith);
            
        }
    }
}
