# Beatbreak

A mouse-based rhythm game built in Unity where players hit approaching notes in sync with music.

## Getting Started

**Clone the repository:**
```bash
git clone https://github.com/anishdpatel28/beatbreak.git
```

**Open in Unity Hub:**
- Add project by selecting the cloned folder
- Unity will import all packages and regenerate project files
- First import may take a few minutes

**Setup:**
- See `GAME_GUIDE.md` for complete scene setup and level creation

## Requirements

- Unity 6000.0 or 2022.3+ LTS
- All required packages are defined in `Packages/manifest.json`

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
