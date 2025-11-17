# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WaveStory is a Unity narrative puzzle game where players manipulate waves to unlock souls and progress through dialogue-driven gameplay. Built with Unity 6000.2.10f1 using URP 2D.

## Build and Development

**Opening Project:** Requires Unity 6000.2.10f1, open via Unity Hub

**Running:** Open `Assets/Scenes/Title.unity` or `Assets/Scenes/Main.unity` and press Play

**Building:** Standard Unity build workflow (File -> Build Settings)

**Testing:** Run via Unity Test Runner window (`Assets/Scripts/Test/Test_Dialogue.cs`)

## Architecture

### Singleton Pattern
Base class `MonoSingleton<T>` in `/Assets/Scripts/Common/MonoSingleton.cs`
- `GM` - Central hub holding all game contexts (InputWave, Room, WordInventory, SoulMode, Unlock)
- `AudioManager` - BGM/SFX with fade support
- `StaticDataHolder` - Loads all ScriptableObjects from Resources/Data/
- `TitleUI` - Title screen manager

### MVP Pattern
- **Contexts (Models):** `WaveContext`, `RoomContext`, `WordInventoryContext`, `SoulModeContext`, `UnlockContext`
- **Views (UIs):** Inherit from `UIBase` with Show()/Hide() lifecycle
- **Presenters:** Subscribe to context changes via `UnityEvent`

### Key Systems

**Wave System** (`Assets/Scripts/Wave/`)
- `WaveLogic.cs` - Static wave calculations (Sin, Square, PingPong types)
- Real-time LineRenderer visualization
- Parameters: amplitude, frequency, speed (configurable via `WaveConstant` ScriptableObject)

**Dialogue System** (`Assets/Scripts/Dialogue/`)
- CSV-based dialogue files in `Resources/Dialogues/`
- `DialoguePlayer` executes commands sequentially
- `DialogueCommandFactory` uses reflection to auto-register commands via `[DialogueCommand("Name")]` attribute
- Commands: Text, Speaker, Choice, Bg, Clear, Jump, WindowVisible, End

**Room System** (`Assets/Scripts/Room/`)
- Dual sprites: RealSprite (normal) and SoulSprite (soul mode view)
- Interactable types: Always, OnlyRealMode, OnlySoulMode
- Classes: `SoulInteractable`, `ClueInteractable`, `RoomMoveInteractable`

**Word/Puzzle System** (`Assets/Scripts/Word/`)
- Players combine 2 words from inventory
- Validates against soul requirements to unlock/progress

### Data Management
All game data as ScriptableObjects in `Resources/Data/`:
- `WordData`, `SoulData`, `RoomData`, `ClueData`, `WaveConstant`
- Loaded at startup by `StaticDataHolder`

## Code Conventions

- C# only (no UnityScript)
- Private fields: `_camelCase` or `camelCase` (inconsistent)
- Properties/Methods: `PascalCase`
- UnityEvent for pub/sub
- Korean comments throughout codebase
- One class per file
- Resources-based asset loading (no Addressables)
- PlayerPrefs for simple persistence (tutorial flag: "Title_Played")

## Adding New Features

**New Dialogue Command:**
1. Create class inheriting `DialogueCommandBase`
2. Add `[DialogueCommand("CommandName")]` attribute
3. Implement `Initialize(string[])` and `Execute(IDialogueRuntime)`
4. Set `IsWaitingInput` property (auto-discovered via reflection)

**New ScriptableObject:**
1. Create class with `[CreateAssetMenu(...)]` attribute
2. Add loading logic to `StaticDataHolder.Initialize()`
3. Create instances in `Resources/Data/`

## Key Entry Points

- `Assets/Scripts/Game/GM.cs` - Central context hub
- `Assets/Scripts/Dialogue/DialoguePlayer.cs` - Dialogue engine
- `Assets/Scripts/Wave/WaveLogic.cs` - Core wave mechanic
- `Assets/Scripts/Common/MonoSingleton.cs` - Singleton base
- `Assets/Scripts/Room/RoomPresenter.cs` - MVP pattern example
