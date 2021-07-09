using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class WorkGiver_Bodyguard : WorkGiver
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            //Log.Message("BODYGUARD "+pawn.Label);
            bool ret = !ThinkNode_ConditionalShouldGuard.ShouldGuard(pawn);
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
            //Log.Message("BODYGUARD " + pawn.Label);
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp.guardJobOK == 0)
            {
                //Log.Message("DO bodyguard JOB");
                comp.guardJobOK = 1;
                pawn.jobs.StopAll();
                pawn.jobs.ClearQueuedJobs();

                ThinkNode_ConditionalShouldGuard ret = pawn.thinker.GetMainTreeThinkNode<ThinkNode_ConditionalShouldGuard>();
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
