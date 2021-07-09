using UnityEngine;
using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class Settings : ModSettings
    {
        public static bool exterminatorMode = false;
        public static bool disableDisplayJobText = false;
        public static bool fixedStandGuard = false;
        public static bool guardAsJob = false;
        public static bool hideGuardspots = false;
        public static bool hideIcons = false;
        public static int pathEndMode = 0;
        public static int patrolWalkMode =3;
        public static float minFoodStopJob = 0.25f;
        public static float minRestStopJob = 0.25f;
        public static float minJoyStopJob = 0.25f;
        public static float minMoodStopJob = 0.25f;
        public static float minHygieneStopJob = 0.25f;
        public static float minBladderStopJob = 0.25f;
        public static bool showColonistTitle = false;
        public static int meleeAttackRadiusSpot = 20;
        public static int standingGuardMaxDistanceWithGS = 20;
        public static bool autoUnforbidThreatsKilledByGuards = false;
        public static bool killDownedThreats = false;

        public static bool hideBodyguardButton = false;
        public static bool hideReactionForceButton = false;
        public static bool hideStandingGuardButton = false;
        public static bool hidePatrolButton = false;

        public static Vector2 scrollPosition = Vector2.zero;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            inRect.yMin += 15f;
            inRect.yMax -= 15f;

            var defaultColumnWidth = (inRect.width - 50);
            Listing_Standard list = new Listing_Standard() { ColumnWidth = defaultColumnWidth };


            var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            var scrollRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height * 2f);
            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect, true);

            list.Begin(scrollRect);

            list.ButtonImage(Tex.settingsHeader, 850, 128);
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("GFM_SettingsSectionGeneral".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();

            list.CheckboxLabeled("GFM_SettingsHideStandGuardButton".Translate(), ref hideStandingGuardButton);
            list.CheckboxLabeled("GFM_SettingsHidePatrolButton".Translate(), ref hidePatrolButton);
            list.CheckboxLabeled("GFM_SettingsHideBodyguardButton".Translate(), ref hideBodyguardButton);
            list.CheckboxLabeled("GFM_SettingsHideReactionForceButton".Translate(), ref hideReactionForceButton);

            list.CheckboxLabeled("GFM_SettingsColonistTitleUnderName".Translate(), ref showColonistTitle);

            bool prevExterminatorMode = exterminatorMode;

            list.CheckboxLabeled("GFM_SettingsExterminatorGuard".Translate(), ref exterminatorMode);

            if(prevExterminatorMode != exterminatorMode)
            {
                if (exterminatorMode)
                {
                    foreach(var map in Find.Maps)
                    {
                        AttackTargetsCache.AttackTargetsCacheStaticUpdate();
                        //Notify_ThingSpawned
                        foreach (var p in map.mapPawns.AllPawnsSpawned)
                        {

                            //if (p.RaceProps.Animal && p.Faction != Faction.OfPlayer)
                            map.attackTargetsCache.Notify_ThingDespawned(p);
                            map.attackTargetsCache.Notify_ThingSpawned(p);
                        }
                    }
                }
            }

            list.CheckboxLabeled("GFM_SettingsGuardAsJob".Translate(), ref guardAsJob);
            list.CheckboxLabeled("GFM_SettingsAutoUnforbidThreatsKilledByGuards".Translate(), ref autoUnforbidThreatsKilledByGuards);
            list.CheckboxLabeled("GFM_SettingsKillDownedThreats".Translate(), ref killDownedThreats);
            list.CheckboxLabeled("GFM_SettingsDisableJobDisplay".Translate(), ref disableDisplayJobText);
            list.CheckboxLabeled("GFM_SettingsStandGuardAreFixed".Translate(), ref fixedStandGuard);
            list.CheckboxLabeled("GFM_SettingsHideGuardSpot".Translate(), ref hideGuardspots);
            list.CheckboxLabeled("GFM_SettingsHideIcon".Translate(), ref hideIcons);
            list.Label("GFM_SettingsEndPathMode".Translate());
            if (list.RadioButton("GFM_SettingsEndPathMode1".Translate(), (pathEndMode == 0)))
                pathEndMode = 0;
            if (list.RadioButton("GFM_SettingsEndPathMode2".Translate(), (pathEndMode == 1)))
                pathEndMode = 1;

            list.Label("GFM_SettingsJobStop".Translate() + " : " + "GFM_SettingsMoodMinimumThresholdStopJob".Translate((int)(minMoodStopJob * 100)));
            minMoodStopJob = list.Slider(minMoodStopJob, 0.0f, 1.0f);
            list.Label("GFM_SettingsJobStop".Translate()+" : "+"GFM_SettingsFoodMinimumThresholdStopJob".Translate( (int)(minFoodStopJob*100)));
            minFoodStopJob = list.Slider(minFoodStopJob, 0.0f, 1.0f);
            list.Label("GFM_SettingsJobStop".Translate() + " : " + "GFM_SettingsRestMinimumThresholdStopJob".Translate((int)(minRestStopJob * 100)));
            minRestStopJob = list.Slider(minRestStopJob, 0.0f, 1.0f);
            list.Label("GFM_SettingsJobStop".Translate() + " : " + "GFM_SettingsJoyMinimumThresholdStopJob".Translate((int)(minJoyStopJob * 100)));
            minJoyStopJob = list.Slider(minJoyStopJob, 0.0f, 1.0f);
            
            list.Label("GFM_SettingsJobStop".Translate() + " : " + "GFM_SettingsBladderMinimumThresholdStopJob".Translate((int)(minBladderStopJob * 100)));
            minBladderStopJob = list.Slider(minBladderStopJob, 0.0f, 1.0f);
            list.Label("GFM_SettingsJobStop".Translate() + " : " + "GFM_SettingsHygieneMinimumThresholdStopJob".Translate((int)(minHygieneStopJob * 100)));
            minHygieneStopJob = list.Slider(minHygieneStopJob, 0.0f, 1.0f);

            list.Label("GFM_SettingsPatrolWalkMode".Translate());
            if (list.RadioButton("GFM_SettingsPatrolWalkModeWalk".Translate(), (patrolWalkMode == 2)))
                patrolWalkMode = 2;
            if (list.RadioButton("GFM_SettingsPatrolWalkModeJog".Translate(), (patrolWalkMode == 3)))
                patrolWalkMode = 3;
            if (list.RadioButton("GFM_SettingsPatrolWalkModeSprint".Translate(), (patrolWalkMode == 4)))
                patrolWalkMode = 4;

            //Melee attack spotting radius 
            list.Label("GFM_SettingsMeleeAttackRadiusSpot".Translate(meleeAttackRadiusSpot));
            meleeAttackRadiusSpot = (int)list.Slider(meleeAttackRadiusSpot, 1,80);

            list.Label("GFM_SettingsStandingGuardMaxDistanceWithGS".Translate(standingGuardMaxDistanceWithGS));
            standingGuardMaxDistanceWithGS = (int)list.Slider(standingGuardMaxDistanceWithGS, 1, 80);
            

            list.End();
            Widgets.EndScrollView();
            //settings.Write();
        }

        public override void ExposeData()
        {
            base.ExposeData();


            Scribe_Values.Look<bool>(ref hideBodyguardButton, "hideBodyguardButton", false);
            Scribe_Values.Look<bool>(ref hidePatrolButton, "hidePatrolButton", false);
            Scribe_Values.Look<bool>(ref hideStandingGuardButton, "hideStandingGuardButton", false);
            Scribe_Values.Look<bool>(ref hideReactionForceButton, "hideReactionForceButton", false);


            Scribe_Values.Look<bool>(ref autoUnforbidThreatsKilledByGuards, "autoUnforbidThreatsKilledByGuards", false);
            Scribe_Values.Look<bool>(ref exterminatorMode, "exterminatorMode", false);
            Scribe_Values.Look<bool>(ref disableDisplayJobText, "disableDisplayJobText", false);
            Scribe_Values.Look<bool>(ref guardAsJob, "guardAsJob", false);
            
            Scribe_Values.Look<bool>(ref fixedStandGuard, "fixedStandGuard", false);
            Scribe_Values.Look<bool>(ref hideGuardspots, "hideGuardspots", false);
            Scribe_Values.Look<bool>(ref hideIcons, "hideIcons", false);
            Scribe_Values.Look<int>(ref pathEndMode, "pathEndMode", 0);
            Scribe_Values.Look<int>(ref patrolWalkMode, "patrolWalkMode", 3);


            Scribe_Values.Look<float>(ref minMoodStopJob, "minMoodStopJob", 0.25f);
            Scribe_Values.Look<float>(ref minJoyStopJob, "minJoyStopJob", 0.25f);
            Scribe_Values.Look<float>(ref minFoodStopJob, "minFoodStopJob", 0.3f);
            Scribe_Values.Look<float>(ref minRestStopJob, "minRestStopJob", 0.3f);

            Scribe_Values.Look<float>(ref minHygieneStopJob, "minHygieneStopJob", 0.3f);
            Scribe_Values.Look<float>(ref minBladderStopJob, "minBladderStopJob", 0.3f);

            Scribe_Values.Look<bool>(ref showColonistTitle, "showColonistTitle", false);

            Scribe_Values.Look<int>(ref meleeAttackRadiusSpot, "meleeAttackRadiusSpot", 20);
            Scribe_Values.Look<int>(ref standingGuardMaxDistanceWithGS, "standingGuardMaxDistanceWithGS", 20);
            Scribe_Values.Look<bool>(ref killDownedThreats, "killDownedThreats", false);
            



        }
    }
}