using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace GunNut.HarmonyPatches.Base_functionality
{
    [HarmonyPatch(typeof(ThingDef), "SpecialDisplayStats")]
    public static class IR_SpecialDisplayStatsPatch
    {
        private static readonly FieldInfo damageAmountBase = AccessTools.Field(typeof(ThingDef), "verbs");

        [HarmonyPostfix]
        public static IEnumerable<StatDrawEntry> GN_GetDamageAmount_PostFix(IEnumerable<StatDrawEntry> __result, ThingDef __instance, StatRequest req)
        {
            var weapon = req.Thing.TryGetComp<GN_AttachmentComp>();
            if (weapon != null)
            {
                foreach (StatDrawEntry statDrawEntry in ProcessParentStats(weapon, __result, __instance, req))
                {
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

        private static IEnumerable<StatDrawEntry> ProcessParentStats(GN_AttachmentComp weapon, IEnumerable<StatDrawEntry> __result, ThingDef __instance, StatRequest req)
        {
            bool hasBurstShots = false;
            foreach (StatDrawEntry statDrawEntry in __result)
            {
                if (statDrawEntry.LabelCap.Equals("RangedWarmupTime".Translate()))
                {
                    float warmupImprove = 1.0f;

                    foreach (var attachment in weapon.ActiveAttachments)
                    {
                        warmupImprove -= attachment.warmupMult;

                    }

                    float orgValue = req.Thing.def.Verbs[0].warmupTime;



                    string explanation = GetBurstWarmupExplanation(orgValue, orgValue*warmupImprove);

                    yield return new StatDrawEntry(StatCategoryDefOf.Weapon_Ranged, "RangedWarmupTime".Translate(), (warmupImprove * req.Thing.def.Verbs[0].warmupTime).ToString() + " s", "Stat_Thing_Weapon_RangedWarmupTime_Desc".Translate() +explanation, 100, null, null, false);
                    continue;
                }

                List<VerbProperties> verbProperties = (List<VerbProperties>)damageAmountBase.GetValue(__instance);
                VerbProperties verb = verbProperties.First((VerbProperties x) => x.isPrimary);

                int burstCount = GetBurstCount(weapon, verb.burstShotCount);
                int ticksCount = verb.ticksBetweenBurstShots;

                if (burstCount > 1)
                {
                    if (statDrawEntry.LabelCap.Equals("BurstShotCount".Translate()))
                    {
                        yield return GetBurstCountStat(burstCount, statDrawEntry.category, GetBurstCountExplanation(verb.burstShotCount, burstCount));
                        hasBurstShots = true;
                        continue;
                    }

                    if (statDrawEntry.LabelCap.Equals("BurstShotFireRate".Translate()))
                    {
                        yield return GetBurstFireRateStat(ticksCount, statDrawEntry.category);
                        hasBurstShots = true;
                        continue;
                    }

                    if (!hasBurstShots && statDrawEntry.LabelCap.Equals("Range".Translate()))
                    {
                        yield return GetBurstCountStat(burstCount, statDrawEntry.category, GetBurstCountExplanation(verb.burstShotCount, burstCount));
                        yield return GetBurstFireRateStat(ticksCount, statDrawEntry.category);

                        yield return statDrawEntry;

                        continue;
                    }
                }

                yield return statDrawEntry;
            }
        }

        public static StatDrawEntry GetBurstCountStat(int burstCount, StatCategoryDef statCat, string explanation)
        {
            return new StatDrawEntry(statCat, "BurstShotCount".Translate(), burstCount.ToString(), "Stat_Thing_Weapon_BurstShotCount_Desc".Translate() + explanation, 5391, null, null, false);
        }
        
        public static StatDrawEntry GetBurstFireRateStat(int ticks, StatCategoryDef statCat)
        {
            float dmgBuildingsPassable = 60f / ticks.TicksToSeconds();

            return new StatDrawEntry(statCat, "BurstShotFireRate".Translate(), dmgBuildingsPassable.ToString("0.##") + " rpm", "Stat_Thing_Weapon_BurstShotFireRate_Desc".Translate(), 5392, null, null, false);
        }

        public static int GetBurstCount(GN_AttachmentComp comp, int initCount)
        {
            int shotsPerBurst = initCount;
            if (comp != null)
            {
                foreach (var attachment in comp.ActiveAttachments)
                {
                    shotsPerBurst += attachment.burstShotsOffset;
                }
            }

            return shotsPerBurst;
        }

        public static string GetBurstCountExplanation(int orgValue, int newValue)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Base Value: " + orgValue);
            //sb.AppendLine();
            sb.AppendLine("Attachments Offset: " + (newValue- orgValue));
            sb.AppendLine();
            sb.AppendLine("Final Value: " + newValue);

            return sb.ToString();
        }

        public static string GetBurstWarmupExplanation(float orgValue, float newValue)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Base Value: " + orgValue +"s");
            //sb.AppendLine();
            sb.AppendLine("Attachments Offset: " + (newValue - orgValue)+"s");
            sb.AppendLine();
            sb.AppendLine("Final Value: " + newValue);

            return sb.ToString();
        }
    }
}