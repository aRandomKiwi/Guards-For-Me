using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class JobGiver_AIFightEnemiesNearGuardSpot : JobGiver_AIFightEnemy
    {
        private Pawn curPawn;


        protected override void UpdateEnemyTarget(Pawn pawn)
        {
            this.curPawn = pawn;
            base.UpdateEnemyTarget(pawn);
        }

        protected override Job MeleeAttackJob(Thing enemyTarget)
        {
            Comp_Guard comp = this.curPawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return null;

            Building_GuardSpot gs = comp.getAffectedGuardSpot();
            if (gs != null && gs.Position.DistanceTo(enemyTarget.Position) > Settings.meleeAttackRadiusSpot )
            {
                return null;
            }
            else
            {
                Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, enemyTarget);
                job.expiryInterval = ExpiryInterval_Melee.RandomInRange;
                job.checkOverrideOnExpire = true;
                job.killIncappedTarget = Settings.killDownedThreats;
                job.expireRequiresEnemiesNearby = true;
                return job;
            }
                
        }

        protected override Thing FindAttackTarget(Pawn pawn)
        {
            Thing thing = null;
            Utils.FindAttackTarget(pawn, out selVec, out thing, true, true);

            return thing;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            selVec = IntVec3.Invalid;
            //Log.Message("FIGHT NEAR GS ? "+pawn.LabelCap);
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
                //Log.Message("Attack lanced !!!");
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
                return null;

            //check if target in the guard's authorization zone
            if (!selVec.InAllowedArea(pawn))
            {
                //Attempt to check if he can reach the enemy from his position
                if (verb.CanHitTarget(enemyTarget))
                {
                    selVec = pawn.Position;
                }
                else
                {
                    return null;
                }
            }

            if (selVec == pawn.Position)
            {
                return JobMaker.MakeJob(JobDefOf.AttackStatic, enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, false);
            }
            Job job = JobMaker.MakeJob(JobDefOf.Goto, selVec);
            job.expiryInterval = 500;
            job.checkOverrideOnExpire = true;
            return job;
        }

        protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest, Verb verbToUse = null)
        {
            Thing enemyTarget = pawn.mindState.enemyTarget;
            bool allowManualCastWeapons = !pawn.IsColonist;
            Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            if (verb == null)
            {
                dest = IntVec3.Invalid;
                return false;
            }
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();

            Building_GuardSpot gs = comp.getAffectedGuardSpot();
            bool wantCover;

            if (Settings.fixedStandGuard)
                wantCover = false;
            else
                wantCover = (verb.verbProps.range > 5f) && gs.Position.DistanceTo(pawn.Position) < 10;

            bool ret = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = verb,
                maxRangeFromTarget = verb.verbProps.range,
                wantCoverFromTarget = wantCover
            }, out dest);

            if (!dest.IsValid || Settings.fixedStandGuard || gs.Position.DistanceTo(dest) > 10)
            {
                if (pawn.Position == gs.Position)
                {
                    if (gs.direction == "bottom")
                        pawn.Rotation = Rot4.South;
                    else if (gs.direction == "top")
                        pawn.Rotation = Rot4.North;
                    else if (gs.direction == "left")
                        pawn.Rotation = Rot4.West;
                    else if (gs.direction == "right")
                        pawn.Rotation = Rot4.East;
                }
                dest = gs.Position;
                ret = true;
            }

            return ret;
        }
        
        private IntVec3 selVec;
        private static readonly new IntRange ExpiryInterval_Melee = new IntRange(360, 480);
    }
}
