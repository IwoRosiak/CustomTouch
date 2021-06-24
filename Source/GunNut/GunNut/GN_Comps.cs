using System.Collections.Generic;
using Verse;

namespace GunNut
{
    public class CompProperties_GunNut : CompProperties
    {
        public CompProperties_GunNut()
        {
            this.compClass = typeof(GN_ThingComp);
        }

        public List<Slot> slots;

        //public List<string> attachments;

        public JobDef jobDef;

    }
    public class GN_ThingComp : ThingComp
    {
        public CompProperties_GunNut Props
        {
            get
            {
                return (CompProperties_GunNut)this.props;
            }

        }




        public List<Slot> Slots = new List<Slot>();





        //new List<GN_SlotDef>();
        public override void CompTick()
        {
            base.CompTick();
            if (Slots.Count == 0)
            {
                Slots = Props.slots;
            }

        }
        /*
        private bool TryGiveWeaponRepairJobToPawn(Pawn pawn)
        {
            Thing attachment = GetAvailableAttachment(pawn);
            if (attachment != null)
            {
                Job job = new Job(this.Props.jobDef, parent, attachment);
                job.count = 1;
                return pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);

            }
            return false;
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            Action hoverAction = delegate ()
            {
                //Thing availableTwinThing = this.GetAvailableTwinThing(selPawn);
                //MoteMaker.MakeStaticMote(availableTwinThing.Position, this.parent.Map, ThingDefOf.Mote_FeedbackGoto, 1f);
            };
            Action giveRepairJob = delegate ()
            {
                this.TryGiveWeaponRepairJobToPawn(selPawn);
            };
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Test", giveRepairJob, MenuOptionPriority.Default, hoverAction, null, 0f, null, null), selPawn, this.parent, "ReservedBy");
            yield break;
        }

        private Thing GetAvailableAttachment(Pawn pawn)
        {
            bool flag = pawn == null || !pawn.Spawned || pawn.Downed || pawn.Map == null;
            Thing result = null;
            if (flag)
            {
                result = null;
            }
            else
            {
                List<Thing> list = new List<Thing>();
                foreach (Thing thing in this.parent.Map.listerThings.ThingsOfDef(GN_ThingDefOf.Scope))
                {
                    return thing;
                }
            }
            return result;
        }*/
    }
}
