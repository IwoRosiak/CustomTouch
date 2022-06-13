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
    internal class IR_SectionDrawer_PatchCreator : IR_SectionDrawer
    {
        public IR_SectionDrawer_PatchCreator(IR_SectionDrawer_Coordinator _parent) : base(_parent)
        {
        }

        public override void Draw(Rect rect)
        {

        }


        public string GetModXPatch()
        {
            StringBuilder patch = new StringBuilder();

            patch.AppendLine("<Patch>");
            patch.AppendLine("<Operation Class=\"PatchOperationFindMod\">");
            patch.AppendLine("<mods>");
            patch.AppendLine("<li>" + parent.curMod.Name + "</li>");
            patch.AppendLine("</mods>");
            patch.AppendLine("<match Class=\"PatchOperationSequence\">");
            patch.AppendLine("<operations>");
            foreach (var weapon in parent.weapons)
            {
                if (weapon.modContentPack.Equals(parent.curMod))
                {
                    if (IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
                    {
                        patch.AppendLine(GetWeaponXPatch(weapon));
                    }
                   
                }
            }
            patch.AppendLine("</operations>");
            patch.AppendLine("</match>");
            patch.AppendLine("</Operation>");
            patch.AppendLine("</Patch>");
            return patch.ToString();
        }

        public string GetWeaponXPatch(ThingDef weapon)
        {
            StringBuilder patch = new StringBuilder();

            patch.AppendLine(GetWeaponAttachmentInfoXPatch(weapon));
            patch.AppendLine(GetWeaponTagsXPatch(weapon));

            return patch.ToString();
        }

        private string GetWeaponAttachmentInfoXPatch(ThingDef weapon)
        {
            StringBuilder patch = new StringBuilder();
            string attachmentInfo = GetAttachmentInfoAsListForXPatch(weapon);

            patch.AppendLine("<li Class=\"PatchOperationAdd\">");
            patch.AppendLine("<xpath>/Defs/ThingDef[defName=\"" + weapon.defName + "\"]/comps/li[@Class=\"GunNut.GN_AttachmentCompProperties\"]</xpath>");
            patch.AppendLine("<value>");
            patch.AppendLine("<slots>");
            patch.AppendLine(attachmentInfo);
            patch.AppendLine("</slots>");
            patch.AppendLine("</value>");
            patch.AppendLine("</li>");

            return patch.ToString();
        }


        private string GetWeaponTagsXPatch(ThingDef weapon)
        {
            StringBuilder patch = new StringBuilder();

            patch.AppendLine("<li Class=\"PatchOperationAdd\">");
            patch.AppendLine("<xpath>/Defs/ThingDef[defName=\"" + weapon.defName + "\"]/comps/li[@Class=\"GunNut.GN_AttachmentCompProperties\"]</xpath>");
            patch.AppendLine("<value>");
            patch.AppendLine("<tags>");
            patch.AppendLine(GetWeaponTagsAsListForXPatch(weapon));
            patch.AppendLine("</tags>");
            patch.AppendLine("</value>");
            patch.AppendLine("</li>");

            return patch.ToString();
        }

        private string GetAttachmentInfoAsListForXPatch(ThingDef weapon)
        {
            StringBuilder tags = new StringBuilder();

            foreach (IR_AttachmentType part in   Enum.GetValues(typeof(IR_AttachmentType)))
            {
                if (!IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
                {
                    continue;
                }

                bool isActive = IR_Settings.WeaponsCustomInfo[weapon.defName].isEnabled[part];

                if (!isActive) continue;

                Vector2 pos = IR_Settings.GetPos(weapon.defName, part);

                Rect rect = IR_Settings.GetMask(weapon.defName, part);

                tags.AppendLine("<li>");
                tags.AppendLine("<weaponPart>" + part.ToString() + "</weaponPart>");
                tags.AppendLine("<defaultPosition>" + pos.ToString() + "</defaultPosition>");
                tags.AppendLine("<defaultMask>" + rect.ToString() + "</defaultMask>");
                tags.AppendLine("</li>");

                
            }
            return tags.ToString();
        }

        private string GetWeaponTagsAsListForXPatch(ThingDef weapon)
        {
            StringBuilder tags = new StringBuilder();

            foreach(IR_AttachmentTag tag in IR_Settings.GetWeaponTags(weapon.defName))
            {
                tags.AppendLine("<li>"+tag.defName +"</li>");
            }

            return tags.ToString();

        }
    }
}
