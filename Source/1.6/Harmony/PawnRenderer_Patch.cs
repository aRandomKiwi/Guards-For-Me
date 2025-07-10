using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class PawnRenderer_Patch
    {
        [HarmonyPatch(typeof(PawnRenderUtility), "CarryWeaponOpenly")]
        public class CarryWeaponOpenly
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, ref bool __result)
            {
                   Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
                if(comp != null){
                    if( comp.isActiveGuard() )
                    {
                        __result = true;
                    }
                }
            }
        }
    }
}