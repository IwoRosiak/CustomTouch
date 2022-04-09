using HarmonyLib;
using System.Reflection;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    internal static class IR_HarmonyInitialise
    {
        static IR_HarmonyInitialise()
        {
            var harmony = new Harmony("com.company.QarsoonMeel.GunNut");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}