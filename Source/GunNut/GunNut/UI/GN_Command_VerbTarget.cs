using UnityEngine;
using Verse;

namespace GunNut
{
    public class GN_Command_VerbTarget : Command_VerbTarget
    {
        protected override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
        {
            base.DrawIcon(rect, buttonMat, parms);
            rect.position += new Vector2(this.iconOffset.x * rect.size.x, this.iconOffset.y * rect.size.y);

            if (verb.EquipmentSource.TryGetComp<GN_AttachmentComp>() != null)
            {
                var weapon = verb.EquipmentSource.TryGetComp<GN_AttachmentComp>();
                foreach (var slot in weapon.Slots)
                {
                    if (slot.attachment != null)
                    {
                        Widgets.DrawTextureFitted(rect, slot.attachment.onWeaponGraphic.Graphic.MatSingle.mainTexture,
                            this.iconDrawScale * 0.85f, this.iconProportions, this.iconTexCoords);
                    }
                }
            }
        }
    }
}