using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RRYautja
{
    // Token: 0x0200000E RID: 14
    public class MapComponent_XenomorphCocoonTracker : MapComponent
    {
        // Token: 0x17000010 RID: 16
        // (get) Token: 0x06000086 RID: 134 RVA: 0x0000651D File Offset: 0x0000471D
        public HashSet<Thing> AllCocoons
        {
            get
            {
                HashSet<Thing> hashSet = this.WildCocoons;
                IEnumerable<Thing> enumerable;
                if (hashSet == null)
                {
                    enumerable = null;
                }
                else
                {
                    IEnumerable<Thing> enumerable2 = hashSet.Concat(this.DomesticCocoons);
                    enumerable = (enumerable2?.ToList<Thing>());
                }
                return new HashSet<Thing>(enumerable ?? null);
            }
        }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000087 RID: 135 RVA: 0x00006550 File Offset: 0x00004750
        public HashSet<Thing> WildCocoons
        {
            get
            {
                bool flag = this.wildCocoons == null;
                if (flag)
                {
                    Map map = this.map;
                    IEnumerable<Thing> collection;
                    if (map == null)
                    {
                        collection = null;
                    }
                    else
                    {
                        ListerThings listerThings = map.listerThings;
                        if (listerThings == null)
                        {
                            collection = null;
                        }
                        else
                        {
                            List<Thing> allThings = listerThings.AllThings;
                            if (allThings == null)
                            {
                                collection = null;
                            }
                            else
                            {
                                collection = allThings.FindAll(delegate (Thing x)
                                {
                                    Building_XenomorphCocoon building_Cocoon;
                                    if ((building_Cocoon = (x as Building_XenomorphCocoon)) != null && building_Cocoon.Spawned)
                                    {
                                        Map map2 = x.Map;
                                        bool? flag2;
                                        if (map2 == null)
                                        {
                                            flag2 = null;
                                        }
                                        else
                                        {
                                            AreaManager areaManager = map2.areaManager;
                                            flag2 = ((areaManager != null) ? new bool?(!areaManager.Home[x.Position]) : null);
                                        }
                                        if (flag2 ?? false)
                                        {
                                            return x != null && !StoreUtility.IsInAnyStorage(x);
                                        }
                                    }
                                    return false;
                                });
                            }
                        }
                    }
                    this.wildCocoons = new HashSet<Thing>(collection);
                }
                return this.wildCocoons;
            }
        }

        // Token: 0x17000012 RID: 18
        // (get) Token: 0x06000088 RID: 136 RVA: 0x000065CC File Offset: 0x000047CC
        public HashSet<Thing> DomesticCocoons
        {
            get
            {
                bool flag = this.domesticCocoons == null;
                if (flag)
                {
                    Map map = this.map;
                    List<Thing> list;
                    if (map == null)
                    {
                        list = null;
                    }
                    else
                    {
                        ListerThings listerThings = map.listerThings;
                        if (listerThings == null)
                        {
                            list = null;
                        }
                        else
                        {
                            List<Thing> allThings = listerThings.AllThings;
                            if (allThings == null)
                            {
                                list = null;
                            }
                            else
                            {
                                list = allThings.FindAll(delegate (Thing x)
                                {
                                    Building_XenomorphCocoon building_Cocoon;
                                    return (building_Cocoon = (x as Building_XenomorphCocoon)) != null && building_Cocoon.Spawned;
                                });
                            }
                        }
                    }
                    List<Thing> list2 = list;
                    this.domesticCocoons = new HashSet<Thing>(list2.FindAll(delegate (Thing x)
                    {
                        Map map2 = x.Map;
                        bool? flag2;
                        if (map2 == null)
                        {
                            flag2 = null;
                        }
                        else
                        {
                            AreaManager areaManager = map2.areaManager;
                            flag2 = ((areaManager != null) ? new bool?(areaManager.Home[x.Position]) : null);
                        }
                        return (flag2 ?? false) || (x != null && StoreUtility.IsInAnyStorage(x));
                    }));
                }
                return this.domesticCocoons;
            }
        }

        // Token: 0x06000089 RID: 137 RVA: 0x0000666D File Offset: 0x0000486D
        public MapComponent_XenomorphCocoonTracker(Map map) : base(map)
        {
        }

        // Token: 0x0600008A RID: 138 RVA: 0x00006686 File Offset: 0x00004886
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.isSpiderPair, "isSpiderPair", false, false);
            Scribe_Values.Look<bool>(ref this.isGiantSpiderPair, "isGiantSpiderPair", false, false);
        }

        // Token: 0x04000027 RID: 39
        public bool isSpiderPair = false;

        // Token: 0x04000028 RID: 40
        public bool isGiantSpiderPair = false;

        // Token: 0x04000029 RID: 41
        private HashSet<Thing> wildCocoons;

        // Token: 0x0400002A RID: 42
        private HashSet<Thing> domesticCocoons;
    }
}
