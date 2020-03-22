using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using RRYautja.settings;
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    
    [HarmonyPatch(typeof(SelfDefenseUtility), "ShouldFleeFrom")]
    public static class AvP_SelfDefenseUtility_ShouldFleeFrom_Patch
    {
        [HarmonyPostfix]
        public static void ShouldFleeFromPostfix(Thing t, Pawn pawn, bool checkDistance, bool checkLOS, ref bool __result)
        {
            bool result = __result;
            if (pawn!=null && t!=null)
            {
                Pawn other = null;
                bool XenoP = pawn.isXenomorph(out Comp_Xenomorph P_Xeno);
                bool NeoP = pawn.isNeomorph();
                if (t.GetType() == typeof(Pawn))
                {
                    other = (Pawn)t;
                if (Find.Selector.SelectedObjects.Contains(other)) Log.Message(string.Format("{0} vs {1}", pawn.LabelShortCap, other.LabelShortCap));
                }
                if (other == null)
                {
                    if (t.GetType() == typeof(Fire))
                    {
                        result = XenoP || NeoP;
                    }
                }
                else
                {
                    bool XenoOP = other.isXenomorph(out Comp_Xenomorph OP_Xeno);
                    bool NeoOP = other.isNeomorph();
                    if (!XenoP && !NeoP)
                    {
                        if (XenoOP)
                        {
                            if (pawn.RaceProps.Animal)
                            {
                                if (pawn.playerSettings != null)
                                {
                                    if (!pawn.playerSettings.animalsReleased && !pawn.playerSettings.followDrafted)
                                    {
                                        result = true;
                                    }
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                            if (pawn.Faction == null)
                            {
                                result = true;
                            }
                        }
                        if (NeoOP)
                        {
                            if (pawn.RaceProps.Animal)
                            {
                                if (pawn.playerSettings != null)
                                {
                                    if (!pawn.playerSettings.animalsReleased && !pawn.playerSettings.followDrafted)
                                    {
                                        result = true;
                                    }
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (NeoP && XenoOP)
                        {
                            if (pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_NeomorphFullyFormed)
                            {
                                result = true;
                            }
                        }
                        if (XenoP && NeoOP)
                        {
                            if (pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_XenomorphFullyFormed || pawn.def == XenomorphRacesDefOf.RRY_Xenomorph_FaceHugger)
                            {
                                result = true;
                            }
                        }
                    }
                }

            }
            else
            {
                return;
            }
            IAttackTarget attackTarget = t as IAttackTarget;
            bool r = attackTarget != null && !attackTarget.ThreatDisabled(pawn) && t is IAttackTargetSearcher && (!checkLOS || GenSight.LineOfSight(pawn.Position, t.Position, pawn.Map, false, null, 0, 0));
            __result = (result && r);
        }
    }
    
}