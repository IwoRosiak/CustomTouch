using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class CompProperties_GunNut : CompProperties
    {
        public CompProperties_GunNut()
        {
            this.compClass = typeof(GN_ThingComp);
        }

        public List<Slot> Slots
        {
            get
            {
                return this.slots;
            }
        }

        public List<Slot> slots;

        public JobDef jobDefInstall;

        public JobDef jobDefRemove;
    }
}