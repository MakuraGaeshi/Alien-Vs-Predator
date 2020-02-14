﻿using HunterMarkingSystem.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace HunterMarkingSystem
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
                    if (p.Markable(out Comp_Markable Markable))
                    {
                        if (p.health.hediffSet.hediffs.Any(x=> HunterMarkingSystem.BloodedUMHediffList.Contains(x.def)))
                        {
                            Hediff hediff = p.health.hediffSet.hediffs.Find(x=> HunterMarkingSystem.BloodedUMHediffList.Contains(x.def));
                            if (hediff != null)
                            {
                                if (Markable.MarkableCorpse)
                                {
                                    yield return p;
                                    Markable.Markcorpse.SetForbidden(true);
                                    if (Find.Selector.SingleSelectedThing == p)
                                    {
                                        yield return Markable.Markcorpse;
                                    }
                                    else
                                    {
                                    //    yield return Markable.Markcorpse;
                                    }
                                }
                                else
                                {
                                    if (Markable.Markcorpse != null)
                                    {
                                        if (Markable.Markcorpse.Destroyed)
                                        {
                                            //    Log.Message(string.Format("{0}'s target {1}, is destroyed", p.LabelShortCap, Markable.Markcorpse));
                                            p.health.RemoveHediff(hediff);
                                            if (Markable.MarkerRace)
                                            {
                                                p.health.AddHediff(Markable.Unmarkeddef, Markable.partRecord);
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else
                                        {
                                        //    Log.Message(string.Format("{0}'s target {1}, is NOT destroyed", p.LabelShortCap, Markable.Markcorpse));
                                        }

                                    }
                                    else
                                    {
                                        if (Markable.Mark!=null)
                                        {
                                            if (Markable.Mark.Downed)
                                            {
                                            //    Log.Message(string.Format("{0}'s Mark is Downed", p.LabelShortCap));
                                            }
                                            else
                                            {
                                            //    Log.Message(string.Format("{0}'s Mark is Not Downed", p.LabelShortCap));
                                            }
                                        }
                                        else
                                        {
                                        //    Log.Message(string.Format("{0}'s Mark is null", p.LabelShortCap));
                                            p.health.RemoveHediff(hediff);
                                            if (Markable.MarkerRace)
                                            {
                                                p.health.AddHediff(Markable.Unmarkeddef, Markable.partRecord);
                                            }
                                            else
                                            {
                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                        //    Log.Message(string.Format("{0}'s target corpse, is null", p.LabelShortCap));
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
                    Comp_Markable _Yautja = pawn.TryGetComp<Comp_Markable>();
                    stringBuilder.AppendLine("    " + thing.LabelShort + " on :" + _Yautja.Markcorpse.LabelShortCap);
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