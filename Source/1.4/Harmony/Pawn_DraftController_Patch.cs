using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class Pawn_DraftController_Patch
    {
        [HarmonyPatch(typeof(Pawn_DraftController), "set_Drafted")]
        public class set_Drafted
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, bool value)
            {
                if (!Settings.guardAsJob || !value)
                    return;
                Comp_Guard comp = ___pawn.TryGetComp<Comp_Guard>();
                if(comp != null && comp.guardJobOK == 1){
                    comp.guardJobOK = 0;
                }
            }
        }
    }
}