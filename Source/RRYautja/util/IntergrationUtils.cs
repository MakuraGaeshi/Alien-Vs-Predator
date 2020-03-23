using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace RRYautja
{
    [StaticConstructorOnStartup]
    static class UtilCE
    {
        public static bool CombatExtended = false;
        public static ModContentPack modContent=null;
        static UtilCE()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing.Contains("CETeam.CombatExtended"))
                {
                    modContent = p;
                    CombatExtended = true;
                }
            }
        }

    }
    
    static class UtilChjAndroids
    {
        public static bool ChjAndroid = false;
        public static ModContentPack modContent = null;
        static UtilChjAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing.Contains("ChJees.Androids"))
                {
                    modContent = p;
                //    Log.Message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing));
                    ChjAndroid = true;
                }
            }

        }

        public static bool isChjAndroid(PawnKindDef pawn)
        {
            //    bool Result = pawn.race.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            bool Result = false;
            if (pawn.race.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isChjAndroid(Pawn pawn)
        {
            //    bool Result = pawn.def.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            bool Result = false;
            if (pawn.def.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isChjAndroid(ThingDef td)
        {
            //    bool Result = td.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            bool Result = false;
            if (td.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
    }

    static class UtilTieredAndroids
    {
        public static bool TieredAndroid = false;
        public static ModContentPack modContent = null;
        static UtilTieredAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing.Contains("Atlas.AndroidTiers"))
                {
                    modContent = p;
                //    Log.Message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing));
                    TieredAndroid = true;
                }
            }
        }

        public static bool isAtlasAndroid(PawnKindDef pawn)
        {

            bool Result = false;
            if (pawn.race.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isAtlasAndroid(Pawn pawn)
        {
            bool Result = false;
            if (pawn.def.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isAtlasAndroid(ThingDef td)
        {
            bool Result = false;
            if (td.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
    }

    static class UtilDinosauria
    {
        public static bool Dinosauria = false;
        public static ModContentPack modContent = null;
        static UtilDinosauria()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.Contains("Dinosauria"))
                {
                    modContent = p;
                    Dinosauria = true;
                }
            }
        }

    }

    static class UtilJurassicRimworld
    {
        public static bool JurassicRimworld = false;
        public static ModContentPack modContent = null;
        static UtilJurassicRimworld()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.Contains("Jurassic Rimworld"))
                {
                    modContent = p;
                    JurassicRimworld = true;
                }
            }
        }

    }
}