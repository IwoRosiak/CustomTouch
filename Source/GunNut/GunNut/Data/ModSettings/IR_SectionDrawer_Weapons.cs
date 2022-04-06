using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_SectionDrawer_Weapons : IR_SectionDrawer
    {
        public IR_SectionDrawer_Weapons(IR_SectionDrawer_Coordinator _parent) : base(_parent)
        {
        }

        public override void Draw(Rect rect)
        {
            DrawWeaponChoiceButtons(rect);
            DrawFiltersOptions(rect);
        }

        Vector2 scrollWeapons = new Vector2();
        bool displayCustom = true;
        bool  displayReady = true;
        bool displayNotReady = true;

        bool filterByMod = false;

        private void DrawWeaponChoiceButtons(Rect rect)
        {
            Widgets.LabelFit(new Rect(rect.x + "Weapon Choice".Length * 2, rect.y, rect.width, buttonHeight), "Weapon Choice");
            Widgets.DrawLineHorizontal(rect.x, rect.y + (buttonHeight * 0.8f), buttonWidth);

            Rect viewRect = new Rect(rect.x, rect.y + buttonHeight, buttonWidth, buttonHeight * parent.weapons.Count - buttonHeight);
            Rect scrollRect = new Rect(rect.x, rect.y + buttonHeight, buttonWidth + sliderWidth, rect.height - buttonHeight - 12);
            
            Widgets.BeginScrollView(scrollRect, ref scrollWeapons, viewRect);
            DrawWeaponButtons(rect);
            Widgets.EndScrollView();
        }

        private void DrawWeaponButtons(Rect rect)
        {
            float y = rect.y;
            foreach (var weapon in parent.weapons)
            {
                if (!ShouldDisplay(weapon)) continue;


                if (Widgets.ButtonText(new Rect(rect.x, y, buttonWidth, buttonHeight), weapon.label + ChooseWeaponAppendix(weapon)))
                {
                    parent.CurWeapon = weapon;
                }

                y += buttonHeight;
            }
        }

        

        private int index= 0;

        private void DrawFiltersOptions(Rect rect)
        {
            Widgets.LabelFit(new Rect(rect.x + buttonWidth + sliderWidth * 2 + "Weapon Filters".Length * 2, rect.y, buttonWidth, buttonHeight), "Weapon Filters");
            Widgets.DrawLineHorizontal(rect.x + buttonWidth + sliderWidth * 2, rect.y + (buttonHeight * 0.8f), buttonWidth);

            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight, buttonWidth, buttonHeight), "Custom", ref displayCustom);
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight * 2, buttonWidth, buttonHeight), "Adjusted", ref displayReady);
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight * 3, buttonWidth, buttonHeight), "Not adjusted", ref displayNotReady);
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight * 4, buttonWidth, buttonHeight), "Filter by mod", ref filterByMod);

            parent.curMod = parent.loadedModsWithWeapons[index];

            if (Widgets.ButtonText(new Rect(rect.x + buttonWidth + sliderWidth * 2+40 + (buttonWidth-80), rect.y + buttonHeight * 5, 40, buttonHeight), ">"))
            {
                if (index < parent.loadedModsWithWeapons.Count-1)
                {
                    index++;
                }
            }

            if (Widgets.ButtonText( new Rect(rect.x + buttonWidth + sliderWidth * 2 +40, rect.y + buttonHeight * 5, buttonWidth -80, buttonHeight), parent.curMod.Name))
            {
                

                
            }

            if (Widgets.ButtonText(new Rect(rect.x + buttonWidth + sliderWidth * 2, rect.y + buttonHeight * 5, 40, buttonHeight), "<"))
            {
                if (index > 0)
                {
                    index--;
                }
            }

        }

        private bool ShouldDisplay(ThingDef weapon)
        {
            if (!displayCustom && IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
            {
                return false;
            }

            if (!displayReady && IsWeaponReady(weapon))
            {
                return false;
            }

            if (!displayNotReady && !(IsWeaponReady(weapon) || IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName)))
            {
                return false;
            }

            if (filterByMod && !parent.curMod.AllDefs.Contains(weapon))
            {
                return false;
            }

            return true;
        }

        private string ChooseWeaponAppendix(ThingDef weapon)
        {
            string weaponTag = " (X)";

            if (IsWeaponReady(weapon))
            {
                weaponTag = " (R)";
            }

            if (IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
            {
                weaponTag = " (C)";
            }

            return weaponTag;
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
