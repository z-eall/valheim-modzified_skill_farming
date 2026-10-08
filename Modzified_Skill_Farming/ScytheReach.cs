using System.Collections.Generic;
using UnityEngine;

namespace Modzified_Skill_Farming;

internal static class ScytheReach
{
  private static readonly HashSet<Pickable> Done = new();
  private static readonly HashSet<int> Logged = new();
  private static int _frame = -1;

  internal static Pickable? Resolve(GameObject target)
  {
    Pickable? pickable = target.GetComponent<Pickable>();
    if (Settings.ScytheAllowHashes.Count == 0)
    {
      return pickable;
    }

    if (pickable != null && pickable.m_harvestable)
    {
      return pickable;
    }

    bool child = pickable == null;
    if (child)
    {
      pickable = target.GetComponentInParent<Pickable>();
    }

    if (pickable == null || !pickable.m_nview || !pickable.m_nview.IsValid())
    {
      return child ? null : pickable;
    }

    int prefab = pickable.m_nview.GetZDO().GetPrefab();
    if (!Settings.ScytheAllowHashes.Contains(prefab))
    {
      return child ? null : pickable;
    }

    if (_frame != Time.frameCount)
    {
      _frame = Time.frameCount;
      Done.Clear();
    }

    if (!Done.Add(pickable))
    {
      return null;
    }

    pickable.m_harvestable = true;
    if (Settings.DebugOn && Logged.Add(prefab))
    {
      SkillFarmingPlugin.LogAt(BepInEx.Logging.LogLevel.Debug,
        $"{SkillFarmingPlugin.ModName}: Scythe reached {pickable.name} (child collider {child}).");
    }

    return pickable;
  }
}
