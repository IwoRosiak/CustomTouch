using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_Drawer_Table : IR_Drawer
    {
        public IR_Drawer_Table(IR_Drawer_Coordinator _parent) : base(_parent)
        {
        }

        public override void Draw(Rect rect)
        {
            
        }

        public void DrawCraftingTable(Rect rect)
        {
            Widgets.LabelFit(new Rect(rect.x, rect.y, buttonWidth * 2, buttonHeight), "Current weapon: " + parent.CurWeapon.label);
            //Widgets.LabelFit(new Rect(rect.x, rect.y + buttonHeight, buttonWidth * 2, buttonHeight), "Position: " + IR_Settings.GetPos(CurrentWeapon.defName, curAttachmentType).ToString());

            Widgets.DrawLineHorizontal(rect.x, rect.y + (buttonHeight * 0.8f), rect.width);

            Rect craftingTable = new Rect(rect.x, rect.y + buttonHeight, rect.width, rect.height - buttonHeight);
            DrawWeapon(craftingTable);

            DrawAttachments(craftingTable);
            DrawAttachmentsPosButtons(craftingTable );

            //Draw attachments

            if (Widgets.ButtonText(new Rect(craftingTable.x + craftingTable.width - smallButtonWidth, craftingTable.y, smallButtonWidth, buttonHeight), "Reset weapon"))
            {
                IR_Settings.Reset(parent.CurWeapon);
                return;
            }
        }

        public void DrawWeapon(Rect rect)
        {
            Texture text = parent.CurWeapon.graphic.MatNorth.mainTexture;
            Widgets.DrawTextureRotated(rect.center, text, 0, parent.CurWeapon.graphic.drawSize.x * weaponScale);
        }

        public void DrawAttachmentsSizeButtons(Rect rect)
        {
            Rect sizeMinusBtn = new Rect(rect.x, rect.y + rect.height - buttonHeight, smallButtonWidth, buttonHeight);
            Rect sizePlusBtn = new Rect(rect.x + smallButtonWidth, rect.y + rect.height - buttonHeight, smallButtonWidth, buttonHeight);

            if (Widgets.ButtonText(sizeMinusBtn, "Size -"))
            {
                IR_Settings.NotifyChangeMade(parent.CurWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].size[parent.curAttachmentType];
                temp -= 0.05f;
                IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].size[parent.curAttachmentType] = temp;
            }
            if (Widgets.ButtonText(sizePlusBtn, "Size +"))
            {
                IR_Settings.NotifyChangeMade(parent.CurWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].size[parent.curAttachmentType];
                temp += 0.05f;
                IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].size[parent.curAttachmentType] = temp;
            }
        }

        public void DrawAttachmentsPosButtons(Rect rect)
        {

            Rect westBtn = new Rect(rect.x, rect.y + rect.height / 2 - buttonHeight / 2, tinyButtonWidth, buttonHeight);
            Rect eastBtn = new Rect(rect.x + rect.width - tinyButtonWidth, rect.y + rect.height / 2 - buttonHeight / 2, tinyButtonWidth, buttonHeight);
            Rect northBtn = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y, tinyButtonWidth, buttonHeight);
            Rect southBtn = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y + rect.height - buttonHeight, tinyButtonWidth, buttonHeight);

            if (Widgets.ButtonText(westBtn, "-X"))
            {
                IR_Settings.NotifyChangeMade(parent.CurWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].GetPosition(parent.curAttachmentType);
                temp.y -= 0.05f;
                IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].SetPosition(parent.curAttachmentType, temp);
            }
            if (Widgets.ButtonText(eastBtn, "+X"))
            {
                IR_Settings.NotifyChangeMade(parent.CurWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].GetPosition(parent.curAttachmentType);
                temp.y += 0.05f;
                IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].SetPosition(parent.curAttachmentType, temp);
            }
            if (Widgets.ButtonText(northBtn, "+Y"))
            {
                IR_Settings.NotifyChangeMade(parent.CurWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].GetPosition(parent.curAttachmentType);
                temp.x += 0.05f;
                IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].SetPosition(parent.curAttachmentType, temp);
            }
            if (Widgets.ButtonText(southBtn, "-Y"))
            {
                IR_Settings.NotifyChangeMade(parent.CurWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].GetPosition(parent.curAttachmentType);
                temp.x -= 0.05f;
                IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].SetPosition(parent.curAttachmentType, temp);
            }
        }

        public void DrawAttachments(Rect rect)
        {
            foreach (var attachmentType in parent.attachmentsLists.Keys)
            {
                /*
                if (!IR_Settings.IsActive(curWeapon.defName, attachmentType) || attachmentsLists[attachmentType][0].Equals(null) || attachmentsLists[attachmentType][0].onWeaponGraphic.Equals(null))
                {
                    continue;
                }

                Vector2 drawLoc = rect.center;
                Vector2 offset = IR_Settings.GetPos(curWeapon.defName, attachmentType) * pixelRatio;

                drawLoc += new Vector2 (offset.y, -offset.x) *2;

                Texture text = attachmentsLists[attachmentType][0].onWeaponGraphic.Graphic.MatNorth.mainTexture;
                float scale = attachmentsLists[attachmentType][0].onWeaponGraphic.Graphic.drawSize.x;

                Widgets.DrawTextureRotated(drawLoc, text, 0, (scale * weaponScale) * IR_Settings.GetSize(curWeapon.defName, attachmentType));
                */
            }

        }
    }
}
