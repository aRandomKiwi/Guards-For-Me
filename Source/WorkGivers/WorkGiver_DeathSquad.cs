using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class WorkGiver_DeathSquad : WorkGiver
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            //Log.Message("DEATHSQUAD " + pawn.Label);
            bool ret = !ThinkNode_ConditionalShouldSearchAndKill.ShouldSearchAndKill(pawn);

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

            //Attribution Reaction force if no JOB OR if a guard / patrol in progress
            if (comp.guardJobOK == 0 || (comp.guardJobOK == 1 && (comp.GuardMode() || comp.affectedPatrol != "" )))
            {
                //Log.Message("REACTION FORCE " + pawn.LabelCap + " " + comp.guardJobOK);
                //Log.Message("DO SEARCH and KILL JOB");
                comp.guardJobOK = 1;
                pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
                pawn.jobs.ClearQueuedJobs();
                pawn.jobs.StopAll();
                pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);

                ThinkNode_ConditionalShouldSearchAndKill ret = pawn.thinker.GetMainTreeThinkNode<ThinkNode_ConditionalShouldSearchAndKill>();
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
