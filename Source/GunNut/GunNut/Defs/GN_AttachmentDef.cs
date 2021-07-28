using Verse;

namespace GunNut
{
    public class GN_AttachmentDef : ThingDef
    {
        public GN_WeaponParts.WeaponPart weaponPart;

        public GraphicData onWeaponGraphic;

        public float damageIncreasePer = 0;

        public float damageIncrease = 0;

        public float warmupTimeReduction = 0;

        public float cooldownTimeReduction = 0;

        public float massReduction = 0;

        public float accuracyImproveTouch = 0;

        public float accuracyImproveShort = 0;

        public float accuracyImproveMedium = 0;

        public float accuracyImproveLong = 0;
    }
}