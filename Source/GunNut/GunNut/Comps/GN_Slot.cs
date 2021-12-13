using UnityEngine;
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

        //public bool slotActive = true;

        public GN_WeaponParts.WeaponPart weaponPart;
        public GN_AttachmentDef attachment = null;

        public Vector2 defaultPosition = Vector2.zero;
        public bool isActive = true;
    }
}