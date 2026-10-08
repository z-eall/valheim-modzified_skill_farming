using UnityEngine;

namespace Modzified_Skill_Farming;

internal static class GrowthSpeed
{
  private static int _calls;
  private static float _windowStart;

  internal static void Adjust(Plant plant, ref float growTime)
  {
    if (Settings.DebugOn)
    {
      Count();
    }

    if (!Settings.GrowthActive || !plant.m_nview || !plant.m_nview.IsValid())
    {
      return;
    }

    float level = PlanterLevel.Read(plant.m_nview.GetZDO());
    if (level < 0f)
    {
      return;
    }

    float cut = PlanterLevel.Curve(Settings.GrowthMin, Settings.GrowthMax, level) / 100f;
    growTime *= 1f - Mathf.Clamp(cut, 0f, 0.9f);
  }

  private static void Count()
  {
    _calls++;
    float now = Time.unscaledTime;
    if (now - _windowStart < 5f)
    {
      return;
    }

    SkillFarmingPlugin.LogAt(BepInEx.Logging.LogLevel.Debug,
      $"{SkillFarmingPlugin.ModName}: GetGrowTime ran {_calls} times in the last {now - _windowStart:0.0}s.");
    _calls = 0;
    _windowStart = now;
  }
}

internal static class PlantingStamina
{
  internal static float Factor(Player player, Skills.SkillType skill)
  {
    if (!PlanterLevel.JustPlacedPlant || skill != Skills.SkillType.Farming)
    {
      return player.GetSkillFactor(skill);
    }

    PlanterLevel.JustPlacedPlant = false;
    return Cut(player) / 50f;
  }

  internal static float Cut(Player player)
  {
    float cut = PlanterLevel.Curve(Settings.StaminaMin, Settings.StaminaMax, PlanterLevel.RawLevel(player));
    return Mathf.Clamp(cut, 0f, 100f);
  }
}

internal static class ScytheStamina
{
  private const float VanillaWeight = 33f;

  internal static float Factor(Character character, Skills.SkillType skill, Attack attack)
  {
    if (!attack.m_harvest || character is not Player player)
    {
      return character.GetSkillFactor(skill);
    }

    float cut = PlanterLevel.Curve(Settings.ScytheStaminaMin, Settings.ScytheStaminaMax, PlanterLevel.RawLevel(player));
    return Mathf.Clamp(cut, 0f, 100f) / VanillaWeight;
  }
}

internal static class BountifulHarvest
{
  internal static float Factor(Player player, Skills.SkillType skill, Pickable pickable)
  {
    float vanilla = pickable.m_maxLevelBonusChance;
    if (skill != Skills.SkillType.Farming || vanilla <= 0f)
    {
      return player.GetSkillFactor(skill);
    }

    float chance = PlanterLevel.Curve(Settings.BonusMin, Settings.BonusMax, PlanterLevel.RawLevel(player)) / 100f;
    return chance / vanilla;
  }
}

internal static class WiderSweep
{
  internal static float Radius(float near, float far, float factor)
  {
    float radius = Mathf.Lerp(near, far, factor);
    if (!Settings.SweepActive || !Player.m_localPlayer)
    {
      return radius;
    }

    float widen = PlanterLevel.Curve(Settings.SweepMin, Settings.SweepMax, PlanterLevel.RawLevel(Player.m_localPlayer)) / 100f;
    return radius * (1f + Mathf.Max(widen, 0f));
  }
}
