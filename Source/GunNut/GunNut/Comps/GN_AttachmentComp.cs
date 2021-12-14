using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace GunNut
{
    public class GN_AttachmentComp : ThingComp
    {
        public GN_AttachmentCompProperties Props
        {
            get
            {
                return (GN_AttachmentCompProperties)this.props;
            }
        }

        public List<GN_Slot> SlotsProps
        {
            get
            {
                return Props.Slots;
            }
        }

        public IEnumerable<GN_AttachmentDef> AttachmentsOnWeapon
        {
            get
            {
                foreach (var slot in SlotsOnWeapon)
                {
                    if (slot.attachment != null)
                    {
                        yield return slot.attachment;
                    }
                }
            }
        }

        public void InitSlots()
        {
            GN_Slot[] array = new GN_Slot[SlotsProps.Count()];

            SlotsProps.CopyTo(array);

            Slots.AddRange(array);
        }

        private List<GN_Slot> Slots = new List<GN_Slot>();

        public IEnumerable<GN_Slot> SlotsOnWeapon
        {
            get
            {
                if (Slots.NullOrEmpty())
                {
                    InitSlots();
                }

                foreach (var slot in Slots)
                {
                    if (IR_Init.IsActive(parent.def.defName, slot.weaponPart))
                    {
                        yield return slot;
                    }
                }
            }
        }

        public bool HasAnyAttachments
        {
            get
            {
                foreach (var slot in SlotsOnWeapon)
                {
                    if (slot.attachment != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Collections.Look<GN_Slot>(ref Slots, "SlotsAttachment" + this.parent.ThingID, LookMode.Deep);
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

            if (this.HasAnyAttachments)
            {
                Action giveJob = delegate ()
                {
                    this.TryRemoveAttachment(pawn);
                };
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Remove all attachments.", giveJob, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, this.parent, "ReservedBy");
            }

            foreach (var slot in SlotsOnWeapon)
            {
                bool slotHasAttachment = slot.attachment != null;

                foreach (var attachment in FindAvailableAttachmentForWeaponPart(pawn, slot.weaponPart))
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

        private IEnumerable<Thing> FindAvailableAttachmentForWeaponPart(Pawn pawn, GN_WeaponParts.WeaponPart desiredPart)
        {
            if (pawn == null || !pawn.Spawned || pawn.Downed || pawn.Map == null)
            {
                yield break;
            }
            else
            {
                foreach (var attachmentDef in GetDefsOfAttachmentsOnMap(pawn.Map, desiredPart))
                {
                    if (attachmentDef.weaponPart == desiredPart)
                    {
                        yield return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(attachmentDef), PathEndMode.InteractionCell, TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false, false, false), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false), null, 0, -1, false, RegionType.Set_Passable, false);
                    }
                }
            }
        }

        private IEnumerable<GN_AttachmentDef> GetDefsOfAttachmentsOnMap(Map map, GN_WeaponParts.WeaponPart desiredPart)
        {
            List<GN_AttachmentDef> uniqueAttachmentDefs = new List<GN_AttachmentDef>();

            foreach (var thing in map.listerThings.ThingsInGroup(ThingRequestGroup.HaulableEver))
            {
                if (thing.def.thingCategories[0].ToString() == "Attachment" && !uniqueAttachmentDefs.Contains((GN_AttachmentDef)thing.def))
                {
                    uniqueAttachmentDefs.Add((GN_AttachmentDef)thing.def);
                }
            }
            return uniqueAttachmentDefs;
        }
    }
}