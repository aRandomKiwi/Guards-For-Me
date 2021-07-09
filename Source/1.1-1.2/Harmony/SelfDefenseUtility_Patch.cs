using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class SelfDefenseUtility_Patch
    {
        [HarmonyPatch(typeof(SelfDefenseUtility), "ShouldStartFleeing")]
        public class ShouldStartFleeing
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, ref bool __result)
            {
                Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
                if(comp != null){
                    if (comp.guardSpotModeGT > Find.TickManager.TicksGame 
                        || (comp.DeathSquadMode() && (!Settings.guardAsJob || (Settings.guardAsJob && comp.guardJobOK == 1))) )
                        __result = false;
                }
            }
        }
    }
}