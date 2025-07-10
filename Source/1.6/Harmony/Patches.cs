using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace aRandomKiwi.GFM
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var inst = new Harmony("rimworld.randomKiwi.GFM");
            inst.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static FieldInfo MapFieldInfo;
    }
}
