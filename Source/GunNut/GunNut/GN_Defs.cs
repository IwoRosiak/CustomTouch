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

        public static GN_ThingDef Gun_BoltActionRifle_Scope;

        public static GN_AttachmentDef Scope;

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

    public class Slot
    {
        public GN_ThingDefOf.WeaponPart weaponPart;
        public string attachmentName = "Default";
        public string attachmentDesc = " ";
    }

    public class GN_AttachmentDef : ThingDef
    {
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
                foreach (var slot in slots)
                {
                    yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, slot.weaponPart.ToString(), slot.attachmentName, slot.attachmentDesc, 5391, null, null, false);
                }
            }
        }
    }

}



