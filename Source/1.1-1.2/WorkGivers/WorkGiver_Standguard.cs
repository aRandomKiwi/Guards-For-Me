using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class WorkGiver_Standguard : WorkGiver
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            //Log.Message("WGGGGGGGGGGGGGG " + pawn.LabelCap);
            bool ret =  !ThinkNode_ConditionalShouldGuardSpot.ShouldGuardSpot(pawn);
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return true;

            if (!ret)
                comp.touchGuardSpotGT();
            else
                comp.clearGuardSpotGT();

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
            //Log.Message("WGGGGSSS " + pawn.LabelCap+" "+ comp.guardJobOK);
            if (comp.guardJobOK == 0)
            {
                //Log.Message("DO standingguard JOB");
                comp.guardJobOK = 1;
                pawn.jobs.StopAll();
                pawn.jobs.ClearQueuedJobs();

                ThinkNode_ConditionalShouldGuardSpot ret = pawn.thinker.GetMainTreeThinkNode<ThinkNode_ConditionalShouldGuardSpot>();
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
