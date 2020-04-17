using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace RRYautja
{
    [StaticConstructorOnStartup]
    static class UtilCE
    {
        public static string tag = "CETeam.CombatExtended";
        public static bool CombatExtended = false;
        public static ModContentPack modContent=null;
        static UtilCE()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == tag || p.PackageId == tag)
                {
                    modContent = p;
                    CombatExtended = true;
                }
            }
        }

    }
    
    static class UtilChjAndroids
    {
        public static string tag = "ChJees.Androids";
        public static bool ChjAndroid = false;
        public static ModContentPack modContent = null;
        static UtilChjAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == tag || p.PackageId == tag)
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
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap ,pawn.race.modContentPack.PackageId));
            return pawn.race.modContentPack == modContent;
        }
        public static bool isChjAndroid(Pawn pawn)
        {
            //    bool Result = pawn.def.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.def.modContentPack.PackageId));
            return pawn.def.modContentPack == modContent;
        }
        public static bool isChjAndroid(ThingDef td)
        {
            //    bool Result = td.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            Log.Message(string.Format("{0}: {1}", td.LabelCap, td.modContentPack.PackageId));
            return td.modContentPack == modContent;
        }
    }

    static class UtilTieredAndroids
    {
        public static string tag = "Atlas.AndroidTiers";
        public static bool TieredAndroid = false;
        public static ModContentPack modContent = null;
        static UtilTieredAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == tag || p.PackageId == tag)
                {
                    modContent = p;
                    //    Log.Message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing)); Haplo.Miscellaneous.Robots
                    TieredAndroid = true;
                }
            }
        }

        public static bool isAtlasAndroid(PawnKindDef pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.race.modContentPack.PackageId));
            return pawn.race.modContentPack == modContent;
        }
        public static bool isAtlasAndroid(Pawn pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.def.modContentPack.PackageId));
            return pawn.def.modContentPack == modContent;
        }
        public static bool isAtlasAndroid(ThingDef td)
        {
            Log.Message(string.Format("{0}: {1}", td.LabelCap, td.modContentPack.PackageId));
            return td.modContentPack == modContent;
        }
    }

    static class UtilMiscRobots
    {
        public static string tag = "Haplo.Miscellaneous.Robots";
        public static bool TieredAndroid = false;
        public static ModContentPack modContent = null;
        static UtilMiscRobots()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == tag || p.PackageId == tag)
                {
                    modContent = p;
                    //    Log.Message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing)); Alaestor.MiscRobots.PlusPlus
                    TieredAndroid = true;
                }
            }
        }

        public static bool isMiscRobot(PawnKindDef pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.race.modContentPack.PackageId));
            return pawn.race.modContentPack == modContent;
        }
        public static bool isMiscRobot(Pawn pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.def.modContentPack.PackageId));
            return pawn.def.modContentPack == modContent;
        }
        public static bool isMiscRobot(ThingDef td)
        {
            Log.Message(string.Format("{0}: {1}", td.LabelCap, td.modContentPack.PackageId));
            return td.modContentPack == modContent;
        }
    }
    static class UtilMiscRobotsPP
    {
        public static string tag = "Alaestor.MiscRobots.PlusPlus";
        public static bool TieredAndroid = false;
        public static ModContentPack modContent = null;
        static UtilMiscRobotsPP()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == tag || p.PackageId == tag)
                {
                    modContent = p;
                    //    Log.Message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing)); Alaestor.MiscRobots.PlusPlus
                    TieredAndroid = true;
                }
            }
        }

        public static bool isMiscRobot(PawnKindDef pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.race.modContentPack.PackageId));
            return pawn.race.modContentPack == modContent;
        }
        public static bool isMiscRobot(Pawn pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.def.modContentPack.PackageId));
            return pawn.def.modContentPack == modContent;
        }
        public static bool isMiscRobot(ThingDef td)
        {
            Log.Message(string.Format("{0}: {1}", td.LabelCap, td.modContentPack.PackageId));
            return td.modContentPack == modContent;
        }
    }

    static class UtilDinosauria
    {
        public static string tag = "spincrus.dinosauria";
        public static bool Dinosauria = false;
        public static ModContentPack modContent = null;
        static UtilDinosauria()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == tag || p.PackageId == tag)
                {
                    modContent = p;
                    Dinosauria = true;
                }
            }
        }

        public static bool isDinosauria(PawnKindDef pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.race.modContentPack.PackageId));
            return pawn.race.modContentPack == modContent;
        }
        public static bool isDinosauria(Pawn pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.def.modContentPack.PackageId));
            return pawn.def.modContentPack == modContent;
        }
        public static bool isDinosauria(ThingDef td)
        {
            Log.Message(string.Format("{0}: {1}", td.LabelCap, td.modContentPack.PackageId));
            return td.modContentPack == modContent;
        }

    }

    static class UtilJurassicRimworld
    {
        public static string tag = "Serpy.JurassicRimworld";
        public static bool JurassicRimworld = false;
        public static ModContentPack modContent = null;
        static UtilJurassicRimworld()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == tag || p.PackageId == tag)
                {
                    modContent = p;
                    JurassicRimworld = true;
                }
            }
        }

        public static bool isJurassic(PawnKindDef pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.race.modContentPack.PackageId));
            return pawn.race.modContentPack == modContent;
        }
        public static bool isJurassic(Pawn pawn)
        {
            Log.Message(string.Format("{0}: {1}", pawn.LabelCap, pawn.def.modContentPack.PackageId));
            return pawn.def.modContentPack == modContent;
        }
        public static bool isJurassic(ThingDef td)
        {
            Log.Message(string.Format("{0}: {1}", td.LabelCap, td.modContentPack.PackageId));
            return td.modContentPack == modContent;
        }
    }
}