using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GunNut.Functionality.Display
{
    [HarmonyPatch(typeof(Graphic), "MatSingleFor")]
    public static class IR_TextureMask_Patch
    {
        [HarmonyPostfix]
        private static void PawnRendererPatch(Thing thing, ref Material __result)
        {
            GN_AttachmentComp comp;
            if ((comp = (thing as ThingWithComps).TryGetComp<GN_AttachmentComp>()) != null)
            {
                comp.UpdateTexture(__result);
                

                __result.mainTexture = comp.attachmentTexture;
            }
        }
    }
}
