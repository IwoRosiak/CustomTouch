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
                foreach (var attachment in thing.TryGetComp<GN_AttachmentComp>()?.AttachmentsOnWeapon)
                {
                    Vector3 center = thing.TrueCenter() + __instance.DrawOffset(thing.Rotation);
                    center.y += 0.0001f;

                    Printer_Plane.PrintPlane(layer, center, __instance.drawSize, attachment.onWeaponGraphic.Graphic.MatSingle, extraRotation);
                }
            }
        }
    }
}