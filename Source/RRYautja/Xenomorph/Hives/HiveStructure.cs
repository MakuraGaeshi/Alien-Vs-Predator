using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RRYautja.Xenomorph.Hives
{
    public static class HiveStructure
    {
        public static List<IntVec3> HiveStruct(IntVec3 HiveCenter)
        {
            List<IntVec3> hiveStruct = new List<IntVec3>()
            {
                // Support Collums
                // Cardinals
                new IntVec3
                {
                    x = HiveCenter.x + 3,
                    z = HiveCenter.z
                },
                new IntVec3
                {
                    x = HiveCenter.x - 3,
                    z = HiveCenter.z
                },
                new IntVec3
                {
                    x = HiveCenter.x,
                    z = HiveCenter.z + 3
                },
                new IntVec3
                {
                    x = HiveCenter.x,
                    z = HiveCenter.z - 3
                },
                // Corners
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x - 2,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z - 2
                },
                new IntVec3
                {
                    x = HiveCenter.x - 2,
                    z = HiveCenter.z - 2
                }

            };
            return hiveStruct;
        }

        public static List<IntVec3> HiveWalls(IntVec3 HiveCenter)
        {
            List<IntVec3> hiveWalls = new List<IntVec3>()
            {
                // Exterior Walls
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 1
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 3
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 4
                },
                new IntVec3
                {
                    x = HiveCenter.x + 5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + -1
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + -2
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + -3
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -4
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + -5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 1
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 3
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 4
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -1
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -2
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -3
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -4
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + -5
                },
                // 
                new IntVec3
                {
                    x = HiveCenter.x + 1,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 3,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 4,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + -1,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -2,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -3,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -4,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + 1,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 3,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 4,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -1,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -2,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -3,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -4,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + -5
                },
            

        };
            return hiveWalls;
        }
    }
}
