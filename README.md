# Simple Monster RPG (Unity)

This repository contains the source assets and scripts for a lightweight, single-player, monster-collecting RPG inspired by classic handheld titles. The project targets **Unity 2021.3+** and builds for **Windows x64**.

## Getting Started
1. Open Unity Hub and choose **Open**. Select this repository folder.
2. Once Unity finishes importing, open the `Title` scene located in `Assets/Scenes` (create it if it does not yet exist).
3. In the Unity Editor top menu select **Generate > Monsters & Moves**. This deterministic tool will:
   - Create 500 monster `ScriptableObject` assets with stats, learnsets, evolutions and sprites.
   - Create elemental move assets.
   - Generate placeholder pixel-art sprites and simple WAV audio stubs.
4. Wire the provided prefabs/UI to match your desired layout. Sample scene notes are listed in `Assets/Scenes/README.txt`.

## Gameplay Overview
- **Overworld:** A 2D top-down map (town, route, cave). Use `OverworldController` for player movement and wild encounter triggers. Place `NPC` objects for dialog and trainer battles.
- **Battles:** The `BattleSystem` script orchestrates turn-based combat featuring elemental types, status effects, damage calculations, capture attempts, XP gain and evolution checks. Connect UI widgets via `BattleUI`.
- **Inventory & Shop:** The `Inventory` class tracks capture items and potions. `ShopController` provides a minimal purchasing interface.
- **Quest:** Track progress toward defeating the gym leader using `QuestManager`.
- **Pause Menu:** `PauseMenuController` displays the Monster-Dex (search/filter), party management and save/load buttons.
- **Saving:** `SaveSystem` serializes game state to JSON within `Application.persistentDataPath`.

## Key Scripts
- `Assets/Scripts/Data/MonsterSpeciesData.cs` — Species definitions (types, stats, learnsets, evolution).
- `Assets/Scripts/Data/MoveData.cs` — Move definitions with type, power and status effects.
- `Assets/Scripts/Data/MonsterInstance.cs` — Runtime monster state (HP, XP, level, status).
- `Assets/Scripts/Data/MonsterDatabase.cs` — Loads all generated monsters/moves from `Resources`.
- `Assets/Scripts/Data/TypeChart.cs` — Elemental effectiveness table.
- `Assets/Scripts/Battle/BattleSystem.cs` & `Assets/Scripts/UI/BattleUI.cs` — Turn-based battle flow and UI bindings.
- `Assets/Scripts/Overworld/OverworldController.cs` & `Assets/Scripts/Overworld/NPC.cs` — Overworld movement, NPC interactions and encounter initiation.
- `Assets/Scripts/System/Party.cs`, `Inventory.cs`, `EncounterTable.cs`, `EncounterContext.cs`, `SaveSystem.cs`, `QuestManager.cs` — Core systems for party/inventory/save/quests.
- `Assets/Scripts/UI/PauseMenuController.cs`, `ShopController.cs`, `TitleScreenController.cs` — UI controllers for menus, shop and title screen.
- `Assets/Editor/MonsterContentGenerator.cs` — Editor-only tool that builds monsters, moves, sprites and audio.

## Customisation
Adjust the generator behaviour inside `Assets/Editor/MonsterContentGenerator.cs`:
- **Type table:** Extend or tweak elemental relationships within `Assets/Scripts/Data/TypeChart.cs`.
- **Name fragments:** Update the `prefixes` and `suffixes` arrays.
- **Stat ranges:** Modify the `rng.Next` ranges when writing stats.
- **Sprite palettes:** Edit `RandomColor` or the `CreateTexture` method for different looks.

## Building for Windows x64
1. After configuring scenes and prefabs, open **File > Build Settings**.
2. Add the `Title`, `Overworld` and `Battle` scenes to the build list.
3. Select **PC, Mac & Linux Standalone**, set target platform to **Windows**, architecture **x86_64**.
4. Click **Build** (or **Build & Run**) to produce the executable.

## Audio & Art
Generated art lands in `Assets/Art/Monsters`. Audio stubs appear in `Assets/Audio`. Replace them with your own assets as desired.

## Saving & Data Files
Save data is written to `Application.persistentDataPath/savegame.json`. Delete the file to reset progress.
