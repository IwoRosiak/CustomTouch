using Verse;

namespace GunNut
{
    public class GN_Slot : IExposable
    {
        public void ExposeData()
        {
            Scribe_Defs.Look(ref attachment, "attachment");
            Scribe_Values.Look(ref weaponPart, "weaponPart");
        }

        public GN_WeaponParts.WeaponPart weaponPart;
        public GN_AttachmentDef attachment = null;
    }
}