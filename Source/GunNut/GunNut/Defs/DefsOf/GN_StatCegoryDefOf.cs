using RimWorld;

namespace GunNut
{
    [DefOf]
    public static class GN_StatCategoryDefOf
    {
        static GN_StatCategoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GN_StatCategoryDefOf));
        }

        public static StatCategoryDef Attachments;
    }
}