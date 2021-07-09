using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class MentalStateHandler_Patch
    {
        [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
        public class TryStartMentalState
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, MentalStateDef stateDef, string reason = null, bool forceWake = false, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false)
            {
                if (___pawn == null)
                    return;

                Comp_Guard comp = ___pawn.TryGetComp<Comp_Guard>();

                if(comp != null && comp.isGuardian())
                {
                    comp.guardedPawn = null;
                }
            }
        }
    }
}