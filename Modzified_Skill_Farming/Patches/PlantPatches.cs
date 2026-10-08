using HarmonyLib;

namespace Modzified_Skill_Farming.Patches;

[HarmonyPatch(typeof(Plant), nameof(Plant.Awake))]
internal static class Plant_Awake_Patch
{
  private static void Postfix(Plant __instance)
  {
    if (!__instance.m_nview)
    {
      return;
    }

    ZDO zdo = __instance.m_nview.GetZDO();
    if (zdo == null)
    {
      return;
    }

    if (PlanterLevel.Pending >= 0f && PlanterLevel.Read(zdo) < 0f && zdo.IsOwner())
    {
      PlanterLevel.Write(zdo, PlanterLevel.Pending);
    }

    BiomeFree.Apply(__instance);
  }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Grow))]
internal static class Plant_Grow_Patch
{
  private static bool Prefix(Plant __instance)
  {
    if (__instance.m_status != Plant.Status.Healthy || !__instance.m_nview || !__instance.m_nview.IsValid())
    {
      return true;
    }

    ZDO zdo = __instance.m_nview.GetZDO();
    float level = PlanterLevel.Read(zdo);
    if (level < 0f)
    {
      return true;
    }

    int percent = GrowChance.Percent(level, GrowChance.OutsideBiome(__instance, zdo));
    if (percent >= 100 || GrowChance.Roll(__instance.m_seed) < percent)
    {
      return true;
    }

    if (Settings.DebugOn)
    {
      SkillFarmingPlugin.LogAt(BepInEx.Logging.LogLevel.Debug,
        $"{SkillFarmingPlugin.ModName}: {__instance.name} failed its grow roll ({percent}%).");
    }

    __instance.m_nview.Destroy();
    return false;
  }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.GetGrowTime))]
internal static class Plant_GetGrowTime_Patch
{
  private static void Postfix(Plant __instance, ref float __result)
  {
    GrowthSpeed.Adjust(__instance, ref __result);
  }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.GetHoverText))]
internal static class Plant_GetHoverText_Patch
{
  private static void Postfix(Plant __instance, ref string __result)
  {
    if (!string.IsNullOrEmpty(__result))
    {
      __result += PlantHover.Extra(__instance);
    }
  }
}
