using RimWorld;

namespace GunNut
{
    [DefOf]
    public static class IR_StatCategoryDefOf
    {
        static IR_StatCategoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(IR_StatCategoryDefOf));
        }

        public static StatCategoryDef Attachments;
        public static StatCategoryDef Attachment_Req_Tags;
        public static StatCategoryDef Attachment_Conf_Tags;
        public static StatCategoryDef Weapon_Tags;
    }
}