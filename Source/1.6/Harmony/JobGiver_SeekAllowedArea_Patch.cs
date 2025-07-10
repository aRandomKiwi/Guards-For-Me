using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class JobGiver_SeekAllowedArea_Patch
    {
        [HarmonyPatch(typeof(JobGiver_SeekAllowedArea), "TryGiveJob")]
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

                if(comp != null && ((Settings.guardAsJob && (comp.guardJobOK == 1 || comp.guardJobOK == 2)) || !Settings.guardAsJob) && ( comp.affectedPatrol != "" || comp.DeathSquadMode()))
                {
                    __result = null;
                    return false;
                }

                return true;
            }
        }
    }
}