using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System;

namespace aRandomKiwi.GFM
{
    internal class JobGiver_ConfigurableHostilityResponse_Patch
    {
        [HarmonyPatch(typeof(JobGiver_ConfigurableHostilityResponse), "TryGiveJob")]
        public class JobGiver_ConfigurableHostilityResponse_TryGiveJob
        {
            [HarmonyPrefix]
            static public bool Listener(Pawn pawn, Job __result)
            {
                try
                {
                    if (pawn == null)
                        return true;
                    Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
                    if (comp == null)
                        return true;

                    //no escape possible if guard
                    //Log.Message(pawn.LabelCap+" "+ comp.guardJobOK);
                    if (comp.isActiveGuard())
                    {
                        __result = null;
                        return false;
                    }

                    return true;
                }
                catch(Exception )
                {
                    return true;
                }
            }
        }
    }
}