using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class JobGiver_AIFightEnemiesCustom : JobGiver_AIFightEnemies
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn == null)
                return null;

            Thing enemyTarget = null;
            bool forcedTarget = false;
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();

            if (comp == null || comp.deathSquadForcedTarget == null)
            {
                this.UpdateEnemyTarget(pawn);
                enemyTarget = pawn.mindState.enemyTarget;
            }
            else
            {
                forcedTarget = true;
                enemyTarget = comp.deathSquadForcedTarget;
                //Log.Message("AIFight target = " + enemyTarget.LabelCap);
            }

            if (enemyTarget == null)
            {
                //Log.Message("EnemyTarger == NULL");
                return null;
            }
            Pawn pawn2 = enemyTarget as Pawn;
            if (pawn2 != null && pawn2.IsPsychologicallyInvisible())
            {
                clearForcedTarget(pawn, forcedTarget);
                return null;
            }
            bool allowManualCastWeapons = !pawn.IsColonist;
            Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            if (verb == null)
            {
                clearForcedTarget(pawn, forcedTarget);
                return null;
            }
            if (verb.verbProps.IsMeleeAttack && enemyTarget.Position.InAllowedArea(pawn))
            {
                return this.MeleeAttackJob(pawn, enemyTarget);
            }
            bool flag = CoverUtility.CalculateOverallBlockChance(pawn, enemyTarget.Position, pawn.Map) > 0.01f;
            bool flag2 = pawn.Position.Standable(pawn.Map) && pawn.Map.pawnDestinationReservationManager.CanReserve(pawn.Position, pawn, pawn.Drafted);
            bool flag3 = verb.CanHitTarget(enemyTarget);
            bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;
            if ((flag && flag2 && flag3) || (flag4 && flag3))
            {
                //Log.Message(">> "+pawn.LabelCap+" Focus on "+enemyTarget.LabelCap);
                return JobMaker.MakeJob(JobDefOf.AttackStatic, enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, false);
            }

            //Not allowed coord
            if (!selVec.IsValid || !selVec.InAllowedArea(pawn))
            {
                clearForcedTarget(pawn, forcedTarget);
                return null;
            }

            if (selVec == pawn.Position)
            {
                return JobMaker.MakeJob(JobDefOf.AttackStatic, enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, false);
            }
            //Log.Message("GOTO_AIFIGHT "+pawn.LabelCap);
            Job job = JobMaker.MakeJob(JobDefOf.Goto, selVec);
            job.expiryInterval = 500;
            job.checkOverrideOnExpire = false;
            return job;
        }

        private void clearForcedTarget(Pawn pawn, bool allow)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();

            if (comp != null && allow)
            {
                comp.deathSquadForcedTarget = null;
            }
        }

        protected override Thing FindAttackTarget(Pawn pawn)
        {
            Thing thing = null;
            Utils.FindAttackTarget(pawn, out selVec, out thing,false);

            return thing;
        }

        protected override Job MeleeAttackJob(Pawn pawn, Thing enemyTarget)
        {
            Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, enemyTarget);
            job.expiryInterval = ExpiryInterval_Melee.RandomInRange;
            job.checkOverrideOnExpire = false;
            job.killIncappedTarget = Settings.killDownedThreats;
            job.expireRequiresEnemiesNearby = true;
            return job;
        }

        private IntVec3 selVec = IntVec3.Invalid;
        private static readonly new IntRange ExpiryInterval_Melee = new IntRange(360, 480);
    }
}
