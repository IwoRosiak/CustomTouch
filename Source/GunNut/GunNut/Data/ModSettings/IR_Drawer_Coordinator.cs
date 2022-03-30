using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut
{
    internal class IR_Drawer_Coordinator : IR_Drawer
    {
        private IR_Drawer_Attachments drawerAttachment;
        private IR_Drawer_Weapons drawerWeapons;
        private IR_Drawer_Table drawerTable;

        public List<ThingDef> weapons = new List<ThingDef>();
        private ThingDef curWeapon;

        public Dictionary<GN_WeaponParts.WeaponPart, List<GN_AttachmentDef>> attachmentsLists = new Dictionary<GN_WeaponParts.WeaponPart, List<GN_AttachmentDef>>();
        public GN_WeaponParts.WeaponPart curAttachmentType;

        public ThingDef CurWeapon
        {
            get { return curWeapon; }
            set { curWeapon = value; }
        }

        public IR_Drawer_Coordinator(IR_Drawer_Coordinator _parent) : base(_parent)
        {
            drawerAttachment = new IR_Drawer_Attachments(this);
            drawerWeapons = new IR_Drawer_Weapons(this);
            drawerTable = new IR_Drawer_Table(this);

            parent = this;

            LoadCompatibleWeapons();
            LoadAttachments();
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



            drawerWeapons.DrawWeaponChoiceButtons(weaponsButtonsRect);
            
            if (curWeapon != null)
            {
                drawerWeapons.DrawWeaponsTagsChoiceButtons(weaponTagsRect);

                drawerTable.DrawCraftingTable(weaponRect);

                drawerAttachment.DrawAttachmentsChoiceButtons(attachmentChoiceRect);
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
    }
}
