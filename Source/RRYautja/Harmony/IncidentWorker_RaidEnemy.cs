using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;

namespace RRYautja
{
    /*
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecute")]
    public static class IncidentWorker_RaidEnemyPatch
    {
        [HarmonyPostfix]
        public static void PopSaboteurs(bool __result, IncidentParms parms)
        {
            if(__result && parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                List<Pawn> saboteurs = (from p in ((Map)parms.target).mapPawns.FreeColonistsSpawned
                                        where p.health.hediffSet.HasHediff(HediffDefOfIncidents.Saboteur)
                                        select p).ToList();
                if (saboteurs.Count > 0)
                {
                    Pawn saboteur = saboteurs.RandomElement();
                    if (Rand.Value < 0.33f)
                    {
                        saboteur.health.hediffSet.hediffs.RemoveAll(h => h.def == HediffDefOfIncidents.Saboteur);
                        Faction oldFaction = saboteur.Faction;
                        saboteur.SetFaction(parms.faction);
                        Lord enemyLord = saboteur.Map.lordManager.lords.Find((Lord x) => x.faction == parms.faction);
                        enemyLord.ownedPawns.Add(saboteur);
                        saboteur.mindState.duty = new PawnDuty(DutyDefOf.AssaultColony);
                        Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(), "SaboteurRevealedFaction".Translate(saboteur.LabelShort, parms.faction.Name, saboteur.Named("PAWN")), LetterDefOf.ThreatBig, saboteur, null);
                    }
                }
            }
        }
    }
    */
}
