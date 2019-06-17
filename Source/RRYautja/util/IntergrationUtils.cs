using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace RRYautja
{
    [StaticConstructorOnStartup]
    static class UtilCE
    {
        private static bool logging = false;
        private static bool initialized = false;
        public static bool CombatExtended = false;
        static UtilCE()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Identifier == "1631756268")
                {
                    CombatExtended = true;
                }
            }
        }

    }

    static class UtilChjAndroids
    {
        private static bool logging = false;
        private static bool initialized = false;
        public static bool ChjAndroid = false;
        static UtilChjAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Identifier == "1541064015" || p.Name == "ChjAndroid")
                {
                    ChjAndroid = true;
                }
            }
        }

    }

    static class UtilTieredAndroids
    {
        private static bool logging = false;
        private static bool initialized = false;
        public static bool TieredAndroid = false;
        static UtilTieredAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Identifier == "1386412863" || p.Name.Contains("Android tiers"))
                {
                    TieredAndroid = true;
                }
            }
        }

    }

    static class UtilDinosauria
    {
        private static bool logging = false;
        private static bool initialized = false;
        public static bool Dinosauria = false;
        static UtilDinosauria()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Identifier == "1136958577" && p.Name.Contains("Dinosauria"))
                {
                    Dinosauria = true;
                }
            }
        }

    }

    static class UtilJurassicRimworld
    {
        private static bool logging = false;
        private static bool initialized = false;
        public static bool JurassicRimworld = false;
        static UtilJurassicRimworld()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Identifier == "1419618659" && p.Name.Contains("Jurassic Rimworld"))
                {
                    JurassicRimworld = true;
                }
            }
        }

    }
}