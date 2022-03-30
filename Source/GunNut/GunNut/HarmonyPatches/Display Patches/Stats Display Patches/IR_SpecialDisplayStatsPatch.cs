using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
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

                        foreach (var attachment in weapon.AttachmentsOnWeapon)
                        {
                            warmupImprove -= attachment.warmupTimeReduction;
                            
                        }

                        yield return new StatDrawEntry(StatCategoryDefOf.Weapon_Ranged, "RangedWarmupTime".Translate(), (warmupImprove * req.Thing.def.Verbs[0].warmupTime).ToString() + " s", "Final warmup time after applying attachments.", 100, null, null, false);
                        continue;
                    }


                    yield return statDrawEntry;
                }

                foreach (var slot in weapon.SlotsOnWeapon)
                {
                    string attachmentName;
                    string attachmentDesc;
                    if (slot.attachment != null)
                    {
                        attachmentName = slot.attachment.label;
                        attachmentDesc = slot.attachment.description;
                    }
                    else
                    {
                        attachmentName = "Default";
                        attachmentDesc = "Default " + slot.weaponPart.ToString() + " that came with the weapon.";
                    }

                    yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, slot.weaponPart.ToString(), attachmentName.CapitalizeFirst(), attachmentDesc, 5391, null, null, false);
                }

               

               
            }
        }
    
    }
}

/*
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 * private static readonly FieldInfo cachedDrawEntriesRef = AccessTools.Field(typeof(StatsReportUtility), "cachedDrawEntries");

        private static readonly MethodInfo FinalizeCachedDrawEntries = AccessTools.Method(typeof(StatsReportUtility), "FinalizeCachedDrawEntries");

        [HarmonyPrefix]
        public static bool GN_GetDamageAmount_PostFix(StatsReportUtility __instance, RectTrigger rect, Def def, ThingDef stuff)
        {
            List<StatDrawEntry> cachedDrawEntries = (List<StatDrawEntry>)cachedDrawEntriesRef.GetValue(__instance);

            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                BuildableDef buildableDef = def as BuildableDef;
                StatRequest req = (buildableDef != null) ? StatRequest.For(buildableDef, stuff, QualityCategory.Normal) : StatRequest.ForEmpty();
                cachedDrawEntries.AddRange(def.SpecialDisplayStats(req));
                cachedDrawEntries.AddRange(from r in StatsReportUtility.StatsToDraw(def, stuff) where r.ShouldDisplay select r);

               FinalizeCachedDrawEntries.Invoke(__instance, cachedDrawEntries);
                StatsReportUtility.FinalizeCachedDrawEntries(StatsReportUtility.cachedDrawEntries);
            }
            StatsReportUtility.DrawStatsWorker(rect, null, null);

            return false;
        }*/