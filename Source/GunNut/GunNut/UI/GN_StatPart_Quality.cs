using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace GunNut
{
    public class GN_StatPart_Quality : StatPart_Quality
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (val <= 0f && !this.applyToNegativeValues)
            {
                return;
            }
            float num;
            if (req.Thing.TryGetComp<GN_AttachmentComp>() != null && req.Thing.TryGetComp<GN_AttachmentComp>().SlotsOnWeapon != null)
            {
                float improvement = 1;
                string defName = this.parentStat.defName;

                foreach (var attachment in req.Thing.TryGetComp<GN_AttachmentComp>().AttachmentsOnWeapon)
                {
                    if (defName == "AccuracyLong")
                    {
                        improvement += attachment.accuracyLong;
                    }
                    else if (defName == "AccuracyMedium")
                    {
                        improvement += attachment.accuracyMedium;
                    }
                    else if (defName == "AccuracyShort")
                    {
                        improvement += attachment.accuracyShort;
                    }
                    else if (defName == "AccuracyTouch")
                    {
                        improvement += attachment.accuracyTouch;
                    }
                    else if (defName == "RangedWeapon_DamageMultiplier")
                    {
                        foreach (var item in this.parentStat.parts)
                        {
                            Log.Message(item.ToString());
                        }

                        improvement += attachment.damageMult;
                    }
                }

                num = val * improvement * this.QualityMultiplier(req.QualityCategory) - val;
            }
            else
            {
                num = val * this.QualityMultiplier(req.QualityCategory) - val;
            }

            num = Mathf.Min(num, this.MaxGain(req.QualityCategory));
            val += num;
        }

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

                if (req.Thing.TryGetComp<GN_AttachmentComp>() != null && req.Thing.TryGetComp<GN_AttachmentComp>().SlotsOnWeapon != null)
                {
                    float improvement = 1;
                    string defName = this.parentStat.defName;

                    foreach (var attachment in req.Thing.TryGetComp<GN_AttachmentComp>().AttachmentsOnWeapon)
                    {
                        if (defName == "AccuracyLong")
                        {
                            improvement += attachment.accuracyLong;
                        }
                        else if (defName == "AccuracyMedium")
                        {
                            improvement += attachment.accuracyMedium;
                        }
                        else if (defName == "AccuracyShort")
                        {
                            improvement += attachment.accuracyShort;
                        }
                        else if (defName == "AccuracyTouch")
                        {
                            improvement += attachment.accuracyTouch;
                        }
                        else if (defName == "RangedWeapon_DamageMultiplier")
                        {
                            improvement += attachment.damageMult;
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

        private bool applyToNegativeValues;

        private float factorAwful = 1f;

        private float factorPoor = 1f;

        private float factorNormal = 1f;

        private float factorGood = 1f;

        private float factorExcellent = 1f;

        private float factorMasterwork = 1f;

        private float factorLegendary = 1f;

        private float maxGainAwful = 9999999f;

        private float maxGainPoor = 9999999f;

        private float maxGainNormal = 9999999f;

        private float maxGainGood = 9999999f;

        private float maxGainExcellent = 9999999f;

        private float maxGainMasterwork = 9999999f;

        private float maxGainLegendary = 9999999f;
    }
}