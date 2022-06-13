using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_SectionDrawer_Table : IR_SectionDrawer
    {
        public IR_SectionDrawer_Table(IR_SectionDrawer_Coordinator _parent) : base(_parent)
        {
        }

        public override void Draw(Rect rect)
        {
            DrawCraftingTable(rect);
        }

        private void DrawCraftingTable(Rect rect)
        {
            Widgets.LabelFit(new Rect(rect.x, rect.y, buttonWidth * 2, buttonHeight), "Current weapon: " + parent.CurWeapon.label);
            Widgets.LabelFit(new Rect(rect.x + ("Current weapon: " + parent.CurWeapon.label).Length * 8, rect.y, buttonWidth * 2, buttonHeight), "Editing: " + parent.curAttachmentType + (parent.isEditingMasks ? "'s mask" : "").ToString());
            //Widgets.LabelFit(new Rect(rect.x, rect.y + buttonHeight, buttonWidth * 2, buttonHeight), "Position: " + IR_Settings.GetPos(CurrentWeapon.defName, curAttachmentType).ToString());

            Widgets.DrawLineHorizontal(rect.x, rect.y + (buttonHeight * 0.8f), rect.width);

            Rect craftingTable = new Rect(rect.x, rect.y + buttonHeight, rect.width, rect.height - buttonHeight);
            DrawWeapon(craftingTable);

            DrawAttachments(craftingTable);
            DrawAttachmentsPosButtons(craftingTable );
            DrawAttachmentsSizeButtons(craftingTable);

            //Draw attachments

            if (Widgets.ButtonText(new Rect(craftingTable.x + craftingTable.width - smallButtonWidth, craftingTable.y, smallButtonWidth, buttonHeight), "Reset weapon"))
            {
                IR_Settings.Reset(parent.CurWeapon);
                return;
            }
        }

        private void DrawWeapon(Rect rect)
        {
            Texture text = parent.CurWeapon.graphic.MatNorth.mainTexture;


            Texture mainTexture = text;

            int width = mainTexture.width;
            int width2 = mainTexture.width;
            Texture2D texture2D = new Texture2D(width, width2, TextureFormat.ARGB32, false);
            RenderTexture renderTexture = new RenderTexture(width, width2, 0, (RenderTextureFormat)0, 1);
            Graphics.Blit(mainTexture, renderTexture);
            RenderTexture.active = renderTexture;


            texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)width2), 0, 0, false);

            foreach (IR_AttachmentType weaponPart in Enum.GetValues(typeof(IR_AttachmentType)))
            {
                if (!IR_Settings.IsActive(parent.CurWeapon.defName, weaponPart))
                {
                    continue;
                }

                Rect mask1 = IR_Settings.GetMask(parent.CurWeapon.defName, weaponPart);


                bool drawBorder= false;
                if (weaponPart == parent.curAttachmentType)
                {
                    drawBorder = true;
                }

                for (int i = 0; i <= Mathf.FloorToInt(mask1.width); i++)
                {
                    for (int j = 0; j <= Mathf.FloorToInt(mask1.height); j++)
                    {
                        Color colorToFill = new Color(0, 0, 0, 0);

                        if (parent.isEditingMasks && drawBorder && (i == 0 || j ==0 || i == Mathf.FloorToInt(mask1.width) || j == Mathf.FloorToInt(mask1.height)))
                        {
                            colorToFill=new Color(255, 255, 255, 100);
                        } 

                        texture2D.SetPixel(Mathf.FloorToInt(mask1.x) + i, Mathf.FloorToInt(mask1.y) + j, colorToFill);

                    }
                }
            }


            texture2D.Apply();

            text = texture2D;

            RenderTexture.active = null;
            Widgets.DrawTextureRotated(rect.center, text, 0, parent.CurWeapon.graphic.drawSize.x * weaponScale);
        }

        private void DrawAttachmentsSizeButtons(Rect rect)
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

        private void DrawAttachmentsPosButtons(Rect rect)
        {

            Rect westBtn = new Rect(rect.x, rect.y + rect.height / 2 - buttonHeight / 2, tinyButtonWidth, buttonHeight);
            Rect eastBtn = new Rect(rect.x + rect.width - tinyButtonWidth, rect.y + rect.height / 2 - buttonHeight / 2, tinyButtonWidth, buttonHeight);
            Rect northBtn = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y, tinyButtonWidth, buttonHeight);
            Rect southBtn = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y + rect.height - buttonHeight, tinyButtonWidth, buttonHeight);

            Rect westBtnR = new Rect(rect.x+ tinyButtonWidth, rect.y + rect.height / 2 - buttonHeight / 2, tinyButtonWidth, buttonHeight);
            Rect eastBtnR = new Rect(rect.x + rect.width - tinyButtonWidth*2, rect.y + rect.height / 2 - buttonHeight / 2, tinyButtonWidth, buttonHeight);
            Rect northBtnR = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y+ buttonHeight, tinyButtonWidth, buttonHeight);
            Rect southBtnR = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y + rect.height - buttonHeight*2, tinyButtonWidth, buttonHeight);

            if (parent.isEditingMasks)
            {
                if (Widgets.ButtonText(westBtn, "-X"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.x -= 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }
                if (Widgets.ButtonText(eastBtn, "+X"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.x += 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }
                if (Widgets.ButtonText(northBtn, "+Y"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.y += 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }
                if (Widgets.ButtonText(southBtn, "-Y"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.y -= 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }

                if (Widgets.ButtonText(westBtnR, "-W"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.width -= 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }
                if (Widgets.ButtonText(eastBtnR, "+W"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.width += 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }
                if (Widgets.ButtonText(northBtnR, "+H"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.height += 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }
                if (Widgets.ButtonText(southBtnR, "-H"))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    Rect temp = IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType];
                    temp.height -= 1;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].masks[parent.curAttachmentType] = temp;
                }

            } else
            {
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

        }

        private void DrawAttachments(Rect rect)
        {
            foreach (var attachmentType in parent.attachmentsLists.Keys)
            {
                
                if (!IR_Settings.IsActive(parent.CurWeapon.defName, attachmentType) || parent.attachmentsLists[attachmentType][0].Equals(null) )//|| parent.attachmentsLists[attachmentType][0].onWeaponGraphic.Equals(null))
                {
                    continue;
                }

                Vector2 drawLoc = rect.center;
                Vector2 offset = IR_Settings.GetPos(parent.CurWeapon.defName, attachmentType) * 96;

                drawLoc += new Vector2 (offset.y, -offset.x) *2;

                //Texture text = parent.attachmentsLists[attachmentType][0].onWeaponGraphic.Graphic.MatNorth.mainTexture;
                //float scale = parent.attachmentsLists[attachmentType][0].onWeaponGraphic.Graphic.drawSize.x;

                //Widgets.DrawTextureRotated(drawLoc, text, 0, (scale * weaponScale) * IR_Settings.GetSize(parent.CurWeapon.defName, attachmentType));
                
            }

        }
    }
}
