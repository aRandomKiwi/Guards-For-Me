using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class JobGiver_WanderNearVIP : JobGiver_Wander
    {
        public JobGiver_WanderNearVIP()
        {
            this.wanderRadius = 3f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
            this.wanderDestValidator = delegate (Pawn p, IntVec3 c, IntVec3 root)
            {
                if (this.MustUseRootRoom(p))
                {
                    Room room = root.GetRoom(p.Map, RegionType.Set_Passable);
                    if (room != null && !WanderRoomUtility.IsValidWanderDest(p, c, root))
                    {
                        return false;
                    }
                }
                return true;
            };
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            return WanderUtility.BestCloseWanderRoot(comp.guardedPawn.PositionHeld, pawn);
        }

        private bool MustUseRootRoom(Pawn pawn)
        {
            return true;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            bool flag = pawn.CurJob != null && pawn.CurJob.def == Utils.gotoCombatJobDef;
            bool nextMoveOrderIsWait = pawn.mindState.nextMoveOrderIsWait;
            if (!flag)
            {
                pawn.mindState.nextMoveOrderIsWait = !pawn.mindState.nextMoveOrderIsWait;
            }
            if (nextMoveOrderIsWait && !flag)
            {
                Job job = JobMaker.MakeJob(JobDefOf.Wait_Combat);
                job.expiryInterval = this.ticksBetweenWandersRange.RandomInRange;
                return job;
            }
            IntVec3 exactWanderDest = this.GetExactWanderDest(pawn);
            if (!exactWanderDest.IsValid)
            {
                pawn.mindState.nextMoveOrderIsWait = false;
                return null;
            }
            Job job2 = JobMaker.MakeJob(Utils.gotoCombatJobDef, exactWanderDest);
            job2.locomotionUrgency = this.locomotionUrgency;
            job2.expiryInterval = this.expiryInterval;
            job2.checkOverrideOnExpire = true;
            return job2;
        }
    }
}
