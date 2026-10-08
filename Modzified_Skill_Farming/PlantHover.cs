using System.Text;

namespace Modzified_Skill_Farming;

internal static class PlantHover
{
  private const string Yellow = "yellow";
  private const string Green = "green";

  private static Plant? _plant;
  private static int _second = -2;
  private static int _version = -1;
  private static Plant.Status _status;
  private static string _text = "";

  internal static string Extra(Plant plant)
  {
    if (!plant.m_nview || !plant.m_nview.IsValid())
    {
      return "";
    }

    ZDO zdo = plant.m_nview.GetZDO();
    float level = PlanterLevel.Read(zdo);
    if (level < 0f)
    {
      return "";
    }

    Plant.Status status = plant.GetStatus();
    int second = -1;
    if (status == Plant.Status.Healthy)
    {
      double left = plant.GetGrowTime() - plant.TimeSincePlanted();
      second = left > 0.0 ? (int)left : -1;
    }

    if (plant == _plant && second == _second && Settings.Version == _version && status == _status)
    {
      return _text;
    }

    _plant = plant;
    _second = second;
    _version = Settings.Version;
    _status = status;
    _text = Build(plant, zdo, level, second);
    return _text;
  }

  private static string Build(Plant plant, ZDO zdo, float level, int seconds)
  {
    bool outside = GrowChance.OutsideBiome(plant, zdo);
    float percent = GrowChance.Percent(level, outside);
    var text = new StringBuilder("\nGrow Chance: <color=");
    text.Append(percent >= 75f ? Green : Yellow).Append('>').Append(percent.ToString("0.##")).Append("%</color>");
    if (outside && Settings.Penalty > 0)
    {
      text.Append(" ( -").Append(Settings.Penalty.ToString("0.##")).Append("% Biome penalty )");
    }

    if (seconds >= 0)
    {
      text.Append("\nFully grown in ").Append(Format(seconds));
    }

    return text.ToString();
  }

  private static string Format(int seconds)
  {
    int hours = seconds / 3600;
    int minutes = seconds % 3600 / 60;
    int rest = seconds % 60;
    if (hours > 0)
    {
      return $"{hours}h {minutes}m {rest}s";
    }

    return minutes > 0 ? $"{minutes}m {rest}s" : $"{rest}s";
  }
}
