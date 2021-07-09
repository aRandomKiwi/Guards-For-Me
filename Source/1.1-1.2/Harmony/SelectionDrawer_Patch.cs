using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Linq;
using UnityEngine;

namespace aRandomKiwi.GFM
{
    internal class SelectionDrawer_Patch
    {

        [HarmonyPatch(typeof(SelectionDrawer), "DrawSelectionOverlays")]
        public class SelectionDrawerX_Patch
        {
            [HarmonyPostfix]
            public static void Listener()
            {
                bool cond2 = (Find.Selector.SingleSelectedObject != null && Find.Selector.SingleSelectedObject is Pawn);
                bool cond1 = (Find.DesignatorManager.SelectedDesignator != null && Find.DesignatorManager.SelectedDesignator is Designator_Build);
                if (Find.CurrentMap != null && ( cond1 
                    || cond2))
                {
                    string defName = "";
                    if (!cond1 && cond2)
                    {
                        Pawn pawn = (Pawn) Find.Selector.SingleSelectedObject;
                        Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
                        if (comp == null || comp.affectedPatrol == "")
                            return;

                        defName = comp.affectedPatrol;
                    }
                    else
                    {
                        //If waypoint selector selected, display of waypoints
                        Designator_Build sel = (Designator_Build)Find.DesignatorManager.SelectedDesignator;
                        defName = sel.PlacingDef.defName;
                    }

                    if (Utils.waypointsDefName.Contains(defName))
                    {
                        //If the DEL key is pressed, the current waypoint to be displayed is deleted
                        if (Event.current.type == EventType.KeyDown && cond1 && Find.CurrentMap != null)
                        {
                            if (Event.current.keyCode == KeyCode.Delete)
                            {
                                bool somethingDestroyed = false;
                                //Path deletion
                                foreach (var el in Find.CurrentMap.listerBuildings.allBuildingsColonist.ToList())
                                {
                                    if (el.def.defName == defName)
                                    {
                                        somethingDestroyed = true;
                                        el.Destroy();
                                    }
                                }

                                if (somethingDestroyed)
                                {
                                    Messages.Message("GFM_MsgPatrolDestroyed".Translate(), MessageTypeDefOf.PositiveEvent);
                                }

                                //Stop settlers take the path
                                foreach (var el in Find.CurrentMap.mapPawns.FreeColonistsAndPrisoners.ToList())
                                {
                                    Comp_Guard comp = el.TryGetComp<Comp_Guard>();
                                    if (comp != null && comp.affectedPatrol == defName)
                                    {
                                        comp.stopPatrol();
                                    }
                                }
                            }

                            if (Event.current.keyCode == KeyCode.Backspace)
                            {
                                //Last waypoint search
                                Building_PatrolWaypoint target = null;
                                foreach (var el in Find.CurrentMap.listerBuildings.allBuildingsColonist.ToList())
                                {
                                    if (el.def.defName == defName)
                                    {
                                        Building_PatrolWaypoint found = (Building_PatrolWaypoint)el;
                                        if (found.next == null)
                                        {
                                            if(found.prev != null)
                                            {
                                                found.prev.next = null;
                                            }
                                            el.Destroy();
                                            Messages.Message("GFM_MsgPatrolLastWaypointDestroyed".Translate(), MessageTypeDefOf.PositiveEvent);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        Building_PatrolWaypoint cur = null;
                        Building_PatrolWaypoint curOrig = null;

                        //Obtaining one of the elements selected at random
                        foreach (var el in Find.CurrentMap.listerBuildings.allBuildingsColonist)
                        {
                            if (el.def.defName == defName)
                            {
                                curOrig = (Building_PatrolWaypoint)el;
                                break;
                            }
                        }

                        cur = curOrig;

                        if (cur == null)
                            return;

                        //Drawing previous portion of the path

                        //GenMapUI.DrawThingLabel(cur, index.ToString());
                        while (cur != null)
                        {
                            if (cur.prev != null)
                            {
                                GenDraw.DrawLineBetween(cur.TrueCenter(), cur.prev.TrueCenter(), SimpleColor.White);
                                //GenMapUI.DrawThingLabel(cur.prev, cur.prev.index.ToString());
                            }

                            cur = cur.prev;
                        }

                        //Drawing next portion of the path
                        cur = curOrig;
                        while (cur != null)
                        {
                            if (cur.next != null)
                            {
                                GenDraw.DrawLineBetween(cur.TrueCenter(), cur.next.TrueCenter(), SimpleColor.White);
                                //GenMapUI.DrawThingLabel(cur.prev, cur.next.index.ToString());
                            }

                            cur = cur.next;
                        }
                    }
                }
            }
        }
    }
}