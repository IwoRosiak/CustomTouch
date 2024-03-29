﻿using RimWorld;
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
            if (req.Thing.TryGetComp<GN_AttachmentComp>() != null && req.Thing.TryGetComp<GN_AttachmentComp>().ActiveSlots != null)
            {
                float improvement = 1;
                foreach (var attachment in req.Thing.TryGetComp<GN_AttachmentComp>().ActiveAttachments)
                {

                    if (this.parentStat.defName == "RangedWeapon_Cooldown")
                    {
                        improvement -= attachment.cooldownMult;
                    }
                    else if (this.parentStat.defName == "RangedWeapon_DamageMultiplier")
                    {
                        improvement += attachment.damageMult;
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

                if (req.Thing.TryGetComp<GN_AttachmentComp>() != null && req.Thing.TryGetComp<GN_AttachmentComp>().ActiveSlots != null)
                {
                    float improvement = 1;
                    foreach (var attachment in req.Thing.TryGetComp<GN_AttachmentComp>().ActiveAttachments)
                    {
                        if (this.parentStat.defName == "RangedWeapon_Cooldown")
                        {
                            improvement -= attachment.cooldownMult;
                        }
                        else if (this.parentStat.defName == "RangedWeapon_DamageMultiplier")
                        {
                            improvement += attachment.damageMult;
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