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
                return xenoHatcher.QueenPresent;
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
                if (xenoHatcher.royalProgress>0f)
                {
                    //Graphic g = base.Graphic.GetCopy(DrawSize); //
                    Graphic graphic = base.Graphic.GetCopy(DrawSize); //new Graphic();

                    //graphic.Color.r += 40f;
                    //graphic.drawSize = new Vector2(1f + (num), 1f + (num));
                    if (xenoHatcher.royalProgress > 1f)
                    {
                        graphic.path = "Things/Resources/Raw/Xenomorph_RoyalEgg";
                    }
                    
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
            if (Find.TickManager.TicksGame % 100 == 0)
            {
                if (this.Faction==null)
                {
                    this.SetFaction(Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph));
                }
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
