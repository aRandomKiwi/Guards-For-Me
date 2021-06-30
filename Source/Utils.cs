using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using Verse.AI.Group;
using RimWorld.Planet;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    static class Utils
    {
        static public JobDef followCloseCombatJobDef;
        static public JobDef gotoCombatJobDef;
        static public NeedDef needHygiene;
        static public NeedDef needBladder;
        static public NeedDef needEnergy;
        static public bool ANDROIDLOADED = false;
        static public bool CAMERAPLOADED = false;
        static public WorkTypeDef guardWorkType;
        static public ThoughtDef SawnGuard;
        static public bool MFMLOADED = false;
        static public GC_GFM GCGFM;
        static public readonly string[] waypointsDefName = new string[]{ "GFM_WPPatrolAlpha", "GFM_WPPatrolBeta", "GFM_WPPatrolGamma", "GFM_WPPatrolDelta", "GFM_WPPatrolEpsilon", "GFM_WPPatrolZeta", "GFM_WPPatrolEta", "GFM_WPPatrolTheta", "GFM_WPPatrolIota" };
        static public readonly string[] wayPointsLabel = new string[] { "GFM_patrolAlpha", "GFM_patrolBeta", "GFM_patrolGamma", "GFM_patrolDelta", "GFM_patrolEpsilon", "GFM_patrolZeta", "GFM_patrolEta", "GFM_patrolTheta", "GFM_patrolIota" };
        static public readonly string[] guardSpotsLabel = new string[] { "GFM_GuardSpot", "GFM_GuardSpotRed", "GFM_GuardSpotOrange", "GFM_GuardSpotYellow", "GFM_GuardSpotGreen", "GFM_GuardSpotBlue", "GFM_GuardSpotPurple", "GFM_GuardSpotPink", "GFM_GuardSpotGray" };
        static public readonly List<string> workGivers = new List<string>() { "GFM_DoStandguard", "GFM_DoPatrolGuard", "GFM_DoBodyGuard", "GFM_DoDeathSquad" };


        public static string TranslateTicksToTextIRLSeconds(int ticks)
        {
            //If less than one hour ingame then display seconds
            if (ticks < 2500)
                return ticks.ToStringSecondsFromTicks();
            else
                return ticks.ToStringTicksToPeriodVerbose(true);
        }

        public static bool guardNeedRest(Pawn pawn)
        {
            return (pawn.needs.rest != null && pawn.needs.rest.CurLevelPercentage < Settings.minRestStopJob) || (ANDROIDLOADED && chjDroidNeedRest(pawn));
        }

        public static bool guardNeedFood(Pawn pawn)
        {
            return (pawn.needs.food != null && pawn.needs.food.CurLevelPercentage < Settings.minFoodStopJob);
        }

        public static bool guardNeedJoy(Pawn pawn)
        {
            return (pawn.needs.joy != null && pawn.needs.joy.CurLevelPercentage < Settings.minJoyStopJob);
        }

        public static bool guardNeedMood(Pawn pawn)
        {
            return (pawn.needs.mood != null && pawn.needs.mood.CurLevelPercentage < Settings.minMoodStopJob);
        }

        public static bool guardNeedHygiene(Pawn pawn)
        {
            return (needHygiene != null && pawn.needs.TryGetNeed(needHygiene) != null && pawn.needs.TryGetNeed(needHygiene).CurLevelPercentage < Settings.minHygieneStopJob);
        }

        public static bool guardNeedBladder(Pawn pawn)
        {
            return (needBladder != null && pawn.needs.TryGetNeed(needBladder) != null && pawn.needs.TryGetNeed(needBladder).CurLevelPercentage < Settings.minBladderStopJob);
        }

        public static bool chjDroidNeedRest(Pawn pawn)
        {
             foreach (Need need in pawn.needs.AllNeeds)
			 {
				if (need != null && need.def.defName == "ChJEnergy")
				{
					if(need.CurLevelPercentage < Settings.minRestStopJob)
                    {
                        return true;
                    }
				}
			 }
            return false;
        }

        public static void isAnimalHostileFaction(Thing a, Faction fac, ref bool __result)
        {
            try
            {
                if (!Settings.exterminatorMode || !(a is Pawn))   
                    return;
                Pawn p = (Pawn)a;
                if (Settings.exterminatorMode && a is Pawn && p.def.race != null && p.def.race.Animal && p.Faction != Faction.OfPlayer && p.Faction == null)
                    __result = true;
            }
            catch (Exception e)
            {
                Log.Message("[GFM Error] : " + e.Message + " " + e.StackTrace);
            }
        }

        public static void isAnimalHostile(Thing a, Thing b, ref bool ret)
        {
            try
            {
                Comp_Guard comp = a.TryGetComp<Comp_Guard>();
                //If exterminator mode activated and animal breed
                if (Settings.exterminatorMode && comp != null && (b.def.race != null && b.def.race.Animal) && b.Faction != Faction.OfPlayer && b.Faction == null)
                {
                    //If in spot guard or patrol guard mode
                    if ((!Settings.guardAsJob || (Settings.guardAsJob && comp.guardJobOK == 1)) && (comp.GuardMode() || comp.affectedPatrol != ""))
                    {
                        ret = true;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Message("[GFM Error] : " + e.Message + " " + e.StackTrace);
            }
        }

        /*public static void checkGuardWorkTypeDef()
        {
            if (!Settings.guardAsJob)
            {
                DefDatabase<WorkTypeDef>.AllDefs
            }
            else
            {
                if(guardWorkType != null)
                {

                }
            }
        }*/

        public static void getPatrolCommandStuff(string curAffectedPatrol, out Texture2D icon, out string label)
        {
            icon = null;
            label = "";

            switch (curAffectedPatrol)
            {
                case "":
                    label = "GFM_patrol".Translate();
                    icon = Tex.noPatrolMode;
                    break;
                case "GFM_WPPatrolAlpha":
                    icon = Tex.blackWP;
                    label = "GFM_patrolAlpha".Translate();
                    break;
                case "GFM_WPPatrolBeta":
                    icon = Tex.redWP;
                    label = "GFM_patrolBeta".Translate();
                    break;
                case "GFM_WPPatrolGamma":
                    icon = Tex.orangeWP;
                    label = "GFM_patrolGamma".Translate();
                    break;
                case "GFM_WPPatrolDelta":
                    icon = Tex.yellowWP;
                    label = "GFM_patrolDelta".Translate();
                    break;
                case "GFM_WPPatrolEpsilon":
                    icon = Tex.greenWP;
                    label = "GFM_patrolEpsilon".Translate();
                    break;
                case "GFM_WPPatrolZeta":
                    icon = Tex.blueWP;
                    label = "GFM_patrolZeta".Translate();
                    break;
                case "GFM_WPPatrolEta":
                    icon = Tex.purpleWP;
                    label = "GFM_patrolEta".Translate();
                    break;
                case "GFM_WPPatrolTheta":
                    icon = Tex.pinkWP;
                    label = "GFM_patrolTheta".Translate();
                    break;
                case "GFM_WPPatrolIota":
                    icon = Tex.grayWP;
                    label = "GFM_patrolIota".Translate();
                    break;
            }
        }

        public static void getGuardSpotCommandStuff(string curAffectedGS, out Texture2D icon, out string label)
        {
            icon = null;
            label = "";

            switch (curAffectedGS)
            {
                case "":
                    label = "GFM_GuardMode".Translate();
                    icon = Tex.guardMode;
                    break;
                case "GFM_GuardSpot":
                    icon = Tex.blackGS;
                    label = "GFM_GuardSpot".Translate();
                    break;
                case "GFM_GuardSpotRed":
                    icon = Tex.redGS;
                    label = curAffectedGS.Translate();
                    break;
                case "GFM_GuardSpotOrange":
                    icon = Tex.orangeGS;
                    label = curAffectedGS.Translate();
                    break;
                case "GFM_GuardSpotYellow":
                    icon = Tex.yellowGS;
                    label = curAffectedGS.Translate();
                    break;
                case "GFM_GuardSpotGreen":
                    icon = Tex.greenGS;
                    label = curAffectedGS.Translate();
                    break;
                case "GFM_GuardSpotBlue":
                    icon = Tex.blueGS;
                    label = curAffectedGS.Translate();
                    break;
                case "GFM_GuardSpotPurple":
                    icon = Tex.purpleGS;
                    label = curAffectedGS.Translate();
                    break;
                case "GFM_GuardSpotPink":
                    icon = Tex.pinkGS;
                    label = curAffectedGS.Translate();
                    break;
                case "GFM_GuardSpotGray":
                    icon = Tex.grayGS;
                    label = curAffectedGS.Translate();
                    break;
            }
        }

        public static ThingComp TryGetCompByTypeName(ThingWithComps thing, string typeName, string assemblyName = "")
        {
            return thing.AllComps.FirstOrDefault((ThingComp comp) => comp.GetType().Name == typeName);
        }

        public static void FindAttackTarget(Pawn pawn, out IntVec3 selVec, out Thing thing, bool onlyHittableFromPos = false, bool checkAllowedArea = true )
        {
            float num = float.MaxValue;
            float numDowned = float.MaxValue;
            Thing thingDowned = null;
            thing = null;
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            bool isMeleeAttack = false;
            IntVec3 intVec = IntVec3.Invalid;
            selVec = IntVec3.Invalid;

            int CGT = Find.TickManager.TicksGame;

            Pawn targetPawn = comp.cachedAttackTarget as Pawn;

            if (comp.cachedAttackTargetGT != 0 && comp.cachedAttackTargetGT > CGT && comp.cachedAttackTarget.Spawned && (targetPawn == null || !targetPawn.Downed || (targetPawn.Downed && comp.tmpCanKillDowned)))
            {
                thing = comp.cachedAttackTarget;
                selVec = comp.cachedAttackTargetSelVec;

                //Log.Message("DATA PROVIDED BY CACHE");

                return;
            }

            int meleeAttackRadiusSpot = Settings.meleeAttackRadiusSpot * Settings.meleeAttackRadiusSpot;


            List<IAttackTarget> potentialTargetsFor = pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
            for (int i = 0; i < potentialTargetsFor.Count; i++)
            {
                IAttackTarget attackTarget = potentialTargetsFor[i];
                Verb verb = pawn.TryGetAttackVerb(thing, false);
                if (verb == null)
                    continue;

                isMeleeAttack = verb.verbProps.IsMeleeAttack;

                /*if (attackTarget.ThreatDisabled(pawn)){
                Thing thing3 = (Thing)attackTarget;
                Log.Message(thing3.LabelCap + " no longer DANGEROUS FOR " + pawn.LabelCap);
                }*/

                Thing thing2 = (Thing)attackTarget;
                Pawn p2 = thing2 as Pawn;

                if ((!attackTarget.ThreatDisabled(pawn) || (Settings.killDownedThreats && p2 != null && p2.Downed)) && AttackTargetFinder.IsAutoTargetable(attackTarget))
                {

                    if (p2 != null && (!Settings.killDownedThreats && p2 != null && p2.Downed))
                        continue;

                    int num2 = thing2.Position.DistanceToSquared(pawn.Position);

                    /*if (!isMeleeAttack)
                    {

                        if (!TryFindShootingPosition(pawn, out intVec, thing2))
                            intVec = IntVec3.Invalid;
                    }*/

                    //If in mode only enemies within range
                    if (onlyHittableFromPos)
                    {
                        //Melee
                        if (isMeleeAttack)
                        {
                            if (num2 > meleeAttackRadiusSpot)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (!verb.CanHitTarget(thing2))
                            {
                                continue;
                            }
                        }
                    }

                    bool cond = ((isMeleeAttack && (!checkAllowedArea || (checkAllowedArea && thing2.Position.InAllowedArea(pawn))) && pawn.CanReach(thing2, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn)) || (!isMeleeAttack && (!checkAllowedArea || (checkAllowedArea && ( (thing2.Position.InAllowedArea(pawn) && pawn.CanReach(thing2, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn)) || verb.CanHitTarget(thing2))))));

                    //If target pawn and downed
                    if (p2 != null && p2.Downed)
                    {
                        if ((float)num2 < numDowned && cond)
                        {
                            //Log.Message("BLOOOOOP1");
                            numDowned = (float)num2;
                            thingDowned = thing2;
                        }
                    }
                    else
                    {
                        //If melee attack then check reachability otherwise check castposition accessible
                        if ((float)num2 < num && cond)
                        {
                            //Log.Message("BLOOOOOP");
                            num = (float)num2;
                            thing = thing2;
                        }
                    }
                }
            }

            //Check IF the active target (not downed) has no result then we provide the downed found if applicable
            if (num == float.MaxValue && numDowned != float.MaxValue)
            {
                //Log.Message("=>DOWNED FOUND " + thingDowned.LabelCap);
                num = numDowned;
                thing = thingDowned;
            }

            //Effective calculation of the cast position
            if (num != float.MaxValue)
            {
                Verb verb = pawn.TryGetAttackVerb(thing, false);
                if (verb != null && !verb.verbProps.IsMeleeAttack) {

                    if (!TryFindShootingPosition(pawn, out selVec, thing))
                    {
                        selVec = IntVec3.Invalid;
                    }

                    //If coordinates received wrong OR Coordinates received good check but guards cannot access them (allowedArea)
                    if ( !selVec.IsValid ||(checkAllowedArea && !selVec.InAllowedArea(pawn)))
                    {
                        //Attempt to check if he can reach the enemy from his position
                        if (verb.CanHitTarget(thing))
                        {
                            selVec = pawn.Position;
                        }
                        else
                        {
                            //Log.Message("Cancel ALL ("+thing.LabelCap);
                            //We erase the target because no means of access
                            num = float.MaxValue;
                            thing = null;
                            selVec = IntVec3.Invalid;
                        }
                    }
                }
            }

            num = float.MaxValue;
            //If no real threat found and exterminator mode activated then look for an animal to shoot
            if (thing == null && Settings.exterminatorMode)
            {
                numDowned = float.MaxValue;
                thingDowned = null;

                foreach (var cp in pawn.Map.mapPawns.AllPawns)
                {
                    if (!cp.Spawned || !(cp.Faction == null) || !cp.AnimalOrWildMan() || (!Settings.killDownedThreats && cp.Downed))
                        continue;

                    Verb verb = pawn.TryGetAttackVerb(cp, false);
                    if (verb == null)
                        continue;

                    isMeleeAttack = verb.verbProps.IsMeleeAttack;


                    int num2 = cp.Position.DistanceToSquared(pawn.Position);

                    //If in mode only enemies within range
                    if (onlyHittableFromPos)
                    {
                        //Melee
                        if (isMeleeAttack)
                        {
                            if (num2 > meleeAttackRadiusSpot)
                            {
                                //Log.Message("DISTANCE "+num2);
                                continue;
                            }
                        }
                        else
                        {
                            if (!verb.CanHitTarget(cp))
                            {
                                continue;
                            }
                        }
                    }

                    bool cond = ((isMeleeAttack && (!checkAllowedArea || (checkAllowedArea && cp.Position.InAllowedArea(pawn))) && pawn.CanReach(cp, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn)) || (!isMeleeAttack && (!checkAllowedArea || (checkAllowedArea && ((cp.Position.InAllowedArea(pawn) && pawn.CanReach(cp, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn)) || verb.CanHitTarget(cp))))));


                    //If target pawn and downed
                    if (cp != null && cp.Downed)
                    {
                        if ((float)num2 < numDowned && cond)
                        {
                            //Log.Message("BLOOOOOP1");
                            numDowned = (float)num2;
                            thingDowned = cp;
                        }
                    }
                    else
                    {
                        //If melee attack then check reachability otherwise check castposition accessible
                        if ((float)num2 < num && cond)
                        {
                            //Log.Message("BLOOOOOP");
                            num = (float)num2;
                            thing = cp;
                        }
                    }

                }

                //Check IF the active target (not downed) has no result then we provide the downed found if applicable
                if (num == float.MaxValue && numDowned != float.MaxValue)
                {
                    num = numDowned;
                    thing = thingDowned;
                }

                //Effective calculation of the cast position
                if (num != float.MaxValue)
                {
                    Verb verb = pawn.TryGetAttackVerb(thing, false);
                    if (verb != null && !verb.verbProps.IsMeleeAttack)
                    {

                        if (!TryFindShootingPosition(pawn, out selVec, thing))
                        {
                            selVec = IntVec3.Invalid;
                        }

                        //If coordinates received wrong OR Coordinates received good check but guards cannot access them (allowedArea)
                        if (!selVec.IsValid || (checkAllowedArea && !selVec.InAllowedArea(pawn)))
                        {
                            //Attempt to check if he can reach the enemy from his position
                            if (verb.CanHitTarget(thing))
                            {
                                selVec = pawn.Position;
                            }
                            else
                            {
                                //We erase the target because no means of access
                                num = float.MaxValue;
                                thing = null;
                                selVec = IntVec3.Invalid;
                            }
                        }
                    }
                }
            }

            comp.cachedAttackTarget = thing;
            comp.cachedAttackTargetSelVec = selVec;

            Pawn thingPawn = thing as Pawn;

            //We specify that it is a deliberately assigned downed so when it comes out in cache we can kill it
            if (thingPawn != null && thingPawn.Downed)
            {
                comp.tmpCanKillDowned = true;
            }
            else
            {
                comp.tmpCanKillDowned = false;
            }

            if(thing == null)
                comp.cachedAttackTargetGT = CGT;
            else
                comp.cachedAttackTargetGT = CGT + CacheInterval_AttackTarget.RandomInRange;

            /*if (thing == null)
                Log.Message("FindAttackTar NOTHING FOR "+pawn.LabelCap);
            else
                Log.Message("FindAttackTar " + thing.LabelCap + " FOR " + pawn.LabelCap);*/
        }

        public static bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest, Thing enemyTarget)
        {
            bool allowManualCastWeapons = !pawn.IsColonist;
            Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            if (verb == null)
            {
                dest = IntVec3.Invalid;
                return false;
            }
            return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = verb,
                maxRangeFromTarget = verb.verbProps.range,
                wantCoverFromTarget = (verb.verbProps.range > 5f)
            }, out dest);
        }


        public static bool AnyHostileActiveThreatToPlayer(Map map)
        {
            bool ret = GenHostility.AnyHostileActiveThreatToPlayer(map);

            if (ret)
                return ret;

            if (Settings.exterminatorMode)
            {
                foreach (var cp in map.mapPawns.AllPawns)
                {
                    if (!cp.Spawned || !(cp.Faction == null) || !cp.AnimalOrWildMan())
                        continue;

                    return true;
                }

                return ret;
            }
            else
            {
                return ret;
            }
        }

        public static List<Pawn> getColonistsAndAllyAndNeutral(Map map)
        {
            List<Pawn> cur = new List<Pawn>();

            foreach(var el  in map.mapPawns.AllPawnsSpawned)
            {
                if (el.Dead || el.Downed || el.NonHumanlikeOrWildMan() || el.Faction == null)
                    continue;

                if (el.Faction == Faction.OfPlayer)
                {
                    cur.Add(el);
                }
                else
                {
                    FactionRelationKind cr = el.Faction.RelationKindWith(Faction.OfPlayer);
                    if (cr == FactionRelationKind.Ally || cr == FactionRelationKind.Neutral)
                    {
                        cur.Add(el);
                    }
                }
            }

            return cur;
        }

        public static bool enemyInAllowedArea(Pawn pawn)
        {
            List<IAttackTarget> potentialTargetsFor = pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
            for (int i = 0; i < potentialTargetsFor.Count; i++)
            {
                IAttackTarget attackTarget = potentialTargetsFor[i];
                Thing thing = attackTarget.Thing;
                Pawn p1 = thing as Pawn;

                if (thing == null)
                    continue;

                if ((!attackTarget.ThreatDisabled(pawn) || (Settings.killDownedThreats && p1 != null && p1.Downed))){
                    Verb verb = pawn.TryGetAttackVerb(thing, false);
                    if (verb == null)
                        continue;
                    bool isMeleeAttack = verb.verbProps.IsMeleeAttack;

                    if ((thing.Position.InAllowedArea(pawn) && pawn.CanReach(thing, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn)) || (!isMeleeAttack && verb.CanHitTarget(thing)))
                    {
                        return true;
                    }
                }
            }

            if (Settings.exterminatorMode)
            {
                foreach (var cp in pawn.Map.mapPawns.AllPawns)
                {
                    if (!cp.Spawned || !(cp.Faction == null) || !cp.AnimalOrWildMan() || (!Settings.killDownedThreats && cp.Downed))
                        continue;

                    Verb verb = pawn.TryGetAttackVerb(cp, false);
                    if (verb == null)
                        continue;
                    bool isMeleeAttack = verb.verbProps.IsMeleeAttack;

                    if ((cp.Position.InAllowedArea(pawn) && pawn.CanReach(cp, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn)) || (!isMeleeAttack && verb.CanHitTarget(cp)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static readonly IntRange CacheInterval_AttackTarget = new IntRange(360, 480);
    }
}
