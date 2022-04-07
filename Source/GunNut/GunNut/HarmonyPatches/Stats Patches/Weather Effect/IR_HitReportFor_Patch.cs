using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.AccessTools;

namespace GunNut.HarmonyPatches.Stats_Patches
{
    
    [HarmonyPatch(typeof(ShotReport), "HitReportFor")]
    public static class IR_HitReportFor_Patch
    {
        //public 
        private static readonly FieldInfo weatherEffect = AccessTools.Field(typeof(ShotReport), "factorFromWeather");
        //private static readonly FieldInfo smokeEffect = AccessTools.Field(typeof(ShotReport), "factorFromWeather");
        private static readonly FieldInfo darknessEffect = AccessTools.Field(typeof(ShotReport), "offsetFromDarkness");
        private static readonly FieldInfo bodySizeEffect = AccessTools.Field(typeof(ShotReport), "factorFromTargetSize");

        [HarmonyPostfix]
        public static void WeatherOffsetPostfix(Thing caster, Verb verb, LocalTargetInfo target, ref ShotReport __result)
        {   
            GN_AttachmentComp comp =  verb.EquipmentSource.TryGetComp<GN_AttachmentComp>();

            if (comp == null)
                return;

            float nv = 0;
            float smallSizeEffect = 0;
            float infrav = 0;

            foreach (var item in comp.ActiveAttachments)
            {
                infrav += item.infravision;
                smallSizeEffect += item.zoomvision;
                nv += item.nightvision;
            }

            float weatherInfluence = (nv * 0.25f) + (infrav * 0.75f);
            float darknessInfluence = (nv * 0.75f) + (infrav * 0.25f);
            
            if (smallSizeEffect != 0)
            {
                ChangeSizeOffset(ref __result, smallSizeEffect);
            }

            if (weatherInfluence!= 0)
            {
                ChangeWeatherOffset(ref __result, weatherInfluence);
            }
        }

        public static void ChangeDarknessOffset(ref ShotReport __result, float nv)
        {
            float newDarknessEffect = (float)darknessEffect.GetValue(__result);

            newDarknessEffect = 1 - newDarknessEffect;

            AccessTools.StructFieldRefAccess<ShotReport, float>(ref __result, darknessEffect) = 1 - newDarknessEffect * nv;
        }

        public static void ChangeWeatherOffset(ref ShotReport __result, float nv)
        {
            float newWeatherEffect = (float)weatherEffect.GetValue(__result);

            newWeatherEffect = 1 - newWeatherEffect;

            newWeatherEffect -= newWeatherEffect * nv;

            AccessTools.StructFieldRefAccess<ShotReport, float>(ref __result, weatherEffect) = 1 - newWeatherEffect;
        }

        public static void ChangeSizeOffset(ref ShotReport __result, float zoom)
        {
            float newBodySizeEffect = (float)bodySizeEffect.GetValue(__result);

            if (newBodySizeEffect >= 1)
            {
                return;
            }

            newBodySizeEffect = 1 - newBodySizeEffect;

            newBodySizeEffect -= newBodySizeEffect * zoom;

            AccessTools.StructFieldRefAccess<ShotReport, float>(ref __result, bodySizeEffect) = 1 - newBodySizeEffect;
        }

    }
}
