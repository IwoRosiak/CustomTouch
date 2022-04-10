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
    public static class IR_ZoomEffect_TextReadout_Patch
    {
        static readonly FieldInfo bodySizeEffect = AccessTools.Field(typeof(ShotReport), "factorFromTargetSize");
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
                    yield return new CodeInstruction(OpCodes.Ldfld, bodySizeEffect);
                    yield return CodeInstruction.Call(typeof(IR_ZoomEffect_TextReadout_Patch), nameof(GetStringReadout), new Type[] { typeof(TargetInfo), typeof(float) });
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StringBuilder), nameof(StringBuilder.AppendLine), new Type[] { typeof(string) }));
                    yield return new CodeInstruction(OpCodes.Pop);
                    isClosed = true;
                    continue;
                }

                if (instruction.opcode == OpCodes.Ldstr && instruction.OperandIs("TargetSize"))
                {
                    isAtRightLoc = true;
                }

                yield return instruction;
            }
        }

        public static string GetStringReadout(TargetInfo thing, float orgValue)
        {
            Pawn pawn;
            string appResult = "";

            if ((pawn = thing.Thing as Pawn) != null)
            {

                float modEffect = orgValue - Mathf.Clamp(pawn.BodySize, 0.5f, 2);

                double newEffect = Math.Round(modEffect*100, 1);

                if (modEffect != 0)
                {
                    appResult = "      Zoom +" + newEffect.ToString() + "%";
                }
            }
            return appResult;
        }
    }

}


