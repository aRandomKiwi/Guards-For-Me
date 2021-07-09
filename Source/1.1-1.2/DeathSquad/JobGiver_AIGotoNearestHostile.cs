using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class JobGiver_AIGotoNearestHostile : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            //Log.Message("<HHHHHHHH>");
            Thing thing = null;
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            bool isMeleeAttack = false;
            IntVec3 selVec = IntVec3.Invalid;

            //We force the target if necessary
            if (comp != null && comp.deathSquadForcedTarget != null)
            {
                thing = comp.deathSquadForcedTarget;
                //Log.Message("GotoNearest target = " + thing.LabelCap);
            }
            else
            {
                Utils.FindAttackTarget(pawn,out selVec,out thing, false);
            }
            if (thing != null)
            {
                Job job = null;

                if (isMeleeAttack)
                {
                    job = JobMaker.MakeJob(JobDefOf.Goto, thing);
                    job.checkOverrideOnExpire = false;
                    job.expiryInterval = 500;
                    job.collideWithPawns = true;
                }
                else
                {
                    //Log.Message("GOTO JOB "+pawn.LabelCap);
                    
                    if (!selVec.IsValid || selVec == pawn.Position)
                    {
                        return null;
                    }
                    else
                    {
                        job = JobMaker.MakeJob(JobDefOf.Goto, selVec);
                        job.expiryInterval = 500;
                        job.checkOverrideOnExpire = false;
                    }
                }
                return job;
            }
            return null;
        }

        
    }
}
