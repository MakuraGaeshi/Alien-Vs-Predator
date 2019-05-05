using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RRYautja
{
    // Token: 0x02000D7E RID: 3454
    public class XenoHediffWithComps : HediffWithComps
    {
        // Token: 0x06004CB6 RID: 19638 RVA: 0x0009BB84 File Offset: 0x00099F84
        public override void PostTick()
        {
            base.PostTick();
            if (this.comps != null)
            {
                float num = 0f;
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompPostTick(ref num);
                }
                if (num != 0f)
                {
                    this.Severity += num;
                }
                HediffComp_DrawImplant comp = this.TryGetComp<HediffComp_DrawImplant>();
                if (comp != null)
                {
                //    Draw();
                    
                }
            }
        }

        public void Draw()
        {
            HediffComp_DrawImplant comp = this.TryGetComp<HediffComp_DrawImplant>();
            string direction = "";
            float angle = 0f;
            float offset = 0f;
            Vector3 drawPos = pawn.Dead ? pawn.PositionHeld.ToVector3() : pawn.Drawer.DrawPos;
            drawPos.y = Altitudes.AltitudeFor((AltitudeLayer)17);
            Vector3 s = new Vector3(1.5f, 1.5f, 1.5f);
            PawnRenderer pawnRenderer = this.pawn.Drawer.renderer;
            Rot4 rot = LayingFacing();
            bool selected = Find.Selector.SingleSelectedThing == pawn;
            bool flag3 = pawn.Rotation == Rot4.North;
            if (flag3)
            {
                //offset = NorthOffset;
                //    drawPos.x -= 0.1f;
                //    drawPos.z -= (0.2f);
                direction = "North";
            }
            else
            {
                bool flag4 = pawn.Rotation == Rot4.South;
                if (flag4)
                {
                    //offset = SouthOffset;
                    //    drawPos.x += 0.1f;
                    //    drawPos.z -= (0.2f);
                    direction = "South";
                }
                else
                {
                    bool flag5 = pawn.Rotation == Rot4.East;
                    if (flag5)
                    {
                        //offset = EastOffset;
                        //    drawPos.z -= (0.2f);
                        //    angle = 22.5f;
                        direction = "East";
                    }
                    else
                    {
                        bool flag6 = pawn.Rotation == Rot4.West;
                        if (flag6)
                        {
                            //offset = WestOffset;
                            //    drawPos.z -= (0.2f);
                            //    angle = 337.5f;
                            direction = "West";
                        }
                    }
                }
            }
            if (offset < 0)
            {
                drawPos.y -= offset;
            }
            else drawPos.y += offset;
            //Log.Message(string.Format("PauldronGraphic drawPos.y: {1}", PauldronGraphic.path, drawPos.y));
            angle = pawnRenderer.wiggler.downedAngle;
            //Material mat = apparelGraphic.graphic.MatAt(rotation);
            if (selected)
            {
                Log.Message(string.Format("{0}'s {1} CompPauldronDrawer, {2} offset: {3}, drawPos.y:{4}", pawn.Label, this.def.label, direction, offset, drawPos.y));
            }
            Material matSingle = comp.ImplantMaterial(pawn, rot); //.GetColoredVersion(ShaderDatabase.Cutout, this.mainColor, this.secondaryColor).MatAt(rotation);
            //    Log.Message(string.Format("PauldronGraphic this.mainColor:{0}, this.secondaryColor: {1}", this.mainColor, this.secondaryColor));
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(rot == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, matSingle, 0);
        }

        // Copied from PawnRenderer
        private Rot4 LayingFacing()
        {
            if (pawn == null)
            {
                return Rot4.Random;
            }
            if (pawn.GetPosture() == PawnPosture.LayingOnGroundFaceUp)
            {
                return Rot4.South;
            }
            if (pawn.RaceProps.Humanlike)
            {
                switch (pawn.thingIDNumber % 4)
                {
                    case 0:
                        return Rot4.South;
                    case 1:
                        return Rot4.South;
                    case 2:
                        return Rot4.East;
                    case 3:
                        return Rot4.West;
                }
            }
            else
            {
                switch (pawn.thingIDNumber % 4)
                {
                    case 0:
                        return Rot4.South;
                    case 1:
                        return Rot4.East;
                    case 2:
                        return Rot4.West;
                    case 3:
                        return Rot4.West;
                }
            }
            return Rot4.Random;
        }
    }
}
