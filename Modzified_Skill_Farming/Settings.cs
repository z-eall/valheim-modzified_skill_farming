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
  internal static ConfigEntry<int> GrowChanceAt0 { get; private set; } = null!;
  internal static ConfigEntry<int> GrowChanceAtMax { get; private set; } = null!;
  internal static ConfigEntry<int> GrowthCutAt0 { get; private set; } = null!;
  internal static ConfigEntry<int> GrowthCutAtMax { get; private set; } = null!;
  internal static ConfigEntry<int> CultivatorStaminaAt0 { get; private set; } = null!;
  internal static ConfigEntry<int> CultivatorStaminaAtMax { get; private set; } = null!;
  internal static ConfigEntry<int> PlantAnywhereLevel { get; private set; } = null!;
  internal static ConfigEntry<int> BiomePenalty { get; private set; } = null!;
  internal static ConfigEntry<int> SweepAt0 { get; private set; } = null!;
  internal static ConfigEntry<int> SweepAtMax { get; private set; } = null!;
  internal static ConfigEntry<int> ScytheStaminaAt0 { get; private set; } = null!;
  internal static ConfigEntry<int> ScytheStaminaAtMax { get; private set; } = null!;
  internal static ConfigEntry<int> BonusChanceAt0 { get; private set; } = null!;
  internal static ConfigEntry<int> BonusChanceAtMax { get; private set; } = null!;

  internal static int AnywhereLevel = -1;
  internal static int StaminaMin, StaminaMax;
  internal static int ScytheStaminaMin, ScytheStaminaMax = 33;
  internal static int ChanceMin = 50, ChanceMax = 100, Penalty = 20;
  internal static int GrowthMin, GrowthMax;
  internal static int BonusMin, BonusMax = 25;
  internal static int SweepMin, SweepMax;
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

    var percent = new AcceptableValueRange<int>(0, 100);
    var growth = new AcceptableValueRange<int>(0, 90);
    var penalty = new AcceptableValueRange<int>(0, 50);

    GrowChanceAt0 = BindSynced(config, SectionCultivator, "Grow chance at skill 0", 50,
      new ConfigDescription(
        "Plant grow chance % at Farming skill 0. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 8, ShowRangeAsPercent = false }));
    GrowChanceAtMax = BindSynced(config, SectionCultivator, "Grow chance at skill max", 100,
      new ConfigDescription(
        "Plant grow chance % at Farming skill max. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 7, ShowRangeAsPercent = false }));
    GrowthCutAt0 = BindSynced(config, SectionCultivator, "Growth time reduction at skill 0", 0,
      new ConfigDescription(
        "Growth time reduction % at Farming skill 0. [Server Synced]",
        growth,
        new ConfigurationManagerAttributes { Order = 6, ShowRangeAsPercent = false }));
    GrowthCutAtMax = BindSynced(config, SectionCultivator, "Growth time reduction at skill max", 50,
      new ConfigDescription(
        "Growth time reduction % at Farming skill max. [Server Synced]",
        growth,
        new ConfigurationManagerAttributes { Order = 5, ShowRangeAsPercent = false }));
    CultivatorStaminaAt0 = BindSynced(config, SectionCultivator, "Cultivator stamina reduction at skill 0", 0,
      new ConfigDescription(
        "Stamina reduction % of Cultivator use at Farming skill 0. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 4, ShowRangeAsPercent = false }));
    CultivatorStaminaAtMax = BindSynced(config, SectionCultivator, "Cultivator stamina reduction at skill max", 50,
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
    BiomePenalty = BindSynced(config, SectionCultivator, "Out-of-biome grow chance penalty", 20,
      new ConfigDescription(
        "Plant grow chance % reduction when a plant grows outside its native biome.\n" +
        "This only matters when \"Plant anywhere\" is on. [Server Synced]",
        penalty,
        new ConfigurationManagerAttributes { Order = 1, ShowRangeAsPercent = false }));

    SweepAt0 = BindSynced(config, SectionScythe, "Scythe radius expansion at skill 0", 0,
      new ConfigDescription(
        "Harvest radius expansion % of the Scythe at Farming skill 0.\n" +
        "The number is a percent on top of the vanilla radius. 0 keeps the vanilla radius. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 4, ShowRangeAsPercent = false }));
    SweepAtMax = BindSynced(config, SectionScythe, "Scythe radius expansion at skill max", 0,
      new ConfigDescription(
        "Harvest radius expansion % of the Scythe at Farming skill max.\n" +
        "The number is a percent on top of the vanilla radius. 0 keeps the vanilla radius. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 3, ShowRangeAsPercent = false }));
    ScytheStaminaAt0 = BindSynced(config, SectionScythe, "Scythe stamina reduction at skill 0", 0,
      new ConfigDescription(
        "Stamina reduction % of Scythe use at Farming skill 0. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 2, ShowRangeAsPercent = false }));
    ScytheStaminaAtMax = BindSynced(config, SectionScythe, "Scythe stamina reduction at skill max", 33,
      new ConfigDescription(
        "Stamina reduction % of Scythe use at Farming skill max. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 1, ShowRangeAsPercent = false }));

    BonusChanceAt0 = BindSynced(config, SectionBonusYield, "Bonus yield chance at skill 0", 0,
      new ConfigDescription(
        "Bonus chance to get one extra yield at Farming skill 0.\n" +
        "Affects picking and scything. [Server Synced]",
        percent,
        new ConfigurationManagerAttributes { Order = 2, ShowRangeAsPercent = false }));
    BonusChanceAtMax = BindSynced(config, SectionBonusYield, "Bonus yield chance at skill max", 25,
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
    GrowthActive = GrowthMin > 0 || GrowthMax > 0;
    SweepActive = SweepMin > 0 || SweepMax > 0;
    DebugOn = (LogLevels.Value & LogLevel.Debug) != LogLevel.None;
    Version++;
    if (oldAnywhere != AnywhereLevel)
    {
      BiomeFree.Sweep();
    }
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
