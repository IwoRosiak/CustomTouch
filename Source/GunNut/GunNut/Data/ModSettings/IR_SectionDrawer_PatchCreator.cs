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

            patch.AppendLine("<Operation Class=\"PatchOperationReplace\">");
            patch.AppendLine("<xpath>/Defs/ThingDef[defName=\"" + weapon.defName + "\"]/comps/li[@Class=\"GunNut.GN_AttachmentCompProperties\"]</xpath>");
            patch.AppendLine("<value>");
            patch.AppendLine("<li Class=\"GunNut.GN_AttachmentCompProperties\">");
            patch.AppendLine("<jobDefInstall>EquipAttachment</jobDefInstall>");
            patch.AppendLine("<jobDefRemove>RemoveAttachment</jobDefRemove>");
            patch.AppendLine("<slots>");
            patch.AppendLine(attachmentInfo);
            patch.AppendLine("</slots>");
            patch.AppendLine("</li>");
            patch.AppendLine("</value>");
            patch.AppendLine("</Operation>");

            return patch.ToString();
        }


        private string GetWeaponTagsXPatch(ThingDef weapon)
        {
            StringBuilder patch = new StringBuilder();

            patch.AppendLine("<Operation Class=\"PatchOperationAdd\">");
            patch.AppendLine("<xpath>/Defs/ThingDef[defName=\"" + weapon.defName + "\"]/comps/li[@Class=\"GunNut.GN_AttachmentCompProperties\"]</xpath>");
            patch.AppendLine("<value>");
            patch.AppendLine("<tags>");
            patch.AppendLine(GetWeaponTagsAsListForXPatch(weapon));
            patch.AppendLine("</tags>");
            patch.AppendLine("</value>");
            patch.AppendLine("</Operation>");

            return patch.ToString();
        }

        private string GetAttachmentInfoAsListForXPatch(ThingDef weapon)
        {
            StringBuilder tags = new StringBuilder();

            foreach (GN_WeaponParts.WeaponPart part in   Enum.GetValues(typeof(GN_WeaponParts.WeaponPart)))
            {
                if (!IR_Settings.WeaponsCustomInfo.ContainsKey(weapon.defName))
                {
                    continue;
                }

                bool isActive = IR_Settings.WeaponsCustomInfo[weapon.defName].isEnabled[part];

                if (!isActive) continue;

                Vector2 pos = IR_Settings.GetPos(weapon.defName, part);



                tags.AppendLine("<li>");
                tags.AppendLine("<weaponPart>" + part.ToString() + "</weaponPart>");
                tags.AppendLine("<isActive>" + isActive + "</isActive>");
                tags.AppendLine("<defaultPosition>" + pos.ToString() + "</defaultPosition>");
                tags.AppendLine("</li>");

                
            }
            return tags.ToString();
        }

        private string GetWeaponTagsAsListForXPatch(ThingDef weapon)
        {
            StringBuilder tags = new StringBuilder();

            foreach(WeaponTags tag in IR_Settings.GetWeaponTags(weapon.defName))
            {
                tags.AppendLine("<li>"+tag.ToString() +"</li>");
            }

            return tags.ToString();

        }
    }
}
