using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x02000BE8 RID: 3048
    public class ScenPart_PlayerPawnsArriveMethod : ScenPart
    {
        // Token: 0x060047C3 RID: 18371 RVA: 0x00180E25 File Offset: 0x0017F025
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<PlayerPawnsArriveMethod>(ref this.method, "method", (PlayerPawnsArriveMethod)RRYautja.PlayerPawnsArriveMethod.Standing, false);
        }

        // Token: 0x060047C4 RID: 18372 RVA: 0x00180E40 File Offset: 0x0017F040
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            if (Widgets.ButtonText(listing.GetScenPartRect(this, ScenPart.RowHeight), this.method.ToStringHuman(), true, true, true))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (object obj in Enum.GetValues(typeof(PlayerPawnsArriveMethod)))
                {
                    PlayerPawnsArriveMethod localM2 = (PlayerPawnsArriveMethod)obj;
                    PlayerPawnsArriveMethod localM = localM2;
                    list.Add(new FloatMenuOption(localM.ToStringHuman(), delegate ()
                    {
                        this.method = localM;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        // Token: 0x060047C5 RID: 18373 RVA: 0x00180F18 File Offset: 0x0017F118
        public override string Summary(Scenario scen)
        {
            if (this.method == PlayerPawnsArriveMethod.DropPods)
            {
                return "ScenPart_ArriveInDropPods".Translate();
            }
            if ((int)this.method == (int)RRYautja.PlayerPawnsArriveMethod.DropShip)
            {
                return "ScenPart_ArriveInDropShip".Translate();
            }
            return null;
        }

        // Token: 0x060047C6 RID: 18374 RVA: 0x00180F34 File Offset: 0x0017F134
        public override void Randomize()
        {
            this.method = ((Rand.Value < 0.5f) ? PlayerPawnsArriveMethod.DropPods : PlayerPawnsArriveMethod.Standing);
        }

        // Token: 0x060047C7 RID: 18375 RVA: 0x00180F4C File Offset: 0x0017F14C
        public override void GenerateIntoMap(Map map)
        {
            if (Find.GameInitData == null)
            {
                return;
            }
            List<List<Thing>> list = new List<List<Thing>>();
            foreach (Pawn item in Find.GameInitData.startingAndOptionalPawns)
            {
                list.Add(new List<Thing>
                {
                    item
                });
            }
            List<Thing> list2 = new List<Thing>();
            foreach (ScenPart scenPart in Find.Scenario.AllParts)
            {
                list2.AddRange(scenPart.PlayerStartingThings());
            }
            int num = 0;
            foreach (Thing thing in list2)
            {
                if (thing.def.CanHaveFaction)
                {
                    thing.SetFactionDirect(Faction.OfPlayer);
                }
                list[num].Add(thing);
                num++;
                if (num >= list.Count)
                {
                    num = 0;
                }
            }
            if ((int)this.method == (int)RRYautja.PlayerPawnsArriveMethod.DropShip)
            {

                GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(USCMDefOf.RRY_USCM_DropshipUD4LIncoming, ThingMaker.MakeThing(USCMDefOf.RRY_USCM_DropshipUD4L, null)), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
            }
            else
            DropPodUtility.DropThingGroupsNear(MapGenerator.PlayerStartSpot, map, list, 110, Find.GameInitData.QuickStarted || this.method != PlayerPawnsArriveMethod.DropPods, true, true, true);
        }

        // Token: 0x060047C8 RID: 18376 RVA: 0x001810B4 File Offset: 0x0017F2B4
        public override void PostMapGenerate(Map map)
        {
            if (Find.GameInitData == null)
            {
                return;
            }
            if (this.method == PlayerPawnsArriveMethod.DropPods || (int)this.method == (int)RRYautja.PlayerPawnsArriveMethod.DropShip)
            {
                PawnUtility.GiveAllStartingPlayerPawnsThought(ThoughtDefOf.CrashedTogether);
            }
        }

        // Token: 0x040028C7 RID: 10439
        private PlayerPawnsArriveMethod method;
    }
}
