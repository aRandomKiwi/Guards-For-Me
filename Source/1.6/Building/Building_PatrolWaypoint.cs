using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace aRandomKiwi.GFM
{
    public class Building_PatrolWaypoint : Building
    {
        public int index = -1;
        public Building_PatrolWaypoint prev;
        public Building_PatrolWaypoint next;

        public override void DrawGUIOverlay()
        {
            if (canBeVisible())
            {
                Color defaultThingLabelColor = GenMapUI.DefaultThingLabelColor;
                GenMapUI.DrawThingLabel(this, index.ToString(), defaultThingLabelColor);
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (!canBeVisible())
            {
                if (this.def.selectable)
                {
                    this.def.selectable = false;
                    this.DirtyMapMesh(this.Map);
                }
                return;
            }
            else
                base.DrawAt(drawLoc, flip);
        }

        public override void Print(SectionLayer layer)
        {
            if (!canBeVisible())
            {
                if (this.def.selectable)
                {
                    this.def.selectable = false;
                    this.DirtyMapMesh(this.Map);
                }
                return;
            }
            else
                base.Print(layer);

        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            

        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if(index == -1)
            {
                Building_PatrolWaypoint curPWP = null;
                int lastIndex = -1;
                foreach(var build in map.listerBuildings.allBuildingsColonist)
                {
                    if(build.def.defName == this.def.defName)
                    {
                        Building_PatrolWaypoint b = (Building_PatrolWaypoint)build;
                        if (b.index > lastIndex)
                        {
                            lastIndex = b.index;
                            curPWP = b;
                        }
                    }
                }

                if (curPWP != null)
                {
                    prev = curPWP;
                    curPWP.next = this;
                    index = lastIndex + 1;
                }
                else
                    index = 0;

                //Log.Message(index.ToString());
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            //Utils.GCMFM.popGuardSpot(this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            yield return new Command_Action
            {
                icon = Tex.affectGuardToGS,
                defaultLabel = "GFM_GuardSpotAffectGuard".Translate(),
                defaultDesc = "GFM_GuardSpotAffectGuardDesc".Translate(),
                action = delegate ()
                {
                    
                }
            };

            yield break;
        }


        public bool canBeVisible()
        {
            bool cond2 = (Find.Selector.SingleSelectedObject != null && Find.Selector.SingleSelectedObject is Pawn);
            bool cond1 = (Find.DesignatorManager.SelectedDesignator != null && Find.DesignatorManager.SelectedDesignator is Designator_Build);
            if ( cond1
                || cond2 )
            {
                if (!cond1 && cond2)
                {
                    Pawn pawn = (Pawn)Find.Selector.SingleSelectedObject;
                    Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
                    if(comp != null)
                    {
                        if (comp.affectedPatrol == this.def.defName)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                Designator_Build sel = (Designator_Build)Find.DesignatorManager.SelectedDesignator;
                if (sel.PlacingDef == this.def)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref index, "GFM_index",-1);
            Scribe_References.Look(ref prev, "GFM_prev");
            Scribe_References.Look(ref next, "GFM_next");
        }
    }
}
