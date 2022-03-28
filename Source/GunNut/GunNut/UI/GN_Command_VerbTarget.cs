using UnityEngine;
using Verse;

namespace GunNut
{
    public class GN_Command_VerbTarget : Command_VerbTarget //This one has some problems, so for now it will not be a priority
    {
        
        public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
        {
            base.DrawIcon(rect, buttonMat, parms);
            /*
            if (verb.EquipmentSource.TryGetComp<GN_AttachmentComp>() != null)
            {
                var weapon = verb.EquipmentSource.TryGetComp<GN_AttachmentComp>();

                foreach (var attachment in weapon.AttachmentsOnWeapon)
                {
                    Rect drawPos = iconTexCoords;

                    Vector2 offset = IR_Settings.GetPos(weapon.parent.def.defName, attachment.weaponPart);
                    //drawPos.position += new Vector2(this.iconOffset.x * rect.size.x, this.iconOffset.y * rect.size.y);
                    drawPos.position -= new Vector2(offset.y, offset.x);

                    float scale = this.iconDrawScale * 0.85f;
                    scale *= IR_Settings.GetSize(weapon.parent.def.defName, attachment.weaponPart);

                    Widgets.DrawTextureFitted(rect, attachment.onWeaponGraphic.Graphic.MatSingle.mainTexture, scale, this.iconProportions, drawPos);
                }
            }*/
        }
    }
}