using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut.HarmonyPatches.Stats_Patches.Weather_Effect
{
    
    [HarmonyPatch(typeof(ShotReport), "GetTextReadout")]
    public static class IR_WeatherEffect_TextReadout_Patch
    {
        static readonly FieldInfo weatherEffect = AccessTools.Field(typeof(ShotReport), "factorFromWeather");
        static readonly FieldInfo targetField = AccessTools.Field(typeof(ShotReport), "target");


        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool isAtRightLoc = false;
            bool isClosed = false;

            foreach (var instruction in instructions)
            {
                if (!isClosed && isAtRightLoc && instruction.opcode == OpCodes.Pop)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, targetField);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, weatherEffect);
                    yield return CodeInstruction.Call(typeof(IR_WeatherEffect_TextReadout_Patch), nameof(GetStringReadout), new Type[] { typeof(TargetInfo), typeof(float) });
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StringBuilder), nameof(StringBuilder.AppendLine), new Type[] { typeof(string) }));
                    yield return new CodeInstruction(OpCodes.Pop);
                    isClosed = true;
                    continue;
                }

                if (instruction.opcode == OpCodes.Ldstr && instruction.OperandIs("Weather"))
                {
                    isAtRightLoc = true;
                }

                yield return instruction;
            }
        }

        public static string GetStringReadout(TargetInfo thing, float orgValue)
        {
            Map map = thing.Thing.Map;

            float modEffect = orgValue - map.weatherManager.CurWeatherAccuracyMultiplier;

            double newEffect = Math.Round(modEffect*100, 1);

            //modEffect = (float)Math.Ceiling(modEffect);

            string appResult="";

            if (modEffect != 0)// || modEffect <= -1)
            {
                appResult = "      Night-vision +" + newEffect.ToString() + "%";
            }

            return appResult;
        }
    }

}


