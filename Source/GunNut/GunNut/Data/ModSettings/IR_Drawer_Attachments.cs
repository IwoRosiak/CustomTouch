using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_Drawer_Attachments : IR_Drawer
    {


        public override void Draw(Rect rect)
        {
        }

        private Vector2 scrollAttachments = new Vector2();

        public IR_Drawer_Attachments(IR_Drawer_Coordinator _parent) : base(_parent)
        {
        }

        public void DrawAttachmentsChoiceButtons(Rect rect)
        {
            Widgets.LabelFit(new Rect(rect.x, rect.y, rect.width, buttonHeight), "Attachments Choice");
            Widgets.DrawLineHorizontal(rect.x, rect.y + (buttonHeight * 0.8f), smallButtonWidth + tinyButtonWidth);

            Rect viewRect = new Rect(rect.x, rect.y + buttonHeight, smallButtonWidth + tinyButtonWidth, buttonHeight * parent.attachmentsLists.Keys.Count - buttonHeight);
            Rect scrollRect = new Rect(rect.x, rect.y + buttonHeight, smallButtonWidth + tinyButtonWidth + sliderWidth, rect.height - buttonHeight);

            float x = rect.x;
            float y = rect.y;

            Widgets.BeginScrollView(scrollRect, ref scrollAttachments, viewRect);
            foreach (var attachmentType in parent.attachmentsLists.Keys)
            {
                if (Widgets.ButtonText(new Rect(x, y, smallButtonWidth, buttonHeight), attachmentType.ToString()))
                {
                    parent.curAttachmentType = attachmentType;
                }

                if (Widgets.ButtonText(new Rect(x + smallButtonWidth, y, smallButtonWidth / 2, buttonHeight), IR_Settings.IsActive(parent.CurWeapon.defName, attachmentType).ToString()))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    bool temp = IR_Settings.IsActive(parent.CurWeapon.defName, attachmentType);
                    temp = !temp;
                    IR_Settings.WeaponsCustomInfo[parent.CurWeapon.defName].isEnabled[attachmentType] = temp;
                }

                y += buttonHeight;
            }
            Widgets.EndScrollView();
        }
    }
}
