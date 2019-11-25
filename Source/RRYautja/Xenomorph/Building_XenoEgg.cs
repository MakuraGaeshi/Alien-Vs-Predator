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
                float basic = 0f;
                if (xenoHatcher.eggState == CompXenoHatcher.EggState.Praetorian)
                {
                    basic += 0.25f;
                }
                if (xenoHatcher.eggState == CompXenoHatcher.EggState.Royal)
                {
                    basic += 0.5f;
                }
                if (xenoHatcher.eggState == CompXenoHatcher.EggState.Hyperfertile)
                {
                    basic += 0.75f;
                }
                float num = (basic * xenoHatcher.mutateProgress);
                return  new Vector2( 1f + (num) , 1f + (num) );
            }
        }
        
        public override Graphic Graphic
        {
            get
            {
                float num = (float)Math.Round((0.7 * (double)xenoHatcher.mutateProgress), 1);
                if (xenoHatcher.mutateProgress>0f)
                {
                    //Graphic g = base.Graphic.GetCopy(DrawSize); //
                    Graphic graphic = base.Graphic.GetCopy(DrawSize); //new Graphic();

                    //graphic.Color.r += 40f;
                    //graphic.drawSize = new Vector2(1f + (num), 1f + (num));
                    if (xenoHatcher.mutateProgress > 1f)
                    {
                        graphic.path = "Things/Resources/Raw/Xenomorph_RoyalEgg";
                    }
                    
                    return graphic;
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
