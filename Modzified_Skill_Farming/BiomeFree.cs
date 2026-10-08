using System.Collections.Generic;
using UnityEngine;

namespace Modzified_Skill_Farming;

internal static class BiomeFree
{
  internal sealed class NativeRules
  {
    internal Heightmap.Biome Biome;
    internal bool Heat;
    internal bool Cold;
  }

  internal static readonly Dictionary<int, NativeRules> Native = new();

  internal static bool Eligible(float plantedLevel)
  {
    int threshold = Settings.AnywhereLevel;
    if (threshold < 0 || plantedLevel < 0f)
    {
      return false;
    }

    return plantedLevel >= Mathf.Min(threshold, FarmingCap.Value);
  }

  internal static void Apply(Plant plant)
  {
    if (!plant.m_nview || !plant.m_nview.IsValid())
    {
      return;
    }

    ZDO zdo = plant.m_nview.GetZDO();
    int prefab = zdo.GetPrefab();
    if (!Native.TryGetValue(prefab, out NativeRules native))
    {
      native = new NativeRules { Biome = plant.m_biome, Heat = plant.m_tolerateHeat, Cold = plant.m_tolerateCold };
      Native[prefab] = native;
    }

    bool free = Eligible(PlanterLevel.Read(zdo));
    plant.m_biome = free ? native.Biome | Heightmap.Biome.All : native.Biome;
    plant.m_tolerateHeat = free || native.Heat;
    plant.m_tolerateCold = free || native.Cold;
  }

  internal static void Sweep()
  {
    List<SlowUpdate> all = SlowUpdate.GetAllInstaces();
    int count = 0;
    for (int i = 0; i < all.Count; i++)
    {
      if (all[i] is Plant plant)
      {
        Apply(plant);
        count++;
      }
    }

    if (Settings.DebugOn)
    {
      SkillFarmingPlugin.LogAt(BepInEx.Logging.LogLevel.Debug, $"{SkillFarmingPlugin.ModName}: biome sweep touched {count} plants.");
    }
  }
}
