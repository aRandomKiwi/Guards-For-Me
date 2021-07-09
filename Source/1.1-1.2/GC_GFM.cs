using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using UnityEngine;
using RimWorld.Planet;

namespace aRandomKiwi.GFM
{
    public class GC_GFM : GameComponent
    {

        public GC_GFM(Game game)
        {
            this.game = game;
            Utils.GCGFM = this;

            Utils.SawnGuard =  DefDatabase<ThoughtDef>.GetNamedSilentFail("GFM_SawGuard");
            Utils.needBladder = DefDatabase<NeedDef>.GetNamed("Bladder", false);
            Utils.needHygiene = DefDatabase<NeedDef>.GetNamed("Hygiene", false);
            Utils.gotoCombatJobDef = DefDatabase<JobDef>.GetNamedSilentFail("GFM_GotoCombat");
            if(Utils.gotoCombatJobDef == null)
            {
                Utils.gotoCombatJobDef = JobDefOf.Goto;
            }
            Utils.followCloseCombatJobDef = DefDatabase<JobDef>.GetNamedSilentFail("GFM_FollowCloseCombat");
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                reset();
            }

            Scribe_Collections.Look(ref this.listerGuardSpot, "listerGuardSpot", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                initNull();
        }

       
        
        public void pushGuardSpot(Building_GuardSpot build)
        {
            if (!listerGuardSpot.Contains(build))
                listerGuardSpot.Add(build);
        }

        public void popGuardSpot(Building_GuardSpot build)
        {
            listerGuardSpot.Remove(build);
        }

        public Building_GuardSpot getRandomFreeGuardSpot(Map map, Pawn pawn)
        {
            Comp_Guard comp = pawn.TryGetComp<Comp_Guard>();
            if (listerGuardSpot.Count() == 0)
                return null;

            /*foreach(var el in listerGuardSpot)
            {
                Log.Message("'"+el.def.defName + "' '" + comp.affectedGSKind+"'"+ (el.def.defName == comp.affectedGSKind)+ " "+(!el.Destroyed)+" "+(el.getAffectedGuard() == null)+" "+(el.Map == map)+" "+(pawn.CanReach(el.Position, PathEndMode.OnCell, Danger.Deadly)));
            }*/

            List<Building_GuardSpot> sel = listerGuardSpot.Where(gs => gs != null && gs.def.defName == comp.affectedGSKind && !gs.Destroyed && (gs.getAffectedGuard() == null && gs.Map == map && pawn.CanReach(gs.Position,PathEndMode.OnCell, Danger.Deadly) && gs.Position.InAllowedArea(pawn))).ToList();
            if (sel.Count() == 0)
            {
                //Log.Message("NO RES B1");
                return null;
            }
            return sel.RandomElement();
        }

        public List<Building_GuardSpot> getGuardSpot()
        {
            return listerGuardSpot;
        }

        public Building_GuardSpot getReservedGuardSpot(Pawn guard)
        {
            foreach (var gs in listerGuardSpot)
            {
                if (gs == null)
                    continue;
                if (gs.getReservedGuard() == guard)
                    return gs;
            }
            return null;
        }
        
        private void reset()
        {
            listerGuardSpot.Clear();
        }

        private void initNull()
        {
            if (listerGuardSpot == null)
                listerGuardSpot = new List<Building_GuardSpot>();
        }

        private List<Building_GuardSpot> listerGuardSpot = new List<Building_GuardSpot>();
        private Game game;
    }
}