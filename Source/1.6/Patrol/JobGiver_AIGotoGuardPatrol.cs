using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class JobGiver_AIGotoPatrolWaypoint : JobGiver_AIFightEnemy
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            //Log.Message(">A1");
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return null;

            //Log.Message(">A2");
            Building_PatrolWaypoint wp = comp.curPatrolWP;
            if (wp == null)
                return null;

            //Log.Message(">A3");
            if (comp.targetPatrolWP.IsValid &&  pawn.Position == comp.targetPatrolWP)
            {
                comp.firstWPReached = true;
                comp.targetPatrolWP = IntVec3.Invalid;
                //Log.Message("REACHED");

                //If in loopback mode first element
                if (Settings.pathEndMode == 1)
                {
                    //We go to the next waypoint
                    if (comp.curPatrolWP.next == null)
                    {
                        //search for node 0
                        foreach (var el in pawn.Map.listerBuildings.allBuildingsColonist)
                        {
                            if(el.def == comp.curPatrolWP.def)
                            {
                                Building_PatrolWaypoint build = (Building_PatrolWaypoint)el;
                                if( build.index == 0)
                                {
                                    comp.curPatrolWP = build;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        comp.curPatrolWP = comp.curPatrolWP.next;
                    }
                }
                else
                {
                    if (comp.patrolMoveForward)
                    {
                        //We go to the next waypoint
                        if (comp.curPatrolWP.next == null)
                        {
                            comp.patrolMoveForward = false;
                            comp.curPatrolWP = comp.curPatrolWP.prev;
                        }
                        else
                        {
                            comp.curPatrolWP = comp.curPatrolWP.next;
                        }
                    }
                    else
                    {
                        //We go to the next waypoint
                        if (comp.curPatrolWP.prev == null)
                        {
                            comp.patrolMoveForward = true;
                            comp.curPatrolWP = comp.curPatrolWP.next;
                        }
                        else
                        {
                            comp.curPatrolWP = comp.curPatrolWP.prev;
                        }
                    }
                }

                //return null;
            }

            if(!comp.targetPatrolWP.IsValid)
            {
                //Log.Message("LA4");
                //Coordinated deducition near the reachable waypoint (within a maximum radius of 10)
                if (!pawn.CanReach(comp.curPatrolWP.Position, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn)
                    || !pawn.Map.pawnDestinationReservationManager.CanReserve(comp.curPatrolWP.Position, pawn))
                {
                    int i = 1;
                    //Sequential search of free position by enlarging the radius
                    while (i != 10)
                    {
                        if (CellFinder.TryFindRandomCellNear(comp.curPatrolWP.Position, pawn.Map, i, (IntVec3 x) => pawn.CanReach(x, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn) && pawn.Map.pawnDestinationReservationManager.CanReserve(x, pawn), out comp.targetPatrolWP))
                            break;
                        i++;
                    }

                    if(!comp.targetPatrolWP.IsValid)
                        return null;
                }
                else
                    comp.targetPatrolWP = comp.curPatrolWP.Position;
            }

            //Log.Message("GOTO TARGET");

            LocomotionUrgency cur;
            if (!comp.firstWPReached)
            {
                cur = LocomotionUrgency.Sprint;
            }
            else
            {
                cur = (LocomotionUrgency)Settings.patrolWalkMode;
            }

            this.UpdateEnemyTarget(pawn);
            Thing enemyTarget = pawn.mindState.enemyTarget;
            Pawn pawn2 = enemyTarget as Pawn;
            bool allowManualCastWeapons = !pawn.IsColonist;
            Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);

            //If threat is near then we take care of it
            if (enemyTarget != null && !(pawn2 != null && pawn2.IsPsychologicallyInvisible()) && verb != null)
            {
                selVec = IntVec3.Invalid;
                if (verb.verbProps.IsMeleeAttack)
                {
                    return this.MeleeAttackJob(pawn, enemyTarget);
                }
                bool flag = CoverUtility.CalculateOverallBlockChance(pawn, enemyTarget.Position, pawn.Map) > 0.01f;
                bool flag2 = pawn.Position.Standable(pawn.Map) && pawn.Map.pawnDestinationReservationManager.CanReserve(pawn.Position, pawn, pawn.Drafted);
                bool flag3 = verb.CanHitTarget(enemyTarget);
                bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;
                if ((flag && flag2 && flag3) || (flag4 && flag3))
                {
                    return JobMaker.MakeJob(JobDefOf.AttackStatic, enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, false);
                }

                if (!Utils.TryFindShootingPosition(pawn, out selVec, enemyTarget))
                {
                    selVec = IntVec3.Invalid;
                }

                //If coordinates received wrong OR Coordinates received good check but guards cannot access them (allowedArea)
                if (!selVec.IsValid || !selVec.InAllowedArea(pawn))
                {
                    //Tentative de check si il peut atteindre depuis sa position l'enemis
                    if (verb.CanHitTarget(enemyTarget))
                    {
                        selVec = pawn.Position;
                    }
                    else
                    {
                        selVec = IntVec3.Invalid;
                    }
                }

                if (selVec == pawn.Position)
                {
                    return JobMaker.MakeJob(JobDefOf.AttackStatic, enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, false);
                }

                /*if (selVec.IsValid)
                    return null;
                else
                {*/
                    Job job = JobMaker.MakeJob(Utils.gotoCombatJobDef, selVec);
                    job.expiryInterval = 500;
                    job.checkOverrideOnExpire = false;
                    return job;
                //}
                 
            }

            //Otherwise we continue the patrol job
            return new Job(Utils.gotoCombatJobDef, comp.targetPatrolWP)
            {
                checkOverrideOnExpire = false,
                expiryInterval = 500,
                collideWithPawns = true,
                locomotionUrgency = cur
            };
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
            return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = verb,
                maxRangeFromTarget = verb.verbProps.range,
                wantCoverFromTarget = (verb.verbProps.range > 5f)
            }, out dest);
        }

        protected override Thing FindAttackTarget(Pawn pawn)
        {
            Thing thing = null;
            //Dont check allowed area
            Utils.FindAttackTarget(pawn, out selVec, out thing, true, false);

            return thing;
        }

        protected override Job MeleeAttackJob(Pawn pawn, Thing enemyTarget)
        {
            Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, enemyTarget);
            job.expiryInterval = ExpiryInterval_Melee.RandomInRange;
            job.checkOverrideOnExpire = true;
            job.killIncappedTarget = Settings.killDownedThreats;
            job.expireRequiresEnemiesNearby = true;
            return job;
        }

        private IntVec3 selVec;
        private static readonly new IntRange ExpiryInterval_Melee = new IntRange(360, 480);
    }
}
