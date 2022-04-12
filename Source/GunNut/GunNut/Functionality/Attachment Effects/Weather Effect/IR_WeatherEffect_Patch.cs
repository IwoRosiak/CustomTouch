using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GunNut.Functionality.Attachment_Effects.Zoom_and_NV_Effects
{
    [HarmonyPatch(typeof(ShotReport), "HitReportFor")]
    public static class IR_WeatherEffect_Patch
    {
        static readonly FieldInfo weatherEffect = AccessTools.Field(typeof(ShotReport), "factorFromWeather");
        static MethodInfo setWeather = AccessTools.Method(typeof(IR_WeatherEffect_Patch), nameof(IR_WeatherEffect_Patch.ChangeWeatherOffset), new Type[] { typeof(Verb), typeof(float) }); //SymbolExtensions.GetMethodInfo(() => Instruction());
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Stfld && instruction.OperandIs(weatherEffect))
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, weatherEffect);
                    yield return CodeInstruction.Call(typeof(IR_WeatherEffect_Patch), nameof(ChangeWeatherOffset), new Type[] { typeof(Verb), typeof(float) });
                    yield return new CodeInstruction(OpCodes.Stfld, weatherEffect);
                    continue;
                }
                yield return instruction;
            }
        }

        public static float ChangeWeatherOffset(Verb verb, float shotReportVal)
        {
            float result = shotReportVal;

            GN_AttachmentComp comp = verb.EquipmentSource.TryGetComp<GN_AttachmentComp>();

            if (comp != null)
            {
                float debuffNegation = CalcWeatherDebuffReduction(comp);
               
                float newWeatherEffectOffset = (1-result) *(1 -debuffNegation);

                result= 1 - newWeatherEffectOffset;
            }
            return result;
        }

        public static float CalcWeatherDebuffReduction(GN_AttachmentComp comp)
        {
            float nv = 0;
            float infrav = 0;

            foreach (var item in comp.ActiveAttachments)
            {
                infrav += item.infravision;
                nv += item.nightvision;
            }

            return (nv * 0.25f) + (infrav * 0.75f);
            //float darknessInfluence = (nv * 0.75f) + (infrav * 0.25f);
        }
    }
}
