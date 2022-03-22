using UnityEngine;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    internal static class IR_Textures
    {
        public static Texture2D frame = ContentFinder<Texture2D>.Get("frame", true);
    }
}