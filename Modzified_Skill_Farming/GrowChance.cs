using UnityEngine;

namespace Modzified_Skill_Farming;

internal static class GrowChance
{
  internal static int Percent(float plantedLevel, bool outsideBiome)
  {
    int percent = Mathf.RoundToInt(PlanterLevel.Curve(Settings.ChanceMin, Settings.ChanceMax, plantedLevel));
    if (outsideBiome)
    {
      percent -= Settings.Penalty;
    }

    return Mathf.Clamp(percent, 0, 100);
  }

  internal static bool OutsideBiome(Plant plant, ZDO zdo)
  {
    if (Settings.Penalty <= 0 || !BiomeFree.Native.TryGetValue(zdo.GetPrefab(), out BiomeFree.NativeRules native))
    {
      return false;
    }

    Vector3 position = plant.transform.position;
    Heightmap heightmap = Heightmap.FindHeightmap(position);
    if (!heightmap)
    {
      return false;
    }

    return (heightmap.GetBiome(position) & native.Biome) == 0;
  }

  internal static int Roll(int seed)
  {
    uint h = (uint)seed ^ 0x9E3779B9u;
    h ^= h >> 16;
    h *= 0x7feb352du;
    h ^= h >> 15;
    h *= 0x846ca68bu;
    h ^= h >> 16;
    return (int)(h % 100u);
  }
}
