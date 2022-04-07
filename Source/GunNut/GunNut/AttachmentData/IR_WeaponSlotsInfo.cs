using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GunNut
{
    internal class IR_WeaponSlotsInfo : IExposable
    {
        GN_AttachmentComp parent;

        private Dictionary<GN_WeaponParts.WeaponPart, GN_Slot> slots = new Dictionary<GN_WeaponParts.WeaponPart, GN_Slot>();

        public IR_WeaponSlotsInfo(GN_AttachmentComp _parent)
        {
            parent = _parent;

            InitSlots();
        }

        public List<GN_Slot> GetSlots()
        {
            List<GN_Slot> resultSlots = new List<GN_Slot>();

            if (slots.Count != Enum.GetValues(typeof(GN_WeaponParts.WeaponPart)).EnumerableCount())
            {
                UpdateSlots();
            }

            foreach (KeyValuePair<GN_WeaponParts.WeaponPart, GN_Slot> slot in slots)
            {
                if (IR_Settings.IsActive(parent.parent.def.defName, slot.Key))
                {
                    resultSlots.Add(slot.Value);
                }
            }

            return resultSlots;
        }

        private void InitSlots()
        {
            slots = new Dictionary<GN_WeaponParts.WeaponPart, GN_Slot>();

            List<GN_Slot> initSlots = new List<GN_Slot>();

            GN_Slot[] array = new GN_Slot[parent.SlotsProps.Count()];

            parent.SlotsProps.CopyTo(array);

            initSlots.AddRange(array);

            foreach (GN_Slot slot in initSlots)
            {
                slots.Add(slot.weaponPart, slot);
            }
        }

        private void UpdateSlots()
        {
            foreach (GN_WeaponParts.WeaponPart part in Enum.GetValues(typeof(GN_WeaponParts.WeaponPart)))
            {
                if (!slots.ContainsKey(part))
                {
                    GN_Slot slot = new GN_Slot();

                    slot.weaponPart = part;

                    slots.Add(part, slot);
                }
            }
        }


        public void ExposeData()
        {
            //Scribe_Values.Look(ref parent, "parent");
            Scribe_Collections.Look(ref slots, "SlotsAttachment" +parent.parent.ThingID, LookMode.Value, LookMode.Deep);
        }
    }
}
