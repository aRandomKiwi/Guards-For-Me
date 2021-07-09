using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class ThinkNode_ConditionalShouldPatrol : ThinkNode_Conditional
    {
        public ThinkNode_ConditionalShouldPatrol()
        {
        }

        protected override bool Satisfied(Pawn pawn)
        {
            bool ret = ShouldPatrol(pawn);

            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return false;

            return (!Settings.guardAsJob || (Settings.guardAsJob && comp.guardJobOK == 1)) && ret;
        }


        public static bool ShouldPatrol(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();

            if (comp == null || comp.affectedPatrol == ""
                || (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                || (pawn.health != null && pawn.health.summaryHealth.SummaryHealthPercent <= 0.55f && !GenAI.EnemyIsNear(pawn, 55f))
                || Utils.guardNeedFood(pawn)
                || Utils.guardNeedJoy(pawn)
                || Utils.guardNeedMood(pawn)
                || Utils.guardNeedRest(pawn)
                || Utils.guardNeedBladder(pawn)
                || ThinkNode_ConditionalShouldSearchAndKill.ShouldSearchAndKill(pawn)
                || Utils.guardNeedHygiene(pawn))
            {
                return false;
            }

            //Log.Message("<C");
            if (pawn.Drafted) return false;

            //Calculation of the closest waypoint if not currently on a path so that the colonist can join it
            if (comp.curPatrolWP == null)
            {
                Building_PatrolWaypoint sel = null;
                float dist = -1;

                foreach(var build in pawn.Map.listerBuildings.allBuildingsColonist)
                {
                    if(build.def.defName == comp.affectedPatrol)
                    {
                        float tmp = pawn.Position.DistanceTo(build.Position);
                        if(dist == -1 || tmp < dist)
                        {
                            //Log.Message("Initial FOund ");
                            sel = (Building_PatrolWaypoint)build;
                            dist = tmp;
                        }
                    }
                }

                if (sel == null)
                    return false;

                comp.curPatrolWP = sel;
            }
            comp.firstWPReached = false;

            return true;
        }
    }
}
