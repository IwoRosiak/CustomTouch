using System.Collections.Generic;
using UnityEngine;

namespace GunNut
{
    public class IR_SlotsData
    {
        public GN_WeaponParts.WeaponPart weaponPart;

        public Dictionary<GN_WeaponParts.WeaponPart, Vector2> defaultPlacement;
        public Dictionary<GN_WeaponParts.WeaponPart, bool> defaultActive;
    }
}