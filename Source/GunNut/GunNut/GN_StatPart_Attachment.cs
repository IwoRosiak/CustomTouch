using RimWorld;
using Verse;

namespace GunNut
{
    public class GN_StatPart_Attachment : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (val <= 0f)
            {
                return;
            }
            float num = 0;
            if (req.Thing.TryGetComp<GN_ThingComp>() != null && req.Thing.TryGetComp<GN_ThingComp>().Slots != null)
            {
                float improvement = 1;
                foreach (var slot in req.Thing.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        if (this.parentStat.defName == "RangedWeapon_Cooldown")
                        {
                            improvement -= slot.attachment.cooldownTimeReduction;
                        }
                        else if (this.parentStat.defName == "RangedWeapon_DamageMultiplier")
                        {
                            improvement += slot.attachment.damageIncrease;
                        }
                    }
                }

                num = val * improvement - val;
            }

            val += num;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing && req.Thing.GetStatValue(this.parentStat, true) <= 0f)
            {
                return null;
            }
            if (req.HasThing)
            {
                string text = "";

                if (req.Thing.TryGetComp<GN_ThingComp>() != null && req.Thing.TryGetComp<GN_ThingComp>().Slots != null)
                {
                    float improvement = 1;
                    foreach (var slot in req.Thing.TryGetComp<GN_ThingComp>().Slots)
                    {
                        if (slot.attachment != null)
                        {
                            if (this.parentStat.defName == "RangedWeapon_Cooldown")
                            {
                                improvement -= slot.attachment.cooldownTimeReduction;
                            }
                            else if (this.parentStat.defName == "RangedWeapon_DamageMultiplier")
                            {
                                improvement += slot.attachment.damageIncrease;
                            }
                        }
                    }

                    text += "Multiplier from attachments: x" + improvement.ToStringPercent();
                }
                return text;
            }
            return null;
        }
    }
}