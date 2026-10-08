using UnityEngine;

namespace Modzified_Skill_Farming;

internal static class PlanterLevel
{
  private const string KeyName = "modzified_skill_farming_level";
  private static readonly int Key = KeyName.GetStableHashCode();

  internal static float Pending = -1f;
  internal static bool JustPlacedPlant;

  internal static float Read(ZDO zdo)
  {
    return zdo.GetFloat(Key, -1f);
  }

  internal static void Write(ZDO zdo, float level)
  {
    zdo.Set(Key, level);
  }

  internal static float RawLevel(Player player)
  {
    return player.GetSkills().GetSkillLevel(Skills.SkillType.Farming);
  }

  internal static float Ratio(float level)
  {
    return Mathf.Clamp01(level / FarmingCap.Value);
  }

  internal static float Curve(float min, float max, float level)
  {
    return min + (max - min) * Ratio(level);
  }
}
