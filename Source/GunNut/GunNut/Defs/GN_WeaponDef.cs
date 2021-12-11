using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class GN_WeaponDef : ThingDef
    {
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        {
            foreach (StatDrawEntry statDrawEntry in base.SpecialDisplayStats(req))
            {
                yield return statDrawEntry;
            }
            var weapon = req.Thing.TryGetComp<GN_AttachmentComp>();

            float warmupImprove = 1.0f;

            foreach (var attachment in weapon.AttachmentsOnWeapon)
            {
                warmupImprove -= attachment.warmupTimeReduction;
            }

            foreach (var slot in weapon.Slots)
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

            yield return new StatDrawEntry(StatCategoryDefOf.Weapon, "Final Warmup: ", (warmupImprove * req.Thing.def.Verbs[0].warmupTime).ToString() + " s", "Final warmup time after applying attachments.", 100, null, null, false);
        }
    }
}