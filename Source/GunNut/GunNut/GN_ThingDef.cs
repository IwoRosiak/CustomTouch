using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
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


            /*

            MethodInfo targetmethod = AccessTools.Method(typeof(VerbProperties), "AdjustedAccuracy");

            HarmonyMethod postfixmethod = new HarmonyMethod(typeof(GunNut.HarmonyPatches).GetMethod("CheckWeapon_Postfix"));

            harmony.Patch(targetmethod, null, postfixmethod);

            MethodInfo targetmethodScope = AccessTools.Method(typeof(ThingComp), "CompTick");

            HarmonyMethod postfixmethodScope = new HarmonyMethod(typeof(GunNut.HarmonyPatches).GetMethod("CheckScope_Postfix"));

            harmony.Patch(targetmethodScope, null, postfixmethodScope);*/

        }
        /*


        public static void CheckScope_Postfix(ThingDef __instance)
        {
            if (__instance.IsWeapon)
            {
                GN_ThingDef weapon = (GN_ThingDef)__instance;
                if (weapon.attachments != null)
                {
                    foreach (string attachment in weapon.attachments)
                    {
                        if (attachment == "Scope")
                        {




                            //Log.Message();

                            //weapon.Verbs[0].defaultProjectile.projectile.damageDef.defaultDamage = 1000;  // statBases[5] = StatDefOf.AccuracyLong.statFactors. ;
                        }
                    }
                    //Log.Message("Huj ci w pupe - " + weapon.defName);

                }
            }



        }







        public static float CheckWeapon_Postfix(float __result, VerbProperties __instance)
        {
            //var RangeCategory = Traverse.Create<VerbProperties>().Property("RangeCategory").GetValue();



            Log.Message((__result * 1.05f).ToString());

            return __result * 1.05f;
            /*if (equipment != null)
            {
                StatDef stat = null;
                switch (cat)
                {
                    case :
                        stat = StatDefOf.AccuracyTouch;
                        break;
                    case __instance.RangeCategory.Short:
                        stat = StatDefOf.AccuracyShort;
                        break;
                    case VerbProperties.RangeCategory.Medium:
                        stat = StatDefOf.AccuracyMedium;
                        break;
                    case VerbProperties.RangeCategory.Long:
                        stat = StatDefOf.AccuracyLong;
                        break;
                }
                return equipment.GetStatValue(stat, true);
            }
            switch (cat)
            {
                case RangeCategory.Touch:
                    return __instance.accuracyTouch;
                case VerbProperties.RangeCategory.Short:
                    return __instance.accuracyShort;
                case VerbProperties.RangeCategory.Medium:
                    return __instance.accuracyMedium;
                case VerbProperties.RangeCategory.Long:
                    return __instance.accuracyLong;
                default:
                    throw new InvalidOperationException();
            }
            */






    }

    [HarmonyPatch(typeof(ThingComp), "CompTick")]
    public static class Test_patch
    {

        [HarmonyPostfix]
        public static void Test(ThingComp __instance)
        {
            /* if (__instance.parent.def.IsWeapon)
             {
                 var weapon = (GN_ThingDef)__instance.parent.def;

                 if (weapon.attachments != null)
                 {

                     if (__instance.parent.HitPoints < __instance.parent.def.BaseMaxHitPoints)
                     {
                         Log.Message("Im here!");
                         weapon.attachments = null;
                     }
                     //def.Verbs[0].accuracyTouch = 1.0f;


                     //__instance.parent.def.Verbs[0] = 1.0f;

                     foreach (var verb in __instance.parent.def.Verbs)
                     {
                         //Log.Message(verb.ToString());
                     }                    //__instance.parent.def.statBases[0];
                 }
             }*/
        }
    }

    [HarmonyPatch(typeof(VerbProperties), "AdjustedAccuracy")]
    public static class Test_patchie
    {

        [HarmonyPostfix]
        public static void Test(ref float __result, VerbProperties __instance, Thing equipment)
        {
            if (equipment.TryGetComp<GN_ThingComp>().Attachments != null)
            {

                __result = __result * 1.1f;

            }

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

    /*
    [HarmonyPatch(typeof(ThingComp), "CompTick")]
    public static class Test_patch
    {

        [HarmonyPostfix]
        public static void Test(ThingComp __instance)
        {
            if (__instance.parent.def.IsWeapon)
            {
                var weapon = (GN_ThingDef)__instance.parent.def;

                if (weapon.attachments != null)
                {
                    Log.Message("Im here!");
                    __instance.parent.verb  //def.Verbs[0].accuracyTouch = 1.0f;


                    //__instance.parent.def.Verbs[0] = 1.0f;

                    foreach (var verb in __instance.parent.def.Verbs)
                    {
                        Log.Message(verb.ToString());
                    }                    //__instance.parent.def.statBases[0];
                }
            }
        }
    }

    */
    public class ModExtension_BulletPower : DefModExtension
    {
        public float damage = 100;
        public List<string> attachments;

    }

    [DefOf]
    public static class GN_ThingDefOf
    {
        static GN_ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GN_ThingDefOf));
        }

        public static GN_ThingDef Gun_BoltActionRifle_Scope;

    }

    public class GN_ThingWithComps : ThingWithComps
    {
        public override void Tick()
        {
            base.Tick();
            var weapon = (GN_ThingDef)this.def;

            if (weapon.IsWeapon)
            {
                //weapon.
                //Log.Message("Huj ci w pupe - " + this.def.defName);
            }
        }
    }

    public class GN_ThingDef : ThingDef
    {
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        {
            base.SpecialDisplayStats(req);
            var attachments = req.Thing.TryGetComp<GN_ThingComp>().Attachments;
            if (attachments != null)
            {
                Log.Message("Tutaj!");
                foreach (var attachment in attachments)
                {
                    yield return new StatDrawEntry(GN_StatCategoryDefOf.Attachments, "Attachments", attachment, "Attachment for a weapon.", 5391, null, null, false);
                }


            }
        }

    }

    public class CompProperties_GunNut : CompProperties
    {
        // Token: 0x0600365B RID: 13915 RVA: 0x001293AA File Offset: 0x001275AA
        public CompProperties_GunNut()
        {
            this.compClass = typeof(GN_ThingComp);
        }

        public List<string> Attachments
        {
            get
            {
                return this.attachments;
            }
        }

        // Token: 0x04001CEA RID: 7402
        public List<string> attachments;

    }
    public class GN_ThingComp : ThingComp
    {

        // Token: 0x17000EC7 RID: 3783
        // (get) Token: 0x060054B3 RID: 21683 RVA: 0x001C7E9E File Offset: 0x001C609E
        public CompProperties_GunNut Props
        {
            get
            {
                return (CompProperties_GunNut)this.props;
            }

        }

        public List<string> Attachments = new List<string>();




        public override void CompTick()
        {
            base.CompTick();
            if (this.Attachments.Count != 0)
            {
                if (this.parent.HitPoints < this.parent.def.BaseMaxHitPoints)
                {
                    this.Attachments.Clear();
                }
                //this.parent.def.Verbs[0].
            }
            else
            {

                Attachments.Add("Scope");
                Attachments.Add("Laser tag");
                Attachments.Add("Silencer");

                Log.Message(this.Attachments[0]);
            }
        }

    }
}