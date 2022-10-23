using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace aRandomKiwi.GFM
{
    [StaticConstructorOnStartup]
    static class Tex
    {
        static Tex()
        {
            bodyGuard = MaterialPool.MatFrom("UI/GFM_Guard", ShaderDatabase.MetaOverlay);
            standGuard = MaterialPool.MatFrom("UI/GFM_GuardMode", ShaderDatabase.MetaOverlay);
            patrolGuard = MaterialPool.MatFrom("UI/GFM_NoPatrolMode", ShaderDatabase.MetaOverlay);
            deathSquadGuard = MaterialPool.MatFrom("UI/GFM_DeathSquad", ShaderDatabase.MetaOverlay);
        }

        
        public static readonly Texture2D affectGuardToGS = ContentFinder<Texture2D>.Get("UI/GFM_AffectGuardToGS", true);
        public static readonly Texture2D guardMode = ContentFinder<Texture2D>.Get("UI/GFM_GuardMode", true);
        public static readonly Texture2D guard = ContentFinder<Texture2D>.Get("UI/GFM_Guard", true);
        public static readonly Texture2D stopGuard = ContentFinder<Texture2D>.Get("UI/GFM_StopGuard", true);
        public static readonly Texture2D stopAllGuardians = ContentFinder<Texture2D>.Get("UI/GFM_StopAllGuardians", true);
        public static readonly Texture2D guardThreatDistance = ContentFinder<Texture2D>.Get("UI/GFM_GuardDistanceThreatCommand", true);
        public static readonly Texture2D deathSquad = ContentFinder<Texture2D>.Get("UI/GFM_DeathSquad", true);
        public static readonly Texture2D deathSquadFocus = ContentFinder<Texture2D>.Get("UI/GFM_DeathSquadFocus", true);


        //Waypoints
        public static readonly Texture2D blackWP = ContentFinder<Texture2D>.Get("Waypoints/BlackWP", true);
        public static readonly Texture2D redWP = ContentFinder<Texture2D>.Get("Waypoints/RedWP", true);
        public static readonly Texture2D orangeWP = ContentFinder<Texture2D>.Get("Waypoints/OrangeWP", true);
        public static readonly Texture2D yellowWP = ContentFinder<Texture2D>.Get("Waypoints/YellowWP", true);
        public static readonly Texture2D pinkWP = ContentFinder<Texture2D>.Get("Waypoints/PinkWP", true);
        public static readonly Texture2D greenWP = ContentFinder<Texture2D>.Get("Waypoints/GreenWP", true);
        public static readonly Texture2D grayWP = ContentFinder<Texture2D>.Get("Waypoints/GrayWP", true);
        public static readonly Texture2D purpleWP = ContentFinder<Texture2D>.Get("Waypoints/PurpleWP", true);
        public static readonly Texture2D blueWP = ContentFinder<Texture2D>.Get("Waypoints/BlueWP", true);

        //GuardSpots
        public static readonly Texture2D blackGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpot", true);
        public static readonly Texture2D redGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotRed", true);
        public static readonly Texture2D orangeGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotOrange", true);
        public static readonly Texture2D yellowGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotYellow", true);
        public static readonly Texture2D pinkGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotPink", true);
        public static readonly Texture2D greenGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotGreen", true);
        public static readonly Texture2D grayGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotGray", true);
        public static readonly Texture2D purpleGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotPurple", true);
        public static readonly Texture2D blueGS = ContentFinder<Texture2D>.Get("Building/GFM_GuardSpotBlue", true);


        public static readonly Texture2D noPatrolMode = ContentFinder<Texture2D>.Get("UI/GFM_NoPatrolMode", true);
        public static readonly Texture2D settingsHeader = ContentFinder<Texture2D>.Get("Settings/Header", true);

        public static readonly Texture2D dirTop = ContentFinder<Texture2D>.Get("UI/GFM_dirTop", true);
        public static readonly Texture2D dirBottom = ContentFinder<Texture2D>.Get("UI/GFM_dirBottom", true);
        public static readonly Texture2D dirLeft = ContentFinder<Texture2D>.Get("UI/GFM_dirLeft", true);
        public static readonly Texture2D dirRight = ContentFinder<Texture2D>.Get("UI/GFM_dirRight", true);

        public static readonly Material bodyGuard;
        public static readonly Material standGuard;
        public static readonly Material patrolGuard;
        public static readonly Material deathSquadGuard;

    }
}
