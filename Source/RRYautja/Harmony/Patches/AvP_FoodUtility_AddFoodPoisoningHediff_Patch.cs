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
    
    [HarmonyPatch(typeof(FoodUtility), "AddFoodPoisoningHediff")]
    public static class AvP_FoodUtility_AddFoodPoisoningHediff_Patch
    {
        public static bool Prefix(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        { 
            //    Log.Message(string.Format("checkin if {0} can get food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
            CompFoodPoisonProtection compFood = pawn.TryGetComp<CompFoodPoisonProtection>();
            if (compFood != null)
            {
                if (!compFood.Props.Poisonable)
                {
                    //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because compFood.Props.Poisonable {2}", pawn.Name, ingestible, compFood.Props.Poisonable));
                    return false;
                }
                if (!compFood.Props.FoodTypeFlags.NullOrEmpty<FoodTypeFlags>())
                {
                    foreach (var ftf in compFood.Props.FoodTypeFlags)
                    {
                        if (ftf == ingestible.def.ingestible.foodType)
                        {
                            //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, ingestible.def.ingestible.foodType));
                            return false;
                        }
                    }
                }
                if (!compFood.Props.FoodPoisonCause.NullOrEmpty<FoodPoisonCause>())
                {
                    foreach (var fpc in compFood.Props.FoodPoisonCause)
                    {
                        if (fpc == cause)
                        {
                            //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
    
}