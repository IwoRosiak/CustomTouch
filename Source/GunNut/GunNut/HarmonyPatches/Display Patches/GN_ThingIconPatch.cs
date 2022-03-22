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
                var weapon = thing.TryGetComp<GN_AttachmentComp>();

                foreach (var attachment in weapon.AttachmentsOnWeapon)
                {
                    Texture texture = attachment.onWeaponGraphic.Graphic.MatSingle.mainTexture;

                    Vector2 texProportions = new Vector2((float)texture.width, (float)attachment.uiIcon.height);

                    Vector2 offset = IR_Settings.GetPos(thing.def.defName, attachment.weaponPart);

                    Rect texCoords = new Rect(0f + offset.y, 0f - offset.x, 1f + offset.y, 1f + offset.x);
                    if (attachment.onWeaponGraphic != null)
                    {
                        texProportions = attachment.onWeaponGraphic.drawSize.RotatedBy(attachment.defaultPlacingRot);
                        if (attachment.uiIconPath.NullOrEmpty() && attachment.onWeaponGraphic.linkFlags != LinkFlags.None)
                        {
                            texCoords = new Rect(0f, 0.5f, 0.25f, 0.25f);
                        }
                    }
                    Widgets.DrawTextureFitted(rect, texture, GenUI.IconDrawScale(attachment) * 1, texProportions, texCoords, attachment.uiIconAngle, null);
                }
            }
        }
    }
}