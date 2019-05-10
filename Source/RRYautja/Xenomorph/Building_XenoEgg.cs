using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RRYautja
{
    class Building_XenoEgg : Building
    {

        public override IEnumerable<Gizmo> GetGizmos()
        {
            /*
            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }
            */
            if (this.def.Minifiable) // && base.Faction == Faction.OfPlayer)
            {
                yield return InstallationDesignatorDatabase.DesignatorFor(this.def);
            }
            /*
            Command buildCopy = BuildCopyCommandUtility.BuildCopyCommand(this.def, base.Stuff);
            if (buildCopy != null)
            {
                yield return buildCopy;
            }

            if (base.Faction == Faction.OfPlayer)
            {
                foreach (Command facility in BuildFacilityCommandUtility.BuildFacilityCommands(this.def))
                {
                    yield return facility;
                }
            }
            */
            yield break;
        }
    }
}
