using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x020006C1 RID: 1729
	public class Building_XenomorphCocoon : Building_Bed
	{
		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x060024CC RID: 9420 RVA: 0x0011820F File Offset: 0x0011660F
		public override Color DrawColor
		{
			get
            {
                return base.DrawColor;
            }
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x060024CD RID: 9421 RVA: 0x00118230 File Offset: 0x00116630
		public override Color DrawColorTwo
		{
			get
            {
                return base.DrawColorTwo;
            }
		}
        
		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x060024D2 RID: 9426 RVA: 0x001182F4 File Offset: 0x001166F4
		private bool PlayerCanSeeOwners
		{
			get
			{
				return false;
			}
		}

        public bool Occupied
        {
            get
            {
                return !this.AnyUnoccupiedSleepingSlot;//this.CurOccupants.Count() > 0;
            }
        }

        public override Graphic Graphic
        {
            get
            {
            //    Log.Message(string.Format("Occupied: {0}", Occupied));
                if (Occupied && GetCurOccupant(0).RaceProps.Humanlike)
                {
                    return base.Graphic;
                }
                else
                {
                    return new Graphic_Invisible();
                }
            }
        }
        public Pawn LastOccupant;
        public int ticksSinceHeal;
        public int healIntervalTicks = 60;
        public int ticksSinceOccupied;
        public int occupiedIntervalTicks = 100;

        // Token: 0x060024D6 RID: 9430 RVA: 0x001183A8 File Offset: 0x001167A8
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			Region validRegionAt_NoRebuild = map.regionGrid.GetValidRegionAt_NoRebuild(base.Position);
			this.Medical = true;
            //this.def.graphicData.texPath = "DummyTexture";
        }

        public override void Tick()
        {
            base.Tick();
            if (Occupied)
            {
                if (CurOccupants.Count()>0)
                {
                //    Log.Message(string.Format("{0} ocuppied", this));
                    foreach (Pawn p in CurOccupants)
                    {
                    //    Log.Message(string.Format("p: {0}\nDead: {1} ", p, p.Dead));
                        if (p.RaceProps.Humanlike)
                        {
                            this.def.building.bed_showSleeperBody = false;
                        }
                        if (!p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned))
                        {
                            p.health.AddHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
                        }
                        if (p.Dead)
                        {
                            this.Destroy();
                        }
                    }
                }
            }
            else
            {
                Log.Message(string.Format("{0} unocuppied", this));
                List<IntVec3> celllist = this.CellsAdjacent8WayAndInside().ToList();
                if (!celllist.NullOrEmpty())
                {
                    foreach (var cell in celllist)
                    {
                        if (cell.GetFirstPawn(this.Map) != null && cell.GetFirstPawn(this.Map) is Pawn p)
                        {
                            Log.Message(string.Format("{0}", cell.GetFirstPawn(this.Map)));
                            if (p.Downed && !p.Dead && !p.InBed() && !(p.kindDef.race.defName.Contains("RRY_Xenomorph_")))
                            {
                                Log.Message(string.Format("{0} tucking", p));
                                p.jobs.Notify_TuckedIntoBed(this);
                                p.mindState.Notify_TuckedIntoBed();
                            }
                        }
                    }
                }
                else if (!Occupied)
                {
                    this.def.building.bed_showSleeperBody = true;
                }
                Log.Message(string.Format("Destroying : {0} ", this));
                this.Destroy();
            }
        }

        /*
        public override void Tick()
        {
            base.Tick();
            if (this.CurOccupants!=null)
            {
                foreach (Pawn p in this.CurOccupants)
                {
                    if (p != LastOccupant && !p.def.defName.Contains("Xenomorph"))
                    {
                        LastOccupant = p;
                        Occupied = true;
                        NutritionNeed = p.needs.food.ShowOnNeedList ? p.needs.food.CurLevel: 0f;
                    }
                    bool flag = p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) && !p.def.defName.Contains("Xenomorph");
                    if (p.health.hediffSet.hediffs!=null)
                    {
                        //    Log.Message(string.Format("{0}", p.health.hediffSet.hediffs.ToString()));
                        if (!flag)
                        {
                            p.health.AddHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
                        }
                        else
                        {
                            foreach (var item in p.health.hediffSet.hediffs)
                            {

                            }
                        }

                    }
                    else
                    {
                        if (!flag)
                        {
                            p.health.AddHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
                        }

                    }
                }
            }
            else
            {
                if (ticksSinceOccupied > occupiedIntervalTicks)
                {
                    ticksSinceOccupied = 0;
                    Occupied = false;
                    this.RemoveAllOwners();
                }
                ticksSinceOccupied++;
                if (this.Position.GetFirstPawn(this.Map)!=null && this.Position.GetFirstPawn(this.Map) is Pawn pawn && !pawn.def.defName.Contains("Xenomorph") && pawn.Downed)
                {
                //    Log.Message(string.Format("{0} found", pawn.LabelShortCap));
                    pawn.jobs.Notify_TuckedIntoBed(this);

                }
            }
        }
        */
        // Token: 0x060024E0 RID: 9440 RVA: 0x00118B58 File Offset: 0x00116F58
        public new Pawn GetCurOccupant(int slotIndex)
        {
            if (!base.Spawned)
            {
                return null;
            }
            IntVec3 sleepingSlotPos = this.GetSleepingSlotPos(slotIndex);
            List<Thing> list = base.Map.thingGrid.ThingsListAt(sleepingSlotPos);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is Pawn pawn)
                {
                    if (pawn.CurJob != null)
                    {
                        if (pawn.GetPosture() == PawnPosture.LayingInBed)
                        {
                            return pawn;
                        }
                    }
                }
            }
            return null;
        }

        // Token: 0x060024E1 RID: 9441 RVA: 0x00118BD8 File Offset: 0x00116FD8
        public new int GetCurOccupantSlotIndex(Pawn curOccupant)
        {
            for (int i = 0; i < this.SleepingSlotsCount; i++)
            {
                if (this.GetCurOccupant(i) == curOccupant)
                {
                    return i;
                }
            }
        //    Log.Error("Could not find pawn " + curOccupant + " on any of sleeping slots.", false);
            return 0;
        }

        public new Pawn GetCurOccupantAt(IntVec3 pos)
        {
            for (int i = 0; i < this.SleepingSlotsCount; i++)
            {
                if (this.GetSleepingSlotPos(i) == pos)
                {
                    return this.GetCurOccupant(i);
                }
            }
            return null;
        }

        public new IntVec3 GetSleepingSlotPos(int index)
        {
        //    Log.Message(string.Format("{0}", XenomorphCocoonUtility.GetSleepingSlotPos(index, base.Position, base.Rotation, this.def.size)));
            return XenomorphCocoonUtility.GetSleepingSlotPos(index, base.Position, base.Rotation, this.def.size);
        }

        // Token: 0x060024D8 RID: 9432 RVA: 0x0011845F File Offset: 0x0011685F
        public override void ExposeData()
		{
			base.ExposeData();
            /*
			Scribe_Values.Look<bool>(ref this.forPrisonersInt, "forPrisoners", false, false);
			Scribe_Values.Look<bool>(ref this.medicalInt, "medical", false, false);
			Scribe_Values.Look<bool>(ref this.alreadySetDefaultMed, "alreadySetDefaultMed", false, false);
            */
		}


        // Token: 0x060024DE RID: 9438 RVA: 0x001189DC File Offset: 0x00116DDC
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(myPawn))
            {
                yield return o;
            }
            if (base.AllComps != null)
            {
                for (int i = 0; i < this.AllComps.Count; i++)
                {
                    foreach (FloatMenuOption o2 in this.AllComps[i].CompFloatMenuOptions(myPawn))
                    {
                        yield return o2;
                    }
                }
            }
            yield break;
        }

        // Token: 0x060024D9 RID: 9433 RVA: 0x001184A0 File Offset: 0x001168A0
        public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
		}
        
		// Token: 0x060024DB RID: 9435 RVA: 0x001184F8 File Offset: 0x001168F8
		public override IEnumerable<Gizmo> GetGizmos()
        {
            /*
			foreach (Gizmo g in base.GetGizmos())
			{
				yield return g;
			}
			if (this.def.building.bed_humanlike && base.Faction == Faction.OfPlayer)
			{
				Command_Toggle pris = new Command_Toggle();
				pris.defaultLabel = "CommandBedSetForPrisonersLabel".Translate();
				pris.defaultDesc = "CommandBedSetForPrisonersDesc".Translate();
				pris.icon = ContentFinder<Texture2D>.Get("UI/Commands/ForPrisoners", true);
				pris.isActive = new Func<bool>(this.get_ForPrisoners);
				pris.toggleAction = delegate()
				{
					this.ToggleForPrisonersByInterface();
				};
				if (!Building_Bed.RoomCanBePrisonCell(this.GetRoom(RegionType.Set_Passable)) && !this.ForPrisoners)
				{
					pris.Disable("CommandBedSetForPrisonersFailOutdoors".Translate());
				}
				pris.hotKey = KeyBindingDefOf.Misc3;
				pris.turnOffSound = null;
				pris.turnOnSound = null;
				yield return pris;
				yield return new Command_Toggle
				{
					defaultLabel = "CommandBedSetAsMedicalLabel".Translate(),
					defaultDesc = "CommandBedSetAsMedicalDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Commands/AsMedical", true),
					isActive = new Func<bool>(this.get_Medical),
					toggleAction = delegate()
					{
						this.Medical = !this.Medical;
					},
					hotKey = KeyBindingDefOf.Misc2
				};
				if (!this.ForPrisoners && !this.Medical)
				{
					yield return new Command_Action
					{
						defaultLabel = "CommandBedSetOwnerLabel".Translate(),
						icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
						defaultDesc = "CommandBedSetOwnerDesc".Translate(),
						action = delegate()
						{
							Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
						},
						hotKey = KeyBindingDefOf.Misc3
					};
				}
			}
            */
            yield break;
		}
        /*
		// Token: 0x060024DC RID: 9436 RVA: 0x0011851C File Offset: 0x0011691C
		private void ToggleForPrisonersByInterface()
		{
			if (Building_Bed.lastPrisonerSetChangeFrame == Time.frameCount)
			{
				return;
			}
			Building_Bed.lastPrisonerSetChangeFrame = Time.frameCount;
			bool newForPrisoners = !this.ForPrisoners;
			SoundDef soundDef = (!newForPrisoners) ? SoundDefOf.Checkbox_TurnedOff : SoundDefOf.Checkbox_TurnedOn;
			soundDef.PlayOneShotOnCamera(null);
			List<Building_Bed> bedsToAffect = new List<Building_Bed>();
			foreach (Building_Bed building_Bed in Find.Selector.SelectedObjects.OfType<Building_Bed>())
			{
				if (building_Bed.ForPrisoners != newForPrisoners)
				{
					Room room = building_Bed.GetRoom(RegionType.Set_Passable);
					if (room == null || !Building_Bed.RoomCanBePrisonCell(room))
					{
						if (!bedsToAffect.Contains(building_Bed))
						{
							bedsToAffect.Add(building_Bed);
						}
					}
					else
					{
						foreach (Building_Bed item in room.ContainedBeds)
						{
							if (!bedsToAffect.Contains(item))
							{
								bedsToAffect.Add(item);
							}
						}
					}
				}
			}
			Action action = delegate()
			{
				List<Room> list = new List<Room>();
				foreach (Building_Bed building_Bed3 in bedsToAffect)
				{
					Room room2 = building_Bed3.GetRoom(RegionType.Set_Passable);
					building_Bed3.ForPrisoners = (newForPrisoners && !room2.TouchesMapEdge);
					for (int j = 0; j < this.SleepingSlotsCount; j++)
					{
						Pawn curOccupant = this.GetCurOccupant(j);
						if (curOccupant != null)
						{
							curOccupant.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
						}
					}
					if (!list.Contains(room2) && !room2.TouchesMapEdge)
					{
						list.Add(room2);
					}
				}
				foreach (Room room3 in list)
				{
					room3.Notify_RoomShapeOrContainedBedsChanged();
				}
			};
			if ((from b in bedsToAffect
			where b.owners.Any<Pawn>() && b != this
			select b).Count<Building_Bed>() == 0)
			{
				action();
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (newForPrisoners)
				{
					stringBuilder.Append("TurningOnPrisonerBedWarning".Translate());
				}
				else
				{
					stringBuilder.Append("TurningOffPrisonerBedWarning".Translate());
				}
				stringBuilder.AppendLine();
				foreach (Building_Bed building_Bed2 in bedsToAffect)
				{
					if ((newForPrisoners && !building_Bed2.ForPrisoners) || (!newForPrisoners && building_Bed2.ForPrisoners))
					{
						for (int i = 0; i < building_Bed2.owners.Count; i++)
						{
							stringBuilder.AppendLine();
							stringBuilder.Append(building_Bed2.owners[i].LabelShort);
						}
					}
				}
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.Append("AreYouSure".Translate());
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(stringBuilder.ToString(), action, false, null));
			}
		}
        */

        private IntVec3 nextValidPlacementSpot;
        public IntVec3 NextValidPlacementSpot
        {
            get
            {
                bool flag = this.nextValidPlacementSpot == default(IntVec3) || this.nextValidPlacementSpot == IntVec3.Invalid;
                if (flag)
                {
                    HashSet<Building_XenomorphCocoon> hashSet = new HashSet<Building_XenomorphCocoon>();
                    IEnumerable<IntVec3> enumerable = GenAdj.CellsAdjacent8Way(new TargetInfo(base.PositionHeld, base.MapHeld, false));
                    foreach (IntVec3 intVec in enumerable)
                    {
                        bool flag2 = GenGrid.Walkable(intVec, base.MapHeld);
                        if (flag2)
                        {
                            Building_XenomorphCocoon item;
                            bool flag3 = (item = (GridsUtility.GetThingList(intVec, base.Map).FirstOrDefault((Thing x) => x is Building_XenomorphCocoon) as Building_XenomorphCocoon)) != null;
                            if (flag3)
                            {
                                hashSet.Add(item);
                            }
                            else
                            {
                                bool flag4 = GridsUtility.GetThingList(intVec, base.Map).FirstOrDefault((Thing x) => x is Building_XenomorphCocoon) == null;
                                if (flag4)
                                {
                                    bool accepted = GenConstruct.CanPlaceBlueprintAt(this.def, intVec, Rot4.North, base.Map, false, null).Accepted;
                                    if (accepted)
                                    {
                                        this.nextValidPlacementSpot = intVec;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    foreach (Building_XenomorphCocoon building_Cocoon in hashSet)
                    {
                        bool flag5 = !building_Cocoon.resolvingCurrently;
                        if (flag5)
                        {
                            building_Cocoon.ResolvedNeighborPos();
                        }
                    }
                }
                return this.nextValidPlacementSpot;
            }
        }
        private bool resolvingCurrently = false;
        private Pawn spinner;
        public Pawn Spinner
        {
            get
            {
                return this.spinner;
            }
            set
            {
                this.spinner = value;
            }
        }
        // Token: 0x06000009 RID: 9 RVA: 0x00002510 File Offset: 0x00000710
        public IntVec3 ResolvedNeighborPos()
        {
            this.resolvingCurrently = true;
            IntVec3 intVec = this.NextValidPlacementSpot;
            bool flag = !GenGrid.Walkable(intVec, base.MapHeld);
            if (flag)
            {
                this.nextValidPlacementSpot = default(IntVec3);
                intVec = this.NextValidPlacementSpot;
                for (int i = 0; i < 9; i++)
                {
                    bool flag2 = GridsUtility.GetThingList(intVec, base.Map).FirstOrDefault((Thing x) => x is Building_XenomorphCocoon) != null;
                    if (!flag2)
                    {
                        break;
                    }
                    this.nextValidPlacementSpot = default(IntVec3);
                    intVec = this.NextValidPlacementSpot;
                    bool flag3 = !GenConstruct.CanPlaceBlueprintAt(this.def, intVec, Rot4.North, base.Map, false, null).Accepted;
                    if (!flag3)
                    {
                        break;
                    }
                    this.nextValidPlacementSpot = default(IntVec3);
                    intVec = this.NextValidPlacementSpot;
                }
            }
            this.resolvingCurrently = false;
            return intVec;
        }

        // Token: 0x060024DD RID: 9437 RVA: 0x00118808 File Offset: 0x00116C08
        public override string GetInspectString()
        {
            return "";
        }

        // Token: 0x060024DF RID: 9439 RVA: 0x00118A08 File Offset: 0x00116E08
        public override void DrawGUIOverlay()
        {
            return;
        }
        
		// Token: 0x060024E5 RID: 9445 RVA: 0x00118CAC File Offset: 0x001170AC
		private void RemoveAllOwners()
		{
			for (int i = this.owners.Count - 1; i >= 0; i--)
			{
				this.owners[i].ownership.UnclaimBed();
			}
		}

		// Token: 0x040014CC RID: 5324
		private bool alreadySetDefaultMed;
	}
}
