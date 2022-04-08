using HarmonyLib;
using RimWorld;
using Verse;

namespace GunNut
{
    [HarmonyPatch(typeof(VerbTracker), "CreateVerbTargetCommand")]
    public static class GN_CreateVerbTargetCommand
    {
        
        [HarmonyPrefix]
        public static bool GN_GetDamageAmount_PostFix(VerbTracker __instance, Thing ownerThing, Verb verb, ref Command_VerbTarget __result)
        {
            GN_Command_VerbTarget command_VerbTarget = new GN_Command_VerbTarget();
            ThingStyleDef styleDef = ownerThing.StyleDef;
            command_VerbTarget.defaultDesc = ownerThing.LabelCap + ": " + ownerThing.def.description.CapitalizeFirst();
            command_VerbTarget.icon = ((styleDef != null && styleDef.UIIcon != null) ? styleDef.UIIcon : ownerThing.def.uiIcon);
            command_VerbTarget.iconAngle = ownerThing.def.uiIconAngle;
            command_VerbTarget.iconOffset = ownerThing.def.uiIconOffset;
            command_VerbTarget.tutorTag = "VerbTarget";
            command_VerbTarget.verb = verb;
            if (verb.caster.Faction != Faction.OfPlayer)
            {
                command_VerbTarget.Disable("CannotOrderNonControlled".Translate());
            }
            else if (verb.CasterIsPawn)
            {
                string reason;
                if (verb.CasterPawn.WorkTagIsDisabled(WorkTags.Violent))
                {
                    command_VerbTarget.Disable("IsIncapableOfViolence".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
                else if (!verb.CasterPawn.drafter.Drafted)
                {
                    command_VerbTarget.Disable("IsNotDrafted".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
                else if (verb is Verb_LaunchProjectile)
                {
                    Apparel apparel = verb.FirstApparelPreventingShooting();
                    if (apparel != null)
                    {
                        command_VerbTarget.Disable("ApparelPreventsShooting".Translate(verb.CasterPawn.Named("PAWN"), apparel.Named("APPAREL")).CapitalizeFirst());
                    }
                }
                else if (EquipmentUtility.RolePreventsFromUsing(verb.CasterPawn, verb.EquipmentSource, out reason))
                {
                    command_VerbTarget.Disable(reason);
                }
            }
            __result = command_VerbTarget;
            return false;
        }
    }
}