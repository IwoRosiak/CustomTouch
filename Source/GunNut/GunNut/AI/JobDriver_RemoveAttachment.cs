using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace GunNut
{
    public class JobDriver_RemoveAttachment : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
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
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);

            yield return DoRemoveAttachFromWeapon(150, "Interact_ConstructMetal").FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
        }

        public static Toil DoRemoveAttachFromWeapon(int duration, string soundDefName)
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

                var weapon = curJob.GetTarget(TargetIndex.A).Thing;

                weapon.Notify_ColorChanged();

                var weaponComp = weapon.TryGetComp<GN_AttachmentComp>();

                foreach (var slot in weaponComp.SlotsOnWeapon)
                {
                    if (slot.attachment != null)
                    {
                        Thing attachment = ThingMaker.MakeThing(slot.attachment, null);
                        GenPlace.TryPlaceThing(attachment, weapon.Position, weapon.Map, ThingPlaceMode.Near, null, null, default(Rot4));
                        slot.attachment = null;
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
    }
}