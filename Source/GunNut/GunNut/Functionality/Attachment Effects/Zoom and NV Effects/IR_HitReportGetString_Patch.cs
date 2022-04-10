using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut.HarmonyPatches.Stats_Patches.Weather_Effect
{
    /*
    [HarmonyPatch(typeof(ShotReport), "GetTextReadout")]
    public static class IR_HitReportGetString_Patch
    {
        private static readonly FieldInfo targetInfo = AccessTools.Field(typeof(ShotReport), "target");

        [HarmonyPostfix]
        public static void WeatherOffsetPostfix( ShotReport __instance, ref string __result)
        {
            if (__result.Contains("Weather"))
            {
                TargetInfo thing = (TargetInfo)targetInfo.GetValue(__instance);
                Map map = thing.Thing.Map;

                int index = __result.IndexOf("Weather");
                index += "Weather         ".Count();

                string value = __result.Substring(index, 2);
                int effectRedFromAttach = int.Parse(value);

                float modEffect = effectRedFromAttach - (float)Math.Floor(map.weatherManager.CurWeatherAccuracyMultiplier*100) ;

                modEffect = (float)Math.Ceiling(modEffect);

                if (modEffect >=1 || modEffect<=-1)
                {
                    string replaceWith = "Weather".Translate() + "         " + value+"%" + "\n     Night-vision +" + modEffect.ToString()+"%";

                    __result = __result.ReplaceFirst("Weather" + "         " + value + "%", replaceWith);
                }                
            }



            if (__result.Contains("Target size"))
            {
                TargetInfo thing = (TargetInfo)targetInfo.GetValue(__instance);

                Pawn pawn = thing.Thing as Pawn;

                if (pawn == null)
                {
                    return;
                }

                float bodySize = Mathf.Clamp(pawn.BodySize, 0.5f, 2f); ;

                int index = __result.IndexOf("Target size");
                index += "Target size       ".Count();

                string value = __result.Substring(index, 2);
                int effectRedFromAttach = int.Parse(value);

                float modEffect = effectRedFromAttach - (bodySize * 100);

                if (modEffect >= 1 || modEffect <= -1)
                {
                    string replaceWith = "TargetSize".Translate() + "       " + value + "%" + "\n     Zoom +" + modEffect.ToString() + "%";

                    __result = __result.ReplaceFirst("Target size" + "       " + value + "%", replaceWith);
                }
            }
        }
    }*/
}
