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

namespace GunNut.Functionality.Attachment_Effects.Zoom_and_NV_Effects
{
    [HarmonyPatch(typeof(ShotReport), "HitReportFor")]
    public static class IR_ZoomEffect_Patch
    {
        static readonly FieldInfo sizeEffect = AccessTools.Field(typeof(ShotReport), "factorFromTargetSize");
        static MethodInfo setSizeFactor = AccessTools.Method(typeof(IR_ZoomEffect_Patch), nameof(IR_ZoomEffect_Patch.ChangeSizeOffset), new Type[] { typeof(Verb), typeof(float) });
        static MethodInfo m_get_bodySize = AccessTools.Method(typeof(Pawn), "get_BodySize");
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found= false;
            bool foundForGood = false;

            foreach (var instruction in instructions)
            {
                if (!foundForGood && found && instruction.OperandIs(sizeEffect))
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    yield return new CodeInstruction(OpCodes.Callvirt, m_get_bodySize);
                    yield return CodeInstruction.Call(typeof(IR_ZoomEffect_Patch), nameof(ChangeSizeOffset), new Type[] { typeof(Verb), typeof(float) });
                    yield return new CodeInstruction(OpCodes.Stfld, sizeEffect);
                    found = false;
                    foundForGood = true;
                    continue;
                }


                if (instruction.Calls(m_get_bodySize))
                {
                    yield return instruction;
                    found = true;
                    
                    continue;
                }
                yield return instruction;
            }
        }

        public static float ChangeSizeOffset(Verb verb, float shotReportVal)
        {
            float result = shotReportVal;
            result = Mathf.Clamp(result, 0.5f, 2);

            GN_AttachmentComp comp;

            if ((comp = verb.EquipmentSource.TryGetComp<GN_AttachmentComp>()) != null)
            {
                float zoom = comp.ActiveSlots
                    .Where(slot => slot.isActive && slot.attachment!=null && slot.attachment.zoomvision != 0)
                    .Sum(slot => slot.attachment.zoomvision);
                

                float newEffectOffset = result * zoom;

                result += + newEffectOffset;
            }
            return result;
        }
    }
}
