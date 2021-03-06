﻿using HunterMarkingSystem;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RRYautja.ExtensionMethods
{
    [StaticConstructorOnStartup]
    public static class AvPExtensions
    {
        
        public static void GainEquipmentAbility(this Pawn_AbilityTracker tracker ,EquipmentAbilityDef def, ThingWithComps thing)
        {
            if (!tracker.abilities.Any((Ability a) => a.def == def))
            {
                EquipmentAbility ab = Activator.CreateInstance(def.abilityClass, new object[]
                {
                    tracker.pawn,
                    def
                }) as EquipmentAbility;
                ab.sourceEquipment = thing;
                tracker.abilities.Add(ab);
            }
        }

        public static MapComponent_HiveGrid HiveGrid(this Map m)
        {
            return m.GetComponent<MapComponent_HiveGrid>();
        }
        public static MapComponent_GooGrid GooGrid(this Map m)
        {
            return m.GetComponent<MapComponent_GooGrid>();
        }

        public static bool isYautja(this Pawn p)
        {
            return p.def == YautjaDefOf.RRY_Alien_Yautja;
        }

        public static bool isUnblooded(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_Unblooded"));
        }

        public static bool isBlooded(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_Blooded"));
        }

        public static bool isBloodUnmarked(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_BloodedUM"));
        }

        public static bool isBloodMarked(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_BloodedM"));
        }

        public static bool switchLord (this Pawn p, Lord L)
        {
        //    Log.Message(string.Format("trying to switch {0} to {1}", p.LabelShortCap, L));
            if (p.GetLord() != null && p.GetLord() is Lord l)
            {
            //    Log.Message(string.Format("{0} currently belongs to {1}", p.LabelShortCap, l));
                if (l.ownedPawns.Count > 0)
                {
                //    Log.Message(string.Format("removing {0} from {1}", p.LabelShortCap, l));
                    l.ownedPawns.Remove(p);
                //    Log.Message(string.Format("removed {0} from {1}: {2}", p.LabelShortCap, l, p.GetLord() == null));
                }
                if (l.ownedPawns.Count == 0)
                {
                //    Log.Message(string.Format("removed {0} final pawn, removing l", l));
                    l.lordManager.RemoveLord(l);
                //    Log.Message(string.Format("removed l: {0}", l==null));
                }
            //    Log.Message(string.Format("{0} currently has lord: {1}", p.LabelShortCap, p.GetLord() == null));
            }
        //    Log.Message(string.Format("adding {0} to {1}", p.LabelShortCap, L));
            L.AddPawn(p);
        //    Log.Message(string.Format("addied {0} to {1} = {2}", p.LabelShortCap, L, p.GetLord() == L));
            return p.GetLord() == L;

        }

        public static Comp_Xenomorph xenomorph(this Pawn p)
        {
            if (p.isXenomorph())
            {
                return p.TryGetComp<Comp_Xenomorph>();
            }
            return null;
        }

        public static Comp_Xenomorph xenomorph(this Thing p)
        {
            if (p.isXenomorph())
            {
                return p.TryGetComp<Comp_Xenomorph>();
            }
            return null;
        }

        /*
        public static HiveLike hive(this Pawn p)
        {
            if (p.isXenomorph())
            {
                if (p.GetLord()!=null && p.GetLord().LordJob is LordJob_DefendAndExpandHiveLike)
                {
                    if (p.GetLord().CurLordToil is LordToil_DefendAndExpandHiveLike daehl)
                    {
                        if (daehl.Data.HiveFocus)
                        {

                        }
                    }
                    return p.GetLord().lord;
                }
            }
            return null;
        }
        */

        public static Comp_Neomorph neomorph(this Pawn p)
        {
            if (p.isNeomorph())
            {
                return p.TryGetComp<Comp_Neomorph>();
            }
            return null;
        }
        
        public static bool isCocooned(this Pawn p)
        {
            return p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
        }

        public static bool isXenomorph(this Pawn p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }
        public static bool isXenomorph(this Thing p)
        {
            return p.def.race?.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }
        public static bool isXenomorph(this Pawn p, out Comp_Xenomorph comp)
        {
            comp = p.TryGetComp<Comp_Xenomorph>()?? null;
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }

        public static bool isNeomorph(this Pawn p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Neomorph;
        }

        public static bool isXenomorph(this PawnKindDef p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }

        public static bool isNeomorph(this PawnKindDef p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Neomorph;
        }

        public static bool isXenomorph(this ThingDef p)
        {
            return p.race.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }

        public static bool isNeomorph(this ThingDef p)
        {
            return p.race.FleshType == XenomorphRacesDefOf.RRY_Neomorph;
        }

        public static bool isPotentialHost(this Thing t, bool setDefaults = false, bool allowImpreg = false)
        {
            Pawn p = t as Pawn;
            if (p==null)
            {
                return false;
            }
            return p.isPotentialHost(out string FailReason, setDefaults, allowImpreg);
        }

        public static bool isPotentialHost(this Pawn p, bool setDefaults = false, bool allowImpreg = false, bool allowHugged = true)
        {
            return p.isPotentialHost(out string FailReason, setDefaults, allowImpreg);
        }
        public static bool isPotentialHost(this Pawn p, out string FailReason, bool setDefaults = false, bool allowImpreg = false, bool allowHugged = true)
        {
            if (!p.health.hediffSet.HasHead)
            {
                FailReason = "HasHead";
                return false;
            }
            if (p.Dead)
            {
                FailReason = "Dead";
                return false;
            }
            if (!p.health.capacities.CapableOf(PawnCapacityDefOf.Breathing) || !p.health.capacities.CapableOf(PawnCapacityDefOf.Eating))
            {
                FailReason = "Doesnt Eat or Breath";
                return false;
            }
            if (p.isHost())
            {
                FailReason = "Host";
                if (p.isNeoHost())
                {
                    FailReason = "Neo " + FailReason;
                }
                if (p.isXenoHost() && !allowImpreg)
                {
                    FailReason = "Xeno " + FailReason;
                }
                return false;
            }
            if (XenomorphUtil.IsXenomorphFaction(p))
            {
                FailReason = "Xenomorph Faction Member";
                return false;
            }
            if (p.BodySize < 0.65f && !p.RaceProps.Humanlike)
            {
                FailReason = "NonhumanlikeTooSmall";
                return false;
            }
            if (!p.def.isPotentialHost(out FailReason, setDefaults))
            {
                return false;
            }
            return true;
        }

        public static bool isPotentialHost(this PawnKindDef p, bool setDefaults = false)
        {
            return p.isPotentialHost(out string FailReason, setDefaults);
        }

        public static bool isPotentialHost(this PawnKindDef p, out string FailReason, bool setDefaults = false)
        {
            return p.race.isPotentialHost(out FailReason, setDefaults);
        }

        public static bool isPotentialHost(this ThingDef thingDef, bool setDefaults = false)
        {
            return thingDef.isPotentialHost(out string FailReason, setDefaults);
        }
        public static bool isPotentialHost(this ThingDef thingDef, out string FailReason, bool setDefaults = false)
        {
            FailReason = string.Empty;
            if (thingDef.race == null)
            {
                FailReason = string.Format("{0} has No Race Properties", thingDef);
                return false;
            }
            if (!settings.SettingsHelper.latest.AllowNonHumanlikeHosts && !thingDef.race.Humanlike)
            {
                FailReason = string.Format("Non-Humanlike Not Allowed");
                return false;
            }
            if (!setDefaults && settings.SettingsHelper.latest.RaceKeyPairs != null)
            {
                if (settings.SettingsHelper.latest.RaceKeyPairs.ContainsKey(thingDef.defName))
                {
                    FailReason = string.Format("Not Allowed");
                    return settings.SettingsHelper.latest.RaceKeyPairs.GetValueOrDefault(thingDef.defName);
                }
            }
            string NonBio = "Inorganic";
            if (thingDef.isXenomorph() || thingDef.isNeomorph())
            {
                string str = thingDef.isXenomorph() ? "Xenomorph" : "Neomorph";
                FailReason = string.Format("{0}", str);
                return false;
            }
            bool pawnflag = !((UtilChjAndroids.ChjAndroid && UtilChjAndroids.isChjAndroid(thingDef)) || (UtilTieredAndroids.TieredAndroid && UtilTieredAndroids.isAtlasAndroid(thingDef)) || (UtilSynths.isAvPSynth(thingDef)));
            if (!pawnflag)
            {
                string str = string.Empty;
                if (UtilChjAndroids.ChjAndroid && UtilChjAndroids.isChjAndroid(thingDef))
                {
                    str = NonBio;
                }
                if (UtilTieredAndroids.TieredAndroid && UtilTieredAndroids.isAtlasAndroid(thingDef))
                {
                    str = NonBio;
                }

                if (UtilSynths.isAvPSynth(thingDef))
                {
                    str = NonBio;
                }
                FailReason = string.Format("{0}", str);
                return false;
            }
            if (thingDef.race.IsMechanoid) { FailReason = NonBio; return false; }
            if (thingDef.race.body.defName.Contains("AIRobot")) { FailReason = NonBio; return false; }
            if (thingDef.defName.Contains("TM_"))
            {
                if (thingDef.defName.Contains("Undead") || thingDef.defName.Contains("Minion") || thingDef.defName.Contains("Demon")) { FailReason = NonBio; return false; }
            }
            if (thingDef.race.FleshType.defName.Contains("TM_StoneFlesh")) { FailReason = NonBio; return false; }
            if (thingDef.race.FleshType.defName.Contains("Chaos") && thingDef.race.FleshType.defName.Contains("Deamon")) { FailReason = NonBio; return false; }
            if (thingDef.race.FleshType.defName.Contains("Construct") && thingDef.race.FleshType.defName.Contains("Flesh")) { FailReason = NonBio; return false; }
            if (thingDef.race.baseBodySize < 0.65f && !thingDef.race.Humanlike) { FailReason = string.Format("Too Small", thingDef); return false; }


            return true;
        }
        /*
        public static bool isPotentialHost(this Pawn p, out string failReason)
        {
            failReason = string.Empty;
            if (!p.health.hediffSet.HasHead)
            {
                failReason = "HasHead";
                return false;
            }
            if (p.Dead)
            {
                failReason = "Dead";
                return false;
            }
            if (!XenomorphUtil.isInfectableThing(p.def, out failReason))
            {
                return false;
            }
            if (!p.health.capacities.CapableOf(PawnCapacityDefOf.Breathing) || !p.health.capacities.CapableOf(PawnCapacityDefOf.Eating))
            {
                failReason = "Doesnt Eat or Breath";
                return false;
            }
            if (p.isHost())
            {
                failReason = "Host";
                if (p.isNeoHost())
                {
                    failReason = "Neo "+ failReason;
                }
                if (p.isXenoHost())
                {
                    failReason = "Xeno "+ failReason;
                }
                return false;
            }
            if (XenomorphUtil.IsXenomorphFaction(p))
            {
                failReason = "Xenomorph Faction Member";
                return false;
            }
            if (p.BodySize < 0.65f && !p.RaceProps.Humanlike)
            {
                failReason = "NonhumanlikeTooSmall";
                return false;
            }
            return true;
        }

        public static bool isPotentialHost(this Pawn p)
        {
            return XenomorphUtil.isInfectablePawn(p) && !p.isXenomorph() && !p.isNeomorph() && p.health.hediffSet.HasHead && !p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
        }
        public static bool isPotentialHost(this Thing t)
        {
            Pawn p = (Pawn)t;
            return XenomorphUtil.isInfectablePawn(p) && !p.isXenomorph() && !p.isNeomorph() && p.health.hediffSet.HasHead && !p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
        }

        public static bool isPotentialHost(this PawnKindDef p)
        {
            return XenomorphUtil.isInfectablePawnKind(p) && !p.isXenomorph() && !p.isNeomorph();
        }

        public static bool isPotentialHost(this PawnKindDef p, out string failReason)
        {
            failReason = string.Empty;
            if (!p.race.race.body.AllParts.Any(x=>x.def.defName.Contains("Head")))
            {
                failReason = "HasHead";
                return false;
            }
            if (p.isNeomorph())
            {
                failReason = "isNeomorph";
                return false;
            }
            if (p.isXenomorph())
            {
                failReason = "isXenomorph";
                return false;
            }
            if (UtilChjAndroids.ChjAndroid)
            {
                if (p.race.defName == "ChjAndroid" || p.race.defName == "ChjDroid")
                {
                    failReason = "ChjAndroid";
                    return false;
                }
            }
            if (UtilTieredAndroids.TieredAndroid)
            {
                if (p.race.defName.Contains("Android" + "Tier"))
                {
                    failReason = "TieredAndroid";
                    return false;
                }
            }
            if (p.RaceProps.body.defName.Contains("AIRobot"))
            {
                failReason = "AIRobot";
                return false;
            }
            if (p.race.defName.Contains("Android"))
            {
                failReason = "Android";
                return false;
            }
            if (p.race.defName.Contains("Droid"))
            {
                failReason = "Droid";
                return false;
            }
            if (p.race.defName.Contains("Mech"))
            {
                failReason = "Mech";
                return false;
            }
            if (p.race.defName.Contains("TM_Undead"))
            {
                failReason = "TM_Undead";
                return false;
            }
            if (p.race.race.FleshType.defName.Contains("TM_StoneFlesh"))
            {
                failReason = "TM_StoneFlesh";
                return false;
            }
            if (p.race.defName.Contains("TM_") && p.race.defName.Contains("Minion"))
            {
                failReason = "TM_Minion";
                return false;
            }
            if (p.race.defName.Contains("Demon"))
            {
                failReason = "TM_Demon";
                return false;
            }
            if (p.race.race.FleshType.defName.Contains("Deamon"))
            {
                failReason = "ChaosDeamon";
                return false;
            }
            if (p.race.race.FleshType.defName.Contains("Necron"))
            {
                failReason = "Necron";
                return false;
            }
            if (p.race.race.FleshType.defName.Contains("EldarConstruct"))
            {
                failReason = "EldarConstruct";
                return false;
            }
            if (p.race.race.FleshType.defName.Contains("ImperialConstruct"))
            {
                failReason = "ImperialConstruct";
                return false;
            }
            if (p.race.race.FleshType.defName.Contains("MechanicusConstruct"))
            {
                failReason = "MechanicusConstruct";
                return false;
            }
            if (p.RaceProps.IsMechanoid)
            {
                failReason = "IsMechanoid";
                return false;
            }
            if (!p.RaceProps.IsFlesh)
            {
                failReason = "IsFlesh";
                return false;
            }
            if (p.defaultFactionType==XenomorphDefOf.RRY_Xenomorph)
            {
                failReason = "IsXenomorphFaction";
                return false;
            }
            if (p.race.race.baseBodySize < 0.65f && !p.RaceProps.Humanlike)
            {
                failReason = "NonhumanlikeTooSmall";
                return false;
            }
            return true;
        }

        public static bool isPotentialHost(this ThingDef p, bool setDefaults = false)
        {
            if (!setDefaults && settings.SettingsHelper.latest.RaceKeyPairs!=null)
            {
                if (settings.SettingsHelper.latest.RaceKeyPairs.ContainsKey(p.defName))
                {
                    return settings.SettingsHelper.latest.RaceKeyPairs.GetValueOrDefault(p.defName);
                }
            }
            return XenomorphUtil.isInfectableThing(p) && !p.isXenomorph() && !p.isNeomorph();
        }

        public static bool isPotentialHost(this ThingDef p, out string failReason)
        {
            failReason = string.Empty;
            if (!p.race.body.AllParts.Any(x => x.def.defName.Contains("Head")))
            {
                failReason = "HasHead";
                return false;
            }
            if (p.isNeomorph())
            {
                failReason = "isNeomorph";
                return false;
            }
            if (p.isXenomorph())
            {
                failReason = "isXenomorph";
                return false;
            }
            if (UtilChjAndroids.ChjAndroid)
            {
                if (p.defName == "ChjAndroid" || p.defName == "ChjDroid")
                {
                    failReason = "ChjAndroid";
                    return false;
                }
            }
            if (UtilTieredAndroids.TieredAndroid)
            {
                if (p.defName.Contains("Android" + "Tier"))
                {
                    failReason = "TieredAndroid";
                    return false;
                }
            }
            if (p.race.body.defName.Contains("AIRobot"))
            {
                failReason = "AIRobot";
                return false;
            }
            if (p.defName.Contains("Android"))
            {
                failReason = "Android";
                return false;
            }
            if (p.defName.Contains("Droid"))
            {
                failReason = "Droid";
                return false;
            }
            if (p.defName.Contains("Mech"))
            {
                failReason = "Mech";
                return false;
            }
            if (p.defName.Contains("TM_Undead"))
            {
                failReason = "TM_Undead";
                return false;
            }
            if (p.race.FleshType.defName.Contains("TM_StoneFlesh"))
            {
                failReason = "TM_StoneFlesh";
                return false;
            }
            if (p.defName.Contains("TM_") && p.defName.Contains("Minion"))
            {
                failReason = "TM_Minion";
                return false;
            }
            if (p.defName.Contains("TM_Demon"))
            {
                failReason = "TM_Demon";
                return false;
            }
            if (p.race.FleshType.defName.Contains("ChaosDeamon"))
            {
                failReason = "ChaosDeamon";
                return false;
            }
            if (p.race.FleshType.defName.Contains("Necron"))
            {
                failReason = "Necron";
                return false;
            }
            if (p.race.FleshType.defName.Contains("EldarConstruct"))
            {
                failReason = "EldarConstruct";
                return false;
            }
            if (p.race.FleshType.defName.Contains("ImperialConstruct"))
            {
                failReason = "ImperialConstruct";
                return false;
            }
            if (p.race.FleshType.defName.Contains("MechanicusConstruct"))
            {
                failReason = "MechanicusConstruct";
                return false;
            }
            if (p.race.IsMechanoid)
            {
                failReason = "IsMechanoid";
                return false;
            }
            if (!p.race.IsFlesh)
            {
                failReason = "IsFlesh";
                return false;
            }
            if (p.race.baseBodySize < 0.65f && !p.race.Humanlike)
            {
                failReason = "NonhumanlikeTooSmall";
                return false;
            }
            return true;
        }
        */
        public static List<Pawn> ViableHosts(this Map m)
        {
            return m.mapPawns.AllPawnsSpawned.FindAll(x => x.isPotentialHost());
        }

        public static List<Pawn> InviableHosts(this Map m)
        {
            return m.mapPawns.AllPawnsSpawned.FindAll(x => !x.isPotentialHost());
        }

        public static List<Pawn> CocoonedPawns(this Map m)
        {
            return m.mapPawns.AllPawnsSpawned.FindAll(x => x.isCocooned());
        }

        public static bool isHost(this Pawn p)
        {
            return p.isNeoHost() || p.isXenoHost();
        }
        public static bool isHost(this Pawn p, out Hediff hediff)
        {
            bool result = p.isNeoHost() || p.isXenoHost();
            if (result)
            {
                hediff = p.health.hediffSet.hediffs.Find(x => x.def.defName.Contains("morphImpregnation") || x.def == XenomorphDefOf.RRY_FaceHuggerInfection);
            }
            else
            {
                hediff = null;
            }
            return result;
        }
        public static bool isXenoHost(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("XenomorphImpregnation") || x.def==XenomorphDefOf.RRY_FaceHuggerInfection);
        }
        public static bool isNeoHost(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x =>  x.def.defName.Contains("NeomorphImpregnation"));
        }
        public static List<PawnKindDef> PossibleXenoforms(this Pawn p)
        {
            List<PawnKindDef> list = new List<PawnKindDef>();

            return list;
        }
        public static PawnKindDef resultingXenomorph(this Pawn p)
        {
            PawnKindDef kindDef = null;
            if (!p.isPotentialHost() || p.BodySize > 0.63f)
            {
                return null;
            }
            bool human = p.def.defName.Contains("Human") || HMSUtility.GetMark(p.kindDef) == YautjaDefOf.HMS_Hediff_BloodedMHuman;
            bool yautja = p.def.defName.Contains("Yautja");
            bool thrumbo = p.def.defName.Contains("Human") || HMSUtility.GetMark(p.kindDef) == YautjaDefOf.HMS_Hediff_BloodedMThrumbo;
            bool hound = HMSUtility.GetMark(p.kindDef) == YautjaDefOf.HMS_Hediff_BloodedMHound;

            bool humanlike = p.RaceProps.Humanlike;

            bool large = humanlike ? p.BodySize > 1f : p.BodySize > 4f;
            bool small = p.BodySize > 1f;

            bool predalienEmbryo = p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("XenomorphImpregnation")) && ((XenoHediffWithComps)p.health.hediffSet.hediffs.Find(x => x.def.defName.Contains("XenomorphImpregnation"))).TryGetComp<HediffComp_XenoSpawner>().predalienImpregnation;
            bool royaleEmbryo = p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("XenomorphImpregnation")) && ((XenoHediffWithComps)p.health.hediffSet.hediffs.Find(x => x.def.defName.Contains("XenomorphImpregnation"))).TryGetComp<HediffComp_XenoSpawner>().RoyaleEmbryo;

            if (humanlike)
            {
                if (human)
                {
                    if (predalienEmbryo)
                    {
                        kindDef = p.kindDef.isFighter ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                    }
                    else
                    {
                        if (royaleEmbryo)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Queen;
                        }
                        else
                        {
                            if (large)
                            {
                                kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                            }
                            else
                            {
                                if (small)
                                {
                                    kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                                else
                                {
                                    kindDef = p.kindDef.isFighter ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (yautja)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
                    }
                    else
                    {
                        if (royaleEmbryo)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Queen;
                        }
                        else
                        {
                            if (large)
                            {
                                kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                            }
                            else
                            {
                                if (small)
                                {
                                    kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                                else
                                {
                                    kindDef = p.kindDef.RaceProps.predator ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (large)
                {
                    if (thrumbo)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Thrumbomorph;
                    }
                    else
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    }
                }
                else
                {
                    if (hound)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                        }
                        else
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                    }
                }
            }

            bool selected = Find.Selector.SingleSelectedThing == p;
            if (selected && Prefs.DevMode)
            {
            //    Log.Message(string.Format("{0} will spawn from {1}", kindDef, p.LabelShortCap));
            }
            return kindDef;
        }

        public static PawnKindDef resultingXenomorph(this PawnKindDef p)
        {
            PawnKindDef kindDef = null;
            if (!p.isPotentialHost() || p.RaceProps.baseBodySize > 0.63f)
            {
                return null;
            }
            bool human = p.race.defName.Contains("Human") || HMSUtility.GetMark(p) == YautjaDefOf.HMS_Hediff_BloodedMHuman;
            bool yautja = p.race.defName.Contains("Yautja");
            bool thrumbo = p.race.defName.Contains("Human") || HMSUtility.GetMark(p) == YautjaDefOf.HMS_Hediff_BloodedMThrumbo;
            bool hound = HMSUtility.GetMark(p).defName.Contains("BloodedMHound");

            bool humanlike = p.RaceProps.Humanlike;

            bool large = humanlike ? p.RaceProps.baseBodySize > 1f : p.RaceProps.baseBodySize > 4f;
            bool small = p.RaceProps.baseBodySize > 1f;

            if (humanlike)
            {
                if (human)
                {
                    if (large)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                        else
                        {
                            kindDef = p.isFighter ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                    }
                }
                else
                {
                    if (yautja)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
                    }
                    else
                    {
                        if (large)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                        }
                        else
                        {
                            if (small)
                            {
                                kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                            }
                            else
                            {
                                kindDef = p.RaceProps.predator ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                            }
                        }
                    }
                }
            }
            else
            {
                if (large)
                {
                    if (thrumbo)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Thrumbomorph;
                    }
                    else
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    }
                }
                else
                {
                    if (hound)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                        }
                        else
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                    }
                }
            }
            return kindDef;
        }

        public static List<PawnKindDef> resultingXenomorph(this ThingDef p)
        {
            List<PawnKindDef> kindDef = new List<PawnKindDef>();
            if (!p.isPotentialHost() || p.race.baseBodySize < 0.63f)
            {
                return null;
            }
            bool human = p.defName.Contains("Human");
            bool yautja = p.defName.Contains("Alien_Yautja");
            bool thrumbo = p.defName.Contains("Thrumbo");
            bool hound = p.defName.Contains("Hound") || p.defName.Contains("hound") || p.defName.Contains("Dog") || p.defName.Contains("dog") || p.defName.Contains("Wolf") || p.defName.Contains("wolf") || p.defName.Contains("Warg") || p.defName.Contains("warg") || p.description.Contains("Hound") || p.description.Contains("hound") || p.description.Contains("Dog") || p.description.Contains("dog") || p.description.Contains("Wolf") || p.description.Contains("wolf") || p.description.Contains("Warg") || p.description.Contains("warg");

            bool humanlike = p.race.Humanlike;

            bool large = humanlike ? p.race.baseBodySize > 1f : p.race.baseBodySize >= 3.5f;
            bool small = p.race.baseBodySize > 1f;

            if (humanlike)
            {
                if (human)
                {
                    if (large)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                        }
                        else
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                        }
                    }
                }
                else
                {
                    if (yautja)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Predalien);
                    }
                    else
                    {
                        if (large)
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                        }
                        else
                        {
                            if (small)
                            {
                                kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                            }
                            else
                            {
                                kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                                kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                            }
                        }
                    }
                }
            }
            else
            {
                if (large)
                {
                    if (thrumbo)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Thrumbomorph);
                    }
                    else
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                    }
                }
                else
                {
                    if (hound)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Runner);
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Runner);
                        }
                        else
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                        }
                    }
                }
            }
            return kindDef;
        }

        public static HediffDef unbloodedDef = YautjaDefOf.HMS_Hediff_Unblooded;
        public static HediffDef unmarkedDef = YautjaDefOf.HMS_Hediff_BloodedUM;
        public static HediffDef markedDef = YautjaDefOf.HMS_Hediff_BloodedM;
        public static AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_YoungBlood");
        public static AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_Blooded");
        public static AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodA");
        public static AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodB");

        // Token: 0x02000D68 RID: 3432
        public enum BloodStatusMode
        {
            NoComp,
            None,
            Unblooded,
            Unmarked,
            Marked
        }

    }

}
