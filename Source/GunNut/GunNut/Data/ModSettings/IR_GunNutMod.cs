using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_GunNutMod : Mod
    {
        private IR_Settings settings;

        private List<ThingDef> weapons = new List<ThingDef>();
        private ThingDef curWeapon;
        private List<ThingDef> incompatibleWeapons = new List<ThingDef>();

        private Dictionary<GN_WeaponParts.WeaponPart, List<GN_AttachmentDef>> attachmentsLists = new Dictionary<GN_WeaponParts.WeaponPart,List<GN_AttachmentDef>>();
        private Dictionary<GN_WeaponParts.WeaponPart, int> attachmentsListsCurIndex = new Dictionary<GN_WeaponParts.WeaponPart,int>();

        private int buttonHeight = 40;
        private int buttonWidth = 180;
        private int smallButtonWidth = 90;
        private int tinyButtonWidth = 45;
        private int weaponScale = 3;
        private int pixelRatio = 96;

        public IR_GunNutMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<IR_Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            //Init
            if (weapons.NullOrEmpty() || attachmentsLists.NullOrEmpty())
            {
                LoadCompatibleWeapons();
                LoadAttachments();
            }

            Rect masterRect = inRect;

            //Weapons choice

            Rect weaponsButtonsRect = new Rect(masterRect.x, masterRect.y, masterRect.width , masterRect.height*0.5f);
            DrawWeaponChoiceButtons(weaponsButtonsRect);

            //Draw weapons
            Rect weaponRect = new Rect(masterRect.x, masterRect.y + masterRect.height * 0.5f, masterRect.width * 0.7f, masterRect.height * 0.5f);

            if (curWeapon != null)
            {
                DrawWeapon(weaponRect);

                //Draw attachments

                if (Widgets.ButtonText(new Rect(weaponRect.x +250, weaponRect.y - buttonHeight, smallButtonWidth, buttonHeight), "Reset weapon"))
                {
                    IR_Settings.Reset(curWeapon);
                    return;
                }
                DrawAttachments(weaponRect);
                DrawAttachmentsPosButtons(weaponRect);
                DrawAttachmentsSizeButtons(weaponRect);

                Rect weaponTagsRect = new Rect(masterRect.x + masterRect.width * 0.5f, masterRect.y , masterRect.width * 0.5f, masterRect.height * 0.5f);

                DrawWeaponsTagsChoiceButtons(weaponTagsRect);

                Rect attachmentChoiceRect = new Rect(masterRect.x + masterRect.width * 0.7f, masterRect.y + masterRect.height * 0.5f, masterRect.width * 0.3f, masterRect.height * 0.5f);

                DrawAttachmentsChoiceButtons(attachmentChoiceRect);
            }

        }         
        
        private void DrawWeapon(Rect rect)
        {
            Texture text = curWeapon.graphic.MatNorth.mainTexture;
            Widgets.DrawTextureRotated(rect.center, text, 0, curWeapon.graphic.drawSize.x * weaponScale);
        }

        private void DrawAttachments(Rect rect)
        {
            if (curWeapon != null)
            {
                foreach (var attachmentType in attachmentsLists.Keys)
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

        private void DrawAttachmentsPosButtons(Rect rect)
        {
            Widgets.TextField(new Rect(rect.x, rect.y, buttonWidth, buttonHeight), IR_Settings.GetPos(curWeapon.defName, curAttachmentType).ToString());
            Widgets.TextField(new Rect(rect.x, rect.y+buttonHeight, buttonWidth, buttonHeight), IR_Settings.WeaponsCustomInfo.ContainsKey(curWeapon.defName).ToString());

            Rect westBtn = new Rect(rect.x, rect.y + rect.height/2 - buttonHeight/2, tinyButtonWidth, buttonHeight);
            Rect eastBtn = new Rect(rect.x + rect.width - tinyButtonWidth, rect.y + rect.height / 2 - buttonHeight / 2, tinyButtonWidth, buttonHeight);
            Rect northBtn = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y, tinyButtonWidth, buttonHeight);
            Rect southBtn = new Rect(rect.x + rect.width / 2 - tinyButtonWidth / 2, rect.y + rect.height - buttonHeight, tinyButtonWidth, buttonHeight);

            if (Widgets.ButtonText (westBtn,"-X"))
            {
                IR_Settings.NotifyChangeMade(curWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[curWeapon.defName].GetPosition(curAttachmentType);
                temp.y -= 0.05f;
                IR_Settings.WeaponsCustomInfo[curWeapon.defName].SetPosition(curAttachmentType, temp);
            }
            if (Widgets.ButtonText(eastBtn, "+X"))
            {
                IR_Settings.NotifyChangeMade(curWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[curWeapon.defName].GetPosition(curAttachmentType);
                temp.y += 0.05f;
                IR_Settings.WeaponsCustomInfo[curWeapon.defName].SetPosition(curAttachmentType, temp);
            }
            if (Widgets.ButtonText(northBtn, "+Y"))
            {
                IR_Settings.NotifyChangeMade(curWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[curWeapon.defName].GetPosition(curAttachmentType);
                temp.x += 0.05f;
                IR_Settings.WeaponsCustomInfo[curWeapon.defName].SetPosition(curAttachmentType, temp);
            }
            if (Widgets.ButtonText(southBtn, "-Y"))
            {
                IR_Settings.NotifyChangeMade(curWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[curWeapon.defName].GetPosition(curAttachmentType);
                temp.x -= 0.05f;
                IR_Settings.WeaponsCustomInfo[curWeapon.defName].SetPosition(curAttachmentType, temp);
            }
        }

        private void DrawAttachmentsSizeButtons(Rect rect)
        {
            Rect sizeMinusBtn = new Rect(rect.x, rect.y + rect.height- buttonHeight , smallButtonWidth, buttonHeight);
            Rect sizePlusBtn = new Rect(rect.x + smallButtonWidth, rect.y + rect.height - buttonHeight, smallButtonWidth, buttonHeight);

            if (Widgets.ButtonText(sizeMinusBtn, "Size -"))
            {
                IR_Settings.NotifyChangeMade(curWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[curWeapon.defName].size[curAttachmentType];
                temp -= 0.05f;
                IR_Settings.WeaponsCustomInfo[curWeapon.defName].size[curAttachmentType] = temp;
            }
            if (Widgets.ButtonText(sizePlusBtn, "Size +"))
            {
                IR_Settings.NotifyChangeMade(curWeapon);
                var temp = IR_Settings.WeaponsCustomInfo[curWeapon.defName].size[curAttachmentType];
                temp += 0.05f;
                IR_Settings.WeaponsCustomInfo[curWeapon.defName].size[curAttachmentType] = temp;
            }
        }

        private GN_WeaponParts.WeaponPart curAttachmentType;
        private Vector2 scrollAttachments = new Vector2();
        private void DrawAttachmentsChoiceButtons(Rect rect)
        {
            Rect viewRect = new Rect(rect.x, rect.y, buttonWidth, buttonHeight * attachmentsLists.Keys.Count);
            Rect scrollRect = new Rect(rect.x, rect.y, smallButtonWidth + tinyButtonWidth + 20, rect.height);

            float x = rect.x;
            float y = rect.y;

            

            Widgets.BeginScrollView(scrollRect, ref scrollAttachments, viewRect);
            foreach (var attachmentType in attachmentsLists.Keys)
            {
                if (Widgets.ButtonText(new Rect(x, y, smallButtonWidth, buttonHeight), attachmentType.ToString()))
                {
                    curAttachmentType = attachmentType;
                }

                if (Widgets.ButtonText(new Rect(x+ smallButtonWidth, y, smallButtonWidth/2, buttonHeight), IR_Settings.IsActive(curWeapon.defName, attachmentType).ToString()))
                {
                    IR_Settings.NotifyChangeMade(curWeapon);
                    bool temp = IR_Settings.IsActive(curWeapon.defName, attachmentType);
                    temp = !temp;
                    IR_Settings.WeaponsCustomInfo[curWeapon.defName].isEnabled[attachmentType] = temp;
                }

                y += buttonHeight;
            }
            Widgets.EndScrollView();
        }
        
        private Vector2 scrollWeapons = new Vector2();
        bool displayCustom = true;
        bool displayReady = true;
        bool displayNotReady = true;
        private void DrawWeaponChoiceButtons(Rect rect)
        {
            Rect viewRect = new Rect(rect.x, rect.y, buttonWidth, buttonHeight* weapons.Count);
            Rect scrollRect = new Rect(rect.x, rect.y, buttonWidth+20, rect.height);

            float x = rect.x;
            float y = rect.y;

            //FILTERS
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + 20, rect.y, 200, buttonHeight),"Display custom", ref displayCustom);
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + 20, rect.y + buttonHeight, 200, buttonHeight), "Display ready", ref displayReady);
            Widgets.CheckboxLabeled(new Rect(rect.x + buttonWidth + 20, rect.y + buttonHeight*2, 200, buttonHeight), "Display not ready", ref displayNotReady);

            Widgets.BeginScrollView(scrollRect, ref scrollWeapons, viewRect);
            foreach (var weapon in weapons)
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
                    curWeapon = weapon;
                }

                y += buttonHeight;
            }

            Widgets.EndScrollView();
        }

        private void DrawWeaponsTagsChoiceButtons(Rect rect)
        {
            int pos = 0;

            foreach (var tag in IR_Settings.GetWeaponTags(curWeapon.defName))
            {
                Rect tagRect = new Rect(rect.x, rect.y + (pos * buttonHeight), buttonWidth, buttonHeight);

                if (Widgets.ButtonText(tagRect, tag.ToString()))
                {
                    IR_Settings.NotifyChangeMade(curWeapon);
                    IR_Settings.RemoveWeaponTag(curWeapon.defName, tag);
                    break;
                }

                pos+= 1;
            }

            pos = 0;
            foreach (WeaponTags tag in Enum.GetValues(typeof(WeaponTags)))
            {
                if (IR_Settings.GetWeaponTags(curWeapon.defName).Contains(tag))
                {
                    continue;
                }

                Rect tagRect = new Rect(rect.x + buttonWidth, rect.y + (pos * buttonHeight), buttonWidth, buttonHeight);

                if (Widgets.ButtonText(tagRect, tag.ToString()))
                {
                    IR_Settings.NotifyChangeMade(curWeapon);
                    IR_Settings.AddWeaponTag(curWeapon.defName, tag);
                    break;
                }

                pos += 1;
            }
            Widgets.DrawBox(rect);
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
            } else if (IR_Settings.WeaponsDefaultInfo.ContainsKey(weapon.defName))
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

        private void LoadAttachments()
        {
            attachmentsLists.Clear();

            foreach (GN_AttachmentDef attachmentDef in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(GN_AttachmentDef)))
            {
                if (!attachmentsLists.ContainsKey(attachmentDef.weaponPart))
                {
                    attachmentsLists.Add(attachmentDef.weaponPart, new List<GN_AttachmentDef>());
                }

                attachmentsLists[attachmentDef.weaponPart].Add(attachmentDef);

                curAttachmentType = attachmentDef.weaponPart;
            }
        }

        private void LoadCompatibleWeapons()
        {
            weapons.Clear();

            foreach (ThingDef thingDef in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(ThingDef)))
            {
                if (IR_Settings.WeaponsDefaultInfo.ContainsKey(thingDef.defName) && thingDef.HasComp(typeof(GN_AttachmentComp)))
                {
                    weapons.Add(thingDef);
                }
            }
            
        }

        public override string SettingsCategory()
        {
            return "Gun Nut Alpha v0.3.0";
        }

    }
}