using HarmonyLib;
using System;
using UnityEngine;
using Verse;

namespace GunNut
{
    [HarmonyPatch(typeof(Widgets), "ThingIcon")]
    [HarmonyPatch(new Type[] { typeof(Rect), typeof(Thing), typeof(float), typeof(Rot4) })]
    public static class GN_ThingIconPatch
    {
        [HarmonyPostfix]
        public static void GN_AdjustFullCyclePostfix(Thing thing, Rect rect)
        {
            if (thing.TryGetComp<GN_AttachmentComp>() != null)
            {
                Log.Message("ThingIcon Patch");

                var weapon = thing.TryGetComp<GN_AttachmentComp>();

                foreach (var slot in weapon.Slots)
                {
                    if (slot.attachment != null)
                    {
                        Log.Message("Displaying attachment");

                        Texture texture = slot.attachment.onWeaponGraphic.Graphic.MatSingle.mainTexture;

                        Vector2 texProportions = new Vector2((float)texture.width, (float)slot.attachment.uiIcon.height);
                        Rect texCoords = new Rect(0f, 0f, 1f, 1f);
                        if (slot.attachment.onWeaponGraphic != null)
                        {
                            texProportions = slot.attachment.onWeaponGraphic.drawSize.RotatedBy(slot.attachment.defaultPlacingRot);
                            if (slot.attachment.uiIconPath.NullOrEmpty() && slot.attachment.onWeaponGraphic.linkFlags != LinkFlags.None)
                            {
                                texCoords = new Rect(0f, 0.5f, 0.25f, 0.25f);
                            }
                        }
                        Widgets.DrawTextureFitted(rect, texture, GenUI.IconDrawScale(slot.attachment) * 1, texProportions, texCoords, slot.attachment.uiIconAngle, null);
                    }
                }
            }
        }
    }
}