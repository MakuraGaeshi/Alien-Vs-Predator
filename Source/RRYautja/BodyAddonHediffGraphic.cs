using System;
using System.Xml;
using JetBrains.Annotations;

// Token: 0x02000026 RID: 38
public class BodyAddonHediffGraphic
{
    // Token: 0x060000BC RID: 188 RVA: 0x0000A2BF File Offset: 0x000084BF
    [UsedImplicitly]
    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        this.hediff = xmlRoot.Name;
        this.path = xmlRoot.FirstChild.Value;
    }

    // Token: 0x040000DD RID: 221
    public string hediff;

    // Token: 0x040000DE RID: 222
    public string path;

    // Token: 0x040000DF RID: 223
    public int variantCount;
}
