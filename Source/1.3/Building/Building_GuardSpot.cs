using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace aRandomKiwi.GFM
{
    public class Building_GuardSpot : Building
    {
        Pawn selPawn = null;
        Pawn tempPawn = null;
        public string direction = "bottom";


        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (!Settings.hideGuardspots)
                base.DrawAt(drawLoc, flip);
        }

        public override void Draw()
        {
            if (!Settings.hideGuardspots)
                base.Draw();
        }

        public override void Print(SectionLayer layer)
        {
            if (!Settings.hideGuardspots)
                base.Print(layer);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            Utils.GCGFM.pushGuardSpot(this);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);

            Utils.GCGFM.popGuardSpot(this);
        }

        public Pawn getAffectedGuard()
        {
            if (selPawn != null)
                return selPawn;
            else
                return tempPawn;
        }

        public Pawn getReservedGuard()
        {
            return selPawn;
        }

        public void setTempAffectedGuard(Pawn guard)
        {
            tempPawn = guard;
        }

        public void clearReservedGuard()
        {
            selPawn = null;
            tempPawn = null;
        }

        public void clearTempGuard()
        {
            tempPawn = null;
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
                    List<FloatMenuOption> opts = new List<FloatMenuOption>();
                    foreach (var p in base.Map.mapPawns.FreeColonists.OrderBy(p => p.LabelCap))
                    {
                        if (p == selPawn)
                            continue;

                        Comp_Guard comp = p.TryGetComp<Comp_Guard>();

                        //We filter the guards assigned to different types of Guard spot
                        if (comp == null || comp.affectedGSKind != this.def.defName)
                            continue;

                        opts.Add(new FloatMenuOption(p.LabelCap, delegate
                        {

                            //Check if gs reachable by the pawn
                            if ( !p.CanReach(this.Position, Verse.AI.PathEndMode.OnCell, Danger.Deadly))
                            {
                                Messages.Message("CannotReach".Translate(), MessageTypeDefOf.NegativeEvent);
                                return;
                            }

                            //Check if pawn not reserved for another guardspot (whether its in tempPawn or selPawn) if the case reset
                            foreach (var gs in Utils.GCGFM.getGuardSpot())
                            {
                                if (gs == null)
                                    continue;
                                if(gs.getAffectedGuard() == p)
                                {
                                    gs.clearReservedGuard();
                                }
                            }

                            //If the guard spot is already assigned, we release the guard to which it is assigned
                            if (selPawn != null || tempPawn != null)
                            {
                                if(selPawn != null)
                                {
                                    Comp_Guard comp2 = selPawn.TryGetComp<Comp_Guard>();
                                    comp2.clearAffectedGuardSpot();
                                    comp2.clearReservedGuardSpot();
                                }

                                if (tempPawn != null)
                                {
                                    Comp_Guard comp2 = tempPawn.TryGetComp<Comp_Guard>();
                                    comp2.clearAffectedGuardSpot();
                                    comp2.clearReservedGuardSpot();
                                }
                            }

                            selPawn = p;
                            tempPawn = null;

                            Comp_Guard comp3 = selPawn.TryGetComp<Comp_Guard>();
                            comp3.setReservedGuardSpot(this);

                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    if (opts.Count != 0)
                    {
                        if (selPawn != null)
                        {
                            opts.Add(new FloatMenuOption("GFM_GuardSpotClearAffectedPawn".Translate(), delegate
                            {
                                Comp_Guard comp = selPawn.TryGetComp<Comp_Guard>();
                                comp.clearAffectedGuardSpot();

                                selPawn = null;
                                tempPawn = null;

                            }));
                        }

                        FloatMenu floatMenuMap = new FloatMenu(opts);
                        Find.WindowStack.Add(floatMenuMap);
                    }
                }
            };

            Texture2D dirTex = null;

            switch (direction) {
                case "top":
                    dirTex = Tex.dirTop;
                    break;
                case "bottom":
                    dirTex = Tex.dirBottom;
                    break;
                case "left":
                    dirTex = Tex.dirLeft;
                    break;
                case "right":
                    dirTex = Tex.dirRight;
                    break;
            }

            yield return new Command_Action
            {
                icon = dirTex,
                defaultLabel = "",
                defaultDesc = "",
                action = delegate ()
                {
                    List<FloatMenuOption> opts = new List<FloatMenuOption>();
                    opts.Add(new FloatMenuOption("GFM_GuardSpotDirLeft".Translate(), delegate
                    {
                        direction = "left";
                        if (selPawn != null && selPawn.Position == this.Position)
                            selPawn.Rotation = Rot4.West;
                        if(tempPawn != null && tempPawn.Position == this.Position)
                            tempPawn.Rotation = Rot4.West;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    opts.Add(new FloatMenuOption("GFM_GuardSpotDirRight".Translate(), delegate
                    {
                        direction = "right";
                        if (selPawn != null && selPawn.Position == this.Position)
                            selPawn.Rotation = Rot4.East;
                        if (tempPawn != null && tempPawn.Position == this.Position)
                            tempPawn.Rotation = Rot4.East;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    opts.Add(new FloatMenuOption("GFM_GuardSpotDirTop".Translate(), delegate
                    {
                        direction = "top";
                        if (selPawn != null && selPawn.Position == this.Position)
                            selPawn.Rotation = Rot4.North;
                        if (tempPawn != null && tempPawn.Position == this.Position)
                            tempPawn.Rotation = Rot4.North;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    opts.Add(new FloatMenuOption("GFM_GuardSpotDirBottom".Translate(), delegate
                    {
                        direction = "bottom";
                        if (selPawn != null && selPawn.Position == this.Position)
                            selPawn.Rotation = Rot4.South;
                        if (tempPawn != null && tempPawn.Position == this.Position)
                            tempPawn.Rotation = Rot4.South;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    FloatMenu floatMenuMap = new FloatMenu(opts);
                    Find.WindowStack.Add(floatMenuMap);
                }
            };

            yield break;
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());

            stringBuilder.AppendLine();
            string aff = "-";
            if (selPawn != null)
                aff = selPawn.LabelCap;

            stringBuilder.AppendLine("GFM_GuardSpotAffectedPawn".Translate(aff));

            return stringBuilder.ToString().TrimEndNewlines().TrimStart('\n','\r');
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref selPawn, "GFM_selPawn");
            Scribe_References.Look(ref tempPawn, "GFM_tempPawn");
            Scribe_Values.Look(ref direction, "GFM_direction","bottom");
        }
    }
}
