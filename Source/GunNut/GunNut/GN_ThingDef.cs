using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class GN_ThingDef : ThingDef
    {
        protected override void ResolveIcon()
        {
            base.ResolveIcon();
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        {
            foreach (StatDrawEntry statDrawEntry in base.SpecialDisplayStats(req))
            {
                yield return statDrawEntry;
            }

            var slots = req.Thing.TryGetComp<GN_ThingComp>().Slots;

            //var attachment = req.Thing.TryGetComp<GN_ThingComp>().Slots;
            //var attachments = req.Thing.TryGetComp<GN_ThingComp>().Slots[0].attachment;

            if (slots != null)
            {
                float warmupImprove = 1.0f;

                foreach (var slot in req.Thing.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        warmupImprove = warmupImprove - slot.attachment.warmupTimeReduction;
                    }
                }

                foreach (var slot in slots)
                {
                    string attachmentName = "Default";
                    string attachmentDesc = "Default";
                    if (slot.attachment == null)
                    {
                        attachmentName = "Default";
                        attachmentDesc = "Default " + slot.weaponPart.ToString() + " that came with the weapon.";
                    }
                    else
                    {
                        attachmentName = slot.attachment.label;
                        attachmentDesc = slot.attachment.description;
                    }

                    yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, slot.weaponPart.ToString(), attachmentName.CapitalizeFirst(), attachmentDesc, 5391, null, null, false);
                }

                yield return new StatDrawEntry(StatCategoryDefOf.Weapon, "Final Warmup: ", (warmupImprove * req.Thing.def.Verbs[0].warmupTime).ToString() + " s", "Final warmup time after applying attachments.", 100, null, null, false);
            }
        }
    }
}