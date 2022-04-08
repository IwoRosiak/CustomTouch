using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace GunNut.HarmonyPatches.Base_functionality
{
    [HarmonyPatch(typeof(ThingDef), "SpecialDisplayStats")]
    public static class IR_SpecialDisplayStatsPatch
    {
        
        [HarmonyPostfix]
        public static IEnumerable<StatDrawEntry> GN_GetDamageAmount_PostFix(IEnumerable<StatDrawEntry> __result, ThingDef __instance, StatRequest req)
        {
            //Log.Message("hi!");
            var weapon = req.Thing.TryGetComp<GN_AttachmentComp>();
            if (weapon != null)
            {
                foreach (StatDrawEntry statDrawEntry in __result)
                {
                    if (statDrawEntry.LabelCap.Equals("RangedWarmupTime".Translate()))
                    {
                        float warmupImprove = 1.0f;

                        foreach (var attachment in weapon.ActiveAttachments)
                        {
                            warmupImprove -= attachment.warmupMult;
                            
                        }

                        yield return new StatDrawEntry(StatCategoryDefOf.Weapon_Ranged, "RangedWarmupTime".Translate(), (Mathf.Round(warmupImprove * req.Thing.def.Verbs[0].warmupTime)).ToString() + " s", "Final warmup time after applying attachments.", 100, null, null, false);
                        continue;
                    }


                    yield return statDrawEntry;
                }

                foreach (var slot in weapon.ActiveSlots)
                {
                    string attachmentName;

                    StringBuilder attachmentDesc = new StringBuilder();
                    if (slot.attachment != null)
                    {
                        attachmentName = slot.attachment.label;
                        attachmentDesc.Append(slot.attachment.description);



                        attachmentDesc.AppendLine(" ");
                        attachmentDesc.AppendLine(" ");
                        attachmentDesc.AppendLine("Attachment effects:");

                        foreach (string effect in slot.attachment.AttachmentSpecialStatsInString())
                        {
                            attachmentDesc.AppendLine("     " + effect);
                        }
                        
                    }
                    else
                    {
                        attachmentName = "Empty";
                        attachmentDesc.Append("This slot is open for an attachment. It is either holding the default part or is simply empty. "); //Default " + slot.weaponPart.ToString() + " that came with the weapon.";
                    }

                    yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, slot.weaponPart.ToString(), attachmentName.CapitalizeFirst(), attachmentDesc.ToString(), 5391, null, null, false);
                }
             
            }
        }
    
    }
}