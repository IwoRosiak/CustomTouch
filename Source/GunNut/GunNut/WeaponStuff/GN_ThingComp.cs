using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace GunNut
{
    public class GN_ThingComp : ThingComp
    {
        public CompProperties_GunNut Props
        {
            get
            {
                return (CompProperties_GunNut)this.props;
            }
        }

        public List<Slot> SlotsProps
        {
            get
            {
                return Props.Slots;
            }
        }

        public bool hasAnyAttachments
        {
            get
            {
                foreach (var slot in this.Slots)
                {
                    if (slot.attachment != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public List<Slot> Slots = new List<Slot>();

        public override void CompTick()
        {
            base.CompTick();
            if (Slots == null || Slots.Count == 0)
            {
                Slots = this.SlotsProps;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Collections.Look<Slot>(ref Slots, "SlotsAttachment" + this.parent.ThingID, LookMode.Deep);
        }

        private bool TryInstallAttachment(Pawn pawn, Thing attachment)
        {
            if (attachment != null)
            {
                Job job = new Job(this.Props.jobDefInstall, parent, attachment);
                job.count = 1;
                return pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
            return false;
        }

        private void TryRemoveAttachment(Pawn pawn)
        {
            Job job = new Job(this.Props.jobDefRemove, parent);
            job.count = 1;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            foreach (FloatMenuOption fmo in base.CompFloatMenuOptions(pawn))
            {
                yield return fmo;
            }

            if (this.hasAnyAttachments)
            {
                Action giveJob = delegate ()
                {
                    this.TryRemoveAttachment(pawn);
                };
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Remove all attachments.", giveJob, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, this.parent, "ReservedBy");
            }

            foreach (var slot in this.Slots)
            {
                bool slotHasAttachment = slot.attachment != null;

                List<Thing> CompatibleAttachments = FindAvailableAttachment(pawn, slot.weaponPart);
                if (CompatibleAttachments.NullOrEmpty() == true)
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Haven't found any attachments for " + slot.weaponPart.ToString() + ".", null, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, this.parent, "ReservedBy");
                    continue;
                }

                foreach (var attachment in CompatibleAttachments)
                {
                    if (!pawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, false, TraverseMode.ByPawn))
                    {
                        yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Cannot reach.", null, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, this.parent, "ReservedBy");
                        continue;
                    }
                    if (slotHasAttachment)
                    {
                        Action<Rect> hoverActionReplacing = delegate
                        {
                            FleckMaker.Static(attachment.Position, pawn.Map, FleckDefOf.FeedbackGoto, 1f);
                        };
                        Action giveJobReplacing = delegate ()
                        {
                            this.TryInstallAttachment(pawn, attachment);
                        };
                        yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Replace " + slot.attachment.label + " with " + attachment.def.label, giveJobReplacing, MenuOptionPriority.Default, hoverActionReplacing, null, 0f, null, null), pawn, this.parent, "ReservedBy");
                        continue;
                    }

                    Action<Rect> hoverActionAttaching = delegate
                    {
                        FleckMaker.Static(attachment.Position, pawn.Map, FleckDefOf.FeedbackGoto, 1f);
                    };
                    Action giveJobAttaching = delegate ()
                    {
                        this.TryInstallAttachment(pawn, attachment);
                    };
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Install " + attachment.def.label, giveJobAttaching, MenuOptionPriority.Default, hoverActionAttaching, null, 0f, null, null), pawn, this.parent, "ReservedBy");
                }
            }
            yield break;
        }

        private List<Thing> FindAvailableAttachment(Pawn pawn, GN_ThingDefOf.WeaponPart desiredPart)
        {
            List<Thing> results = new List<Thing>();

            if (pawn == null || !pawn.Spawned || pawn.Downed || pawn.Map == null)
            {
                return null;
            }
            else
            {
                foreach (var attachment in GN_AttachmentList.allAttachments)
                {
                    if (attachment.weaponPart == desiredPart)
                    {
                        var closestAttachmentOfThisType = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(attachment), PathEndMode.InteractionCell, TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false, false, false), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false), null, 0, -1, false, RegionType.Set_Passable, false);
                        if (closestAttachmentOfThisType != null)
                        {
                            results.Add(closestAttachmentOfThisType);
                        }
                    }
                }
            }
            return results;
        }
    }
}