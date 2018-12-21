using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

// Token: 0x02000025 RID: 37
public class BodyAddon
{
    // Token: 0x060000B4 RID: 180 RVA: 0x00009F74 File Offset: 0x00008174
    public virtual bool CanDrawAddon(Pawn pawn)
    {
        return ((GenList.NullOrEmpty<string>(this.hiddenUnderApparelTag) && GenList.NullOrEmpty<BodyPartGroupDef>(this.hiddenUnderApparelFor)) || !GenCollection.Any<Apparel>(pawn.apparel.WornApparel, (Apparel ap) => GenCollection.Any<BodyPartGroupDef>(ap.def.apparel.bodyPartGroups, (BodyPartGroupDef bpgd) => this.hiddenUnderApparelFor.Contains(bpgd)) || GenCollection.Any<string>(ap.def.apparel.tags, (string s) => this.hiddenUnderApparelTag.Contains(s)))) && (PawnUtility.GetPosture(pawn) == null || (RestUtility.InBed(pawn) && this.drawnInBed) || this.drawnOnGround) && (GenText.NullOrEmpty(this.backstoryRequirement) || pawn.story.AllBackstories.Any((Backstory b) => b.identifier == this.backstoryRequirement)) && (GenText.NullOrEmpty(this.bodyPart) || pawn.health.hediffSet.GetNotMissingParts(0, 0, null, null).Any((BodyPartRecord bpr) => bpr.untranslatedCustomLabel == this.bodyPart || bpr.def.defName == this.bodyPart));
    }

    // Token: 0x060000B5 RID: 181 RVA: 0x0000A040 File Offset: 0x00008240
    public virtual Graphic GetPath(Pawn pawn, ref int sharedIndex, int? savedIndex = null)
    {
        List<AlienPartGenerator.BodyAddonBackstoryGraphic> list = this.backstoryGraphics;
        AlienPartGenerator.BodyAddonBackstoryGraphic bodyAddonBackstoryGraphic;
        string text;
        int num;
        if ((bodyAddonBackstoryGraphic = ((list != null) ? list.FirstOrDefault((AlienPartGenerator.BodyAddonBackstoryGraphic babgs) => pawn.story.AllBackstories.Any((Backstory bs) => bs.identifier == babgs.backstory)) : null)) != null)
        {
            text = bodyAddonBackstoryGraphic.path;
            num = bodyAddonBackstoryGraphic.variantCount;
        }
        else
        {
            List<AlienPartGenerator.BodyAddonHediffGraphic> list2 = this.hediffGraphics;
            AlienPartGenerator.BodyAddonHediffGraphic bodyAddonHediffGraphic;
            if ((bodyAddonHediffGraphic = ((list2 != null) ? list2.FirstOrDefault((AlienPartGenerator.BodyAddonHediffGraphic bahgs) => GenCollection.Any<Hediff>(pawn.health.hediffSet.hediffs, (Hediff h) => h.def.defName == bahgs.hediff)) : null)) != null)
            {
                text = bodyAddonHediffGraphic.path;
                num = bodyAddonHediffGraphic.variantCount;
            }
            else
            {
                text = this.path;
                num = this.variantCount;
            }
        }
        if (GenText.NullOrEmpty(text))
        {
            return null;
        }
        int num2;
        return GraphicDatabase.Get<Graphic_Multi>(text += (((num2 = ((savedIndex != null) ? (sharedIndex = savedIndex.Value) : (this.linkVariantIndexWithPrevious ? (sharedIndex % num) : (sharedIndex = Rand.Range(0, num))))) == 0) ? "" : num2.ToString()), (ContentFinder<Texture2D>.Get(text + "_northm", false) == null) ? ShaderDatabase.Cutout : ShaderDatabase.CutoutComplex, this.drawSize * 1.5f, this.useSkinColor ? pawn.story.SkinColor : pawn.story.hairColor, this.useSkinColor ? ((ThingDef_Race)pawn.def).alienRace.generalSettings.alienPartGenerator.SkinColor(pawn, false) : pawn.story.hairColor);
    }

    // Token: 0x040000CC RID: 204
    public string path;

    // Token: 0x040000CD RID: 205
    public string bodyPart;

    // Token: 0x040000CE RID: 206
    public bool useSkinColor = true;

    // Token: 0x040000CF RID: 207
    public AlienPartGenerator.BodyAddonOffsets offsets;

    // Token: 0x040000D0 RID: 208
    public bool linkVariantIndexWithPrevious;

    // Token: 0x040000D1 RID: 209
    public float angle;

    // Token: 0x040000D2 RID: 210
    public bool inFrontOfBody;

    // Token: 0x040000D3 RID: 211
    public float layerOffset;

    // Token: 0x040000D4 RID: 212
    public bool drawnOnGround = true;

    // Token: 0x040000D5 RID: 213
    public bool drawnInBed = true;

    // Token: 0x040000D6 RID: 214
    public Vector2 drawSize = Vector2.one;

    // Token: 0x040000D7 RID: 215
    public int variantCount;

    // Token: 0x040000D8 RID: 216
    public List<AlienPartGenerator.BodyAddonHediffGraphic> hediffGraphics;

    // Token: 0x040000D9 RID: 217
    public List<AlienPartGenerator.BodyAddonBackstoryGraphic> backstoryGraphics;

    // Token: 0x040000DA RID: 218
    public List<BodyPartGroupDef> hiddenUnderApparelFor = new List<BodyPartGroupDef>();

    // Token: 0x040000DB RID: 219
    public List<string> hiddenUnderApparelTag = new List<string>();

    // Token: 0x040000DC RID: 220
    public string backstoryRequirement;
}
