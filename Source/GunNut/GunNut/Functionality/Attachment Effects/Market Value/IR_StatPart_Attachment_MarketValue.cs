using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GunNut
{
    public class IR_StatPart_MarketValue : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
			if (req.HasThing)
			{
				GN_AttachmentComp comp = req.Thing.TryGetComp<GN_AttachmentComp>();

				if (comp != null && req.Thing.TryGetComp<GN_AttachmentComp>().ActiveSlots != null)
                {
					foreach (GN_AttachmentDef attachment in comp.ActiveAttachments)
                    {
                        //float valueOffset = StatDefOf.MarketValue.GetStatPart<StatPart_Quality>()
                       // float num = val;
                        
                        //StatDefOf.MarketValue.GetStatPart<StatPart_Quality>().TransformValue(req, ref num);

                        //float qualityFactor = num/val;

                        //Log.Message((attachment.BaseMarketValue / qualityFactor).ToString());
                        val += attachment.BaseMarketValue ;
                    }
                }
			}
		}

        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing)
            {
                StringBuilder text = new StringBuilder();

                if (req.Thing.TryGetComp<GN_AttachmentComp>() != null && req.Thing.TryGetComp<GN_AttachmentComp>().ActiveSlots != null )
                {
                    foreach (var attachment in req.Thing.TryGetComp<GN_AttachmentComp>().ActiveAttachments)
                    {
                       text.AppendLine(attachment.label + ": " + attachment.BaseMarketValue.ToStringByStyle(ToStringStyle.Money, ToStringNumberSense.Offset)); 
                    }
                }
                return text.ToString();
            }
            return null;
        }
    }
}
