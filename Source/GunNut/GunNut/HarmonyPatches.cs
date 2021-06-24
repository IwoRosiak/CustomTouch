using HarmonyLib;
using System.Reflection;
using Verse;

namespace GunNut
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("com.company.QarsoonMeel.GunNut");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }


    [HarmonyPatch(typeof(VerbProperties), "AdjustedAccuracy")]
    public static class Test_patchie
    {

        [HarmonyPostfix]
        public static void Test(ref float __result, VerbProperties __instance, Thing equipment)
        {
            /*if (equipment.TryGetComp<GN_ThingComp>().Attachments != null)
            {

                __result = __result * 1.1f;

            }*/

            /*if (__instance.nam)

                if (__instance.pa.def.IsWeapon)
                {
                    var weapon = (GN_ThingDef)__instance.parent.def;

                    if (weapon.attachments != null)
                    {
                        __instance.parent.def.Verbs[0].GetHitChanceFactor
                    }
                }
            */
        }
    }



}