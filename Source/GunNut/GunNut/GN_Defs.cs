using RimWorld;
using System.Collections.Generic;
using Verse;


namespace GunNut
{
    [DefOf]
    public static class GN_StatCategoryDefOf
    {
        // Token: 0x06006512 RID: 25874 RVA: 0x00232DA9 File Offset: 0x00230FA9
        static GN_StatCategoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GN_StatCategoryDefOf));
        }

        // Token: 0x04003AC8 RID: 15048
        public static StatCategoryDef Attachments;

        // Token: 0x04003AC9 RID: 15049

    }

    [DefOf]
    public static class GN_ThingDefOf
    {
        static GN_ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GN_ThingDefOf));
        }

        public enum WeaponPart { scope, magazine, stock, barrel }

        //public static GN_ThingDef Gun_BoltActionRifle_Scope;

        //public static GN_ThingDef Gun_BoltActionRifle;

        public static GN_AttachmentDef Scope;

        public static GN_AttachmentDef Chuj;

        public static GN_AttachmentDef Magazine;

        public static GN_AttachmentDef Stock;

        //public static GN_AttachmentDef MakeshiftScope;

        //public static GN_AttachmentDef Magazine;

        //public static GN_AttachmentDef Stock;



    }

    public static class GN_AttachmentList
    {
        public static List<GN_AttachmentDef> allAttachments = new List<GN_AttachmentDef>()
        {
            GN_ThingDefOf.Scope,
            GN_ThingDefOf.Chuj,
            GN_ThingDefOf.Magazine,
            GN_ThingDefOf.Stock
            //GN_ThingDefOf.MakeshiftScope,
            //GN_ThingDefOf.Magazine,
            //GN_ThingDefOf.Stock
        };
    }


    [DefOf]
    public static class GN_SlotDefOf
    {
        static GN_SlotDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GN_SlotDefOf));
        }


        //public static GN_SlotDef slot;

    }

    public class Slot : IExposable
    {
        public void ExposeData()
        {
            Scribe_Defs.Look(ref attachment, "attachment");
            Scribe_Values.Look(ref weaponPart, "weaponPart");
        }

        public GN_ThingDefOf.WeaponPart weaponPart;
        public GN_AttachmentDef attachment = null;
    }

    public class GN_AttachmentDef : ThingDef
    {
        public GN_ThingDefOf.WeaponPart weaponPart;

        public float damageIncreasePer = 0;

        public float damageIncrease = 0;

        public float warmupTimeReduction = 0;

        public float cooldownTimeReduction = 0;

        public float massReduction = 0;

        public float accuracyImproveTouch = 0;

        public float accuracyImproveShort = 0;

        public float accuracyImproveMedium = 0;

        public float accuracyImproveLong = 0;

        //public GN_ThingDefOf.AttachmentSlots slot;

        /*public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        {    
            var attachments = req.Thing.TryGetComp<GN_ThingComp>().Attachments;
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Attachments", attachment, "Attachment for a weapon.", 5391, null, null, false);
                }
            }
        }*/
    }


    public class GN_ThingDef : ThingDef
    {

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



