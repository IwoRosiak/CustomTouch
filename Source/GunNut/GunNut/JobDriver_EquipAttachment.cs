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
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    this.attachmentIngredient = base.GetActor().jobs.curJob.GetTarget(TargetIndex.B).Thing;
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDestroyedOrNull(TargetIndex.B);

            Toil findPlaceTarget = Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.B);
            yield return findPlaceTarget;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, findPlaceTarget, false, false);
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

                GN_ThingComp weapon = curJob.GetTarget(TargetIndex.A).Thing.TryGetComp<GN_ThingComp>();
                Thing attachment = curJob.GetTarget(TargetIndex.B).Thing;
                weapon.Slots[0].attachment = (GN_AttachmentDef)attachment.def;
                attachment.Destroy(DestroyMode.Vanish);

            });
            toil.handlingFacing = true;
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = duration;
            toil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            toil.PlaySustainerOrSound(() => SoundDef.Named(soundDefName));
            return toil;
        }



        //public const TargetIndex WorkbenchIndex = TargetIndex.A;

        public const TargetIndex WeaponMasterIndex = TargetIndex.A;

        public const TargetIndex AttachmentIngredientIndex = TargetIndex.B;

        public const PathEndMode GotoWeaponPathEndMode = PathEndMode.ClosestTouch;

        private Thing weaponMaster;

        // Token: 0x0400000C RID: 12
        private Thing attachmentIngredient;


    }
}
