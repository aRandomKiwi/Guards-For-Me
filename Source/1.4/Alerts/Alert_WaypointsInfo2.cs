using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using System.Linq;

namespace aRandomKiwi.GFM
{
    public class Alert_WaypointsInfo2 : Alert
    {
        public Alert_WaypointsInfo2()
        {
            this.defaultLabel = "GFM_AlertWaypointsInfo2".Translate();
            this.defaultExplanation = "GFM_AlertWaypointsInfo2Desc".Translate();
            //this.defaultExplanation = "KFM_AlertNoRallyZoneDesc".Translate();
            this.defaultPriority = AlertPriority.Critical;
        }

        protected override Color BGColor
        {
            get
            {
                float num = Pulser.PulseBrightness(0.5f, Pulser.PulseBrightness(0.5f, 0.6f));
                return new Color(num, num, num) * color;
            }
        }


        public override AlertReport GetReport()
        {
            if (Find.CurrentMap != null && Find.DesignatorManager.SelectedDesignator != null && Find.DesignatorManager.SelectedDesignator is Designator_Build)
            {
                //If waypoint selector selected, display of waypoints
                Designator_Build sel = (Designator_Build)Find.DesignatorManager.SelectedDesignator;
                if (Utils.waypointsDefName.Contains(sel.PlacingDef.defName))
                    return true;
                else
                    return false;
            }
            return false;
        }

        Color color = new Color(0.15294f, 0.6823f, 0.3764f, 1.0f);
    }
}
