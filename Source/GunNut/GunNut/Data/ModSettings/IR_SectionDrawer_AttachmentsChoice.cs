using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_SectionDrawer_AttachmentsChoice : IR_SectionDrawer
    {
        public IR_SectionDrawer_AttachmentsChoice(IR_SectionDrawer_Coordinator _parent) : base(_parent)
        {
        }

        public override void Draw(Rect rect)
        {
            DrawAttachmentsChoiceButtons(rect);
        }





        private Vector2 scrollAttachments = new Vector2();

        public void DrawAttachmentsChoiceButtons(Rect rect)
        {
            Widgets.LabelFit(new Rect(rect.x, rect.y, rect.width, buttonHeight), "Attachments Choice");
            Widgets.DrawLineHorizontal(rect.x, rect.y + (buttonHeight * 0.8f), smallButtonWidth + tinyButtonWidth);

            Rect viewRect = new Rect(rect.x, rect.y + buttonHeight, smallButtonWidth + tinyButtonWidth*2, buttonHeight * (parent.attachmentsLists.Keys.Count));
            Rect scrollRect = new Rect(rect.x, rect.y + buttonHeight, smallButtonWidth + tinyButtonWidth*2 + sliderWidth, rect.height - buttonHeight);

            float x = rect.x;
            float y = rect.y + buttonHeight;

            Widgets.BeginScrollView(scrollRect, ref scrollAttachments, viewRect);
            foreach (var attachmentType in parent.attachmentsLists.Keys)
            {
                if (Widgets.ButtonText(new Rect(x, y, smallButtonWidth, buttonHeight), attachmentType.ToString()))
                {
                    parent.curAttachmentType = attachmentType;
                    parent.isEditingMasks = false;
                }

                if (Widgets.ButtonText(new Rect(x + smallButtonWidth, y, smallButtonWidth / 2, buttonHeight), IR_Settings.IsActive(parent.CurWeapon.defName, attachmentType).ToString()))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    bool temp = IR_Settings.IsActive(parent.CurWeapon.defName, attachmentType);
                    temp = !temp;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].isEnabled[attachmentType] = temp;
                }

                if (Widgets.ButtonText(new Rect(x + smallButtonWidth + smallButtonWidth / 2, y, smallButtonWidth / 2, buttonHeight), "Mask"))
                {
                    parent.curAttachmentType = attachmentType;
                    parent.isEditingMasks = true;
                }
                y += buttonHeight;

            }
            Widgets.EndScrollView();
        }
    }
}
