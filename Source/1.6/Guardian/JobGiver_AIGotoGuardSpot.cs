using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class JobGiver_AIGotoGuardSpot : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp == null)
                return null;

            Building_GuardSpot gs = comp.getAffectedGuardSpot();
            if (gs == null)
                return null;

            if (pawn.Position == gs.Position)
            {
                if (gs.direction == "bottom")
                    pawn.Rotation = Rot4.South;
                else if (gs.direction == "top")
                    pawn.Rotation = Rot4.North;
                else if (gs.direction == "left")
                    pawn.Rotation = Rot4.West;
                else if (gs.direction == "right")
                    pawn.Rotation = Rot4.East;

                return null;
            }

            pawn.CanReach(gs, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn);

            return new Job(JobDefOf.Goto, gs)
            {
                checkOverrideOnExpire = true,
                expiryInterval = 500,
                collideWithPawns = true,
            };
        }
    }
}
