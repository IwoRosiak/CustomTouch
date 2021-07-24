using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class GN_AttachmentCompProperties : CompProperties
    {
        public GN_AttachmentCompProperties()
        {
            this.compClass = typeof(GN_AttachmentComp);
        }

        public List<GN_Slot> Slots
        {
            get
            {
                return this.slots;
            }
        }

        public List<GN_Slot> slots;

        public JobDef jobDefInstall;

        public JobDef jobDefRemove;
    }
}