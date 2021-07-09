using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Verse.AI.Group;
using System.Linq;
using HarmonyLib;
using System.Reflection;

namespace aRandomKiwi.GFM
{
    public class Comp_Guard : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<int>(ref this.guardJobOK, "GFM_guardJobOK", 0);
            Scribe_Values.Look<bool>(ref this.patrolMoveForward, "GFM_patrolMoveForward", true);
            Scribe_References.Look(ref curPatrolWP, "GFM_curPatrolWP");
            Scribe_Values.Look<string>(ref this.affectedPatrol, "GFM_affectedPatrol", "");
            Scribe_Values.Look<bool>(ref this.guardMode, "GFM_guardMode", false);
            Scribe_Values.Look<bool>(ref this.deathSquadMode, "GFM_deathSquadMode", false);
            Scribe_Values.Look<int>(ref this.guardSpotModeGT, "GFM_guardSpotModeGT", 0);
            Scribe_References.Look(ref affectedGS, "GFM_affectedGS");
            Scribe_References.Look(ref reservedGS, "GFM_reservedGS");
            
            Scribe_Values.Look<string>(ref this.affectedGSKind, "GFM_affectedGSKind", "");



            Scribe_References.Look(ref reservedGS, "GFM_deathSquadForcedTarget");

            Scribe_References.Look<Pawn>(ref guardedPawn, "GFM_guardedPawn");
            Scribe_Values.Look<bool>(ref this.guardOnlyAttackNearThreats, "GFM_guardOnlyAttackNearThreats", true);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            //Fix transition 2.X to 3.X (there was no GS of different type)
            if (guardMode && affectedGSKind == "")
                affectedGSKind = "GFM_GuardSpot";
        }


        public override void PostDraw()
        {
            try
            {
                base.PostDraw();

                Material avatar = null;
                if (Settings.hideIcons)
                    return;
                //If mercenary icon display allowed
                if (this.parent.def.race != null && this.parent.def.race.intelligence == Intelligence.Humanlike && (!Settings.guardAsJob || (Settings.guardAsJob && guardJobOK == 1)))
                {
                    Vector3 vector;


                    if (guardedPawn != null)
                        avatar = Tex.bodyGuard;
                    else if (affectedPatrol != "")
                        avatar = Tex.patrolGuard;
                    else if (guardMode)
                        avatar = Tex.standGuard;

                    Pawn pawn = parent as Pawn;

                    if (deathSquadMode && pawn.mindState.enemyTarget != null)
                        avatar = Tex.deathSquadGuard;

                    if (avatar != null)
                    {
                        vector = this.parent.TrueCenter();
                        vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
                        vector.z += 1.4f;
                        vector.x += this.parent.def.size.x / 2;

                        Graphics.DrawMesh(MeshPool.plane08, vector, Quaternion.identity, avatar, 0);
                    }
                }
            }
            catch (Exception)
            {

            }
        }


        public void setAffectedGuardSpot(Building_GuardSpot gs)
        {
            affectedGS = gs;
            gs.setTempAffectedGuard((Pawn)parent);
        }

        public void setReservedGuardSpot(Building_GuardSpot gs)
        {
            reservedGS = gs;
            affectedGS = null;
        }

        public void clearAffectedGuardSpot()
        {
            affectedGS = null;
        }

        public void clearReservedGuardSpot()
        {
            reservedGS = null;
        }

        public Building_GuardSpot getAffectedGuardSpot()
        {
            if (reservedGS != null)
            {
                if (reservedGS.Destroyed)
                    reservedGS = null;
                return reservedGS;
            }
            else
            {
                if (affectedGS != null)
                {
                    if (affectedGS.Destroyed)
                        affectedGS = null;
                    return affectedGS;
                }
                else
                    return null;
            }
        }

        public void touchGuardSpotGT()
        {
            this.guardSpotModeGT = Find.TickManager.TicksGame + 850;
        }

        public void clearGuardSpotGT()
        {
            this.guardSpotModeGT = 0;
        }

        public bool isActiveGuard()
        {
            return ((Settings.guardAsJob && (guardJobOK == 1 || guardJobOK == 2)) || !Settings.guardAsJob) && (guardedPawn != null || affectedPatrol != "" || guardMode || deathSquadMode);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!this.parent.Spawned || this.parent.Map == null)
                return;

            int CGT = Find.TickManager.TicksGame;

            if( ( (Settings.guardAsJob && guardJobOK == 1) || !Settings.guardAsJob) && guardMode)
            {
                Pawn pawn = (Pawn)parent;
                if (pawn.Spawned && pawn.Map != null)
                {
                    try
                    {
                        pawn.GainComfortFromCellIfPossible();
                    }
                    catch(Exception e)
                    {

                    }
                }
            }

            /*if(CGT % 60 == 0 && guardMode && guardJobOK == 1)
            {
                Building_GuardSpot gs = getAffectedGuardSpot();

                if (gs != null && parent.Position == gs.Position)
                {
                    if (gs.direction == "bottom")
                        pawn.Rotation = Rot4.South;
                    else if (gs.direction == "top")
                        pawn.Rotation = Rot4.North;
                    else if (gs.direction == "left")
                        pawn.Rotation = Rot4.West;
                    else if (gs.direction == "right")
                        pawn.Rotation = Rot4.East;

                    return null;
                }
            }*/

            if(CGT % 180 == 0)
            {
                Pawn pawn = (Pawn)this.parent;

                //Automatic DELETION of the forced target of the deathsquad if it is no longer valid
                if (DeathSquadMode() && deathSquadForcedTarget != null)
                {
                    Pawn cp = deathSquadForcedTarget as Pawn;
                    if(cp != null && (cp.Downed || cp.Dead || !cp.Spawned))
                    {
                        clearDeathSquadForcedTarget();
                    }
                }

                if (isActiveGuard())
                {
                    ReassureSurroundingPawns();
                }

                //If the GS TIME allocation time is exceeded, it will be deactivated
                if (affectedGS != null && (CGT > guardSpotModeGT || affectedGS.Destroyed))
                {
                    affectedGS.clearTempGuard();
                    affectedGS = null;
                }

                if (!Settings.guardAsJob && guardedPawn != null)
                {
                    //If the guarded pawn is no longer nearby we stop the job
                    if (!guardedPawn.Spawned || guardedPawn.Map != parent.Map)
                        guardedPawn = null;
                }
            }
            if (CGT % 650 == 0)
            {
                if (Settings.guardAsJob)
                {
                    //If guard job activated, check if another (priority) job could be issued instead
                    if (guardJobOK == 1)
                    {
                        Pawn pawn = (Pawn)this.parent;

                        guardJobOK = 2;
                        ThinkTreeDef thinkTree = null;
                        MethodInfo mi = AccessTools.Method(typeof(Pawn_JobTracker), "DetermineNextJob");
                        ThinkResult thinkResult = (ThinkResult)mi.Invoke(pawn.jobs, new object[] { thinkTree });

                        //Offer not confirmed
                        if (guardJobOK == 3)
                        {
                            //Log.Message("KKKSSSSSK");
                            //Restore hostility response value
                            guardJobOK = 1;  //NEW
                        }
                        else
                        {
                            stopCurrentGuardJob();
                        }
                    }
                }
                if (guardJobOK == 3)
                {
                    guardJobOK = 0;
                }
            }
        }

        public void ReassureSurroundingPawns()
        {
            if (Utils.SawnGuard == null)
                return;

            Pawn pawn = (Pawn)this.parent;

            Map map = pawn.Map;
            int num = 0;
            while ((float)num < 100f)
            {
                IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[num];
                if (intVec.InBounds(map))
                {
                    if (GenSight.LineOfSight(intVec, pawn.Position, map, true, null, 0, 0))
                    {
                        List<Thing> thingList = intVec.GetThingList(map);
                        for (int i = 0; i < thingList.Count; i++)
                        {
                            Pawn cp = thingList[i] as Pawn;

                            if (cp != null && cp.IsColonist && cp != pawn)
                            {
                                //if he is blind he sees nothing
                                if (!cp.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
                                    continue;

                                cp.needs.mood.thoughts.memories.TryGainMemory(Utils.SawnGuard, null);
                            }
                        }
                    }
                }
                num++;
            }
        }

        public void stopCurrentGuardJob()
        {
            Pawn pawn = (Pawn)this.parent;

            if (!pawn.Drafted)
            {
                pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
                /*pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                if (pawn.CurJob != null)
                    pawn.CurJob.Clear();*/
                if(pawn.jobs != null && pawn.jobs.curJob != null)
                    pawn.jobs.StopAll();
                //pawn.jobs.ClearQueuedJobs();
            }
            guardJobOK = 0;

            /*pawn.jobs.StopAll();
            pawn.jobs.ClearQueuedJobs();
            guardJobOK = 0;*/
        }


        public override string CompInspectStringExtra()
        {
            string ret = "";

            //If map not defined or pawn host has not learned to hunt, we quit
            if (parent.Map == null || Settings.disableDisplayJobText)
                return base.CompInspectStringExtra();

            if( (Settings.guardAsJob && guardJobOK == 1) || !Settings.guardAsJob)
            {
                if (affectedPatrol != "")
                    ret += "GFM_DoPatrolGuard".Translate();
                else if (guardedPawn != null)
                    ret += "GFM_DoBodyGuard".Translate();
                else if (guardMode)
                    ret += "GFM_DoStandguard".Translate();
                else if (deathSquadMode)
                    ret += "GFM_DoDeathSquad".Translate();
            }


            return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //vehicle framework patch
            if (this.parent.def.race != null && this.parent.def.race.intelligence != Intelligence.Humanlike)
            {
                yield break;
            }

            Pawn pawn = (Pawn)parent;
            if (pawn.skills == null)
                yield break;

            bool isNonViolent = (pawn.skills.GetSkill(SkillDefOf.Shooting).TotallyDisabled && pawn.skills.GetSkill(SkillDefOf.Melee).TotallyDisabled);

            Texture2D patrolIcon;
            string label;
            Utils.getPatrolCommandStuff(affectedPatrol, out patrolIcon, out label);

            if (!isNonViolent)
            {
                if (!Settings.hidePatrolButton)
                {
                    //Patrol command
                    yield return new Command_Action
                    {
                        icon = patrolIcon,
                        defaultLabel = label,
                        defaultDesc = "",
                        action = delegate ()
                        {
                            List<FloatMenuOption> opts = new List<FloatMenuOption>();
                            for (int i = 0; i != Utils.wayPointsLabel.Length; i++)
                            {
                                int curIndex = i;
                                opts.Add(new FloatMenuOption(Utils.wayPointsLabel[i].Translate(), delegate
                                {
                                    foreach (var entry in Find.Selector.SelectedObjects)
                                    {
                                        if (!(entry is Pawn))
                                            continue;

                                        Pawn cpawn = (Pawn)entry;
                                        Comp_Guard comp = cpawn.TryGetComp<Comp_Guard>();
                                        if (comp == null)
                                            continue;

                                        //Deduction of the waypoint of the closest selected patrol
                                        comp.targetPatrolWP = IntVec3.Invalid;
                                        comp.curPatrolWP = null;
                                        comp.patrolMoveForward = true;

                                        comp.cancelStandingGuard();
                                        comp.cancelBodyGuardMode();
                                        //comp.cancelDeathSquad();

                                        /*if (Utils.MFMLOADED)
                                        {
                                            try
                                            {
                                                ThingComp cguard = Utils.TryGetCompByTypeName(cpawn, "Comp_USFM", "MercenariesForMe");
                                                if (cguard != null)
                                                {
                                                    Traverse.Create(cguard).Method("cancelBodyGuardMode", new object[] { }).GetValue();
                                                    Traverse.Create(cguard).Method("cancelGuardMode", new object[] { }).GetValue();
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Message("[GFM Error] : " + e.Message);
                                            }
                                        }*/

                                        //Log.Message("Selected patrol => " + Utils.waypointsDefName[curIndex]);
                                        comp.affectedPatrol = Utils.waypointsDefName[curIndex];
                                        comp.guardJobOK = 0;
                                        if (!Settings.guardAsJob)
                                        {
                                            cpawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(cpawn);
                                            cpawn.jobs.StopAll();
                                            cpawn.jobs.ClearQueuedJobs();
                                        }

                                    }

                                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                            }
                            if (opts.Count != 0)
                            {
                                if (affectedPatrol != "")
                                {
                                    opts.Add(new FloatMenuOption("CancelButton".Translate(), delegate
                                    {
                                        stopPatrol();

                                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                                }

                                FloatMenu floatMenuMap = new FloatMenu(opts);
                                Find.WindowStack.Add(floatMenuMap);
                            }
                        }
                    };
                }
            }

            

            if (isGuardedPawn())
            {
                //Cancel all guards
                yield return new Command_Action
                {
                    icon = Tex.stopAllGuardians,
                    defaultLabel = "GFM_GuardStopAll".Translate(),
                    defaultDesc = "",
                    action = delegate ()
                    {
                        stopAllGuardians();
                    }
                };

                //Attack only near / far threats
                yield return new Command_Toggle
                {
                    icon = Tex.guardThreatDistance,
                    defaultLabel = "GFM_GuardAttackOnlyNearThreats".Translate(),
                    defaultDesc = "GFM_GuardAttackOnlyNearThreatsDesc".Translate(),
                    isActive = (() => guardOnlyAttackNearThreats),
                    toggleAction = delegate ()
                    {
                        guardOnlyAttackNearThreats = !guardOnlyAttackNearThreats;
                    }
                };
            }

            if (!Settings.hideBodyguardButton)
            {

                if (guardedPawn != null && guardedPawn.Spawned)
                {
                    yield return new Command_Action
                    {
                        icon = Tex.stopGuard,
                        defaultLabel = "GFM_GuardStop".Translate(),
                        defaultDesc = "GFM_GuardStopDesc".Translate(),
                        action = delegate ()
                        {
                            pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
                            pawn.jobs.StopAll();
                            pawn.jobs.ClearQueuedJobs();

                            guardedPawn = null;
                        }
                    };
                }
                else
                {
                    if (!isNonViolent)
                    {
                        yield return new Command_Action
                        {
                            icon = Tex.guard,
                            defaultLabel = "GFM_Guard".Translate(),
                            defaultDesc = "GFM_GuardDesc".Translate(),
                            action = delegate ()
                            {
                                List<FloatMenuOption> opts = new List<FloatMenuOption>();
                                bool pass = false;
                                foreach (var p in Utils.getColonistsAndAllyAndNeutral(pawn.Map).OrderBy(p => p.LabelCap))
                                {
                                    pass = false;
                                    //Check if pawn not in the list of selected objects (if necessary it is removed from the list of recipients of bodyguards)
                                    foreach (var entry in Find.Selector.SelectedObjects)
                                    {
                                        if (!(entry is Pawn))
                                            continue;

                                        Pawn cpawn = (Pawn)entry;
                                        if (cpawn == p)
                                        {
                                            pass = true;
                                            break;
                                        }
                                    }

                                    if (pass)
                                        continue;

                                    opts.Add(new FloatMenuOption(p.LabelCap, delegate
                                    {
                                        foreach (var entry in Find.Selector.SelectedObjects)
                                        {
                                            if (!(entry is Pawn))
                                                continue;

                                            Pawn cpawn = (Pawn)entry;
                                            Comp_Guard comp = cpawn.TryGetComp<Comp_Guard>();

                                            if (comp == null)
                                                continue;

                                            if (!cpawn.CanReach(p, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                                            {
                                                Messages.Message("GFM_BodyGuardCantReachVIP".Translate(p.LabelShortCap, cpawn.LabelShortCap), MessageTypeDefOf.NegativeEvent);
                                                continue;
                                            }

                                            bool prevOtherGuardType = comp.affectedPatrol != "" || comp.DeathSquadMode() || comp.guardMode;

                                            comp.cancelStandingGuard();
                                            //comp.cancelDeathSquad();
                                            comp.cancelPatrolMode();
                                            comp.cancelDeathSquad();

                                            if (!Settings.guardAsJob || prevOtherGuardType)
                                            {

                                                cpawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(cpawn);
                                                cpawn.jobs.StopAll();
                                                cpawn.jobs.ClearQueuedJobs();
                                                comp.guardJobOK = 0;
                                            }

                                            comp.guardedPawn = p;
                                        }
                                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                                }
                                if (opts.Count != 0)
                                {
                                    FloatMenu floatMenuMap = new FloatMenu(opts);
                                    Find.WindowStack.Add(floatMenuMap);
                                }
                            }
                        };
                    }
                }
            }


            if (pawn.Faction == Faction.OfPlayer && !isNonViolent)
            {
                if (!Settings.hideStandingGuardButton)
                {
                    //Guard mode
                    Texture2D guardBtnTex;
                    string guardBtnLabel;
                    Utils.getGuardSpotCommandStuff(affectedGSKind, out guardBtnTex, out guardBtnLabel);

                    yield return new Command_Action
                    {
                        icon = guardBtnTex,
                        defaultLabel = guardBtnLabel,
                        defaultDesc = "GFM_GuardModeDesc".Translate(),
                        action = delegate ()
                        {
                            List<FloatMenuOption> opts = new List<FloatMenuOption>();
                            for (int i = 0; i != Utils.guardSpotsLabel.Length; i++)
                            {
                                int curIndex = i;
                                opts.Add(new FloatMenuOption(Utils.guardSpotsLabel[i].Translate(), delegate
                                {
                                    foreach (var entry in Find.Selector.SelectedObjects)
                                    {
                                        if (!(entry is Pawn))
                                            continue;

                                        Pawn cpawn = (Pawn)entry;
                                        Comp_Guard comp = cpawn.TryGetComp<Comp_Guard>();
                                        if (comp == null)
                                            continue;

                                        bool prevOtherGuardType = comp.affectedPatrol != "" || comp.guardedPawn != null || comp.guardMode;

                                        if (Utils.guardSpotsLabel[curIndex] != "")
                                        {
                                            //We remove the bindings of invalid GS with the new choice
                                            comp.resetGSIfNotMatching(Utils.guardSpotsLabel[curIndex]);

                                            comp.affectedGSKind = Utils.guardSpotsLabel[curIndex];
                                            comp.guardMode = true;

                                            comp.cancelBodyGuardMode();
                                            //comp.cancelDeathSquad();
                                            comp.cancelPatrolMode();

                                            //Log.Message("HERRRRRE");
                                        }
                                        else
                                        {
                                            comp.cancelStandingGuard();
                                        }

                                        if (!Settings.guardAsJob || prevOtherGuardType)
                                        {
                                            cpawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(cpawn);
                                            cpawn.jobs.StopAll();
                                            cpawn.jobs.ClearQueuedJobs();
                                            comp.guardJobOK = 0;
                                        }

                                    }

                                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                            }
                            if (opts.Count != 0)
                            {
                                if (affectedGSKind != "")
                                {
                                    opts.Add(new FloatMenuOption("CancelButton".Translate(), delegate
                                    {
                                        foreach (var entry in Find.Selector.SelectedObjects)
                                        {
                                            if (!(entry is Pawn))
                                                continue;

                                            Pawn cpawn = (Pawn)entry;
                                            Comp_Guard comp = cpawn.TryGetComp<Comp_Guard>();
                                            if (comp == null)
                                                continue;
                                            comp.guardMode = false;
                                            comp.affectedGSKind = "";
                                            comp.stopCurrentGuardJob();
                                        }

                                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                                }

                                FloatMenu floatMenuMap = new FloatMenu(opts);
                                Find.WindowStack.Add(floatMenuMap);
                            }
                        }
                    };
                }

                if (!Settings.hideReactionForceButton)
                {
                    //DeathSquad Mode
                    yield return new Command_Toggle
                    {
                        icon = Tex.deathSquad,
                        defaultLabel = "GFM_PatrolAndKill".Translate(),
                        defaultDesc = "GFM_PatrolAndKillDesc".Translate(),
                        isActive = (() => deathSquadMode),
                        toggleAction = delegate ()
                        {
                            bool activeGuardJob = isActiveGuard();
                            deathSquadMode = !deathSquadMode;
                            /*if (deathSquadMode)
                            {
                                cancelBodyGuardMode();
                                cancelPatrolMode();
                                cancelStandingGuard();
                            }*/

                            if (!Settings.guardAsJob || activeGuardJob)
                            {
                                pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
                                pawn.jobs.StopAll();
                            }
                        }
                    };

                    //Addition of the target forcing selector
                    if (deathSquadMode)
                    {
                        yield return GetDeathSquadForcedTargetGizmo();
                    }
                }
            }

            yield break;
        }

        public void resetGSIfNotMatching(string desiredGSKind)
        {
            if (affectedGS != null && affectedGS.def.defName != desiredGSKind)
            {
                affectedGS.clearTempGuard();
                affectedGS = null;
            }
            if (reservedGS != null && reservedGS.def.defName != desiredGSKind)
            {
                reservedGS.clearReservedGuard();
                reservedGS = null;
            }
        }

        public void cancelBodyGuardMode()
        {
            Pawn pawn = (Pawn)this.parent;
            if (guardedPawn != null)
            {
                guardedPawn = null;
            }
        }

        public void cancelPatrolMode()
        {
            if (affectedPatrol != "")
            {
                Pawn pawn = (Pawn)this.parent;
                affectedPatrol = "";
            }
        }


        public bool GuardMode()
        {
            return guardMode;
        }

        public bool DeathSquadMode()
        {
            return deathSquadMode;
        }

        public bool isGuardian()
        {
            return guardedPawn != null;
        }

        public void stopAllGuardians()
        {
            Pawn self = (Pawn)parent;
            Map map;
            if (self.Dead)
                map = self.Corpse.Map;
            else
                map = self.Map;

            foreach(var guard in map.mapPawns.FreeColonists)
            {
                Comp_Guard comp = guard.TryGetComp<Comp_Guard>();
                if(comp != null && comp.guardedPawn == self)
                {
                    comp.guardedPawn = null;
                }
            }
        }

        public bool isGuardedPawn()
        {
            Pawn self = (Pawn)parent;
            Map map=null;
            if (self.Dead)
            {
                if(self.Corpse != null)
                    map = self.Corpse.Map;
            }
            else
                map = self.Map;

            if (map != null)
            {
                foreach (var guard in map.mapPawns.FreeColonists)
                {
                    Comp_Guard comp = guard.TryGetComp<Comp_Guard>();
                    if (comp != null && comp.guardedPawn == self)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void cancelDeathSquad()
        {
            Pawn pawn = (Pawn)parent;

            if (deathSquadMode)
            {
                deathSquadMode = false;
                pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
                pawn.jobs.StopAll();
                pawn.jobs.ClearQueuedJobs();
            }
        }

        public void cancelStandingGuard()
        {
            Pawn pawn = (Pawn)parent;

            if (guardMode)
            {
                guardMode = false;
                affectedGSKind = "";
                pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
                pawn.jobs.StopAll();
                pawn.jobs.ClearQueuedJobs();
            }
        }

        public void stopPatrol()
        {
            Pawn pawn = (Pawn)parent;

            affectedPatrol = "";
            pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
            pawn.jobs.StopAll();
            pawn.jobs.ClearQueuedJobs();
        }

        public void clearDeathSquadForcedTarget()
        {
            Pawn pawn = (Pawn)parent;

            if (pawn == null || deathSquadForcedTarget == null)
                return;

            deathSquadForcedTarget = null;

            pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            if (pawn.CurJob != null)
                pawn.CurJob.Clear();
            pawn.jobs.StopAll();
            pawn.jobs.ClearQueuedJobs();
            //pawn.mindState.enemyTarget = null;
        }

        public void setDeathSquadForcedTarget(Thing target)
        {
            Pawn pawn = (Pawn)parent;
            if (pawn == null || target == null)
                return;

            deathSquadForcedTarget = target;
            //Log.Message("HERE FORCED");

            pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            if (pawn.CurJob != null)
                pawn.CurJob.Clear();
            pawn.jobs.StopAll();
            pawn.jobs.ClearQueuedJobs();
            

            pawn.mindState.enemyTarget = target;
        }

        private Gizmo GetDeathSquadForcedTargetGizmo()
        {
            Pawn pawn = (Pawn)parent;

            Command_Target command_Target = new Command_Target();
            command_Target.defaultLabel = "GFM_DeathSquadForceTarget".Translate();
            command_Target.defaultDesc = "";
            command_Target.targetingParams = TargetingParameters.ForAttackHostile();
            command_Target.icon = Tex.deathSquadFocus;

            command_Target.action = delegate (Thing target)
            {
                IEnumerable<Pawn> enumerable = Find.Selector.SelectedObjects.Where(delegate (object x)
                {
                    Pawn pawn2 = x as Pawn;
                    return pawn2 != null && pawn2.IsColonistPlayerControlled;
                }).Cast<Pawn>();
                foreach (Pawn current in enumerable)
                {
                    if (current == null)
                        continue;

                    Comp_Guard c = current.TryGetComp<Comp_Guard>();
                    if (c != null && c.deathSquadMode && (!Settings.guardAsJob || (Settings.guardAsJob && guardJobOK == 1)))
                    {
                        c.setDeathSquadForcedTarget(target);
                    }
                }
            };
            return command_Target;
        }

        //Stores the signal indicating if the pawn has been allocated by the job system for guarding or not
        public int guardJobOK = 0;
        public bool firstWPReached = false;
        //Guardian Job 
        public Building_GuardSpot affectedGS = null;
        public Building_GuardSpot reservedGS = null;

        public Thing deathSquadForcedTarget = null;

        //Store the reachable destination coordinate near the waypoint
        public IntVec3 targetPatrolWP = IntVec3.Invalid;
        //Store the current waypoint used
        public Building_PatrolWaypoint curPatrolWP = null;
        public string affectedPatrol = "";
        public string affectedGSKind = "";
        public bool patrolMoveForward = true;


        private bool guardMode = false;
        private bool deathSquadMode = false;
        public int guardSpotModeGT = 0;
        public Pawn guardedPawn;
        public bool guardOnlyAttackNearThreats = true;

        public Thing cachedAttackTarget = null;
        public int cachedAttackTargetGT = 0;
        public IntVec3 cachedAttackTargetSelVec;

        public bool tmpCanKillDowned = false;
    }
}