using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000861 RID: 2145
    public class CompProperties_USCMDropship : CompProperties
    {
        // Token: 0x06003429 RID: 13353 RVA: 0x0011B149 File Offset: 0x00119349
        public CompProperties_USCMDropship()
        {
            this.compClass = typeof(CompUSCMDropship);
        }
    }
    // Token: 0x02000CFD RID: 3325
    [StaticConstructorOnStartup]
    public class CompUSCMDropship : CompLaunchable
    {
        // Token: 0x17000E48 RID: 3656
        // (get) Token: 0x06005000 RID: 20480 RVA: 0x001A7A80 File Offset: 0x001A5C80
        public bool Autoload
        {
            get
            {
                return this.autoload;
            }
        }
        /*
        // Token: 0x17000E49 RID: 3657
        // (get) Token: 0x06005001 RID: 20481 RVA: 0x001A7A88 File Offset: 0x001A5C88
        public bool LoadingInProgressOrReadyToLaunch
        {
            get
            {
                return this.Transporter.LoadingInProgressOrReadyToLaunch;
            }
        }

        // Token: 0x17000E4A RID: 3658
        // (get) Token: 0x06005002 RID: 20482 RVA: 0x001A7A95 File Offset: 0x001A5C95
        public List<CompTransporter> TransportersInGroup
        {
            get
            {
                return this.Transporter.TransportersInGroup(this.parent.Map);
            }
        }

        // Token: 0x17000E4C RID: 3660
        // (get) Token: 0x06005004 RID: 20484 RVA: 0x001A7AD0 File Offset: 0x001A5CD0
        public bool AnyInGroupIsUnderRoof
        {
            get
            {
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    if (transportersInGroup[i].parent.Position.Roofed(this.parent.Map))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        */
        // Token: 0x17000E4B RID: 3659
        // (get) Token: 0x06005003 RID: 20483 RVA: 0x001A7AAD File Offset: 0x001A5CAD
        public new CompTransporter Transporter
        {
            get
            {
                if (this.cachedCompTransporter == null)
                {
                    this.cachedCompTransporter = this.parent.GetComp<CompTransporter>();
                }
                return this.cachedCompTransporter;
            }
        }
        public new List<CompTransporter> TransportersInGroup
        {
            get
            {
                return this.Transporter.TransportersInGroup(this.parent.Map);
            }
        }


        // Token: 0x17000E4D RID: 3661
        // (get) Token: 0x06005005 RID: 20485 RVA: 0x001A7B1C File Offset: 0x001A5D1C
        private bool Autoloadable
        {
            get
            {
                if (this.cachedTransporterList == null)
                {
                    this.cachedTransporterList = new List<CompTransporter>
                    {
                        this.Transporter
                    };
                }
                foreach (Pawn thing in TransporterUtility.AllSendablePawns(this.cachedTransporterList, this.parent.Map))
                {
                    if (!this.IsRequired(thing))
                    {
                        return false;
                    }
                }
                foreach (Thing thing2 in TransporterUtility.AllSendableItems(this.cachedTransporterList, this.parent.Map))
                {
                    if (!this.IsRequired(thing2))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        // Token: 0x17000E4E RID: 3662
        // (get) Token: 0x06005006 RID: 20486 RVA: 0x001A7BF8 File Offset: 0x001A5DF8
        public bool AllRequiredThingsLoaded
        {
            get
            {
                ThingOwner innerContainer = this.Transporter.innerContainer;
                for (int i = 0; i < this.requiredPawns.Count; i++)
                {
                    if (!innerContainer.Contains(this.requiredPawns[i]))
                    {
                        return false;
                    }
                }
                if (this.requiredColonistCount > 0)
                {
                    int num = 0;
                    for (int j = 0; j < innerContainer.Count; j++)
                    {
                        Pawn pawn = innerContainer[j] as Pawn;
                        if (pawn != null && pawn.IsFreeColonist)
                        {
                            num++;
                        }
                    }
                    if (num < this.requiredColonistCount)
                    {
                        return false;
                    }
                }
                CompUSCMDropship.tmpRequiredItemsWithoutDuplicates.Clear();
                for (int k = 0; k < this.requiredItems.Count; k++)
                {
                    bool flag = false;
                    for (int l = 0; l < CompUSCMDropship.tmpRequiredItemsWithoutDuplicates.Count; l++)
                    {
                        if (CompUSCMDropship.tmpRequiredItemsWithoutDuplicates[l].ThingDef == this.requiredItems[k].ThingDef)
                        {
                            CompUSCMDropship.tmpRequiredItemsWithoutDuplicates[l] = CompUSCMDropship.tmpRequiredItemsWithoutDuplicates[l].WithCount(CompUSCMDropship.tmpRequiredItemsWithoutDuplicates[l].Count + this.requiredItems[k].Count);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        CompUSCMDropship.tmpRequiredItemsWithoutDuplicates.Add(this.requiredItems[k]);
                    }
                }
                for (int m = 0; m < CompUSCMDropship.tmpRequiredItemsWithoutDuplicates.Count; m++)
                {
                    int num2 = 0;
                    for (int n = 0; n < innerContainer.Count; n++)
                    {
                        if (innerContainer[n].def == CompUSCMDropship.tmpRequiredItemsWithoutDuplicates[m].ThingDef)
                        {
                            num2 += innerContainer[n].stackCount;
                        }
                    }
                    if (num2 < CompUSCMDropship.tmpRequiredItemsWithoutDuplicates[m].Count)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        // Token: 0x17000E4F RID: 3663
        // (get) Token: 0x06005007 RID: 20487 RVA: 0x001A7DEC File Offset: 0x001A5FEC
        public TaggedString RequiredThingsLabel
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < this.requiredPawns.Count; i++)
                {
                    stringBuilder.AppendLine("  - " + this.requiredPawns[i].NameShortColored.Resolve());
                }
                for (int j = 0; j < this.requiredItems.Count; j++)
                {
                    stringBuilder.AppendLine("  - " + this.requiredItems[j].LabelCap);
                }
                return stringBuilder.ToString().TrimEndNewlines();
            }
        }

        // Token: 0x06005008 RID: 20488 RVA: 0x001A7E8B File Offset: 0x001A608B
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            if (this.Autoloadable)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "CommandAutoloadTransporters".Translate(),
                    defaultDesc = "CommandAutoloadTransportersDesc".Translate(),
                    icon = CompUSCMDropship.AutoloadToggleTex,
                    isActive = (() => this.autoload),
                    toggleAction = delegate ()
                    {
                        this.autoload = !this.autoload;
                        if (this.autoload && !this.LoadingInProgressOrReadyToLaunch)
                        {
                            TransporterUtility.InitiateLoading(Gen.YieldSingle<CompTransporter>(this.Transporter));
                        }
                        this.CheckAutoload();
                    }
                };
            }
            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = "CommandSendShuttle".Translate();
            command_Action.defaultDesc = "CommandSendShuttleDesc".Translate();
            command_Action.icon = CompUSCMDropship.SendCommandTex;
            command_Action.alsoClickIfOtherInGroupClicked = false;
            command_Action.action = delegate ()
            {
                this.StartChoosingDestination();
            };
            /*
		    if (!this.AllFuelingPortSourcesInGroupHaveAnyFuel)
            {
                command_Action.Disable("CommandLaunchGroupFailNoFuel".Translate());
            }
            else
            */
            if (!this.LoadingInProgressOrReadyToLaunch || !this.AllRequiredThingsLoaded)
            {
                command_Action.Disable("CommandSendShuttleFailMissingRequiredThing".Translate());
            }
            else if (this.AnyInGroupIsUnderRoof)
            {
                command_Action.Disable("CommandSendShuttleFailUnderRoof".Translate());
            }
            yield return command_Action;
            foreach (Gizmo gizmo2 in QuestUtility.GetQuestRelatedGizmos(this.parent))
            {
                yield return gizmo2;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        public new bool AllFuelingPortSourcesInGroupHaveAnyFuel
        {
            get
            {
                Log.Message("1");
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                Log.Message("2");
                try
                {
                    for (int i = 0; i < transportersInGroup.Count; i++)
                    {
                        Log.Message("3");
                        if (!transportersInGroup[i].Launchable.FuelingPortSourceHasAnyFuel)
                        {
                            Log.Message("return false");
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    Log.Message("TransportersInGroup false");
                    throw;
                }
                Log.Message("return true");
                return true;
            }
        }
        // Token: 0x06005009 RID: 20489 RVA: 0x001A7E9B File Offset: 0x001A609B
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (!selPawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield break;
            }
            string text = "EnterShuttle".Translate();
            if (!this.IsAllowed(selPawn))
            {
                yield return new FloatMenuOption(text + " (" + "NotAllowed".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield break;
            }
            yield return new FloatMenuOption(text, delegate ()
            {
                if (!this.LoadingInProgressOrReadyToLaunch)
                {
                    TransporterUtility.InitiateLoading(Gen.YieldSingle<CompTransporter>(this.Transporter));
                }
                Job job = JobMaker.MakeJob(JobDefOf.EnterTransporter, this.parent);
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.Default, null, null, 0f, null, null);
            yield break;
        }

        // Token: 0x0600500A RID: 20490 RVA: 0x001A7EB4 File Offset: 0x001A60B4
        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.IsHashIntervalTick(120))
            {
                this.CheckAutoload();
            }
            if (this.leaveASAP && this.parent.Spawned)
            {
                if (!this.LoadingInProgressOrReadyToLaunch)
                {
                    TransporterUtility.InitiateLoading(Gen.YieldSingle<CompTransporter>(this.Transporter));
                }
                this.Send();
            }
            if (this.leaveImmediatelyWhenSatisfied && this.AllRequiredThingsLoaded)
            {
                this.Send();
            }
        }

        // Token: 0x0600500B RID: 20491 RVA: 0x001A7F28 File Offset: 0x001A6128
        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Required".Translate() + ": ");
            CompUSCMDropship.tmpRequiredLabels.Clear();
            if (this.requiredColonistCount > 0)
            {
                CompUSCMDropship.tmpRequiredLabels.Add(this.requiredColonistCount + " " + ((this.requiredColonistCount > 1) ? Faction.OfPlayer.def.pawnsPlural : Faction.OfPlayer.def.pawnSingular));
            }
            for (int i = 0; i < this.requiredPawns.Count; i++)
            {
                CompUSCMDropship.tmpRequiredLabels.Add(this.requiredPawns[i].LabelShort);
            }
            for (int j = 0; j < this.requiredItems.Count; j++)
            {
                CompUSCMDropship.tmpRequiredLabels.Add(this.requiredItems[j].Label);
            }
            if (CompUSCMDropship.tmpRequiredLabels.Any<string>())
            {
                stringBuilder.Append(CompUSCMDropship.tmpRequiredLabels.ToCommaList(true).CapitalizeFirst());
            }
            else
            {
                stringBuilder.Append("Nothing".Translate());
            }
            return stringBuilder.ToString();
        }

        // Token: 0x06004F0A RID: 20234 RVA: 0x001A37AC File Offset: 0x001A19AC
        private void StartChoosingDestination()
        {
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.parent));
            Find.WorldSelector.ClearSelection();
            int tile = this.parent.Map.Tile;
            Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, CompLaunchable.TargeterMouseAttachment, true, delegate
            {
                GenDraw.DrawWorldRadiusRing(tile, this.MaxLaunchDistance);
            }, delegate (GlobalTargetInfo target)
            {
                if (!target.IsValid)
                {
                    return null;
                }
                int num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile, true, int.MaxValue);
                if (num > this.MaxLaunchDistance)
                {
                    GUI.color = Color.red;
                    if (num > this.MaxLaunchDistanceEverPossible)
                    {
                        return "TransportPodDestinationBeyondMaximumRange".Translate();
                    }
                    return "TransportPodNotEnoughFuel".Translate();
                }
                else
                {
                    IEnumerable<FloatMenuOption> transportPodsFloatMenuOptionsAt = this.GetTransportPodsFloatMenuOptionsAt(target.Tile);
                    if (!transportPodsFloatMenuOptionsAt.Any<FloatMenuOption>())
                    {
                        return "";
                    }
                    if (transportPodsFloatMenuOptionsAt.Count<FloatMenuOption>() == 1)
                    {
                        if (transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Disabled)
                        {
                            GUI.color = Color.red;
                        }
                        return transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Label;
                    }
                    MapParent mapParent = target.WorldObject as MapParent;
                    if (mapParent != null)
                    {
                        return "ClickToSeeAvailableOrders_WorldObject".Translate(mapParent.LabelCap);
                    }
                    return "ClickToSeeAvailableOrders_Empty".Translate();
                }
            });
        }

        // Token: 0x06004F0B RID: 20235 RVA: 0x001A3830 File Offset: 0x001A1A30
        private bool ChoseWorldTarget(GlobalTargetInfo target)
        {
            if (!this.LoadingInProgressOrReadyToLaunch)
            {
                return true;
            }
            if (!target.IsValid)
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            int num = Find.WorldGrid.TraversalDistanceBetween(this.parent.Map.Tile, target.Tile, true, int.MaxValue);
            if (num > this.MaxLaunchDistance)
            {
                Messages.Message("MessageTransportPodsDestinationIsTooFar".Translate(CompLaunchable.FuelNeededToLaunchAtDist((float)num).ToString("0.#")), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            IEnumerable<FloatMenuOption> transportPodsFloatMenuOptionsAt = this.GetTransportPodsFloatMenuOptionsAt(target.Tile);
            if (!transportPodsFloatMenuOptionsAt.Any<FloatMenuOption>())
            {
                if (Find.World.Impassable(target.Tile))
                {
                    Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                    return false;
                }
                this.TryLaunch(target.Tile, null);
                return true;
            }
            else
            {
                if (transportPodsFloatMenuOptionsAt.Count<FloatMenuOption>() == 1)
                {
                    if (!transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Disabled)
                    {
                        transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().action();
                    }
                    return false;
                }
                Find.WindowStack.Add(new FloatMenu(transportPodsFloatMenuOptionsAt.ToList<FloatMenuOption>()));
                return false;
            }
        }

        // Token: 0x06004F0C RID: 20236 RVA: 0x001A3968 File Offset: 0x001A1B68
        public void TryLaunch(int destinationTile, TransportPodsArrivalAction arrivalAction)
        {
            if (!this.parent.Spawned)
            {
                Log.Error("Tried to launch " + this.parent + ", but it's unspawned.", false);
                return;
            }
            List<CompTransporter> transportersInGroup = this.TransportersInGroup;
            if (transportersInGroup == null)
            {
                Log.Error("Tried to launch " + this.parent + ", but it's not in any group.", false);
                return;
            }
            if (!this.LoadingInProgressOrReadyToLaunch || !this.AllInGroupConnectedToFuelingPort || !this.AllFuelingPortSourcesInGroupHaveAnyFuel)
            {
                return;
            }
            Map map = this.parent.Map;
            int num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, destinationTile, true, int.MaxValue);
            if (num > this.MaxLaunchDistance)
            {
                return;
            }
            this.Transporter.TryRemoveLord(map);
            int groupID = this.Transporter.groupID;
            float amount = Mathf.Max(CompLaunchable.FuelNeededToLaunchAtDist((float)num), 1f);
            for (int i = 0; i < transportersInGroup.Count; i++)
            {
                CompTransporter compTransporter = transportersInGroup[i];
                Building fuelingPortSource = compTransporter.Launchable.FuelingPortSource;
                if (fuelingPortSource != null)
                {
                    fuelingPortSource.TryGetComp<CompRefuelable>().ConsumeFuel(amount);
                }
                ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();
                ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDefOf.ActiveDropPod, null);
                activeDropPod.Contents = new ActiveDropPodInfo();
                activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, true, true);
                DropPodLeaving dropPodLeaving = (DropPodLeaving)SkyfallerMaker.MakeSkyfaller(ThingDefOf.DropPodLeaving, activeDropPod);
                dropPodLeaving.groupID = groupID;
                dropPodLeaving.destinationTile = destinationTile;
                dropPodLeaving.arrivalAction = arrivalAction;
                compTransporter.CleanUpLoadingVars(map);
                compTransporter.parent.Destroy(DestroyMode.Vanish);
                GenSpawn.Spawn(dropPodLeaving, compTransporter.parent.Position, map, WipeMode.Vanish);
            }
            CameraJumper.TryHideWorld();
        }
        /*
        // Token: 0x06004F0D RID: 20237 RVA: 0x001A3B0C File Offset: 0x001A1D0C
        public void Notify_FuelingPortSourceDeSpawned()
        {
            if (this.Transporter.CancelLoad())
            {
                Messages.Message("MessageTransportersLoadCanceled_FuelingPortGiverDeSpawned".Translate(), this.parent, MessageTypeDefOf.NegativeEvent, true);
            }
        }

        // Token: 0x06004F0E RID: 20238 RVA: 0x001A3B40 File Offset: 0x001A1D40
        public static int MaxLaunchDistanceAtFuelLevel(float fuelLevel)
        {
            return Mathf.FloorToInt(fuelLevel / 2.25f);
        }

        // Token: 0x06004F0F RID: 20239 RVA: 0x001A3B4E File Offset: 0x001A1D4E
        public static float FuelNeededToLaunchAtDist(float dist)
        {
            return 2.25f * dist;
        }
        */
        // Token: 0x06004F10 RID: 20240 RVA: 0x001A3B57 File Offset: 0x001A1D57
        public new IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptionsAt(int tile)
        {
            bool anything = false;
            if (TransportPodsArrivalAction_FormCaravan.CanFormCaravanAt(this.TransportersInGroup.Cast<IThingHolder>(), tile) && !Find.WorldObjects.AnySettlementBaseAt(tile) && !Find.WorldObjects.AnySiteAt(tile))
            {
                anything = true;
                yield return new FloatMenuOption("FormCaravanHere".Translate(), delegate ()
                {
                    this.TryLaunch(tile, new TransportPodsArrivalAction_FormCaravan());
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            List<WorldObject> worldObjects = Find.WorldObjects.AllWorldObjects;
            int num;
            for (int i = 0; i < worldObjects.Count; i = num + 1)
            {
                if (worldObjects[i].Tile == tile)
                {
                    foreach (FloatMenuOption floatMenuOption in worldObjects[i].GetTransportPodsFloatMenuOptions(this.TransportersInGroup.Cast<IThingHolder>(), this))
                    {
                        anything = true;
                        yield return floatMenuOption;
                    }
                    IEnumerator<FloatMenuOption> enumerator = null;
                }
                num = i;
            }
            if (!anything && !Find.World.Impassable(tile))
            {
                yield return new FloatMenuOption("TransportPodsContentsWillBeLost".Translate(), delegate ()
                {
                    this.TryLaunch(tile, null);
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            yield break;
            yield break;
        }

        // Token: 0x17000E00 RID: 3584
        // (get) Token: 0x06004F04 RID: 20228 RVA: 0x001A3580 File Offset: 0x001A1780
        private float FuelInLeastFueledFuelingPortSource
        {
            get
            {
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                float num = 0f;
                bool flag = false;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    float fuelingPortSourceFuel = transportersInGroup[i].Launchable.FuelingPortSourceFuel;
                    if (!flag || fuelingPortSourceFuel < num)
                    {
                        num = fuelingPortSourceFuel;
                        flag = true;
                    }
                }
                if (!flag)
                {
                    return 0f;
                }
                return num;
            }
        }

        // Token: 0x17000E01 RID: 3585
        // (get) Token: 0x06004F05 RID: 20229 RVA: 0x001A35D7 File Offset: 0x001A17D7
        private int MaxLaunchDistance
        {
            get
            {
                if (!this.LoadingInProgressOrReadyToLaunch)
                {
                    return 0;
                }
                return CompLaunchable.MaxLaunchDistanceAtFuelLevel(this.FuelInLeastFueledFuelingPortSource);
            }
        }

        // Token: 0x17000E02 RID: 3586
        // (get) Token: 0x06004F06 RID: 20230 RVA: 0x001A35F0 File Offset: 0x001A17F0
        private int MaxLaunchDistanceEverPossible
        {
            get
            {
                if (!this.LoadingInProgressOrReadyToLaunch)
                {
                    return 0;
                }
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                float num = 0f;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    Building fuelingPortSource = transportersInGroup[i].Launchable.FuelingPortSource;
                    if (fuelingPortSource != null)
                    {
                        num = Mathf.Max(num, fuelingPortSource.GetComp<CompRefuelable>().Props.fuelCapacity);
                    }
                }
                return CompLaunchable.MaxLaunchDistanceAtFuelLevel(num);
            }
        }

        // Token: 0x17000E03 RID: 3587
        // (get) Token: 0x06004F07 RID: 20231 RVA: 0x001A3658 File Offset: 0x001A1858
        private bool PodsHaveAnyPotentialCaravanOwner
        {
            get
            {
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    ThingOwner innerContainer = transportersInGroup[i].innerContainer;
                    for (int j = 0; j < innerContainer.Count; j++)
                    {
                        Pawn pawn = innerContainer[j] as Pawn;
                        if (pawn != null && CaravanUtility.IsOwner(pawn, Faction.OfPlayer))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        // Token: 0x0600500C RID: 20492 RVA: 0x001A805C File Offset: 0x001A625C
        public void Send()
        {
            if (this.sending)
            {
                return;
            }
            if (!this.parent.Spawned)
            {
                Log.Error("Tried to send " + this.parent + ", but it's unspawned.", false);
                return;
            }
            List<CompTransporter> transportersInGroup = this.TransportersInGroup;
            if (transportersInGroup == null)
            {
                Log.Error("Tried to send " + this.parent + ", but it's not in any group.", false);
                return;
            }
            if (!this.LoadingInProgressOrReadyToLaunch)
            {
                return;
            }
            if (!this.AllRequiredThingsLoaded)
            {
                if (this.dropEverythingIfUnsatisfied)
                {
                    this.Transporter.CancelLoad();
                }
                else if (this.dropNonRequiredIfUnsatisfied)
                {
                    for (int i = 0; i < transportersInGroup.Count; i++)
                    {
                        for (int j = transportersInGroup[i].innerContainer.Count - 1; j >= 0; j--)
                        {
                            Thing thing = transportersInGroup[i].innerContainer[j];
                            Pawn pawn;
                            if (!this.IsRequired(thing) && (this.requiredColonistCount <= 0 || (pawn = (thing as Pawn)) == null || !pawn.IsColonist))
                            {
                                Thing thing2;
                                transportersInGroup[i].innerContainer.TryDrop(thing, ThingPlaceMode.Near, out thing2, null, null);
                            }
                        }
                    }
                }
            }
            this.sending = true;
            bool allRequiredThingsLoaded = this.AllRequiredThingsLoaded;
            Map map = this.parent.Map;
            this.Transporter.TryRemoveLord(map);
            string signalPart = allRequiredThingsLoaded ? "SentSatisfied" : "SentUnsatisfied";
            for (int k = 0; k < transportersInGroup.Count; k++)
            {
                QuestUtility.SendQuestTargetSignals(transportersInGroup[k].parent.questTags, signalPart, transportersInGroup[k].parent.Named("SUBJECT"), transportersInGroup[k].innerContainer.ToList<Thing>().Named("SENT"));
            }
            List<Pawn> list = new List<Pawn>();
            for (int l = 0; l < transportersInGroup.Count; l++)
            {
                CompTransporter compTransporter = transportersInGroup[l];
                for (int m = transportersInGroup[l].innerContainer.Count - 1; m >= 0; m--)
                {
                    Pawn pawn2 = transportersInGroup[l].innerContainer[m] as Pawn;
                    if (pawn2 != null)
                    {
                        if (pawn2.IsColonist && !this.requiredPawns.Contains(pawn2))
                        {
                            list.Add(pawn2);
                        }
                        pawn2.ExitMap(false, Rot4.Invalid);
                    }
                }
                compTransporter.innerContainer.ClearAndDestroyContentsOrPassToWorld(DestroyMode.Vanish);
                Thing newThing = ThingMaker.MakeThing(USCMDefOf.RRY_USCM_DropshipUD4LLeaving, null);
                compTransporter.CleanUpLoadingVars(map);
                compTransporter.parent.Destroy(DestroyMode.QuestLogic);
                GenSpawn.Spawn(newThing, compTransporter.parent.Position, map, WipeMode.Vanish);
            }
            if (list.Count != 0)
            {
                for (int n = 0; n < transportersInGroup.Count; n++)
                {
                    QuestUtility.SendQuestTargetSignals(transportersInGroup[n].parent.questTags, "SentWithExtraColonists", transportersInGroup[n].parent.Named("SUBJECT"), list.Named("SENTCOLONISTS"));
                }
            }
            this.sending = false;
        }

        // Token: 0x0600500D RID: 20493 RVA: 0x001A8360 File Offset: 0x001A6560
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look<ThingDefCount>(ref this.requiredItems, "requiredItems", LookMode.Deep, Array.Empty<object>());
            Scribe_Collections.Look<Pawn>(ref this.requiredPawns, "requiredPawns", LookMode.Reference, Array.Empty<object>());
            Scribe_Values.Look<int>(ref this.requiredColonistCount, "requiredColonistCount", 0, false);
            Scribe_Values.Look<bool>(ref this.acceptColonists, "acceptColonists", false, false);
            Scribe_Values.Look<bool>(ref this.onlyAcceptColonists, "onlyAcceptColonists", false, false);
            Scribe_Values.Look<bool>(ref this.leaveImmediatelyWhenSatisfied, "leaveImmediatelyWhenSatisfied", false, false);
            Scribe_Values.Look<bool>(ref this.autoload, "autoload", false, false);
            Scribe_Values.Look<bool>(ref this.dropEverythingIfUnsatisfied, "dropEverythingIfUnsatisfied", false, false);
            Scribe_Values.Look<bool>(ref this.dropNonRequiredIfUnsatisfied, "dropNonRequiredIfUnsatisfied", false, false);
            Scribe_Values.Look<bool>(ref this.leaveASAP, "leaveASAP", false, false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.requiredPawns.RemoveAll((Pawn x) => x == null);
            }
        }

        // Token: 0x0600500E RID: 20494 RVA: 0x001A8464 File Offset: 0x001A6664
        private void CheckAutoload()
        {
            if (!this.autoload || !this.LoadingInProgressOrReadyToLaunch || !this.parent.Spawned)
            {
                return;
            }
            CompUSCMDropship.tmpRequiredItems.Clear();
            CompUSCMDropship.tmpRequiredItems.AddRange(this.requiredItems);
            CompUSCMDropship.tmpRequiredPawns.Clear();
            CompUSCMDropship.tmpRequiredPawns.AddRange(this.requiredPawns);
            ThingOwner innerContainer = this.Transporter.innerContainer;
            for (int i = 0; i < innerContainer.Count; i++)
            {
                Pawn pawn = innerContainer[i] as Pawn;
                if (pawn != null)
                {
                    CompUSCMDropship.tmpRequiredPawns.Remove(pawn);
                }
                else
                {
                    int num = innerContainer[i].stackCount;
                    for (int j = 0; j < CompUSCMDropship.tmpRequiredItems.Count; j++)
                    {
                        if (CompUSCMDropship.tmpRequiredItems[j].ThingDef == innerContainer[i].def)
                        {
                            int num2 = Mathf.Min(CompUSCMDropship.tmpRequiredItems[j].Count, num);
                            if (num2 > 0)
                            {
                                CompUSCMDropship.tmpRequiredItems[j] = CompUSCMDropship.tmpRequiredItems[j].WithCount(CompUSCMDropship.tmpRequiredItems[j].Count - num2);
                                num -= num2;
                            }
                        }
                    }
                }
            }
            for (int k = CompUSCMDropship.tmpRequiredItems.Count - 1; k >= 0; k--)
            {
                if (CompUSCMDropship.tmpRequiredItems[k].Count <= 0)
                {
                    CompUSCMDropship.tmpRequiredItems.RemoveAt(k);
                }
            }
            if (CompUSCMDropship.tmpRequiredItems.Any<ThingDefCount>() || CompUSCMDropship.tmpRequiredPawns.Any<Pawn>())
            {
                if (this.Transporter.leftToLoad != null)
                {
                    this.Transporter.leftToLoad.Clear();
                }
                CompUSCMDropship.tmpAllSendablePawns.Clear();
                CompUSCMDropship.tmpAllSendablePawns.AddRange(TransporterUtility.AllSendablePawns(this.TransportersInGroup, this.parent.Map));
                CompUSCMDropship.tmpAllSendableItems.Clear();
                CompUSCMDropship.tmpAllSendableItems.AddRange(TransporterUtility.AllSendableItems(this.TransportersInGroup, this.parent.Map));
                CompUSCMDropship.tmpAllSendableItems.AddRange(TransporterUtility.ThingsBeingHauledTo(this.TransportersInGroup, this.parent.Map));
                CompUSCMDropship.tmpRequiredPawnsPossibleToSend.Clear();
                for (int l = 0; l < CompUSCMDropship.tmpRequiredPawns.Count; l++)
                {
                    if (CompUSCMDropship.tmpAllSendablePawns.Contains(CompUSCMDropship.tmpRequiredPawns[l]))
                    {
                        TransferableOneWay transferableOneWay = new TransferableOneWay();
                        transferableOneWay.things.Add(CompUSCMDropship.tmpRequiredPawns[l]);
                        this.Transporter.AddToTheToLoadList(transferableOneWay, 1);
                        CompUSCMDropship.tmpRequiredPawnsPossibleToSend.Add(CompUSCMDropship.tmpRequiredPawns[l]);
                    }
                }
                for (int m = 0; m < CompUSCMDropship.tmpRequiredItems.Count; m++)
                {
                    if (CompUSCMDropship.tmpRequiredItems[m].Count > 0)
                    {
                        int num3 = 0;
                        for (int n = 0; n < CompUSCMDropship.tmpAllSendableItems.Count; n++)
                        {
                            if (CompUSCMDropship.tmpAllSendableItems[n].def == CompUSCMDropship.tmpRequiredItems[m].ThingDef)
                            {
                                num3 += CompUSCMDropship.tmpAllSendableItems[n].stackCount;
                            }
                        }
                        if (num3 > 0)
                        {
                            TransferableOneWay transferableOneWay2 = new TransferableOneWay();
                            for (int num4 = 0; num4 < CompUSCMDropship.tmpAllSendableItems.Count; num4++)
                            {
                                if (CompUSCMDropship.tmpAllSendableItems[num4].def == CompUSCMDropship.tmpRequiredItems[m].ThingDef)
                                {
                                    transferableOneWay2.things.Add(CompUSCMDropship.tmpAllSendableItems[num4]);
                                }
                            }
                            int count = Mathf.Min(CompUSCMDropship.tmpRequiredItems[m].Count, num3);
                            this.Transporter.AddToTheToLoadList(transferableOneWay2, count);
                        }
                    }
                }
                TransporterUtility.MakeLordsAsAppropriate(CompUSCMDropship.tmpRequiredPawnsPossibleToSend, this.TransportersInGroup, this.parent.Map);
                CompUSCMDropship.tmpAllSendablePawns.Clear();
                CompUSCMDropship.tmpAllSendableItems.Clear();
                CompUSCMDropship.tmpRequiredItems.Clear();
                CompUSCMDropship.tmpRequiredPawns.Clear();
                CompUSCMDropship.tmpRequiredPawnsPossibleToSend.Clear();
                return;
            }
            if (this.Transporter.leftToLoad != null)
            {
                this.Transporter.leftToLoad.Clear();
            }
            TransporterUtility.MakeLordsAsAppropriate(CompUSCMDropship.tmpRequiredPawnsPossibleToSend, this.TransportersInGroup, this.parent.Map);
        }

        // Token: 0x0600500F RID: 20495 RVA: 0x001A88D0 File Offset: 0x001A6AD0
        public bool IsRequired(Thing thing)
        {
            Pawn pawn = thing as Pawn;
            if (pawn != null)
            {
                return this.requiredPawns.Contains(pawn);
            }
            for (int i = 0; i < this.requiredItems.Count; i++)
            {
                if (this.requiredItems[i].ThingDef == thing.def && this.requiredItems[i].Count != 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06005010 RID: 20496 RVA: 0x001A8940 File Offset: 0x001A6B40
        public bool IsAllowed(Thing t)
        {
            if (this.IsRequired(t))
            {
                return true;
            }
            if (this.acceptColonists)
            {
                Pawn pawn = t as Pawn;
                if (pawn != null && (pawn.IsColonist || (!this.onlyAcceptColonists && pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer)) && (!this.onlyAcceptColonists || !pawn.IsQuestLodger()))
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06005011 RID: 20497 RVA: 0x001A89A7 File Offset: 0x001A6BA7
        public void CleanUpLoadingVars()
        {
            this.autoload = false;
        }

        // Token: 0x04002C3F RID: 11327
        public List<ThingDefCount> requiredItems = new List<ThingDefCount>();

        // Token: 0x04002C40 RID: 11328
        public List<Pawn> requiredPawns = new List<Pawn>();

        // Token: 0x04002C41 RID: 11329
        public int requiredColonistCount;

        // Token: 0x04002C42 RID: 11330
        public bool acceptColonists;

        // Token: 0x04002C43 RID: 11331
        public bool onlyAcceptColonists;

        // Token: 0x04002C44 RID: 11332
        public bool dropEverythingIfUnsatisfied;

        // Token: 0x04002C45 RID: 11333
        public bool dropNonRequiredIfUnsatisfied = true;

        // Token: 0x04002C46 RID: 11334
        public bool leaveImmediatelyWhenSatisfied;

        // Token: 0x04002C47 RID: 11335
        private bool autoload;

        // Token: 0x04002C48 RID: 11336
        public bool leaveASAP;

        // Token: 0x04002C49 RID: 11337
        private CompTransporter cachedCompTransporter;

        // Token: 0x04002C4A RID: 11338
        private List<CompTransporter> cachedTransporterList;

        // Token: 0x04002C4B RID: 11339
        private bool sending;

        // Token: 0x04002C4C RID: 11340
        private const int CheckAutoloadIntervalTicks = 120;

        // Token: 0x04002C4D RID: 11341
        private static readonly Texture2D AutoloadToggleTex = ContentFinder<Texture2D>.Get("UI/Commands/Autoload", true);

        // Token: 0x04002C4E RID: 11342
        private static readonly Texture2D SendCommandTex = CompLaunchable.LaunchCommandTex;

        // Token: 0x04002C4F RID: 11343
        private static List<ThingDefCount> tmpRequiredItemsWithoutDuplicates = new List<ThingDefCount>();

        // Token: 0x04002C50 RID: 11344
        private static List<string> tmpRequiredLabels = new List<string>();

        // Token: 0x04002C51 RID: 11345
        private static List<ThingDefCount> tmpRequiredItems = new List<ThingDefCount>();

        // Token: 0x04002C52 RID: 11346
        private static List<Pawn> tmpRequiredPawns = new List<Pawn>();

        // Token: 0x04002C53 RID: 11347
        private static List<Pawn> tmpAllSendablePawns = new List<Pawn>();

        // Token: 0x04002C54 RID: 11348
        private static List<Thing> tmpAllSendableItems = new List<Thing>();

        // Token: 0x04002C55 RID: 11349
        private static List<Pawn> tmpRequiredPawnsPossibleToSend = new List<Pawn>();

        // Token: 0x04002BB2 RID: 11186
        private const float FuelPerTile = 2.25f;
    }
}
