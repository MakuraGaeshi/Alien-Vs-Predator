using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class Filth_AddAcidDamageDef : ThingDef
    {

    }

    public class Filth_AddAcidDamage : Filth
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000017 RID: 23 RVA: 0x000027A2 File Offset: 0x000009A2
        // (set) Token: 0x06000018 RID: 24 RVA: 0x000027AA File Offset: 0x000009AA
        public object cachedLabelMouseover { get; private set; }
        public bool active = true;
        // Token: 0x06000019 RID: 25 RVA: 0x000027B3 File Offset: 0x000009B3
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.RecalcPathsOnAndAroundMe(map);
            this.destroyTick = Find.TickManager.TicksGame + (Rand.Range(29, 121) * 100);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Map map = base.Map;
            base.DeSpawn(mode);
            this.RecalcPathsOnAndAroundMe(map);
        }

        private void RecalcPathsOnAndAroundMe(Map map)
        {
            IntVec3[] adjacentCellsAndInside = GenAdj.AdjacentCellsAndInside;
            for (int i = 0; i < adjacentCellsAndInside.Length; i++)
            {
                IntVec3 c = base.Position + adjacentCellsAndInside[i];
                if (c.InBounds(map))
                {
                    map.pathGrid.RecalculatePerceivedPathCostAt(c);
                }
            }
        }
        // Token: 0x0600001A RID: 26 RVA: 0x000027C0 File Offset: 0x000009C0
        public override void Tick()
        {
        //    Log.Message(string.Format("1"));
            bool destroyed = base.Destroyed;
        //    Log.Message(string.Format("2 destroyed: {0}", destroyed));
            if (!destroyed && this.active)
            {
            //    Log.Message(string.Format("3 !destroyed"));
            //    base.Tick();
            //    Log.Message(string.Format("4 !destroyed"));
                bool flag = this.destroyTick <= Find.TickManager.TicksGame && !base.Destroyed;
            //    Log.Message(string.Format("5 !destroyed flag: {0}", flag));
                if (flag)
                {
                //    Log.Message(string.Format("6 !destroyed flag"));
                    this.active = false;
                }
                else
                {
                //    Log.Message(string.Format("7 !destroyed "));
                    this.Ticks--;
                //    Log.Message(string.Format("8 !destroyed "));
                    bool flag2 = this.Ticks <= 0;
                //    Log.Message(string.Format("9 !destroyed flag2: {0}", flag2));
                    if (flag2)
                    {
                    //    Log.Message(string.Format("10 !destroyed flag2"));
                        this.TickTack();
                    //    Log.Message(string.Format("11 !destroyed flag2"));
                        this.Ticks = this.TickRate;
                    //    Log.Message(string.Format("12 !destroyed flag2"));
                    }
                //    Log.Message(string.Format("13 !destroyed"));
                }
            //    Log.Message(string.Format("14 !destroyed"));
            }
        //    Log.Message(string.Format("14"));
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002854 File Offset: 0x00000A54
        public void TickTack()
        {
        //    Log.Message(string.Format("1"));
            bool destroyed = base.Destroyed;
        //    Log.Message(string.Format("2"));
            if (!destroyed)
            {
            //    Log.Message(string.Format("3 !destroyed"));
                List<Thing> thingList = GridsUtility.GetThingList(base.Position, base.Map);
            //    Log.Message(string.Format("4 !destroyed"));
                for (int i = 0; i < thingList.Count; i++)
                {
                //    Log.Message(string.Format("5 !destroyed for {0}", i));
                    bool flag = thingList[i] != null;
                //    Log.Message(string.Format("6 !destroyed for {0}", i));
                    if (flag)
                    {
                    //    Log.Message(string.Format("7 !destroyed for {0} flag", i));
                        Thing thing = thingList[i];
                    //    Log.Message(string.Format("8 !destroyed for {0} flag", i));
                        Pawn pawn = thingList[i] as Pawn;
                    //    Log.Message(string.Format("9 !destroyed for {0} flag", i));
                        bool flag2 = thing != null && !this.touchingThings.Contains(thing) && thing.def != XenomorphDefOf.RRY_FilthBloodXenomorph && thing.GetType() != typeof(Mote) && thing.GetType() != typeof(MoteThrown) && thing.GetType() != typeof(Bullet);
                    //    Log.Message(string.Format("9b !destroyed for {0} flag", i));
                        bool flag2a = !(thing is Corpse corpse && XenomorphUtil.IsXenoCorpse(corpse));
                    //    Log.Message(string.Format("9c !destroyed for {0} flag", i));
                        bool flag2b = !(thing is Pawn && XenomorphUtil.IsXenomorph((Pawn)thing));
                        // !XenomorphUtil.IsXenomorph(pawn);
                    //    Log.Message(string.Format("9d !destroyed for {0} flag", i));
                    //    Log.Message(string.Format("10 !destroyed for {0} flag", i));
                        if (flag2 && flag2a && flag2b )
                        {
                        //    Log.Message(string.Format("11 !destroyed for {0} flag flag2", i));
                            this.touchingThings.Add(thing);
                        //    Log.Message(string.Format("12 !destroyed for {0} flag flag2", i));
                            this.damageEntities(thing, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                        //    Log.Message(string.Format("13 !destroyed for {0} flag flag2", i));
                            MoteMaker.ThrowDustPuff(thing.Position, base.Map, 0.2f);
                        //    Log.Message(string.Format("14 !destroyed for {0} flag flag2", i));
                        }
                    //    Log.Message(string.Format("15 !destroyed for {0} flag", i));
                        bool flag3 = pawn != null;
                    //    Log.Message(string.Format("16 !destroyed for {0} flag", i));
                        if (flag3)
                        {
                        //    Log.Message(string.Format("17 !destroyed for {0} flag flag3", i));
                            this.touchingPawns.Add(pawn);
                        //    Log.Message(string.Format("18 !destroyed for {0} flag flag3", i));
                            bool flag4 = !XenomorphUtil.IsXenomorph(pawn);
                        //    Log.Message(string.Format("19 !destroyed for {0} flag flag3", i));
                            if (flag4)
                            {
                            //    Log.Message(string.Format("20 !destroyed for {0} flag flag3 flag4", i));
                                this.addAcidDamage(pawn);
                            //    Log.Message(string.Format("21 !destroyed for {0} flag flag3 flag4", i));
                                MoteMaker.ThrowDustPuff(pawn.Position, base.Map, 0.2f);
                            //    Log.Message(string.Format("22 !destroyed for {0} flag flag3 flag4", i));
                            }
                        //    Log.Message(string.Format("23 !destroyed for {0} flag flag3", i));
                        }
                    //    Log.Message(string.Format("24 !destroyed for {0} flag", i));
                    }
                //    Log.Message(string.Format("25 !destroyed for {0}", i));
                }
            //    Log.Message(string.Format("26 !destroyed"));
                for (int j = 0; j < this.touchingPawns.Count; j++)
                {
                //    Log.Message(string.Format("27 !destroyed for {0}", j));
                    Pawn pawn2 = this.touchingPawns[j];
                //    Log.Message(string.Format("28 !destroyed for {0}", j));
                    bool flag5 = !pawn2.Spawned || pawn2.Position != base.Position || XenomorphUtil.IsXenomorph(pawn2);
                //    Log.Message(string.Format("29 !destroyed for {0}", j));
                    if (flag5)
                    {
                    //    Log.Message(string.Format("30 !destroyed for {0} flag5", j));
                        this.touchingPawns.Remove(pawn2);
                    //    Log.Message(string.Format("31 !destroyed for {0} flag5", j));
                    }
                    else
                    {
                    //    Log.Message(string.Format("32 !destroyed for {0}", j));
                        bool flag6 = !pawn2.RaceProps.Animal;
                    //    Log.Message(string.Format("33 !destroyed for {0}", j));
                        if (flag6)
                        {
                        //    Log.Message(string.Format("34 !destroyed for {0} flag6", j));
                            this.addAcidDamage(pawn2);
                        //    Log.Message(string.Format("35 !destroyed for {0} flag6", j));
                        }
                    //    Log.Message(string.Format("36 !destroyed for {0}", j));
                    }
                //    Log.Message(string.Format("37 !destroyed for {0}", j));
                }
            //    Log.Message(string.Format("38 !destroyed"));
                for (int k = 0; k < this.touchingThings.Count; k++)
                {
                //    Log.Message(string.Format("39 !destroyed for {0}", k));
                    Thing thing2 = this.touchingThings[k];
                //    Log.Message(string.Format("40 !destroyed for {0}", k));
                    bool flag7 = !thing2.Spawned || thing2.Position != base.Position;
                //    Log.Message(string.Format("41 !destroyed for {0}", k));
                    if (flag7)
                    {
                    //    Log.Message(string.Format("42 !destroyed for {0} flag7", k));
                        this.touchingThings.Remove(thing2);
                    //    Log.Message(string.Format("43 !destroyed for {0} flag7", k));
                    }
                    else
                    {
                    //    Log.Message(string.Format("44 !destroyed for {0}", k));
                        this.damageEntities(thing2, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                    //    Log.Message(string.Format("45 !destroyed for {0}", k));
                    }
                //    Log.Message(string.Format("46 !destroyed for {0}", k));
                }
            //    Log.Message(string.Format("47 !destroyed"));
                this.damageBuildings(Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
            //    Log.Message(string.Format("48 !destroyed"));
                this.cachedLabelMouseover = null;
            //    Log.Message(string.Format("49 !destroyed"));
            }
        //    Log.Message(string.Format("50"));
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00002AD4 File Offset: 0x00000CD4
        public void damageEntities(Thing e, int amt)
        {
            if (e is Pawn)
            {
                return;
            }
           // Log.Message(string.Format("damageEntities: {0}", e.LabelShort));
            DamageInfo damageInfo;
            damageInfo = new DamageInfo(XenomorphDefOf.RRY_AcidBurn, (float)amt, 0f, -1f, null, null, null, 0, null);
            bool flag = e != null;
            if (e.Stuff!=null)
            {
            //    Log.Message(string.Format("{0} Stuff: {1}",e.LabelShortCap, e.Stuff.defName));
            }
            else
            {
            //    Log.Message(string.Format("{0} Stuff: Null, Typeof: {1}", e.LabelShortCap, e.GetType()));
            }
            if (flag)
            {
                e.TakeDamage(damageInfo);
                MoteMaker.ThrowDustPuff(e.Position, base.Map, 0.2f);
            }
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002B28 File Offset: 0x00000D28
        public void damageBuildings(int amt)
        {
            IntVec3 intVec = GenAdj.RandomAdjacentCell8Way(this);
            bool flag = GenGrid.InBounds(intVec, base.Map);
            bool flag2 = flag;
            if (flag2)
            {
                Building firstBuilding = GridsUtility.GetFirstBuilding(intVec, base.Map);
                DamageInfo damageInfo;
                damageInfo = new DamageInfo(XenomorphDefOf.RRY_AcidBurn, (float)amt, 0f, -1f, null, null, null, 0, null);
                bool flag3 = firstBuilding != null;
                bool flag4 = flag3;
                if (flag4)
                {
                    firstBuilding.TakeDamage(damageInfo);
                    MoteMaker.ThrowDustPuff(firstBuilding.Position, base.Map, 0.2f);
                }
            }
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00002BAC File Offset: 0x00000DAC
        public void addAcidDamage(Pawn p)
        {
        //    Log.Message(string.Format("addAcidDamage: {0}", p.LabelShort));
            List<BodyPartRecord> list = new List<BodyPartRecord>();
        //    Log.Message(string.Format("0 "));
            List<Apparel> wornApparel = new List<Apparel>();
            if (p.RaceProps.Humanlike) wornApparel = p.apparel.WornApparel;
        //    Log.Message(string.Format("1 "));
            int num = Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f));
        //    Log.Message(string.Format("2 "));
            DamageInfo damageInfo = default(DamageInfo);
        //    Log.Message(string.Format("3 "));
            MoteMaker.ThrowDustPuff(p.Position, base.Map, 0.2f);
        //    Log.Message(string.Format("4 "));
            BodyPartHeight bodyPartHeight = p.Downed ? BodyPartHeight.Undefined : BodyPartHeight.Bottom;
        //    Log.Message(string.Format("{0}", bodyPartHeight));
            foreach (BodyPartRecord bodyPartRecord in p.health.hediffSet.GetNotMissingParts(bodyPartHeight, BodyPartDepth.Outside, null, null))
            {
            //    Log.Message(string.Format("{0}", bodyPartRecord.Label));
                bool flag = wornApparel.Count > 0;
                if (flag)
                {
                    bool flag2 = false;
                    for (int i = 0; i < wornApparel.Count; i++)
                    {
                        bool flag3 = wornApparel[i].def.apparel.CoversBodyPart(bodyPartRecord);
                        if (flag3)
                        {
                            flag2 = true;
                        //    Log.Message(string.Format("is protected"));
                            break;
                        }
                    }
                    bool flag4 = !flag2;
                    if (flag4)
                    {
                    //    Log.Message(string.Format("{0}", bodyPartRecord));
                        list.Add(bodyPartRecord);
                    }
                }
                else
                {
                //    Log.Message(string.Format("{0}", bodyPartRecord));
                    list.Add(bodyPartRecord);
                }
            }
            for (int k = 0; k < list.Count; k++)
            {
                damageInfo = new DamageInfo(XenomorphDefOf.RRY_AcidBurn, (float)Mathf.RoundToInt(((float)num * list[k].coverage)*10), 0f, -1f, this, list[k], null, 0, null);
                //    Log.Message(string.Format("addAcidDamage TakeDamage: {0}, list[k].coverage: {1}, damageInfo: {2}", list[k].customLabel, list[k].coverage, damageInfo));
                if (Rand.Chance(list[k].coverage))
                {

                    for (int j = 0; j < wornApparel.Count; j++)
                    {
                        bool flag3 = wornApparel[j].def.apparel.CoversBodyPart(list[k]);
                        if (flag3)
                        {
                            this.damageEntities(wornApparel[j], num);
                        }
                    }
                    p.TakeDamage(damageInfo);
                    break;
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
        }

        public int destroyTick;

        private List<Pawn> touchingPawns = new List<Pawn>();

        private List<Thing> touchingThings = new List<Thing>();

        private int Ticks = 100;

        private int TickRate = 100;

        private int AcidDamage = 3;
    }
}
