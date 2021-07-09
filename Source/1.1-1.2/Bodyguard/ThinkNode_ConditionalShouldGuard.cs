using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class ThinkNode_ConditionalShouldGuard : ThinkNode_Conditional
    {
        public ThinkNode_ConditionalShouldGuard()
        {
        }

        protected override bool Satisfied(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return false;

            return (!Settings.guardAsJob || (Settings.guardAsJob && comp.guardJobOK == 1)) && ThinkNode_ConditionalShouldGuard.ShouldGuard(pawn);
        }

        public static bool ShouldGuard(Pawn pawn)
        {
            if (!pawn.Spawned || pawn.playerSettings == null || (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy) )
            {
                return false;
            }

            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null || ThinkNode_ConditionalShouldSearchAndKill.ShouldSearchAndKill(pawn))
                return false;

            return comp.guardedPawn != null
                && comp.guardedPawn.Spawned
                && !comp.guardedPawn.Dead
                && pawn.Awake()
                && comp.guardedPawn.Awake()
                && pawn.CanReach(comp.guardedPawn, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
        }
    }
}
