using System.Collections.Generic;
using HarmonyLib;

namespace Modzified_Skill_Farming.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
internal static class Player_PlacePiece_Patch
{
  private static void Prefix(Player __instance, Piece piece)
  {
    PlanterLevel.Pending = piece && piece.GetComponent<Plant>() ? PlanterLevel.RawLevel(__instance) : -1f;
  }

  private static void Finalizer()
  {
    PlanterLevel.Pending = -1f;
  }
}

[HarmonyPatch(typeof(Player), nameof(Player.TryPlacePiece))]
internal static class Player_TryPlacePiece_Patch
{
  private static void Postfix(Piece piece, bool __result)
  {
    PlanterLevel.JustPlacedPlant = __result && piece && piece.GetComponent<Plant>();
  }
}

[HarmonyPatch(typeof(Player), nameof(Player.GetBuildStamina))]
internal static class Player_GetBuildStamina_Patch
{
  private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    return TranspilerUtil.ReplaceSkillFactor(instructions,
      AccessTools.Method(typeof(PlantingStamina), nameof(PlantingStamina.Factor)), false, "Player.GetBuildStamina");
  }
}

[HarmonyPatch(typeof(Attack), nameof(Attack.GetAttackStamina))]
internal static class Attack_GetAttackStamina_Patch
{
  private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    return TranspilerUtil.ReplaceSkillFactor(instructions,
      AccessTools.Method(typeof(ScytheStamina), nameof(ScytheStamina.Factor)), true, "Attack.GetAttackStamina");
  }
}

[HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))]
internal static class Pickable_Interact_Patch
{
  private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    return TranspilerUtil.ReplaceSkillFactor(instructions,
      AccessTools.Method(typeof(BountifulHarvest), nameof(BountifulHarvest.Factor)), true, "Pickable.Interact");
  }
}

[HarmonyPatch(typeof(Attack), nameof(Attack.DoMeleeAttack))]
internal static class Attack_DoMeleeAttack_Patch
{
  private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    return TranspilerUtil.ReplaceLerpAfterSkillFactor(instructions,
      AccessTools.Method(typeof(WiderSweep), nameof(WiderSweep.Radius)), "Attack.DoMeleeAttack");
  }
}
