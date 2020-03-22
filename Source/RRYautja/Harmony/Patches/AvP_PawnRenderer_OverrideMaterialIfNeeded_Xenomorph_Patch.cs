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
	// InvisibilityMatPool GetInvisibleMat

    [HarmonyPatch(typeof(PawnRenderer), "OverrideMaterialIfNeeded")]
    public static class AvP_PawnRenderer_OverrideMaterialIfNeeded_Xenomorph_Patch
    {
        [HarmonyPostfix]
        public static void OverrideMaterialIfNeeded(PawnRenderer __instance, Material original, Pawn pawn,ref Material __result)
		{

			if (pawn.IsInvisible() && pawn.isXenomorph())
			{
				__result.SetTexture(AvP_PawnRenderer_OverrideMaterialIfNeeded_Xenomorph_Patch.NoiseTex, TexGame.NoiseTex);
				__result.color = AvP_PawnRenderer_OverrideMaterialIfNeeded_Xenomorph_Patch.xenomorphColor;
			}
			/*
			else if (pawn.IsInvisible() && pawn.isCloaked())
			{
				__result.SetTexture(AvP_PawnRenderer_OverrideMaterialIfNeeded_Xenomorph_Patch.NoiseTex, TexGame.RippleTex);
				__result.color = AvP_PawnRenderer_OverrideMaterialIfNeeded_Xenomorph_Patch.xenomorphColor;
			}
			*/
		}

		// Token: 0x04001120 RID: 4384
		private static Color xenomorphColor = new Color(0.25f, 0.25f, 0.25f, 0.0001f);
		private static int NoiseTex = Shader.PropertyToID("_NoiseTex");


	}
   
}