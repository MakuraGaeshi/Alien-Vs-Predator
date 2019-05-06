using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_DrawImplant : HediffCompProperties
    {
        public ImplantDrawerType implantDrawerType;

        public string implantGraphicPath;
    }
    public class HediffComp_DrawImplant : HediffComp
    {
        public HediffCompProperties_DrawImplant implantDrawProps
        {
            get
            {
#if DEBUG

#endif
                return this.props as HediffCompProperties_DrawImplant;
            }
        }

        public Material ImplantMaterial(Pawn pawn, Rot4 bodyFacing)
        {
            string path;
            if (this.implantDrawProps.implantDrawerType == ImplantDrawerType.Head)
            {
                path = implantDrawProps.implantGraphicPath;
            }
            else
            {
                path = implantDrawProps.implantGraphicPath + "_" + pawn.story.bodyType.ToString();
            }
            return GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, Vector2.one, Color.white).MatAt(bodyFacing);
        }
        public void DrawImplant()
        {// this.Pawn

            bool selected = Find.Selector.SelectedObjects.Contains(this.Pawn);
            HediffComp_DrawImplant comp = this;
            string direction = "";
            float angle = 0f;
            float offset = 0f;
            float yvalue = Pawn.Drawer.DrawPos.y;
            Vector3 drawPos = Pawn.Drawer.DrawPos;
            drawPos.y = Altitudes.AltitudeFor((AltitudeLayer)17);
            Vector3 s = new Vector3(1.5f, 1.5f, 1.5f);
            PawnRenderer pawnRenderer = this.Pawn.Drawer.renderer;
            Rot4 rot = LayingFacing();
            if (Pawn.CarriedBy!=null)
            {
#if DEBUG
                if (selected) Log.Message(string.Format("{0} carried by {1} drawpos {2} modified to {3}", Pawn.Name, Pawn.CarriedBy.Name, drawPos, Pawn.CarriedBy.DrawPos));
#endif
                drawPos.z = Pawn.CarriedBy.DrawPos.z;
                drawPos.x = Pawn.CarriedBy.DrawPos.x;
            }
            if (Pawn.InBed())
            {
                if (Pawn.CurrentBed().Rotation == Rot4.South)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos -= pawnRenderer.BaseHeadOffsetAt(Rot4.South);
                }
                else if (Pawn.CurrentBed().Rotation == Rot4.North)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos -= pawnRenderer.BaseHeadOffsetAt(Rot4.North);
                }
                else if (Pawn.CurrentBed().Rotation == Rot4.East)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos.x += 0.3f;
                }
                else if (Pawn.CurrentBed().Rotation == Rot4.West)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos.x -= 0.3f;
                }
                drawPos.y = yvalue;
#if DEBUG
                if (selected) Log.Message(string.Format("{0} in bed {1} drawpos modified to {2}", Pawn.Name, Pawn.InBed(), drawPos));
#endif
            }
            if (offset < 0)
            {
                drawPos.y -= offset;
            }
            else drawPos.y += offset;
            if (Pawn.Downed)
            {
                angle = pawnRenderer.wiggler.downedAngle;
                if (Pawn.CarriedBy != null)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0} carried by {1} angle {2} modified to {3}", Pawn.Name, Pawn.CarriedBy.Name, angle, Pawn.CarriedBy.carryTracker.CarriedThing.Rotation.AsAngle));
#endif
                    angle = Pawn.CarriedBy.carryTracker.CarriedThing.Rotation.AsAngle;

                }
                if (Pawn.InBed())
                {

                    if (Pawn.CurrentBed().Rotation == Rot4.South)
                    {
                        angle = 0f;
                    }
                    else if (Pawn.CurrentBed().Rotation == Rot4.North)
                    {
                        angle = 180f;
                    }
                    else if (Pawn.CurrentBed().Rotation == Rot4.East)
                    {
                        angle = 270;
                    }
                    else if (Pawn.CurrentBed().Rotation == Rot4.West)
                    {
                        angle = 90;
                    }

                   // angle = Pawn.CurrentBed().Rotation.AsAngle;
                }
                if (Pawn.kindDef.race == YautjaDefOf.RRY_Alien_Yautja)
                {
                    s = new Vector3(2f, 1.5f, 2f);
                    if (rot.ToStringHuman() == "West" || rot == Rot4.West)
                    {
                        //drawPos.z -= 0.15f;
                       // drawPos.x += 0.25f;
                    }
                    else
                    {

                    //    drawPos.x += 0.25f;
                    }
                }
            }
#if DEBUG
            if (selected)
            {
                Log.Message(string.Format("{0}'s {1}, Rot:{2}, offset:{3}, x:{4}, z:{5}", Pawn.Label, this.parent.def.label, rot, offset, drawPos.x, drawPos.z));
                Log.Message(string.Format("Rot ToStringHuman:{1}, FacingCell:{2}, AsVector2:{3}, AsByte:{4}, AsAngle:{5}", rot, rot.ToStringHuman(), rot.FacingCell, rot.AsVector2, rot.AsByte, rot.AsAngle));
            }
#endif
            Material matSingle = comp.ImplantMaterial(Pawn, rot);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(rot == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, matSingle, 0);
        }

        // Copied from PawnRenderer
        private Rot4 LayingFacing()
        {
            if (Pawn == null)
            {
                return Rot4.Random;
            }
            if (Pawn.GetPosture() == PawnPosture.LayingOnGroundFaceUp)
            {
                return Rot4.South;
            }
            if (Pawn.RaceProps.Humanlike)
            {
                switch (Pawn.thingIDNumber % 4)
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
                switch (Pawn.thingIDNumber % 4)
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
    public enum ImplantDrawerType
    {
        Undefined,
        Backpack,
        Head
    }
}
