using HarmonyLib;
using System.Reflection;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    internal static class GN_HarmonyInitialise
    {
        static GN_HarmonyInitialise()
        {
            var harmony = new Harmony("com.company.QarsoonMeel.GunNut");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}