﻿using RimWorld;
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
using AvP.settings;
using AvP.ExtensionMethods;

namespace AvP.HarmonyInstance
{
	// InvisibilityMatPool GetInvisibleMat

    [HarmonyPatch(typeof(PawnGraphicSet), "HairMatAt")]
    public static class AvP_PawnGraphicSet_HairMatAt_Invis_Patch
	{
        [HarmonyPostfix]
        public static void HairMatAt(PawnGraphicSet __instance, Rot4 facing, ref Material __result)
		{
			Pawn pawn = __instance.pawn;
			PawnGraphicSet graphics = new PawnGraphicSet_Invisible(pawn)
			{
				nakedGraphic = new Graphic_Invisible(),
				rottingGraphic = null,
				packGraphic = null,
				headGraphic = new Graphic_Invisible(),
				desiccatedHeadGraphic = null,
				skullGraphic = null,
				headStumpGraphic = null,
				desiccatedHeadStumpGraphic = null,
				hairGraphic = new Graphic_Invisible()
			};
			if (pawn.IsInvisible() && pawn.RaceProps.Humanlike)
			{
				if (pawn.CarriedBy!=null)
				{
					if (pawn.CarriedBy.isXenomorph())
					{
						__result.SetTexture(graphics.hairGraphic.MatSingle.name, graphics.hairGraphic.MatSingle.mainTexture);
						__result.shader = ShaderDatabase.Cutout;
						return;
					}
				}
			}
		}


	}
   
}