using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System;

namespace aRandomKiwi.GFM
{
    internal class HuntJobUtility_Patch
    {
        [HarmonyPatch(typeof(HuntJobUtility), "WasKilledByHunter")]
        public class HuntJobUtility_WasKilledByHunter
        {
            [HarmonyPostfix]
            static public void Listener(Pawn pawn, DamageInfo? dinfo, ref bool __result)
            {
                try
                {
                    if (!Settings.autoUnforbidThreatsKilledByGuards)
                        return;

                    if (dinfo != null)
                    {
                        Pawn pawn2 = dinfo.Value.Instigator as Pawn;
                        if (pawn2 == null)
                            return;
                        Comp_Guard comp = pawn2.TryGetComp<Comp_Guard>();
                        if (comp == null)
                            return;

                        if (comp.isActiveGuard())
                        {
                            __result = true;
                        }
                    }
                }
                catch(Exception e){
                    Log.Message("[GFM] HuntJobUtility.WasKilledByHunter "+e.Message);
                }
            }
        }
    }
}