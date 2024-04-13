using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class JobGiver_IdleCombat : ThinkNode_JobGiver
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_Idle jobGiver_Idle = (JobGiver_Idle)base.DeepCopy(resolve);
            jobGiver_Idle.ticks = this.ticks;
            return jobGiver_Idle;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (comp != null)
            {
                Building_GuardSpot gs = comp.getAffectedGuardSpot();

                if (gs != null && pawn.Position == gs.Position)
                {
                    if (gs.direction == "bottom")
                        pawn.Rotation = Rot4.South;
                    else if (gs.direction == "top")
                        pawn.Rotation = Rot4.North;
                    else if (gs.direction == "left")
                        pawn.Rotation = Rot4.West;
                    else if (gs.direction == "right")
                        pawn.Rotation = Rot4.East;
                }
            }

            return new Job(JobDefOf.Wait_Combat)
            {
                expiryInterval = this.ticks,
                canBashDoors = true,
                canBashFences = true,
                checkOverrideOnExpire = true

            };
        }

        public int ticks = 90;
    }
}