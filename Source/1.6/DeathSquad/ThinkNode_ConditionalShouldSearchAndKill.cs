using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class ThinkNode_ConditionalShouldSearchAndKill : ThinkNode_Conditional
    {
        public ThinkNode_ConditionalShouldSearchAndKill()
        {
        }

        protected override bool Satisfied(Pawn pawn)
        {
            bool ret = false;
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return false;

            ret = (!Settings.guardAsJob || (Settings.guardAsJob && comp.guardJobOK == 1)) && ThinkNode_ConditionalShouldSearchAndKill.ShouldSearchAndKill(pawn);


            //Log.Message("Satisfied => " + ret+" "+ comp.guardJobOK+" "+pawn.LabelCap);
            return ret;
        }


        public static bool ShouldSearchAndKill(Pawn pawn)
        {
            
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            int CGT = Find.TickManager.TicksGame;
            bool validCacheButNoTarget = (comp.cachedAttackTarget == null && comp.cachedAttackTargetGT > CGT);

            /*if (effectiveAreaRestrictionInPawnCurrentMap == null)
            {
                enemyPresent = Utils.AnyHostileActiveThreatToPlayer(pawn.Map);
            }*/

            if (comp == null || !comp.DeathSquadMode()
               || (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                || (pawn.health != null && pawn.health.summaryHealth.SummaryHealthPercent <= 0.55f && !GenAI.EnemyIsNear(pawn, 55f))
                || Utils.guardNeedFood(pawn)
                || Utils.guardNeedJoy(pawn)
                || Utils.guardNeedMood(pawn)
                || Utils.guardNeedRest(pawn)
                || Utils.guardNeedBladder(pawn)
                || Utils.guardNeedHygiene(pawn)
                || !Utils.enemyInAllowedArea(pawn)
                || validCacheButNoTarget)
            {
                //Log.Message(pawn.LabelCap+" "+ (!Utils.enemyInAllowedArea(pawn)));
                return false;
            }

            if (pawn.Drafted) return false;

            return true;
        }
    }
}
