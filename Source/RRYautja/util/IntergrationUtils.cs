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
                if (p.PackageId == "1631756268")
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
                if (p.Name.Contains("ChjAndroid"))
                {
                    ChjAndroid = true;
                }
            }
        }

        public static bool isChjAndroid(PawnKindDef pawn)
        {
            bool Result = pawn.race.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));

            return Result;
        }
        public static bool isChjAndroid(Pawn pawn)
        {
            bool Result = pawn.def.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));

            return Result;
        }
        public static bool isChjAndroid(ThingDef td)
        {
            bool Result = td.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));

            return Result;
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
                if (p.Name.Contains("Android tiers"))
                {
                    TieredAndroid = true;
                }
            }
        }

        public static bool isAtlasAndroid(PawnKindDef pawn)
        {

            bool Result = false;
            if (pawn.race.modExtensions != null)
            {
                Result = pawn.race.modExtensions.Any(x => x.GetType().Name.Contains("MOARANDROIDS.AndroidTweaker"));
            }
            return Result;
        }
        public static bool isAtlasAndroid(Pawn pawn)
        {
            bool Result = false;
            if (pawn.def.modExtensions != null)
            {
                Result = pawn.def.modExtensions.Any(x => x.GetType().Name.Contains("MOARANDROIDS.AndroidTweaker"));
            }
            return Result;
        }
        public static bool isAtlasAndroid(ThingDef td)
        {
            bool Result = false;
            if (td.modExtensions != null)
            {
                Result = td.modExtensions.Any(x => x.GetType().Name.Contains("MOARANDROIDS.AndroidTweaker"));
            }
            return Result;
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
                if (p.Name.Contains("Dinosauria"))
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
                if (p.Name.Contains("Jurassic Rimworld"))
                {
                    JurassicRimworld = true;
                }
            }
        }

    }
}