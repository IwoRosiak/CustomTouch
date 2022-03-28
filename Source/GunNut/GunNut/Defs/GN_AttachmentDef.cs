using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class GN_AttachmentDef : ThingDef
    {
        public List<WeaponTags> requiredTags = new List<WeaponTags>();
        public List<WeaponTags> conflictingTags = new List<WeaponTags>();

        public GN_WeaponParts.WeaponPart weaponPart;
        public AttachmentSize size;

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

        public float noiseOFfset = 0;
        public float meleeDamageOffset = 0;
        public float meleeSpeedOffset = 0;
    }

    public enum AttachmentSize
    {
        small,
        medium,
        big
    }
}