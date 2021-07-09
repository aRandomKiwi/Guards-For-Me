using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.GFM
{
    internal class PawnRenderer_Patch
    {
        [HarmonyPatch(typeof(PawnRenderer), "CarryWeaponOpenly")]
        public class CarryWeaponOpenly
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, ref bool __result)
            {
                Comp_Guard comp = ___pawn.TryGetComp<Comp_Guard>();
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