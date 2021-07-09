using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    class JobGiver_AIFollowVIP : JobGiver_AIFollowPawn
    {
        public const float RadiusUnreleased = 3f;

        public const float RadiusReleased = 50f;

        public JobGiver_AIFollowVIP()
        {
        }

        protected override int FollowJobExpireInterval
        {
            get
            {
                return 200;
            }
        }

        protected override Pawn GetFollowee(Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            return comp.guardedPawn;
        }

        protected override float GetRadius(Pawn pawn)
        {
/*            if (pawn.playerSettings.Master.playerSettings.animalsReleased && pawn.training.HasLearned(TrainableDefOf.Release))
            {
                return 50f;
            }*/
            return 8f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Pawn followee = this.GetFollowee(pawn);
            if (followee == null)
            {
                Log.Error(base.GetType() + " has null followee. pawn=" + pawn.ToStringSafe<Pawn>(), false);
                return null;
            }
            if (!followee.Spawned || !pawn.CanReach(followee, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                return null;
            }
            float radius = this.GetRadius(pawn);
            if (!JobDriver_FollowClose.FarEnoughAndPossibleToStartJob(pawn, followee, radius))
            {
                return null;
            }
            Job job = JobMaker.MakeJob(Utils.followCloseCombatJobDef, followee);
            job.expiryInterval = this.FollowJobExpireInterval;
            job.checkOverrideOnExpire = true;
            job.followRadius = radius;
            return job;
        }
    }
}
