using System.Collections.Generic;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    public static class IR_Init
    {
        static IR_Init()
        {
            WeaponsCustomInfo = new Dictionary<string, IR_SlotsData>();

            foreach (ThingDef thingDef in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(ThingDef)))
            {
                if (thingDef.HasComp(typeof(GN_AttachmentComp)))
                {
                    GN_AttachmentCompProperties compProp = thingDef.GetCompProperties<GN_AttachmentCompProperties>();

                    IR_SlotsData slotData = new IR_SlotsData();

                    slotData.defaultActive = new Dictionary<GN_WeaponParts.WeaponPart, bool>();
                    slotData.defaultPlacement = new Dictionary<GN_WeaponParts.WeaponPart, UnityEngine.Vector2>();

                    Log.Message("Adding " + thingDef.defName);

                    foreach (var item in compProp.Slots)
                    {
                        slotData.defaultActive.Add(item.weaponPart, item.isActive);
                        slotData.defaultPlacement.Add(item.weaponPart, item.defaultPosition);
                    }

                    WeaponsCustomInfo.Add(thingDef.defName, slotData);
                }
            }
        }

        public static Dictionary<string, IR_SlotsData> WeaponsCustomInfo;
    }
}