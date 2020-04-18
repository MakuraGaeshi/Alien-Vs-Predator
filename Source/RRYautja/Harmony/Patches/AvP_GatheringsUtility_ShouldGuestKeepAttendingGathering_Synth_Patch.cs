using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AvP.HarmonyInstance
{
    // Token: 0x02000056 RID: 86
    [HarmonyPatch(typeof(GatheringsUtility), "ShouldGuestKeepAttendingGathering")]
    public class AvP_GatheringsUtility_ShouldGuestKeepAttendingGathering_Synth_Patch
    {
        // Token: 0x06000109 RID: 265 RVA: 0x00006F9C File Offset: 0x0000519C
        [HarmonyPrefix]
        public static bool Prefix(Pawn p, ref bool __result)
        {
            if (p.def != USCMDefOf.AvP_Synth)
            {
                return true;
            }
            Pawn_NeedsTracker needs = p.needs;
            bool flag;
            if (((needs != null) ? needs.food : null) != null)
            {
                Pawn_NeedsTracker needs2 = p.needs;
                flag = (((needs2 != null) ? needs2.rest : null) == null);
            }
            else
            {
                flag = true;
            }
            bool flag2 = flag;
            return !flag2 || ReplacementForSynths(p, out __result);
        }

        // Token: 0x0600010A RID: 266 RVA: 0x00006FEC File Offset: 0x000051EC
        private static bool ReplacementForSynths(Pawn p, out bool __result)
        {
            bool flag;
            if (!p.Downed)
            {
                Pawn_NeedsTracker needs = p.needs;
                if ((((needs != null) ? needs.food : null) == null || !p.needs.food.Starving) && p.health.hediffSet.BleedRateTotal <= 0f)
                {
                    Pawn_NeedsTracker needs2 = p.needs;
                    if ((((needs2 != null) ? needs2.rest : null) == null || p.needs.rest.CurCategory < (RestCategory)3) && !p.health.hediffSet.HasTendableNonInjuryNonMissingPartHediff(false) && RestUtility.Awake(p) && !p.InAggroMentalState)
                    {
                        flag = !p.IsPrisoner;
                        goto IL_A0;
                    }
                }
            }
            flag = false;
            IL_A0:
            __result = flag;
            return false;
        }
    }
}
