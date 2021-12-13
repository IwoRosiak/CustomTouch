using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GunNut
{
    [HarmonyPatch(typeof(Graphic), "Print")]
    public static class GN_PrintPatch
    {
        [HarmonyPostfix]
        public static void GN_PrintPostfix(Graphic __instance, SectionLayer layer, Thing thing, float extraRotation)
        {
            if (thing.TryGetComp<GN_AttachmentComp>() != null)
            {
                foreach (var slot in thing.TryGetComp<GN_AttachmentComp>()?.SlotsOnWeapon)
                {
                    if (slot.attachment != null)
                    {
                        Vector2 offsetV2 = IR_Init.WeaponsCustomInfo[thing.def.defName].defaultPlacement[slot.weaponPart];

                        Log.Message("Not Rotated X: " + offsetV2.x);

                        offsetV2 = offsetV2.RotatedBy(extraRotation);

                        Log.Message("Rotated X: " + offsetV2.x);

                        Vector3 offset = new Vector3(offsetV2.y, 0, offsetV2.x);

                        Vector3 center = thing.TrueCenter() + __instance.DrawOffset(thing.Rotation) + offset;

                        Log.Message("Center: " + center.ToString());
                        center.y += 0.0001f;

                        Printer_Plane.PrintPlane(layer, center, __instance.drawSize, slot.attachment.onWeaponGraphic.Graphic.MatSingle, extraRotation);
                    }
                }
            }
        }
    }
}