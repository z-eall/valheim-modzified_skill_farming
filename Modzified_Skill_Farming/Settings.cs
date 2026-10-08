using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using ServerSync;

namespace Modzified_Skill_Farming;

internal static class Settings
{
  internal const string SectionCultivator = "1. Cultivator";
  internal const string SectionScythe = "2. Scythe";
  internal const string SectionBonusYield = "3. Bonus Yield";
  internal const string SectionLogging = "9. Logging";

  internal const LogLevel DefaultLogLevels =
    LogLevel.Fatal | LogLevel.Error | LogLevel.Warning | LogLevel.Message | LogLevel.Info;

  internal static ConfigSync Sync { get; private set; } = null!;

  internal static ConfigEntry<LogLevel> LogLevels { get; private set; } = null!;
  internal static ConfigEntry<float> GrowChanceAt0 { get; private set; } = null!;
  internal static ConfigEntry<float> GrowChanceAtMax { get; private set; } = null!;
  internal static ConfigEntry<float> GrowthCutAt0 { get; private set; } = null!;
  internal static ConfigEntry<float> GrowthCutAtMax { get; private set; } = null!;
  internal static ConfigEntry<float> CultivatorStaminaAt0 { get; private set; } = null!;
  internal static ConfigEntry<float> CultivatorStaminaAtMax { get; private set; } = null!;
  internal static ConfigEntry<int> PlantAnywhereLevel { get; private set; } = null!;
  internal static ConfigEntry<float> BiomePenalty { get; private set; } = null!;
  internal static ConfigEntry<float> SweepAt0 { get; private set; } = null!;
  internal static ConfigEntry<float> SweepAtMax { get; private set; } = null!;
  internal static ConfigEntry<float> ScytheStaminaAt0 { get; private set; } = null!;
  internal static ConfigEntry<float> ScytheStaminaAtMax { get; private set; } = null!;
  internal static ConfigEntry<string> ScytheAllow { get; private set; } = null!;
  internal static ConfigEntry<float> BonusChanceAt0 { get; private set; } = null!;
  internal static ConfigEntry<float> BonusChanceAtMax { get; private set; } = null!;

  internal static HashSet<int> ScytheAllowHashes = new();
  internal static int AnywhereLevel = -1;
  internal static float StaminaMin, StaminaMax;
  internal static float ScytheStaminaMin, ScytheStaminaMax = 33f;
  internal static float ChanceMin = 50f, ChanceMax = 100f, Penalty = 20f;
  internal static float GrowthMin, GrowthMax;
  internal static float BonusMin, BonusMax = 25f;
  internal static float SweepMin, SweepMax;
  internal static bool GrowthActive, SweepActive, DebugOn;
  internal static int Version;

