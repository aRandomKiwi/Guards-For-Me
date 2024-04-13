using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class JobGiver_ReactToCloseMeleeThreat_Patch
    {
        [HarmonyPatch(typeof(JobGiver_ReactToCloseMeleeThreat), "TryGiveJob")]
        public class JobGiver_SeekAllowedArea_TryGiveJob
        {
            [HarmonyPrefix]
            static public bool Listener(Pawn pawn, Job __result)
            {
                if (pawn == null)
                    return true;
                Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
                if (comp == null)
                    return true;

                //no escape possible if guard
                if (comp.isActiveGuard())
                {
                    __result = null;
                    return false;
                }

                return true;
            }
        }
    }
}