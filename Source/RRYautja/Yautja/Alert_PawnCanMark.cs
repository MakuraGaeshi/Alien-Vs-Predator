using RRYautja;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace RimWorld
{
    // Token: 0x020007BB RID: 1979
    public class Alert_PawnCanMark : Alert
    {
        // Token: 0x170006D7 RID: 1751
        // (get) Token: 0x06002BF4 RID: 11252 RVA: 0x001499E8 File Offset: 0x00147DE8
        private IEnumerable<Thing> SickPawns
        {
            get
            {
                foreach (Pawn p in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners_NoCryptosleep)
                {
                    if (p.health.hediffSet.hediffs.Any(x=> x.def == YautjaDefOf.RRY_Hediff_BloodedUM))
                    {
                        Hediff hediff = p.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                        if (hediff != null)
                        {
                            Comp_Yautja _Yautja = p.TryGetComp<Comp_Yautja>();
                            if (_Yautja != null)
                            {
                                if (_Yautja.corpse!=null&&!_Yautja.corpse.DestroyedOrNull())
                                {
                                    yield return p;
                                    _Yautja.corpse.SetForbidden(true);
                                    if (Find.Selector.SingleSelectedThing == p)
                                    {
                                        yield return _Yautja.corpse;
                                    }
                                }
                            }
                        }
                    }
                    /*
                    List<Hediff> hediffs = p.health.hediffSet.hediffs.FindAll(x => x.TryGetComp<HediffComp_BloodedYautja>() != null);
                    foreach (var x in hediffs)
                    {
                        if (x.TryGetComp<HediffComp_BloodedYautja>() != null && x.TryGetComp<HediffComp_BloodedYautja>() is HediffComp_BloodedYautja _Blooded)
                        {

                        }
                    }
                    */
                }
                yield break;
            }
        }

        // Token: 0x06002BF5 RID: 11253 RVA: 0x00149A04 File Offset: 0x00147E04
        public override string GetLabel()
        {
            return "RRY_CanMarkSelf".Translate();
        }

        // Token: 0x06002BF6 RID: 11254 RVA: 0x00149A10 File Offset: 0x00147E10
        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Thing thing in this.SickPawns)
            {
                if (thing is Pawn pawn)
                {
                    Comp_Yautja _Yautja = pawn.TryGetComp<Comp_Yautja>();
                    stringBuilder.AppendLine("    " + thing.LabelShort + " on :" + _Yautja.corpse.LabelShortCap);
                }
            }
            return "RRY_CanMarkSelfDesc".Translate(stringBuilder.ToString());
        }

        // Token: 0x06002BF7 RID: 11255 RVA: 0x00149B50 File Offset: 0x00147F50
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(this.SickPawns);
        }
    }
}
