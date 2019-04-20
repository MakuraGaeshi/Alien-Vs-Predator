using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace RRYautja.Harmony
{
    /*
    [HarmonyPatch(typeof(Pawn), "CheckAcceptArrest")]
    public static class Pawn_AcceptArrestPatch
    {
        [HarmonyPrefix]
        public static bool RevealSaboteur(Pawn __instance, Pawn arrester)
        {
            if (__instance.health.hediffSet.HasHediff(HediffDefOfIncidents.Saboteur))
            {
                __instance.health.hediffSet.hediffs.RemoveAll(h => h.def == HediffDefOfIncidents.Saboteur);
                Faction faction = Find.FactionManager.RandomEnemyFaction();
                __instance.SetFaction(faction);
                List<Pawn> thisPawn = new List<Pawn>();
                thisPawn.Add(__instance);
                IncidentParms parms = new IncidentParms();
                parms.faction = faction;
                parms.spawnCenter = __instance.Position;
                parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                parms.raidStrategy.Worker.MakeLords(parms, thisPawn);
                __instance.Map.avoidGrid.Regenerate();
                LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
                if (faction != null)
                {
                    Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(), "SaboteurRevealedFaction".Translate(__instance.LabelShort, faction.Name, __instance.Named("PAWN")), LetterDefOf.ThreatBig, __instance, null);
                }
                else
                {
                    Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(), "SaboteurRevealed".Translate(__instance.LabelShort, __instance.Named("PAWN")), LetterDefOf.ThreatBig, __instance, null);
                }
            }
            return true;
        }
    }
    */
    [HarmonyPatch(typeof(Pawn), "ThreatDisabled")]
    public static class Pawn_ThreatDisabledPatch
    {
        [HarmonyPostfix]
        public static void IgnoreThief(Pawn __instance, ref bool __result)
        {
            __result = __result || __instance.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked);
        }
    }
}
