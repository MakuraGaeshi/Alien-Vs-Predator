using System;
using System.Reflection;
using Verse;

namespace AvP
{
    // Token: 0x02000003 RID: 3
    internal static class MoreTraitSlotsUtil
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002064 File Offset: 0x00000264
        public static bool TryGetMaxTraitSlots(out int max)
        {
            bool flag = !MoreTraitSlotsUtil.initialized;
            if (flag)
            {
                MoreTraitSlotsUtil.initialized = true;
                MoreTraitSlotsUtil.Initialized();
            }
            bool flag2 = MoreTraitSlotsUtil.settingsFieldInfo != null && MoreTraitSlotsUtil.maxFieldInfo != null;
            if (flag2)
            {
                object settings = MoreTraitSlotsUtil.settingsFieldInfo.GetValue(null);
                bool flag3 = settings != null;
                if (flag3)
                {
                    max = (int)((float)MoreTraitSlotsUtil.maxFieldInfo.GetValue(settings));
                    return true;
                }
            }
            max = 0;
            return false;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000020DC File Offset: 0x000002DC
        private static void Initialized()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                bool flag = p.Name.IndexOf("More Trait Slots") != -1;
                if (flag)
                {
                    foreach (Assembly assembly in p.assemblies.loadedAssemblies)
                    {
                        Type type = assembly.GetType("MoreTraitSlots.RMTS");
                        bool flag2 = type != null;
                        if (flag2)
                        {
                            MoreTraitSlotsUtil.settingsFieldInfo = type.GetField("Settings", BindingFlags.Static | BindingFlags.Public);
                            bool flag3 = MoreTraitSlotsUtil.settingsFieldInfo != null;
                            if (flag3)
                            {
                                Type st = MoreTraitSlotsUtil.settingsFieldInfo.GetValue(null).GetType();
                                MoreTraitSlotsUtil.maxFieldInfo = st.GetField("traitsMax", BindingFlags.Instance | BindingFlags.Public);
                            }
                            return;
                        }
                    }
                }
            }
        }

        // Token: 0x04000003 RID: 3
        private static bool initialized = false;

        // Token: 0x04000004 RID: 4
        private static FieldInfo settingsFieldInfo = null;

        // Token: 0x04000005 RID: 5
        private static FieldInfo maxFieldInfo = null;
    }
}
