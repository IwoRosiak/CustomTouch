using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace GunNut
{
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming", new Type[] { typeof(Thing), typeof(Vector3), typeof(float) })]
    public static class GN_DrawEquipmentAimingPatch
    {
        [HarmonyPostfix]
        private static void PawnRendererPatch(Thing eq, Vector3 drawLoc, float aimAngle, PawnRenderer __instance)
        {
            if (eq.TryGetComp<GN_AttachmentComp>() != null)
            {
                var weapon = eq.TryGetComp<GN_AttachmentComp>();

                foreach (var attachment in weapon.ActiveAttachments)
                {
                    if (attachment.onWeaponGraphic == null)
                    {
                        continue;
                    }

                    float num = aimAngle - 90f;
                    Mesh mesh;

                    if (aimAngle > 20f && aimAngle < 160f)
                    {
                        mesh = MeshPool.plane10;
                        num += eq.def.equippedAngleOffset;
                    }
                    else if (aimAngle > 200f && aimAngle < 340f)
                    {
                        mesh = MeshPool.plane10Flip;
                        num -= 180f;
                        num -= eq.def.equippedAngleOffset;
                    }
                    else
                    {
                        mesh = MeshPool.plane10;
                        num += eq.def.equippedAngleOffset;
                    }
                    num %= 360f;
                    CompEquippable compEquippable = eq.TryGetComp<CompEquippable>();
                    if (compEquippable != null)
                    {
                        Vector3 b;
                        float num2;
                        EquipmentUtility.Recoil(eq.def, EquipmentUtility.GetRecoilVerb(compEquippable.AllVerbs), out b, out num2, aimAngle);
                        drawLoc += b;
                        num += num2;
                    }
                    Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
                    Material matSingle;
                    if (graphic_StackCount != null)
                    {
                        matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                    }
                    else
                    {
                        matSingle = eq.Graphic.MatSingle;
                    }

                    drawLoc.y += 0.00001f;

                    Vector2 offset = IR_Settings.GetPos(eq.def.defName, attachment.weaponPart).RotatedBy(num);

                    Vector3 finalOffset = new Vector3(offset.y, 0, offset.x);

                    Vector3 scale = new Vector3(attachment.onWeaponGraphic.Graphic.drawSize.x,0, attachment.onWeaponGraphic.Graphic.drawSize.y);
                    scale *= IR_Settings.GetSize(eq.def.defName, attachment.weaponPart);

                    Graphics.DrawMesh(mesh, Matrix4x4.TRS(drawLoc + finalOffset, Quaternion.AngleAxis(num, Vector3.up), scale), attachment.onWeaponGraphic.Graphic.MatSingle,0);
                    //Graphics.DrawMesh(mesh, drawLoc + finalOffset, Quaternion.AngleAxis(num, Vector3.up), attachment.onWeaponGraphic.Graphic.MatSingle, 0);
                }
            }
        }
    }
}