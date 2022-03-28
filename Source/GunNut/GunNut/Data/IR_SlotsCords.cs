using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GunNut
{
    public class IR_SlotsCords : IExposable
    {
        public List<WeaponTags> weaponTags;

        public Dictionary<GN_WeaponParts.WeaponPart, Vector2> position;
        public Dictionary<GN_WeaponParts.WeaponPart, bool> isEnabled;

        public Dictionary<GN_WeaponParts.WeaponPart, bool> isFront;
        public Dictionary<GN_WeaponParts.WeaponPart, float> size;

        public Vector2 GetPosition(GN_WeaponParts.WeaponPart part)
        {
            return position[part]/10;
        }

        public void SetPosition(GN_WeaponParts.WeaponPart part, Vector2 pos)
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