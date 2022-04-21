using CustomTouch;
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
        private IR_WeaponSlotsInfo slotsInfo;

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

        public IEnumerable<GN_AttachmentDef> ActiveAttachments
        {
            get
            {
                foreach (var slot in ActiveSlots)
                {
                    if (slot.attachment != null)
                    {
                        yield return slot.attachment;
                    }
                }
            }
        }

        public IEnumerable<GN_Slot> ActiveSlots
        {
            get
            {
                if (slotsInfo == null)
                {
                    InitSlots();
                }
                foreach (var slot in slotsInfo.GetSlots())
                {
                    yield return slot;
                }
            }
        }

        public void InitSlots()
        {
            if (slotsInfo == null)
            {
                slotsInfo = new IR_WeaponSlotsInfo(this);
            }
        }


        public bool HasAnyAttachments
        {
            get
            {
                foreach (var slot in ActiveSlots)
                {
                    if (slot.attachment != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Texture attachmentTexture;
        public bool shouldUpdateTexture= true;

        public void UpdateTexture(Material mat)
        {
            if (shouldUpdateTexture)
            {
                Texture mainTexture = mat.mainTexture;

                int width = mainTexture.width;
                int width2 = mainTexture.width;
                Texture2D texture2D = new Texture2D(width, width2, TextureFormat.ARGB32, false);
                RenderTexture renderTexture = new RenderTexture(width, width2, 0, (RenderTextureFormat)0, 1);
                Graphics.Blit(mainTexture, renderTexture, mat, 0);
                RenderTexture.active = renderTexture;

                texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)width2), 0, 0, false);

                for (int i = 0; i < 20; i++)
                {
                    for (int j = 20; j < 45; j++)
                    {
                        texture2D.SetPixel(i, j, new Color(0, 0, 0, 0));

                    }
                }
                texture2D.Apply();

                attachmentTexture = texture2D;

                shouldUpdateTexture = false;
            }
        }

        //COMP STUFF

        public override void PostExposeData()
        {
            Scribe_Deep.Look<IR_WeaponSlotsInfo>(ref slotsInfo, "SlotsInfo" + parent.ThingID, this);
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

            foreach (var slot in ActiveSlots)
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

        private IEnumerable<Thing> FindAvailableAttachmentForWeaponPart(Pawn pawn, IR_AttachmentType desiredPart)
        {
            if (pawn == null || !pawn.Spawned || pawn.Downed || pawn.Map == null)
            {
                yield break;
            }
            else
            {
                foreach (var attachmentDef in GetDefsOfAttachmentsOnMap(pawn.Map, desiredPart))
                {
                    if (attachmentDef.weaponPart == desiredPart && IsMatchingTags(attachmentDef))
                    {
                        yield return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(attachmentDef), PathEndMode.InteractionCell, TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false, false, false), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false), null, 0, -1, false, RegionType.Set_Passable, false);
                    }
                }
            }
        }

        private bool IsMatchingTags(GN_AttachmentDef attachment)
        {
            foreach (IR_AttachmentTag tag in attachment.requiredTags)
            {
                if (!IR_Settings.GetWeaponTags(parent.def.defName).Contains(tag))
                {
                    return false;
                }
            }

            foreach (IR_AttachmentTag tag in attachment.conflictingTags)
            {
                if (IR_Settings.GetWeaponTags(parent.def.defName).Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<GN_AttachmentDef> GetDefsOfAttachmentsOnMap(Map map, IR_AttachmentType desiredPart)
        {
            List<GN_AttachmentDef> uniqueAttachmentDefs = new List<GN_AttachmentDef>();

            if (map!= null && map.listerThings!= null)
            {
                foreach (var thing in map.listerThings.ThingsInGroup(ThingRequestGroup.HaulableEver))
                {
                    if (thing?.def?.thingCategories[0]?.ToString() == "Attachment" && !uniqueAttachmentDefs.Contains((GN_AttachmentDef)thing.def))
                    {
                        uniqueAttachmentDefs.Add((GN_AttachmentDef)thing.def);
                    }
                }
            }
            
            return uniqueAttachmentDefs;
        }
    }
}