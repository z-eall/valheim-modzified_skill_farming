using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Modzified_Skill_Farming;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class SkillFarmingPlugin : BaseUnityPlugin
{
  internal const string ModName = "Modzified_Skill_Farming";
  internal const string ModVersion = "0.1.0";
  internal const string ModGUID = "modzified_skill_farming";

  internal static ManualLogSource Log { get; private set; } = null!;

  internal static bool Allows(LogLevel level)
  {
    return Settings.LogLevels == null || (Settings.LogLevels.Value & level) != LogLevel.None;
  }

  internal static void LogAt(LogLevel level, string message)
  {
    if (!Allows(level))
    {
      return;
    }

    Log.Log(level, message);
  }

  private readonly Harmony _harmony = new(ModGUID);

  private void Awake()
  {
    Log = Logger;
    Settings.Init(Config);
    _harmony.PatchAll(Assembly.GetExecutingAssembly());
    LogAt(LogLevel.Info, $"{ModName} v{ModVersion} loaded (GUID {ModGUID}).");
  }

  private void Start()
  {
    FarmingCap.Probe();
    Coexistence.WarnOnce();
  }

  private void OnDestroy()
  {
    _harmony.UnpatchSelf();
  }
}
