using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    internal static class IR_Init
    {
        static IR_Init()
        {
            WeaponsCustomInfo = new Dictionary<ThingDef, IR_SlotsData>();

            foreach (ThingDef thingDef in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(GN_WeaponDef)))
            {
                if (thingDef.IsWeapon && thingDef.equipmentType == EquipmentType.Primary && thingDef.tradeability != Tradeability.None)
                {
                    WeaponsCustomInfo.Add(thingDef, new IR_SlotsData());
                }
            }
        }

        public static Dictionary<ThingDef, IR_SlotsData> WeaponsCustomInfo;
    }
}