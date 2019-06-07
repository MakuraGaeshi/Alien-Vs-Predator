using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    class Building_XenoEgg : Building
    {

        public bool QueenPresent
        {
            get
            {
                bool queenPresent = false;
                foreach (var p in this.Map.mapPawns.AllPawnsSpawned)
                {
                    if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                    {
#if DEBUG
                    //    Log.Message(string.Format("Queen found"));
#endif
                        queenPresent = true;
                        break;
                    }
                }
                return queenPresent;
            }
        }

        public Vector2 DrawSize
        {
            get
            {
                float num = (0.6f * xenoHatcher.royalProgress) < 1.7 ? (0.6f * xenoHatcher.royalProgress) : 1.7f;
                return  new Vector2( 1f + (num) , 1f + (num) );
            }
        }

        public override Graphic Graphic
        {
            get
            {
                float num = (float)Math.Round((0.7 * (double)xenoHatcher.royalProgress), 1);
                if (false)// (xenoHatcher.royalProgress>0f)
                {
                    Graphic graphic = base.Graphic;
                    graphic.drawSize = new Vector2(1f + (num), 1f + (num));
                    return graphic;
                    return base.Graphic.GetCopy(new Vector2(1f + (num), 1f + (num)));
                }
                return base.Graphic;
            }
        }
        
        public CompXenoHatcher xenoHatcher
        {
            get
            {
                return this.TryGetComp<CompXenoHatcher>();
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % 200 == 0)
            {
            //    Log.Message(string.Format("this.Graphic.drawSize {0}", this.Graphic.drawSize));
            }
        }

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
