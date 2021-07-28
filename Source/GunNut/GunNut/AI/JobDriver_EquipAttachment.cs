using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace GunNut
{
    public class JobDriver_EquipAttachment : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.job.targetB, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            base.AddEndCondition(delegate
            {
                Thing thing = base.GetActor().jobs.curJob.GetTarget(TargetIndex.A).Thing;
                bool flag = thing == null || !thing.Spawned;
                JobCondition result;
                if (flag)
                {
                    result = JobCondition.Incompletable;
                }
                else
                {
                    result = JobCondition.Ongoing;
                }
                return result;
            });
            this.FailOnBurningImmobile(TargetIndex.B);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    this.attachmentIngredient = base.GetActor().jobs.curJob.GetTarget(TargetIndex.B).Thing;
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDestroyedOrNull(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);

            Toil findPlaceTarget = Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.B);
            yield return findPlaceTarget;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.A, findPlaceTarget, false, false).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    base.GetActor().jobs.curJob.SetTarget(TargetIndex.B, this.attachmentIngredient);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return JobDriver_EquipAttachment.DoAttachModToWeapon(250, "Interact_ConstructMetal").FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            yield break;
        }

        public static Toil DoAttachModToWeapon(int duration, string soundDefName)
        {
            Toil toil = new Toil();
            toil.tickAction = delegate ()
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
            };
            toil.AddFinishAction(delegate
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;

                GN_AttachmentComp weapon = curJob.GetTarget(TargetIndex.A).Thing.TryGetComp<GN_AttachmentComp>();
                Thing attachment = curJob.GetTarget(TargetIndex.B).Thing;

                foreach (var slot in weapon.Slots)
                {
                    var attachmentDef = (GN_AttachmentDef)attachment.def;
                    if (slot.weaponPart == attachmentDef.weaponPart)
                    {
                        if (slot.attachment != null)
                        {
                            Thing thing = ThingMaker.MakeThing(slot.attachment, null);
                            GenPlace.TryPlaceThing(thing, curJob.GetTarget(TargetIndex.A).Thing.Position, curJob.GetTarget(TargetIndex.A).Thing.Map, ThingPlaceMode.Near, null, null, default(Rot4));
                        }
                        slot.attachment = attachmentDef;
                        attachment.Destroy(DestroyMode.Vanish);
                        //curJob.GetTarget(TargetIndex.A).Thing.ForceSetStateToUnspawned();
                        //curJob.GetTarget(TargetIndex.A).Thing.Map.mapDrawer.RegenerateEverythingNow();
                        //curJob.GetTarget(TargetIndex.A).Thing.Map.mapDrawer.SectionAt(curJob.GetTarget(TargetIndex.A).Thing.Position).RegenerateAllLayers();
                        curJob.GetTarget(TargetIndex.A).Thing.DirtyMapMesh(curJob.GetTarget(TargetIndex.A).Thing.Map);
                        break;
                    }
                }
            });

            toil.handlingFacing = true;
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = duration;
            toil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            toil.PlaySustainerOrSound(() => SoundDef.Named(soundDefName));
            return toil;
        }

        public const TargetIndex WeaponMasterIndex = TargetIndex.A;

        public const TargetIndex AttachmentIngredientIndex = TargetIndex.B;

        public const PathEndMode GotoWeaponPathEndMode = PathEndMode.ClosestTouch;

        private Thing weaponMaster;

        // Token: 0x0400000C RID: 12
        private Thing attachmentIngredient;
    }
}