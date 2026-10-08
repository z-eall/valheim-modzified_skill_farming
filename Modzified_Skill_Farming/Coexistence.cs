using BepInEx.Bootstrap;

namespace Modzified_Skill_Farming;

internal static class Coexistence
{
  private const string SmoothbrainGuid = "org.bepinex.plugins.farming";
  private static bool _logged;

  internal static void WarnOnce()
  {
    if (_logged)
    {
      return;
    }

    _logged = true;
    foreach (var pair in Chainloader.PluginInfos)
    {
      if (string.Equals(pair.Key, SmoothbrainGuid, System.StringComparison.OrdinalIgnoreCase))
      {
        SkillFarmingPlugin.LogAt(BepInEx.Logging.LogLevel.Warning,
          $"{SkillFarmingPlugin.ModName}: Farming by Smoothbrain is also loaded. Both mods change growth, biome rules, stamina and harvest. Results can differ from the settings.");
        return;
      }
    }
  }
}
