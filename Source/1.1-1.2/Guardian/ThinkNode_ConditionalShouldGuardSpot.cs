using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class ThinkNode_ConditionalShouldGuardSpot : ThinkNode_Conditional
    {
        public ThinkNode_ConditionalShouldGuardSpot()
        {
        }

        protected override bool Satisfied(Pawn pawn)
        {
            bool ret = ShouldGuardSpot(pawn);

            //Soldier weapon animation definition
            //if(ret)
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return false;

            ret = (!Settings.guardAsJob || (Settings.guardAsJob && comp.guardJobOK == 1)) && ret;

            if (ret)
                comp.touchGuardSpotGT();
            else
                comp.clearGuardSpotGT();

            return ret;
        }


        public static bool ShouldGuardSpot(Pawn pawn)
        {
            
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();

            if (comp == null || !comp.GuardMode()
               || (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                || (pawn.health != null && pawn.health.summaryHealth != null && pawn.health.summaryHealth.SummaryHealthPercent <= 0.55f && !GenAI.EnemyIsNear(pawn, 55f))
                || ThinkNode_ConditionalShouldSearchAndKill.ShouldSearchAndKill(pawn)
                || Utils.guardNeedFood(pawn)
                || Utils.guardNeedJoy(pawn)
                || Utils.guardNeedMood(pawn)
                || Utils.guardNeedRest(pawn)
                || Utils.guardNeedBladder(pawn)
                || Utils.guardNeedHygiene(pawn))
            {
                //Log.Message("NOPE FOR " + pawn.LabelCap);
                return false;
            }

            if (pawn.Drafted) return false;

            //If reserved GS then it's ok
            Building_GuardSpot sgs = comp.getAffectedGuardSpot();
            if ( sgs != null)
            {
                if (!pawn.CanReach(sgs.Position, PathEndMode.OnCell, Danger.Deadly) || !sgs.Position.InAllowedArea(pawn))
                {
                    if (comp.affectedGS == sgs)
                    {
                        comp.affectedGS.clearTempGuard();
                        comp.clearAffectedGuardSpot();
                    }

                    return false;
                }
                else
                    return true;
            }
            //Attempt defition a temporary
            Building_GuardSpot gs = Utils.GCGFM.getRandomFreeGuardSpot(pawn.Map, pawn);
            if (gs == null || gs.Destroyed)
                return false;

            comp.setAffectedGuardSpot(gs);

            return true;
        }
    }
}
