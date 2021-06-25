using RimWorld;
using Verse;


namespace GunNut
{
    public static class GN_Verbs
    {
        public static float AdjustedAccuracy(RangeCategory category, Thing equipment)
        {



            StatDef stat = null;
            switch (category)
            {
                case RangeCategory.Touch:
                    stat = StatDefOf.AccuracyTouch;
                    break;
                case RangeCategory.Short:
                    stat = StatDefOf.AccuracyShort;
                    break;
                case RangeCategory.Medium:
                    stat = StatDefOf.AccuracyMedium;
                    break;
                case RangeCategory.Long:
                    stat = StatDefOf.AccuracyLong;
                    break;
            }
            return equipment.GetStatValue(stat, true);

        }

        public enum RangeCategory : byte
        {
            Touch,
            Short,
            Medium,
            Long
        }
    }
}
