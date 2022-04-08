using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class GN_AttachmentDef : ThingDef
    {
        public List<WeaponTags> requiredTags = new List<WeaponTags>();
        public List<WeaponTags> conflictingTags = new List<WeaponTags>();

        public IR_AttachmentType weaponPart;

        public GraphicData onWeaponGraphic;

        //IMPLEMENTED
        public float accuracyTouch = 0;
        public float accuracyShort = 0;
        public float accuracyMedium = 0;
        public float accuracyLong = 0;

        public float damageMult = 0;
        public float warmupMult = 0;
        public float cooldownMult = 0;

        public float nightvision = 0;
        public float infravision = 0;
        public float zoomvision = 0;

        public float silencerEffect = 0;

        public float burstShotsMult = 0;
        public int burstShotsOffset = 0;

        //NOT IMPLEMENTED
        public float reflectiveEffect = 0; //easier to hit if you have that
        public float massOffset = 0; //adding attachment mass to weapons


        public float meleeDamageOffset = 0; 
        public float meleeSpeedOffset = 0;

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        {
            foreach (StatDrawEntry statDrawEntry in base.SpecialDisplayStats(req))
            {
                yield return statDrawEntry;
            }

            foreach (StatDrawEntry statDrawEntry in AttachmentSpecialStats())
            {
                yield return statDrawEntry;
            }
        }

        public IEnumerable<StatDrawEntry> AttachmentSpecialStats()
        {
            if (accuracyTouch != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Accuracy touch", (1 + accuracyTouch).ToStringByStyle(ToStringStyle.PercentTwo, ToStringNumberSense.Factor), "Accuracy improvement over a touch distance.", 2753, null, null, false);
            }

            if (accuracyShort != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Accuracy short", (1 + accuracyShort).ToStringByStyle(ToStringStyle.PercentTwo, ToStringNumberSense.Factor), "Accuracy improvement over a short distance.", 2753, null, null, false);
            }

            if (accuracyMedium != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Accuracy medium", (1 + accuracyMedium).ToStringByStyle(ToStringStyle.PercentTwo, ToStringNumberSense.Factor), "Accuracy improvement over a medium distance.", 2753, null, null, false);
            }

            if (accuracyLong != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Accuracy long", (1 + accuracyLong).ToStringByStyle(ToStringStyle.PercentTwo, ToStringNumberSense.Factor), "Accuracy improvement over a long distance.", 2753, null, null, false);
            }

            if (cooldownMult != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Cooldown", (1 + cooldownMult).ToStringByStyle(ToStringStyle.PercentTwo, ToStringNumberSense.Factor), "Cooldown multiplier.", 2753, null, null, false);
            }

            if (warmupMult != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Warmup", (1 + warmupMult).ToStringByStyle(ToStringStyle.PercentTwo, ToStringNumberSense.Factor), "Warmup multiplier.", 2753, null, null, false);
            }

            if (damageMult != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Damage", (1 - damageMult).ToStringByStyle(ToStringStyle.PercentOne, ToStringNumberSense.Factor), "Damage multiplier.", 2753, null, null, false);
            }

            if (nightvision != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Night-vision", nightvision.ToStringByStyle(ToStringStyle.PercentOne), "This attachment provides night-vision capabilities to the weapon it is installed to. Night-vision provides 25% of its strength as debuff negation for harsh weather accuracy penalties.", 2753, null, null, false);
            }

            if (infravision != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Infra-vision", infravision.ToStringByStyle(ToStringStyle.PercentOne), "Infra-vision makes it easier to observe targets in un-favourable conditions such as snow or rain. It is not as effective for tracking targets at night.", 2753, null, null, false);
            }

            if (zoomvision != 0)
            {
                yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Zoom", zoomvision.ToStringByStyle(ToStringStyle.PercentOne), "Zoom capabilities make it easier to hit smaller targets.", 2753, null, null, false);
            }
        }

        public IEnumerable<string> AttachmentSpecialStatsInString()
        {
            if (accuracyTouch != 0)
            {
                yield return "Accuracy touch: "+ (1 + accuracyTouch).ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (accuracyShort != 0)
            {
                yield return "Accuracy short: " + (1 + accuracyShort).ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (accuracyMedium != 0)
            {
                yield return "Accuracy medium: " + (1 + accuracyMedium).ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (accuracyLong != 0)
            {
                yield return "Accuracy long: " + (1 + accuracyLong).ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (cooldownMult != 0)
            {
                yield return "Cooldown: " + (1+cooldownMult).ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (warmupMult != 0)
            {
                yield return "Warmup: " + (1 + warmupMult).ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (damageMult != 0)
            {
                yield return "Damage: " + (1 - damageMult).ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (nightvision != 0)
            {
                yield return "Night-vision strength: " + nightvision.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (infravision != 0)
            {
                yield return "Infra-vision strength: " + infravision.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }

            if (zoomvision != 0)
            {
                yield return "Zoom strength: " + zoomvision.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);
            }
        }

    }
}