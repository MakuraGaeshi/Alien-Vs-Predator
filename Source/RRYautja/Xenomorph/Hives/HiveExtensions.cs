using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RRYautja.ExtensionMethods
{
    public static class HiveExtensions
    {

        public static HiveGrid hiveGrid(this Map map)
        {
            if (hivegrid==null)
            {
                hivegrid = new HiveGrid(map);
            }
            return hivegrid;
        }

        public static HiveGrid hivegrid;
    }
    // Token: 0x02000C94 RID: 3220
    [Flags]
    public enum MapMeshFlag
    {
        Hive = 84
    }
}
