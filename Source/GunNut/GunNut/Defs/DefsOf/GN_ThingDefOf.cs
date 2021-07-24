using RimWorld;

namespace GunNut
{
    [DefOf]
    public static class GN_ThingDefOf
    {
        static GN_ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GN_ThingDefOf));
        }

        public enum WeaponPart { scope, magazine, stock, barrel }

        public static GN_AttachmentDef Scope;

        public static GN_AttachmentDef Chuj;

        public static GN_AttachmentDef Magazine;

        public static GN_AttachmentDef Stock;

        //public static GN_AttachmentDef MakeshiftScope;

        //public static GN_AttachmentDef Magazine;

        //public static GN_AttachmentDef Stock;
    }
}