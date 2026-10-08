using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Modzified_Skill_Farming.Patches;

internal static class TranspilerUtil
{
  internal static bool IsCall(CodeInstruction code, string name)
  {
    return (code.opcode == OpCodes.Call || code.opcode == OpCodes.Callvirt)
           && code.operand is MethodInfo method
           && method.Name == name;
  }

  internal static IEnumerable<CodeInstruction> ReplaceSkillFactor(
    IEnumerable<CodeInstruction> instructions, MethodInfo replacement, bool passInstance, string patchName)
  {
    var codes = new List<CodeInstruction>(instructions);
    int hits = 0;
    for (int i = 0; i < codes.Count; i++)
    {
      if (!IsCall(codes[i], nameof(Player.GetSkillFactor)))
      {
        continue;
      }

      if (passInstance)
      {
        var load = new CodeInstruction(OpCodes.Ldarg_0);
        load.MoveLabelsFrom(codes[i]);
        codes.Insert(i, load);
        i++;
      }

      codes[i] = new CodeInstruction(OpCodes.Call, replacement);
      hits++;
    }

    return Finish(codes, instructions, hits, patchName);
  }

  internal static IEnumerable<CodeInstruction> ReplaceLerpAfterSkillFactor(
    IEnumerable<CodeInstruction> instructions, MethodInfo replacement, string patchName)
  {
    var codes = new List<CodeInstruction>(instructions);
    int hits = 0;
    bool seen = false;
    for (int i = 0; i < codes.Count; i++)
    {
      if (IsCall(codes[i], nameof(Player.GetSkillFactor)))
      {
        seen = true;
        continue;
      }

      if (seen && IsCall(codes[i], "Lerp") && hits == 0)
      {
        codes[i] = new CodeInstruction(OpCodes.Call, replacement);
        hits++;
      }
    }

    return Finish(codes, instructions, hits, patchName);
  }

  internal static IEnumerable<CodeInstruction> ReplaceGetComponent(
    IEnumerable<CodeInstruction> instructions, System.Type component, MethodInfo replacement, string patchName)
  {
    var codes = new List<CodeInstruction>(instructions);
    int hits = 0;
    for (int i = 0; i < codes.Count; i++)
    {
      if (IsCall(codes[i], "GetComponent")
          && codes[i].operand is MethodInfo method
          && method.IsGenericMethod
          && method.GetGenericArguments()[0] == component)
      {
        codes[i] = new CodeInstruction(OpCodes.Call, replacement);
        hits++;
      }
    }

    return Finish(codes, instructions, hits, patchName);
  }

  private static IEnumerable<CodeInstruction> Finish(
    List<CodeInstruction> patched, IEnumerable<CodeInstruction> original, int hits, string patchName)
  {
    if (hits > 0)
    {
      return patched;
    }

    SkillFarmingPlugin.LogAt(BepInEx.Logging.LogLevel.Warning,
      $"{SkillFarmingPlugin.ModName}: could not find the patch point for {patchName}. That effect stays as in vanilla.");
    return original;
  }
}
