using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class ThinkNode_ConditionalShouldGuardSpotAttack : ThinkNode_Conditional
    {
        public ThinkNode_ConditionalShouldGuardSpotAttack()
        {
        }


        protected override bool Satisfied(Pawn pawn)
        {
            bool ret = ShouldGuardSpotAttack(pawn);

            //Soldier weapon animation definition
            //if(ret) touchGuardSpotGT
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


        public static bool ShouldGuardSpotAttack(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null || !comp.GuardMode()
                || (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                || (pawn.health != null && pawn.health.summaryHealth != null && pawn.health.summaryHealth.SummaryHealthPercent <= 0.55f && !GenAI.EnemyIsNear(pawn, 55f))
                || ThinkNode_ConditionalShouldSearchAndKill.ShouldSearchAndKill(pawn)
                || Utils.guardNeedFood(pawn)
                || Utils.guardNeedMood(pawn)
                || Utils.guardNeedJoy(pawn)
                || Utils.guardNeedRest(pawn)
                || Utils.guardNeedBladder(pawn)
                || Utils.guardNeedHygiene(pawn))
                return false;

            if (pawn.Drafted) return false;

            //If reserved GS then it's ok
            Building_GuardSpot gs = comp.getAffectedGuardSpot();
            if (gs == null || gs.Destroyed)
                return false;

            if (gs.Position.DistanceTo(pawn.Position) <= Settings.standingGuardMaxDistanceWithGS)
                return true;
            else
                return false;
        }
    }
}
