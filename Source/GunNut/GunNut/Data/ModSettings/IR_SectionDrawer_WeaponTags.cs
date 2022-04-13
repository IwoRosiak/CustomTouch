using CustomTouch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_SectionDrawer_WeaponTags : IR_SectionDrawer
    {
        public IR_SectionDrawer_WeaponTags(IR_SectionDrawer_Coordinator _parent) : base(_parent)
        {
        }

        public override void Draw(Rect rect)
        {
            DrawWeaponsTagsChoiceButtons(rect);
        }

        private Vector2 scrollPosActiveTags = new Vector2();
        private Vector2 scrollPosUnactiveTags = new Vector2();
        private void DrawWeaponsTagsChoiceButtons(Rect rect)
        {
            int pos = 0;


            Rect activeTagsRect = new Rect(rect.x, rect.y + buttonHeight, mediumButtonWidth, IR_Settings.GetWeaponTags(parent.CurWeapon.defName).Count * buttonHeight);
            Rect activeTagsScrollRect = new Rect(rect.x, rect.y + buttonHeight, mediumButtonWidth + sliderWidth, rect.height - buttonHeight - 12);


            Widgets.LabelFit(new Rect(rect.x + "Active Tags".Length * 2, rect.y, mediumButtonWidth, buttonHeight), "Active Tags");
            Widgets.LabelFit(new Rect(rect.x + mediumButtonWidth + sliderWidth + (int)("Unactive Tags".Length * 1.5f), rect.y, mediumButtonWidth, buttonHeight), "Unactive Tags");
            Widgets.DrawLineHorizontal(rect.x, rect.y + (buttonHeight * 0.8f), mediumButtonWidth);
            Widgets.DrawLineHorizontal(rect.x + mediumButtonWidth + sliderWidth, rect.y + (buttonHeight * 0.8f), mediumButtonWidth);

            Widgets.BeginScrollView(activeTagsScrollRect, ref scrollPosActiveTags, activeTagsRect);
            foreach (var tag in IR_Settings.GetWeaponTags(parent.CurWeapon.defName))
            {
                Rect tagRect = new Rect(activeTagsRect.x, activeTagsRect.y + (pos * buttonHeight), mediumButtonWidth, buttonHeight);

                if (Widgets.ButtonText(tagRect, tag.label.CapitalizeFirst()))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    IR_Settings.RemoveWeaponTag(parent.CurWeapon.defName, tag);
                    break;
                }

                pos += 1;
            }
            Widgets.EndScrollView();

            Rect unactiveTagsRect = new Rect(rect.x + mediumButtonWidth + sliderWidth, rect.y + buttonHeight, mediumButtonWidth, (GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(IR_AttachmentTag)).EnumerableCount() - IR_Settings.GetWeaponTags(parent.CurWeapon.defName).Count) * buttonHeight);
            Rect unactiveTagsScrollRect = new Rect(rect.x + mediumButtonWidth + sliderWidth, rect.y + buttonHeight, mediumButtonWidth + sliderWidth, rect.height - buttonHeight - 12);

            Widgets.BeginScrollView(unactiveTagsScrollRect, ref scrollPosUnactiveTags, unactiveTagsRect);

            pos = 0;
            foreach (IR_AttachmentTag tag in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(IR_AttachmentTag)))
            {
                if (IR_Settings.GetWeaponTags(parent.CurWeapon.defName).Contains(tag))
                {
                    continue;
                }

                Rect tagRect = new Rect(unactiveTagsRect.x, unactiveTagsRect.y + (pos * buttonHeight), mediumButtonWidth, buttonHeight);

                if (Widgets.ButtonText(tagRect, tag.label.CapitalizeFirst()))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    IR_Settings.AddWeaponTag(parent.CurWeapon.defName, tag);
                    break;
                }

                pos += 1;
            }

            Widgets.EndScrollView();
        }
    }
}
