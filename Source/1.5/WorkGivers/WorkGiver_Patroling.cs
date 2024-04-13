﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class WorkGiver_Patroling : WorkGiver
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            bool ret = !ThinkNode_ConditionalShouldPatrol.ShouldPatrol(pawn);

            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return true;

            if (comp.guardJobOK == 2 && !ret)
            {
                comp.guardJobOK = 3;
                ret = true;
            }

            return ret;
        }

        public override Job NonScanJob(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp.guardJobOK == 0)
            {
                //Log.Message("DO patroling JOB");
                comp.guardJobOK = 1;
                pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
                pawn.jobs.ClearQueuedJobs();
                pawn.jobs.StopAll();
                pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);

                ThinkNode_ConditionalShouldPatrol ret = pawn.thinker.GetMainTreeThinkNode<ThinkNode_ConditionalShouldPatrol>();
                if (ret != null)
                {
                    ThinkResult tr = ret.TryIssueJobPackage(pawn, default(JobIssueParams));
                    if (tr != null)
                    {
                        return tr.Job;
                    }
                }
            }
            return null;
        }
    }
}