  internal static void Init(ConfigFile config)
  {
    Sync = new ConfigSync(SkillFarmingPlugin.ModGUID)
    {
      DisplayName = SkillFarmingPlugin.ModName,
      CurrentVersion = SkillFarmingPlugin.ModVersion,
      ModRequired = false,
      IsLocked = true
    };

    var percent = new AcceptableValueRange<float>(0f, 100f);
    var growth = new AcceptableValueRange<float>(0f, 90f);
    var penalty = new AcceptableValueRange<float>(0f, 50f);

    GrowChanceAt0 = BindSynced(config, SectionCultivator, "Grow chance at skill 0", 50f,
      new ConfigDescription(
        "Plant grow chance % at Farming skill 0. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 8, ShowRangeAsPercent = false }));
    GrowChanceAtMax = BindSynced(config, SectionCultivator, "Grow chance at skill max", 100f,
      new ConfigDescription(
        "Plant grow chance % at Farming skill max. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 7, ShowRangeAsPercent = false }));
    GrowthCutAt0 = BindSynced(config, SectionCultivator, "Growth time reduction at skill 0", 0f,
      new ConfigDescription(
        "Growth time reduction % at Farming skill 0. [Server Synced]",
        growth,
        new ConfigurationManagerAttributes { Order = 6, ShowRangeAsPercent = false }));
    GrowthCutAtMax = BindSynced(config, SectionCultivator, "Growth time reduction at skill max", 50f,
      new ConfigDescription(
        "Growth time reduction % at Farming skill max. [Server Synced]",
        growth,
        new ConfigurationManagerAttributes { Order = 5, ShowRangeAsPercent = false }));
    CultivatorStaminaAt0 = BindSynced(config, SectionCultivator, "Cultivator stamina reduction at skill 0", 0f,
      new ConfigDescription(
        "Stamina reduction % of Cultivator use at Farming skill 0. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 4, ShowRangeAsPercent = false }));
    CultivatorStaminaAtMax = BindSynced(config, SectionCultivator, "Cultivator stamina reduction at skill max", 50f,
      new ConfigDescription(
        "Stamina reduction % of Cultivator use at Farming skill max. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 3, ShowRangeAsPercent = false }));
    PlantAnywhereLevel = BindSynced(config, SectionCultivator, "Plant anywhere at skill level", -1,
      new ConfigDescription(
        "Farming level threshold to allow planting in any biome.\n" +
        "Value -1 disables this feature. The level is capped at 100 unless a mod raises the skill cap. [Server Synced]",
        new AcceptableValueRange<int>(-1, 250),
        new ConfigurationManagerAttributes { Order = 2 }));
    BiomePenalty = BindSynced(config, SectionCultivator, "Out-of-biome grow chance penalty", 20f,
      new ConfigDescription(
        "Plant grow chance % reduction when a plant grows outside its native biome.\n" +
        "This only matters when \"Plant anywhere\" is on. [Server Synced]",
        penalty,
        new ConfigurationManagerAttributes { Order = 1, ShowRangeAsPercent = false }));

    SweepAt0 = BindSynced(config, SectionScythe, "Scythe radius expansion at skill 0", 0f,
      new ConfigDescription(
        "Harvest radius expansion % of the Scythe at Farming skill 0.\n" +
        "The number is a percent on top of the vanilla radius. 0 keeps the vanilla radius. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 4, ShowRangeAsPercent = false }));
    SweepAtMax = BindSynced(config, SectionScythe, "Scythe radius expansion at skill max", 0f,
      new ConfigDescription(
        "Harvest radius expansion % of the Scythe at Farming skill max.\n" +
        "The number is a percent on top of the vanilla radius. 0 keeps the vanilla radius. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 3, ShowRangeAsPercent = false }));
    ScytheStaminaAt0 = BindSynced(config, SectionScythe, "Scythe stamina reduction at skill 0", 0f,
      new ConfigDescription(
        "Stamina reduction % of Scythe use at Farming skill 0. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 2, ShowRangeAsPercent = false }));
    ScytheStaminaAtMax = BindSynced(config, SectionScythe, "Scythe stamina reduction at skill max", 33f,
      new ConfigDescription(
        "Stamina reduction % of Scythe use at Farming skill max. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 1, ShowRangeAsPercent = false }));

    ScytheAllow = BindSynced(config, SectionScythe, "Allow Scythe to harvest", "Pickable_Mushroom_JotunPuffs, Pickable_Mushroom_Magecap, VineAsh",
      new ConfigDescription(
        "Prefab names the Scythe can also harvest. Separate names with commas.\n" +
        "Use it for plants the vanilla Scythe skips, such as Jotun Puffs, Magecap and the vines. An empty list keeps the vanilla behavior. [Server Synced]",
        null,
        new ConfigurationManagerAttributes { Order = 0 }));

    BonusChanceAt0 = BindSynced(config, SectionBonusYield, "Bonus yield chance at skill 0", 0f,
      new ConfigDescription(
        "Bonus chance to get one extra yield at Farming skill 0.\n" +
        "Affects picking and scything. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 2, ShowRangeAsPercent = false }));
    BonusChanceAtMax = BindSynced(config, SectionBonusYield, "Bonus yield chance at skill max", 25f,
      new ConfigDescription(
        "Bonus chance to get one extra yield at Farming skill max.\n" +
        "Affects picking and scything. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 1, ShowRangeAsPercent = false }));

    LogLevels = BindLocal(config, SectionLogging, "Log levels", DefaultLogLevels,
      new ConfigDescription(
        "Which log levels this mod writes. Add Debug to see patch traces. [Not Server Synced]",
        tags: new object[] { new ConfigurationManagerAttributes { IsAdvanced = true } }));

    config.SettingChanged += (_, _) => Refresh();
    Refresh();
    WarnInvertedEnds();
  }

  internal static void Refresh()
  {
    int oldAnywhere = AnywhereLevel;
    AnywhereLevel = PlantAnywhereLevel.Value;
    StaminaMin = CultivatorStaminaAt0.Value;
    StaminaMax = CultivatorStaminaAtMax.Value;
    ScytheStaminaMin = ScytheStaminaAt0.Value;
    ScytheStaminaMax = ScytheStaminaAtMax.Value;
    ChanceMin = GrowChanceAt0.Value;
    ChanceMax = GrowChanceAtMax.Value;
    Penalty = BiomePenalty.Value;
    GrowthMin = GrowthCutAt0.Value;
    GrowthMax = GrowthCutAtMax.Value;
    BonusMin = BonusChanceAt0.Value;
    BonusMax = BonusChanceAtMax.Value;
    SweepMin = SweepAt0.Value;
    SweepMax = SweepAtMax.Value;
    ScytheAllowHashes = ParseNames(ScytheAllow.Value);
    GrowthActive = GrowthMin > 0 || GrowthMax > 0;
    SweepActive = SweepMin > 0 || SweepMax > 0;
    DebugOn = (LogLevels.Value & LogLevel.Debug) != LogLevel.None;
    Version++;
    if (oldAnywhere != AnywhereLevel)
    {
      BiomeFree.Sweep();
    }
  }

  private static HashSet<int> ParseNames(string list)
  {
    var hashes = new HashSet<int>();
    foreach (string part in list.Split(','))
    {
      string name = part.Trim();
      if (name.Length > 0)
      {
        hashes.Add(name.GetStableHashCode());
      }
    }

    return hashes;
  }

  private static void WarnInvertedEnds()
  {
    Warn(ChanceMin > ChanceMax, "Grow chance at skill 0 is higher than at skill max.");
    Warn(GrowthMin > GrowthMax, "Growth time reduction at skill 0 is higher than at skill max.");
    Warn(BonusMin > BonusMax, "Bonus yield chance at skill 0 is higher than at skill max.");
    Warn(StaminaMin > StaminaMax, "Cultivator stamina reduction at skill 0 is higher than at skill max.");
    Warn(ScytheStaminaMin > ScytheStaminaMax, "Scythe stamina reduction at skill 0 is higher than at skill max.");
    Warn(SweepMin > SweepMax, "Scythe radius expansion at skill 0 is higher than at skill max.");
  }

  private static void Warn(bool inverted, string text)
  {
    if (inverted)
    {
      SkillFarmingPlugin.LogAt(LogLevel.Warning, $"{SkillFarmingPlugin.ModName}: {text} The value falls as skill rises.");
    }
  }

  private static ConfigEntry<T> BindSynced<T>(ConfigFile config, string section, string key, T value, ConfigDescription description)
  {
    ConfigEntry<T> entry = config.Bind(section, key, value, description);
    Sync.AddConfigEntry(entry).SynchronizedConfig = true;
    return entry;
  }

  private static ConfigEntry<T> BindLocal<T>(ConfigFile config, string section, string key, T value, ConfigDescription description)
  {
    ConfigEntry<T> entry = config.Bind(section, key, value, description);
    Sync.AddConfigEntry(entry).SynchronizedConfig = false;
    return entry;
  }
}

internal sealed class ConfigurationManagerAttributes
{
  public int? Order;
  public bool? ShowRangeAsPercent;
  public bool? IsAdvanced;
}
