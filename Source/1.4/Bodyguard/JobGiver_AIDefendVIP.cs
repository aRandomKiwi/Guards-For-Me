using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class JobGiver_AIDefendVIP : JobGiver_AIDefendPawn
    {

        public JobGiver_AIDefendVIP()
        {
        }

        protected override Pawn GetDefendee(Pawn pawn)
        {
            return pawn.TryGetComp<Comp_Guard>().guardedPawn;
        }

        protected override float GetFlagRadius(Pawn pawn)
        {

            if (!pawn.TryGetComp<Comp_Guard>().guardedPawn.TryGetComp<Comp_Guard>().guardOnlyAttackNearThreats)
            {
                return 50f;
            }
            return 8f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Pawn defendee = this.GetDefendee(pawn);
            if (defendee == null)
            {
                Log.Error(base.GetType() + " has null defendee. pawn=" + pawn.ToStringSafe<Pawn>());
                return null;
            }
            Pawn carriedBy = defendee.CarriedBy;
            if (carriedBy != null)
            {
                if (!pawn.CanReach(carriedBy, PathEndMode.OnCell, Danger.Deadly, false, false,TraverseMode.ByPawn))
                {
                    return null;
                }
            }
            else if (!defendee.Spawned || !pawn.CanReach(defendee, PathEndMode.OnCell, Danger.Deadly, false, false,TraverseMode.ByPawn))
            {
                return null;
            }

            this.UpdateEnemyTarget(pawn);
            Thing enemyTarget = pawn.mindState.enemyTarget;
            if (enemyTarget == null)
            {
                return null;
            }
            Pawn pawn2 = enemyTarget as Pawn;
            if (pawn2 != null && pawn2.IsInvisible())
            {
                return null;
            }
            bool allowManualCastWeapons = !pawn.IsColonist;
            Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            if (verb == null)
            {
                return null;
            }
            if (verb.verbProps.IsMeleeAttack)
            {
                return this.MeleeAttackJob(enemyTarget);
            }
            bool flag = CoverUtility.CalculateOverallBlockChance(pawn, enemyTarget.Position, pawn.Map) > 0.01f;
            bool flag2 = pawn.Position.Standable(pawn.Map) && pawn.Map.pawnDestinationReservationManager.CanReserve(pawn.Position, pawn, pawn.Drafted);
            bool flag3 = verb.CanHitTarget(enemyTarget);
            bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;
            if ((flag && flag2 && flag3) || (flag4 && flag3))
            {
                return JobMaker.MakeJob(JobDefOf.AttackStatic, enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, false);
            }

            if (!selVec.IsValid)
            {
                return null;
            }

            if (selVec == pawn.Position)
            {
                return JobMaker.MakeJob(JobDefOf.AttackStatic, enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, false);
            }
            Job job = JobMaker.MakeJob(Utils.gotoCombatJobDef, selVec);
            job.expiryInterval = 500;
            job.checkOverrideOnExpire = true;
            return job;
        }

        protected override Thing FindAttackTarget(Pawn pawn)
        {
            Thing thing = null;
            Utils.FindAttackTarget(pawn, out selVec, out thing, true);

            /*if(thing != null)
                Log.Message("BODYGUARD " + thing.LabelCap);*/

            return thing;
        }

        protected override Job MeleeAttackJob(Thing enemyTarget)
        {
            Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, enemyTarget);
            job.expiryInterval = ExpiryInterval_Melee.RandomInRange;
            job.checkOverrideOnExpire = true;
            job.killIncappedTarget = Settings.killDownedThreats;
            job.expireRequiresEnemiesNearby = true;
            return job;
        }

        private IntVec3 selVec = IntVec3.Invalid;
        private static readonly new IntRange ExpiryInterval_Melee = new IntRange(360, 480);
    }
}
