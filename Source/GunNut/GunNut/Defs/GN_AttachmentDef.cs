using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class GN_AttachmentDef : ThingDef
    {
        public List<WeaponTags> requiredTags = new List<WeaponTags>();
        public List<WeaponTags> conflictingTags = new List<WeaponTags>();

        public GN_WeaponParts.WeaponPart weaponPart;
        public AttachmentSize size = AttachmentSize.medium;

        public GraphicData onWeaponGraphic;

        //IMPLEMENTED
        public float accuracyTouch = 0;
        public float accuracyShort = 0;
        public float accuracyMedium = 0;
        public float accuracyLong = 0;

        public float damageMult = 0;

        public float warmupMult = 0;

        public float cooldownMult = 0;

        //NOT IMPLEMENTED
        public float massOffset = 0;

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