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
    /*
    [HarmonyPatch(typeof(Graphic), "MatSingleFor")]
    public static class IR_TextureMask_Patch
    {
        [HarmonyPostfix]
        private static void PawnRendererPatch(Thing thing, ref Material __result)
        {
            if (((thing as ThingWithComps).TryGetComp<GN_AttachmentComp>()) != null)
            {
                if (TextureHolder.texture != null)
                {
                    UnityEngine.Object.Destroy(TextureHolder.texture);
                } 

                Material newMat = __result;

                Texture2D text = newMat.mainTexture as Texture2D;

                TextureHolder.texture = new Texture2D(__result.mainTexture.width, __result.mainTexture.height);

                Graphics.CopyTexture(__result.mainTexture, TextureHolder.texture);

                //text.width  = text.width/2;
                //text.read



                TextureHolder.texture.SetPixel(0, 0, Color.red);
                for (int i = 0; i < 49; i++)
                {
                    for (int j = 0; j < 49; j++)
                    {
                        TextureHolder.texture.SetPixel(i, j, new Color(255,255,255,255));

                    }
                }
                TextureHolder.texture.Apply();

                __result.mainTexture = TextureHolder.texture;

       
                //__result.SetTexture(__result.mainTexture.name, texture);
            }



        }
    }
 
    [StaticConstructorOnStartup]
    public static class TextureHolder
    {
        public static Texture2D texture = new Texture2D(1, 1);
    }*/
}
