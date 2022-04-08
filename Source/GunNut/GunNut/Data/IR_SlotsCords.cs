using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GunNut
{
    public class IR_SlotsCords : IExposable
    {
        public List<WeaponTags> weaponTags;

        public Dictionary<IR_AttachmentType, Vector2> position;
        public Dictionary<IR_AttachmentType, bool> isEnabled;

        public Dictionary<IR_AttachmentType, bool> isFront;
        public Dictionary<IR_AttachmentType, float> size;

        public Vector2 GetPosition(IR_AttachmentType part)
        {
            return position[part]/10;
        }

        public void SetPosition(IR_AttachmentType part, Vector2 pos)
        {
            position[part] = pos*10;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref weaponTags, "weaponTags", LookMode.Value);
            Scribe_Collections.Look(ref position, "defaultPlacement", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref isEnabled, "defaultActive", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref isFront, "isFront", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref size, "size", LookMode.Value, LookMode.Value);
        }
    }
}