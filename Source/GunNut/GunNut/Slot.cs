using Verse;

namespace GunNut
{
    public class Slot : IExposable
    {
        public void ExposeData()
        {
            Scribe_Defs.Look(ref attachment, "attachment");
            Scribe_Values.Look(ref weaponPart, "weaponPart");
        }

        public GN_ThingDefOf.WeaponPart weaponPart;
        public GN_AttachmentDef attachment = null;
    }
}