# Modzified_Skill_Farming

[![Find me here](https://img.shields.io/badge/Find_me_here-Discord-5865F2?logo=discord&logoColor=white&style=flat)](https://discord.gg/VFRJcPwUdm)
[![Support](https://img.shields.io/badge/Support-Ko--fi-FF5E5B?logo=ko-fi&logoColor=white&style=flat)](https://ko-fi.com/zeall)

Makes the vanilla Farming skill matter for planting, scything and picking.

Install on all clients and the server. Server-synced settings follow the host when the server has the mod.

## Features

- Planting: Grow Chance, Growth time reduction, Plant anywhere. A plant keeps the Farming level of the player who planted it.
  - Hover a plant to see its Grow Chance, `( -X% Biome penalty )` and the time left.
- Scythe: Scythe radius expansion, Stamina reduction, and plants the vanilla Scythe skips.
- Picking and scything: Bonus yield.

## Configuration

File `BepInEx/config/modzified_skill_farming.cfg`. The effect settings are server-synced.

- Grow chance, Growth time reduction, Cultivator stamina reduction, Scythe stamina reduction, Scythe radius expansion and Bonus yield chance each have an "at skill 0" and an "at skill max" key.
  - The value moves in a straight line between the two as Farming rises. Skill max is the live skill cap.
- Plant anywhere at skill level: Farming level that allows planting in any biome. `-1` turns it off.
  - Out-of-biome grow chance penalty: % taken off the Grow Chance outside the native biome.
- Allow Scythe to harvest: prefab names, separated by commas. An empty list keeps vanilla.
  - Default: `Pickable_Mushroom_JotunPuffs, Pickable_Mushroom_Magecap, VineAsh`.

## Mod compatibility

- **Farming by Smoothbrain:** both mods change growth, biome rules, stamina and harvest. The log shows a warning when both load. Results can differ from the settings.

## Credits

Source: [<img src="https://cdn.simpleicons.org/github/181717" width="16" height="16" alt="" /> GitHub](https://github.com/z-eall/valheim-modzified_skill_farming)
