using HarmonyLib;
using System.Reflection;
using Verse;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace aRandomKiwi.GFM
{
    [StaticConstructorOnStartup]
    class GuardsForMe : Mod
    {
        private const string ID_MFM = "Mercenaries For Me";


        public GuardsForMe(ModContentPack content) : base(content)
        {
            //Log.Message("Init MFM");
            base.GetSettings<Settings>();


            //Androids CHj
            if (LoadedModManager.RunningModsListForReading.Any(x => (x.Name == "Androids")))
            {
                Utils.ANDROIDLOADED = true;
                Log.Message("[GFM] Androids found");
            }


            //EPOE Expanded Prosthetics and Organ Engineering
            if (LoadedModManager.RunningModsListForReading.Any(x => (x.Name == ID_MFM )))
            {
                Utils.MFMLOADED = true;
                Log.Message("[GFM] MercenariesForMe found");
            }

            if (LoadedModManager.RunningModsListForReading.Any(x => (x.Name == "Camera+")))
            {
                Utils.CAMERAPLOADED = true;
                Log.Message("[GFM] Camera+ found");
            }

        }

        public void Save()
        {
            LoadedModManager.GetMod<GuardsForMe>().GetSettings<Settings>().Write();
        }

        public override string SettingsCategory()
        {
            return "Guards For Me";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}