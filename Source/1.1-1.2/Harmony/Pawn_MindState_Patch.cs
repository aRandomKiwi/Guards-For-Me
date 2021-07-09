/*using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class Pawn_MindState_Patch
    {
        [HarmonyPatch(typeof(Pawn_MindState), "Notify_WorkPriorityDisabled")]
        public class Notify_WorkPriorityDisabled
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, WorkTypeDef wType)
            {
                if (!Settings.guardAsJob)
                    return;

                if(wType.defName == "GFM_Guard")
                {
                    Comp_Guard comp = ___pawn.TryGetComp<Comp_Guard>();
                    if (comp != null)
                    {
                        //Job de guarde actuellement en cours on l'arrete
                        if(comp.guardJobOK == 1)
                        {
                            comp.stopCurrentGuardJob();
                        }
                    }
                }
            }
        }
    }
}*/