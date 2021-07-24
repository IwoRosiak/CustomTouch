using System.Collections.Generic;

namespace GunNut
{
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
}