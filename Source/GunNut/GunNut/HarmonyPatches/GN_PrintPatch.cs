using HarmonyLib;
using Verse;

namespace GunNut
{
    [HarmonyPatch(typeof(Graphic), "Print")]
    public static class GN_PrintPatch
    {
        [HarmonyPostfix]
        public static void GN_PrintPostfix(Graphic __instance, Thing thing, SectionLayer layer, float extraRotation)
        {
            if (__instance.data.texPath == thing.def.graphicData.texPath)
            {
                if (thing.TryGetComp<GN_AttachmentComp>() != null)
                {
                    var weapon = thing.TryGetComp<GN_AttachmentComp>();

                    foreach (var slot in weapon.Slots)
                    {
                        if (slot.attachment != null)
                        {
                            slot.attachment.onWeaponGraphic.Graphic.Print(layer, thing, extraRotation);
                        }
                    }
                }
            }
        }
    }
}