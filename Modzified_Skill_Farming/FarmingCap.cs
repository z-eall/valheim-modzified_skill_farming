using System;
using System.Reflection;
using UnityEngine;

namespace Modzified_Skill_Farming;

internal static class FarmingCap
{
  internal const float Fallback = 100f;

  private static Func<Skills.SkillType, float>? _getCap;
  private static bool _probed;
  private static float _cached = Fallback;
  private static float _readAt = -10f;

  internal static float Value
  {
    get
    {
      float now = Time.unscaledTime;
      if (now - _readAt < 1f)
      {
        return _cached;
      }

      _readAt = now;
      _cached = Read();
      return _cached;
    }
  }

  internal static void Probe()
  {
    if (_probed)
    {
      return;
    }

    _probed = true;
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      Type? type = assembly.GetType("SkillLimitExtender.SkillConfigManager");
      if (type == null)
      {
        continue;
      }

      Type[] args = { typeof(Skills.SkillType) };
      const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
      MethodInfo? method = type.GetMethod("GetCap", flags, null, args, null)
                           ?? type.GetMethod("GetSkillLimit", flags, null, args, null);
      if (method != null)
      {
        _getCap = Bind(method);
      }

      break;
    }
  }

  private static Func<Skills.SkillType, float>? Bind(MethodInfo method)
  {
    try
    {
      if (method.ReturnType == typeof(float))
      {
        return (Func<Skills.SkillType, float>)Delegate.CreateDelegate(typeof(Func<Skills.SkillType, float>), method);
      }

      if (method.ReturnType == typeof(int))
      {
        var asInt = (Func<Skills.SkillType, int>)Delegate.CreateDelegate(typeof(Func<Skills.SkillType, int>), method);
        return skill => asInt(skill);
      }
    }
    catch (Exception ex)
    {
      SkillFarmingPlugin.LogAt(BepInEx.Logging.LogLevel.Warning,
        $"{SkillFarmingPlugin.ModName}: could not bind the live skill cap ({ex.GetType().Name}). Using {Fallback}.");
    }

    return null;
  }

  private static float Read()
  {
    Probe();
    if (_getCap == null)
    {
      return Fallback;
    }

    try
    {
      float cap = _getCap(Skills.SkillType.Farming);
      return cap > 0f ? cap : Fallback;
    }
    catch (Exception)
    {
      return Fallback;
    }
  }
}
