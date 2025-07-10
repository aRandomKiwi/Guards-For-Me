using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class Pawn_HealthTracker_Patch
    {
        [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
        public class MakeDowned
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff)
            {
                //If guardian downed we stop our job
                Comp_Guard comp = ___pawn.TryGetComp<Comp_Guard>();
                if(comp != null)
                {
                    if (comp.isGuardian())
                    {
                        ___pawn.jobs.StopAll();
                        ___pawn.jobs.ClearQueuedJobs();
                        comp.guardedPawn = null;
                    }
                }
            }
        }
    }
}