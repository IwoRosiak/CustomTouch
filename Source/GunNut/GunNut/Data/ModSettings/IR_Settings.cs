using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GunNut
{
    public class IR_Settings : ModSettings
    {
        public IR_Settings()
        {
        }

        public static Dictionary<string, IR_SlotsCords> WeaponsCustomInfo;
        public static Dictionary<string, IR_SlotsCords> WeaponsDefaultInfo;

        public static bool isFirstLaunch;

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref WeaponsCustomInfo, "WeaponsCustomInfo8",LookMode.Value, LookMode.Deep);
            Scribe_Values.Look(ref isFirstLaunch, "isFirstLaunch", true);

            base.ExposeData();
        }

        public static Vector2 GetPos(ThingDef thing, GN_WeaponParts.WeaponPart part)
        {
            if (WeaponsCustomInfo.ContainsKey(thing.defName))
            {
                return WeaponsCustomInfo[thing.defName].GetPosition(part);
            } 

            return WeaponsDefaultInfo[thing.defName].GetPosition(part);
        }

        public static Vector2 GetPos(string name, GN_WeaponParts.WeaponPart part)
        {
            if (WeaponsCustomInfo.ContainsKey(name))
            {
                return WeaponsCustomInfo[name].GetPosition(part);
            }

            return WeaponsDefaultInfo[name].GetPosition(part);
        }

        public static float GetSize(string name, GN_WeaponParts.WeaponPart part)
        {
            if (WeaponsCustomInfo.ContainsKey(name))
            {
                return WeaponsCustomInfo[name].size[part];
            }

            return WeaponsDefaultInfo[name].size[part];
        }

        public static bool IsActive(string name, GN_WeaponParts.WeaponPart part)
        {
            if (WeaponsCustomInfo.ContainsKey(name))
            {
                return WeaponsCustomInfo[name].isEnabled[part];
            }

            return WeaponsDefaultInfo[name].isEnabled[part];
        }

        public static List<WeaponTags> GetWeaponTags(string name)
        {
            if (WeaponsCustomInfo.ContainsKey(name))
            {
                return WeaponsCustomInfo[name].weaponTags;
            }

            return WeaponsDefaultInfo[name].weaponTags;
        }

        public static void RemoveWeaponTag(string name, WeaponTags tag)
        {
            if (WeaponsCustomInfo.ContainsKey(name))
            {
                WeaponsCustomInfo[name].weaponTags.Remove(tag);
            }
        }

        public static void AddWeaponTag(string name, WeaponTags tag)
        {
            if (WeaponsCustomInfo.ContainsKey(name))
            {
                WeaponsCustomInfo[name].weaponTags.Add(tag);
            }
        }

        public static void NotifyChangeMade(ThingDef thing)
        {
            if (!WeaponsCustomInfo.ContainsKey(thing.defName))
            {
                IR_SlotsCords newSlot = new IR_SlotsCords();
                newSlot.position = new Dictionary<GN_WeaponParts.WeaponPart, Vector2>(WeaponsDefaultInfo[thing.defName].position);
                newSlot.size = new Dictionary<GN_WeaponParts.WeaponPart, float> ( WeaponsDefaultInfo[thing.defName].size);
                newSlot.isEnabled = new Dictionary<GN_WeaponParts.WeaponPart, bool>(WeaponsDefaultInfo[thing.defName].isEnabled);
                newSlot.weaponTags = new List<WeaponTags>(WeaponsDefaultInfo[thing.defName].weaponTags);

                WeaponsCustomInfo.Add(thing.defName, newSlot);
            }
        }
        public static void Reset(ThingDef thing)
        {
            //if (WeaponsCustomInfo.ContainsKey(thing.defName))
            //{
                WeaponsCustomInfo.Remove(thing.defName);
            //}
        }

        public static void Initialise()
        {
            if (WeaponsCustomInfo.NullOrEmpty())
            {
                WeaponsCustomInfo = new Dictionary<string, IR_SlotsCords>();
            }

            WeaponsDefaultInfo = new Dictionary<string, IR_SlotsCords>();

            foreach (ThingDef thingDef in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(ThingDef)))
            {
                if (thingDef.HasComp(typeof(GN_AttachmentComp)))
                {
                    GN_AttachmentCompProperties compProp = thingDef.GetCompProperties<GN_AttachmentCompProperties>();

                    IR_SlotsCords slotData = new IR_SlotsCords();

                    slotData.isEnabled = new Dictionary<GN_WeaponParts.WeaponPart, bool>();
                    slotData.position = new Dictionary<GN_WeaponParts.WeaponPart, Vector2>();
                    slotData.size = new Dictionary<GN_WeaponParts.WeaponPart, float>();
                    slotData.weaponTags = new List<WeaponTags>(compProp.tags);

                    Log.Message("Adding " + thingDef.defName);

                    foreach (GN_WeaponParts.WeaponPart weaponPart in Enum.GetValues(typeof(GN_WeaponParts.WeaponPart)))
                    {
                        if (compProp.ContainsTypeOfSlot(weaponPart))
                        {
                            GN_Slot slot = compProp.GetSlotOfType(weaponPart);
                            slotData.isEnabled.Add(slot.weaponPart, slot.isActive);
                            slotData.position.Add(slot.weaponPart, slot.defaultPosition);
                            slotData.size.Add(slot.weaponPart, 1);
                        } else
                        {
                            slotData.isEnabled.Add(weaponPart, false);
                            slotData.position.Add(weaponPart, Vector3.zero);
                            slotData.size.Add(weaponPart, 1);
                        }
                    }

                    WeaponsDefaultInfo.Add(thingDef.defName, slotData);
                }
            }
        }


    }
}