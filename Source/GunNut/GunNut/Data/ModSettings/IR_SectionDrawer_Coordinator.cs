using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_SectionDrawer_Coordinator : IR_SectionDrawer
    {
        private IR_SectionDrawer_AttachmentsChoice drawerAttachment;
        private IR_SectionDrawer_Weapons drawerWeapons;
        private IR_SectionDrawer_WeaponTags drawerWeaponTags;
        private IR_SectionDrawer_Table drawerTable;

        private IR_SectionDrawer_PatchCreator patchCreator;

        public ModContentPack curMod;

        public List<ThingDef> weapons = new List<ThingDef>();
        private ThingDef curWeapon;

        public Dictionary<GN_WeaponParts.WeaponPart, List<GN_AttachmentDef>> attachmentsLists = new Dictionary<GN_WeaponParts.WeaponPart, List<GN_AttachmentDef>>();
        public GN_WeaponParts.WeaponPart curAttachmentType;

        public List<ModContentPack> loadedModsWithWeapons = new List<ModContentPack>();

        public ThingDef CurWeapon
        {
            get { return curWeapon; }
            set { curWeapon = value; }
        }

        public IR_SectionDrawer_Coordinator(IR_SectionDrawer_Coordinator _parent) : base(_parent)
        {
            drawerAttachment = new IR_SectionDrawer_AttachmentsChoice(this);
            drawerWeapons = new IR_SectionDrawer_Weapons(this);
            drawerTable = new IR_SectionDrawer_Table(this);
            drawerWeaponTags = new IR_SectionDrawer_WeaponTags(this);
            patchCreator= new IR_SectionDrawer_PatchCreator(this);

            parent = this;

            LoadCompatibleWeapons();
            LoadAttachments();
            LoadModsWithWeapons();
        }

        public override void Draw(Rect rect)
        {
            Rect masterRect = rect;

            Rect weaponsButtonsRect = new Rect(masterRect.x, masterRect.y, masterRect.width, masterRect.height * 0.5f);

            Rect weaponRect = new Rect(masterRect.x, masterRect.y + masterRect.height * 0.5f, masterRect.width * 0.6f, masterRect.height * 0.5f);

            float tagsWidthOffset = mediumButtonWidth * 2 + sliderWidth * 2;
            Rect weaponTagsRect = new Rect(masterRect.x + masterRect.width - tagsWidthOffset, masterRect.y, tagsWidthOffset, masterRect.height * 0.5f);

            float attachmentChoiceWidthOffset = smallButtonWidth + tinyButtonWidth + sliderWidth;
            Rect attachmentChoiceRect = new Rect(masterRect.x + masterRect.width - attachmentChoiceWidthOffset, masterRect.y + masterRect.height * 0.5f, attachmentChoiceWidthOffset, masterRect.height * 0.5f);


            drawerWeapons.Draw(weaponsButtonsRect);
            
            if (curWeapon != null)
            {
                if (Widgets.ButtonText(new Rect(weaponRect.x, weaponRect.y +50, buttonWidth, buttonHeight), "Get weapon patch")) 
                {
                    Log.Message(patchCreator.GetWeaponXPatch(curWeapon));
                }

                if (Widgets.ButtonText(new Rect(weaponRect.x, weaponRect.y + 100, buttonWidth, buttonHeight), "Get mod patch"))
                {
                    Log.Message(patchCreator.GetModXPatch());
                }

                drawerWeaponTags.Draw(weaponTagsRect);
                drawerTable.Draw(weaponRect);
                drawerAttachment.Draw(attachmentChoiceRect);
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

        private void LoadModsWithWeapons()
        {
            foreach (ModContentPack mod in LoadedModManager.RunningModsListForReading)
            {
                foreach (Def def in  mod.AllDefs)
                {
                    if (def.GetType() == typeof(ThingDef))
                    {
                        ThingDef thingDef = (ThingDef)def;

                        if (thingDef.IsRangedWeapon)
                        {
                            loadedModsWithWeapons.Add(mod);
                            break;
                        }
                    }
                }
            }
        }
    }
}
