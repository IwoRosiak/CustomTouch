using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_Drawer_Weapons : IR_Drawer
    {
        public IR_Drawer_Weapons(IR_Drawer_Coordinator _parent) : base(_parent)
        {
        }


        public override void Draw(Rect rect)
        {
            
        }

        private Vector2 scrollWeapons = new Vector2();
        bool displayCustom = true;
        bool  displayReady = true;
        bool displayNotReady = true;

        public void DrawWeaponChoiceButtons(Rect rect)
        {
            Widgets.LabelFit(new Rect(rect.x + "Weapon Choice".Length * 2, rect.y, rect.width, buttonHeight), "Weapon Choice");
            Widgets.DrawLineHorizontal(rect.x, rect.y + (buttonHeight * 0.8f), buttonWidth);

            Rect viewRect = new Rect(rect.x, rect.y + buttonHeight, buttonWidth, buttonHeight * parent.weapons.Count - buttonHeight);
            Rect scrollRect = new Rect(rect.x, rect.y + buttonHeight, buttonWidth + sliderWidth, rect.height - buttonHeight - 12);

            float x = rect.x;
            float y = rect.y;

            //FILTERS
            Widgets.LabelFit(new Rect(rect.x + buttonWidth + sliderWidth * 2 + "Weapon Filters".Length * 2, rect.y, buttonWidth, buttonHeight), "Weapon Filters");
            Widgets.DrawLineHorizontal(rect.x + buttonWidth + sliderWidth * 2, rect.y + (buttonHeight * 0.8f), buttonWidth);

            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight, buttonWidth, buttonHeight), "Custom", ref displayCustom);
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight * 2, buttonWidth, buttonHeight), "Adjusted", ref displayReady);
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight * 3, buttonWidth, buttonHeight), "Not adjusted", ref displayNotReady);

            Widgets.BeginScrollView(scrollRect, ref scrollWeapons, viewRect);
            foreach (var weapon in parent.weapons)
            {
                if (!displayCustom && IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
                {
                    continue;
                }

                if (!displayReady && IsWeaponReady(weapon))
                {
                    continue;
                }

                if (!displayNotReady && !(IsWeaponReady(weapon) || IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName)))
                {
                    continue;
                }

                //WEAPON TAG
                string weaponTag = " (X)";

                if (IsWeaponReady(weapon))
                {
                    weaponTag = " (R)";
                }

                if (IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
                {
                    weaponTag = " (C)";
                }

                if (Widgets.ButtonText(new Rect(x, y, buttonWidth, buttonHeight), weapon.label + weaponTag))
                {
                    parent.CurWeapon = weapon;
                }

                y += buttonHeight;
            }

            Widgets.EndScrollView();
        }



        private Vector2 scrollPosActiveTags = new Vector2();
        private Vector2 scrollPosUnactiveTags = new Vector2();


        public void DrawWeaponsTagsChoiceButtons(Rect rect)
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

                if (Widgets.ButtonText(tagRect, tag.ToString()))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    IR_Settings.RemoveWeaponTag(parent.CurWeapon.defName, tag);
                    break;
                }

                pos += 1;
            }
            Widgets.EndScrollView();

            Rect unactiveTagsRect = new Rect(rect.x + mediumButtonWidth + sliderWidth, rect.y + buttonHeight, mediumButtonWidth, (Enum.GetValues(typeof(WeaponTags)).EnumerableCount() - IR_Settings.GetWeaponTags(parent.CurWeapon.defName).Count) * buttonHeight);
            Rect unactiveTagsScrollRect = new Rect(rect.x + mediumButtonWidth + sliderWidth, rect.y + buttonHeight, mediumButtonWidth + sliderWidth, rect.height - buttonHeight - 12);

            Widgets.BeginScrollView(unactiveTagsScrollRect, ref scrollPosUnactiveTags, unactiveTagsRect);

            pos = 0;
            foreach (WeaponTags tag in Enum.GetValues(typeof(WeaponTags)))
            {
                if (IR_Settings.GetWeaponTags(parent.CurWeapon.defName).Contains(tag))
                {
                    continue;
                }

                Rect tagRect = new Rect(unactiveTagsRect.x, unactiveTagsRect.y + (pos * buttonHeight), mediumButtonWidth, buttonHeight);

                if (Widgets.ButtonText(tagRect, tag.ToString()))
                {
                    IR_Settings.NotifyChangeMade(parent.CurWeapon);
                    IR_Settings.AddWeaponTag(parent.CurWeapon.defName, tag);
                    break;
                }

                pos += 1;
            }

            Widgets.EndScrollView();
        }
        private bool IsWeaponReady(ThingDef weapon)
        {
            if (IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
            {
                foreach (var keyPair in IR_Settings.WeaponsCustomInfo[weapon.defName].isEnabled)
                {
                    if (keyPair.Value == true)
                    {
                        return true;
                    }
                }
            }
            else if (IR_Settings.WeaponsDefaultInfo.ContainsKey(weapon.defName))
            {
                foreach (var keyPair in IR_Settings.WeaponsDefaultInfo[weapon.defName].isEnabled)
                {
                    if (keyPair.Value == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
