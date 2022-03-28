using System;
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
                if (slots.NullOrEmpty())
                {
                    slots = new List<GN_Slot>();
                }
                return this.slots;
            }
        }

        public bool ContainsTypeOfSlot(GN_WeaponParts.WeaponPart type)
        {
            foreach (var slot in Slots)
            {
                if (slot.weaponPart == type)
                {
                    return true;
                }
            }
            return false;
        }

        public GN_Slot GetSlotOfType(GN_WeaponParts.WeaponPart type)
        {
            foreach (var slot in Slots)
            {
                if (slot.weaponPart == type)
                {
                    return slot;
                }
            }
            return null;
        }

        public List<GN_Slot> slots = new List<GN_Slot>();

        public List<WeaponTags> tags = new List<WeaponTags>();

        public JobDef jobDefInstall;

        public JobDef jobDefRemove;
    }


    public enum WeaponTags
    {
        shotgun,
        pistol,
        singleHand,
        boltAction,
        noStock,
        automatic,
        semiAutomatic,
        machineGun,
        sniper,
        undermount
    }
}