# Beatbreak

A mouse-based rhythm game built in Unity where players hit approaching notes in sync with music.

## Getting Started

1. **Clone project**
2. **Open Unity Hub** → Click **"Add"** → Select the Beatbreak folder
3. **Open the project** (first time takes 2-5 minutes to import)
4. **See `GAME_GUIDE.md`** for complete setup and level creation

## Requirements

- Unity 6000.0.x
- All packages auto-installed from `Packages/manifest.json`

## Features

- BPM-based timing system using `AudioSettings.dspTime`
- Practice mode with checkpoint system
- Multiple difficulty settings with speed multipliers
- Data-driven level design using ScriptableObjects
- Complete scoring system (Perfect/Good/Miss)
- 360° note approach directions
- Built-in level editor helper

## Controls

| Action | Input |
|--------|-------|
| Move Cursor | Mouse |
| Hit Note | Left Click |
| Pause | ESC |
| Create Checkpoint | Z |
| Remove Checkpoint | X |
| Navigate Checkpoints | [ / ] |

## Documentation

- `GAME_GUIDE.md` - Complete setup guide, level creation, and reference

## Project Structure

```
Assets/
├── Scripts/
│   ├── Data/          # ScriptableObject definitions
│   ├── Core/          # Timing and scoring systems
│   ├── Gameplay/      # Game objects and mechanics
│   ├── Practice/      # Checkpoint system
│   ├── UI/            # User interface
│   ├── Managers/      # Main game orchestration
│   ├── Audio/         # Audio management
│   └── Utilities/     # Helper tools
├── Prefabs/
├── Music/
├── Levels/
└── Difficulties/
```
