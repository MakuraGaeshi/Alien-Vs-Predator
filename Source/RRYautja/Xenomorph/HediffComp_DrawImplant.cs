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

    [StaticConstructorOnStartup]
    public class HediffComp_DrawImplant : HediffComp
    {
        public HediffCompProperties_DrawImplant implantDrawProps
        {
            get
            {
                return this.props as HediffCompProperties_DrawImplant;
            }
        }

        private bool ShouldDisplay
        {
            get
            {
                Pawn wearer = base.Pawn;
                return wearer.Spawned && !wearer.Dead && wearer.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection);
            }
        }

        public Material ImplantMaterial(Pawn pawn, Rot4 bodyFacing)
        {
            string path;
            HediffComp_XenoFacehugger _XenoFacehugger = parent.TryGetComp<HediffComp_XenoFacehugger>();
            if (_XenoFacehugger!=null)
            {
                
                if (this.implantDrawProps.implantDrawerType == ImplantDrawerType.Head)
                {
                    path = _XenoFacehugger.TexPath;
                }
                else
                {
                    path = _XenoFacehugger.TexPath + "_" + pawn.story.bodyType.ToString();
                }
            }
            else // if (!implantDrawProps.implantGraphicPath.NullOrEmpty())
            {
                if (this.implantDrawProps.implantDrawerType == ImplantDrawerType.Head)
                {
                    path = implantDrawProps.implantGraphicPath;
                }
                else
                {
                    path = implantDrawProps.implantGraphicPath + "_" + pawn.story.bodyType.ToString();
                }
            }
            return GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, Vector2.one, Color.white).MatAt(bodyFacing);
        }

        protected void GetAltitudeOffset(Rot4 rotation, out float OffsetX, out float OffsetY, out float OffsetZ, out float DrawSizeX, out float DrawSizeZ)
        {
            FacehuggerOffsetDefExtension myDef = Pawn.kindDef.race.GetModExtension<FacehuggerOffsetDefExtension>() ?? new FacehuggerOffsetDefExtension();
            
            string direction;
            if (Pawn.RaceProps.Humanlike)
            {
                if (rotation == Rot4.North)
                {
                    OffsetX = myDef.NorthXOffset;
                    OffsetY = myDef.NorthYOffset;
                    OffsetZ = myDef.NorthZOffset;
                    DrawSizeX = myDef.NorthXDrawSize;
                    DrawSizeZ = myDef.NorthZDrawSize;
                    direction = "North";
                }
                else if (rotation == Rot4.West)
                {
                    OffsetX = myDef.WestXOffset;
                    OffsetY = myDef.WestYOffset;
                    OffsetZ = myDef.WestZOffset;
                    DrawSizeX = myDef.WestXDrawSize;
                    DrawSizeZ = myDef.WestZDrawSize;
                    direction = "West";
                }
                else if (rotation == Rot4.East)
                {
                    OffsetX = myDef.EastXOffset;
                    OffsetY = myDef.EastYOffset;
                    OffsetZ = myDef.EastZOffset;
                    DrawSizeX = myDef.EastXDrawSize;
                    DrawSizeZ = myDef.EastZDrawSize;
                    direction = "East";
                }
                else if (rotation == Rot4.South)
                {
                    OffsetX = myDef.SouthXOffset;
                    OffsetY = myDef.SouthYOffset;
                    OffsetZ = myDef.SouthZOffset;
                    DrawSizeX = myDef.SouthXDrawSize;
                    DrawSizeZ = myDef.SouthZDrawSize;
                    direction = "South";
                }
                else
                {
                    OffsetX = 0f;
                    OffsetY = 0f;
                    OffsetZ = 0f;
                    DrawSizeX = 1f;
                    DrawSizeZ = 1f;
                    direction = "Unknown";
                }
                if (myDef.ApplyBaseHeadOffset)
                {
                    OffsetX = myDef.SouthXOffset + Pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).x;
                    OffsetY = myDef.SouthYOffset + Pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).y;
                    OffsetZ = myDef.SouthZOffset + Pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).z;
                }
            }
            else
            {
                if (rotation == Rot4.North)
                {
                    OffsetX = myDef.NorthXOffset;
                    OffsetY = myDef.NorthYOffset;
                    OffsetZ = myDef.NorthZOffset;
                    DrawSizeX = myDef.NorthXDrawSize;
                    DrawSizeZ = myDef.NorthZDrawSize;
                    direction = "North";
                }
                else if (rotation == Rot4.West)
                {
                    OffsetX = myDef.WestXOffset;
                    OffsetY = myDef.WestYOffset;
                    OffsetZ = myDef.WestZOffset;
                    DrawSizeX = myDef.WestXDrawSize;
                    DrawSizeZ = myDef.WestZDrawSize;
                    direction = "West";
                }
                else if (rotation == Rot4.East)
                {
                    OffsetX = myDef.EastXOffset;
                    OffsetY = myDef.EastYOffset;
                    OffsetZ = myDef.EastZOffset;
                    DrawSizeX = myDef.EastXDrawSize;
                    DrawSizeZ = myDef.EastZDrawSize;
                    direction = "East";
                }
                else if (rotation == Rot4.South)
                {
                    OffsetX = myDef.SouthXOffset;
                    OffsetY = myDef.SouthYOffset;
                    OffsetZ = myDef.SouthZOffset;
                    DrawSizeX = myDef.SouthXDrawSize;
                    DrawSizeZ = myDef.SouthZDrawSize;
                    direction = "South";
                }
                else
                {
                    OffsetX = 0f;
                    OffsetY = 0f;
                    OffsetZ = 0f;
                    DrawSizeX = 1f;
                    DrawSizeZ = 1f;
                    direction = "Unknown";
                }
            }
            
        }

        public void DrawImplant(Vector3 rootLoc)
        {// this.Pawn

            bool selected = Find.Selector.SelectedObjects.Contains(this.Pawn);
            HediffComp_DrawImplant comp = this;
            string direction = "";
            float angle = 0f;
            float offset = 0f;
            float yvalue = rootLoc.y;
            Vector3 drawPos = rootLoc;
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
            /*
            if (Pawn.InBed())
            {
                if (Pawn.CurrentBed().Rotation == Rot4.South)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos.x -= pawnRenderer.BaseHeadOffsetAt(Rot4.South).x;
                    drawPos.z -= pawnRenderer.BaseHeadOffsetAt(Rot4.South).z;
                }
                else if (Pawn.CurrentBed().Rotation == Rot4.North)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos.x += pawnRenderer.BaseHeadOffsetAt(Rot4.North).x;
                    drawPos.z += pawnRenderer.BaseHeadOffsetAt(Rot4.North).z;
                }
                else if (Pawn.CurrentBed().Rotation == Rot4.East)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos.x -= pawnRenderer.BaseHeadOffsetAt(Rot4.East).x;
                    drawPos.z -= pawnRenderer.BaseHeadOffsetAt(Rot4.East).z;
                }
                else if (Pawn.CurrentBed().Rotation == Rot4.West)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0}", Pawn.CurrentBed().Rotation.ToStringHuman()));
#endif
                    drawPos.x -= pawnRenderer.BaseHeadOffsetAt(Rot4.West).x;
                    drawPos.z -= pawnRenderer.BaseHeadOffsetAt(Rot4.West).z;
                }
                //drawPos.y = yvalue;
#if DEBUG
                if (selected) Log.Message(string.Format("{0} in bed {1} drawpos modified to {2}", Pawn.Name, Pawn.InBed(), drawPos));
#endif
            }
            */
            if (offset < 0)
            {
                drawPos.y -= offset;
            }
            else drawPos.y += offset;
            if (Pawn.Downed)
            {
                if (Pawn.CarriedBy != null)
                {
#if DEBUG
                    if (selected) Log.Message(string.Format("{0} carried by {1} angle {2} modified to {3}", Pawn.Name, Pawn.CarriedBy.Name, angle, Pawn.CarriedBy.carryTracker.CarriedThing.Rotation.AsAngle));
#endif
                    angle = Pawn.CarriedBy.carryTracker.CarriedThing.Rotation.AsAngle;

                }
                if (Pawn.RaceProps.Humanlike && Pawn.InBed())
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
            //    Log.Message(string.Format("{0}'s {1}, Rot:{2}, offset:{3}, x:{4}, z:{5}", Pawn.Label, this.parent.def.label, rot, offset, drawPos.x, drawPos.z));
            //    Log.Message(string.Format("Rot ToStringHuman:{1}, FacingCell:{2}, AsVector2:{3}, AsByte:{4}, AsAngle:{5}", rot, rot.ToStringHuman(), rot.FacingCell, rot.AsVector2, rot.AsByte, rot.AsAngle));
            }
#endif
            if (Pawn.kindDef.race.GetModExtension<FacehuggerOffsetDefExtension>()!=null)
            {
                GetAltitudeOffset(rot, out float X, out float Y, out float Z, out float DsX, out float DsZ);
                drawPos.x += X;
                drawPos.y += Y;
                drawPos.z += Z;
                s.x = DsX;
                s.z = DsZ;
            }
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

        // Token: 0x0600005E RID: 94 RVA: 0x00004008 File Offset: 0x00002208
        public virtual void DrawWornExtras()
        {
            if (this.ShouldDisplay && !base.Pawn.RaceProps.Humanlike)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, base.Pawn.BodySize);
                Vector3 vector = base.Pawn.Drawer.DrawPos;
                vector.y = Altitudes.AltitudeFor(AltitudeLayer.VisEffects);
                float angle = 0f;// (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }

        }
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Ui/FacehuggerInfectionOverlay", ShaderDatabase.Transparent);

    }


    public enum ImplantDrawerType
    {
        Undefined,
        Backpack,
        Head
    }

}
