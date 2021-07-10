using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace GunNut
{
    public class GN_StatPart_Quality : StatPart_Quality
    {
        // Token: 0x06006706 RID: 26374 RVA: 0x0023D7E4 File Offset: 0x0023B9E4
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (val <= 0f && !this.applyToNegativeValues)
            {
                return;
            }
            float num;
            if (req.Thing.TryGetComp<GN_ThingComp>() != null && req.Thing.TryGetComp<GN_ThingComp>().Slots != null)
            {

                float improvement = 1;
                string defName = this.parentStat.defName;

                foreach (var slot in req.Thing.TryGetComp<GN_ThingComp>().Slots)
                {
                    if (slot.attachment != null)
                    {
                        if (defName == "AccuracyLong")
                        {
                            improvement += slot.attachment.accuracyImproveLong;
                        }
                        else if (defName == "AccuracyMedium")
                        {
                            improvement += slot.attachment.accuracyImproveMedium;
                        }
                        else if (defName == "AccuracyShort")
                        {
                            improvement += slot.attachment.accuracyImproveShort;
                        }
                        else if (defName == "AccuracyTouch")
                        {
                            improvement += slot.attachment.accuracyImproveTouch;
                        }
                        else if (defName == "RangedWeapon_DamageMultiplier")
                        {
                            foreach (var item in this.parentStat.parts)
                            {
                                Log.Message(item.ToString());
                            }

                            improvement += slot.attachment.damageIncrease;
                        }
                    }
                }

                num = val * improvement * this.QualityMultiplier(req.QualityCategory) - val;
                // num = val * this.QualityMultiplier(req.QualityCategory) - val;

            }
            else
            {
                num = val * this.QualityMultiplier(req.QualityCategory) - val;
            }





            num = Mathf.Min(num, this.MaxGain(req.QualityCategory));
            val += num;
        }

        // Token: 0x06006707 RID: 26375 RVA: 0x0023D834 File Offset: 0x0023BA34
        public override string ExplanationPart(StatRequest req)
        {



            if (req.HasThing && !this.applyToNegativeValues && req.Thing.GetStatValue(this.parentStat, true) <= 0f)
            {
                return null;
            }
            QualityCategory qc;
            if (req.HasThing && req.Thing.TryGetQuality(out qc))
            {
                string text = "StatsReport_QualityMultiplier".Translate() + ": x" + this.QualityMultiplier(qc).ToStringPercent();
                float num = this.MaxGain(qc);


                if (req.Thing.TryGetComp<GN_ThingComp>() != null && req.Thing.TryGetComp<GN_ThingComp>().Slots != null)
                {

                    float improvement = 1;
                    string defName = this.parentStat.defName;

                    foreach (var slot in req.Thing.TryGetComp<GN_ThingComp>().Slots)
                    {
                        if (slot.attachment != null)
                        {
                            if (defName == "AccuracyLong")
                            {
                                improvement += slot.attachment.accuracyImproveLong;
                            }
                            else if (defName == "AccuracyMedium")
                            {
                                improvement += slot.attachment.accuracyImproveMedium;
                            }
                            else if (defName == "AccuracyShort")
                            {
                                improvement += slot.attachment.accuracyImproveShort;
                            }
                            else if (defName == "AccuracyTouch")
                            {
                                improvement += slot.attachment.accuracyImproveTouch;
                            }
                            else if (defName == "RangedWeapon_DamageMultiplier")
                            {
                                improvement += slot.attachment.damageIncrease;
                            }
                        }
                    }

                    text += "\nMultiplier from attachments: x" + improvement.ToStringPercent();
                }



                if (num < 999999f)
                {
                    text += "\n    (" + "StatsReport_MaxGain".Translate() + ": " + num.ToStringByStyle(this.parentStat.ToStringStyleUnfinalized, this.parentStat.toStringNumberSense) + ")";
                }


                return text;
            }
            return null;
        }

        // Token: 0x06006708 RID: 26376 RVA: 0x0023D920 File Offset: 0x0023BB20
        private float QualityMultiplier(QualityCategory qc)
        {
            switch (qc)
            {
                case QualityCategory.Awful:
                    return this.factorAwful;
                case QualityCategory.Poor:
                    return this.factorPoor;
                case QualityCategory.Normal:
                    return this.factorNormal;
                case QualityCategory.Good:
                    return this.factorGood;
                case QualityCategory.Excellent:
                    return this.factorExcellent;
                case QualityCategory.Masterwork:
                    return this.factorMasterwork;
                case QualityCategory.Legendary:
                    return this.factorLegendary;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Token: 0x06006709 RID: 26377 RVA: 0x0023D988 File Offset: 0x0023BB88
        private float MaxGain(QualityCategory qc)
        {
            switch (qc)
            {
                case QualityCategory.Awful:
                    return this.maxGainAwful;
                case QualityCategory.Poor:
                    return this.maxGainPoor;
                case QualityCategory.Normal:
                    return this.maxGainNormal;
                case QualityCategory.Good:
                    return this.maxGainGood;
                case QualityCategory.Excellent:
                    return this.maxGainExcellent;
                case QualityCategory.Masterwork:
                    return this.maxGainMasterwork;
                case QualityCategory.Legendary:
                    return this.maxGainLegendary;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        // private float improvement = 1;

        // Token: 0x04003F0C RID: 16140
        private bool applyToNegativeValues;

        // Token: 0x04003F0D RID: 16141
        private float factorAwful = 1f;

        // Token: 0x04003F0E RID: 16142
        private float factorPoor = 1f;

        // Token: 0x04003F0F RID: 16143
        private float factorNormal = 1f;

        // Token: 0x04003F10 RID: 16144
        private float factorGood = 1f;

        // Token: 0x04003F11 RID: 16145
        private float factorExcellent = 1f;

        // Token: 0x04003F12 RID: 16146
        private float factorMasterwork = 1f;

        // Token: 0x04003F13 RID: 16147
        private float factorLegendary = 1f;

        // Token: 0x04003F14 RID: 16148
        private float maxGainAwful = 9999999f;

        // Token: 0x04003F15 RID: 16149
        private float maxGainPoor = 9999999f;

        // Token: 0x04003F16 RID: 16150
        private float maxGainNormal = 9999999f;

        // Token: 0x04003F17 RID: 16151
        private float maxGainGood = 9999999f;

        // Token: 0x04003F18 RID: 16152
        private float maxGainExcellent = 9999999f;

        // Token: 0x04003F19 RID: 16153
        private float maxGainMasterwork = 9999999f;

        // Token: 0x04003F1A RID: 16154
        private float maxGainLegendary = 9999999f;
    }

    public class GN_StatPart_Attachment : StatPart
    {
        // Token: 0x06006706 RID: 26374 RVA: 0x0023D7E4 File Offset: 0x0023B9E4
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
                // num = val * this.QualityMultiplier(req.QualityCategory) - val;

            }






            //num = Mathf.Min(num, this.MaxGain(req.QualityCategory));
            val += num;
        }

        // Token: 0x06006707 RID: 26375 RVA: 0x0023D834 File Offset: 0x0023BA34
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
