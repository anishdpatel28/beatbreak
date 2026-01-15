# Beatbreak - Development Guide

Complete reference for setting up and developing the rhythm game.

---

## Overview

Beatbreak is a mouse-based rhythm game with BPM-synchronized note spawning, practice mode with checkpoints, and a modular scoring system. All levels are data-driven using ScriptableObjects for easy version control and modification.

---

## Scene Setup

### Required GameObjects

**Main Camera**
- Projection: Orthographic
- Size: 10
- Position: (0, 0, -10)

**PlayerZone**
- Sprite Renderer (semi-transparent)
- Box Collider 2D
- PlayerZone component

**HitObjectContainer**
- Empty GameObject (notes spawn as children)

**CursorController**
- CursorController component

**NoteSpawner**
- NoteSpawner component
- Requires HitObject prefab

**PracticeManager**
- PracticeManager component

**MusicPlayer**
- AudioSource component
- Play On Awake: disabled
- Loop: disabled

**GameplayManager**
- GameplayManager component
- Central orchestrator - all system references connect here

### UI Hierarchy

**GameplayCanvas**
- Canvas (Screen Space - Overlay)
- Canvas Scaler (Scale With Screen Size: 1920x1080)

**UI Components:**
- GameplayUI (main controller)
- ScoreText (Top-Left anchor)
- ComboText (Center anchor, disabled by default)
- AccuracyText (Top-Right anchor)
- HitResultText (Center anchor, disabled by default)
- ProgressBar (Slider, top stretch anchor)
  - Fill Image
  - CheckpointContainer (Panel, stretch anchors)
- PracticeModeText (Top-Center anchor)
- PracticeControlsPanel (Bottom-Right anchor)
- PauseMenuPanel (Stretch all, disabled by default)

**ResultsCanvas**
- Separate canvas, disabled by default
- ResultsScreenUI component
- Score statistics displays
- Retry and Menu buttons

---

## Prefabs

### HitObject
- Sprite Renderer
- Box Collider 2D
- HitObject component
  - Color settings: normal, hover, hit, miss

### CheckpointMarker
- UI Image (vertical line)
- Size: 4x30
- Color: Yellow

---

## Asset Creation

### Difficulty Settings
`Create → Beatbreak → Difficulty Settings`

**Properties:**
- Speed Multiplier (0.25 - 3.0)
- Perfect Window (seconds)
- Good Window (seconds)
- Scale Hit Windows With Speed (bool)

**Preset Examples:**

| Difficulty | Speed | Perfect | Good |
|------------|-------|---------|------|
| Easy       | 0.75x | 0.08s   | 0.15s |
| Normal     | 1.0x  | 0.05s   | 0.10s |
| Hard       | 1.5x  | 0.04s   | 0.08s |
| Expert     | 2.0x  | 0.03s   | 0.06s |

### Level Data
`Create → Beatbreak → Level Data`

**Properties:**
- Song (AudioClip)
- BPM (beats per minute)
- Song Offset (sync adjustment)
- Default Approach Time (note travel duration)
- Hit Objects (list)

### Hit Object Definition
- Beat Time (when to hit)
- Approach Angle (0-360°)
- Speed Multiplier (per-note modifier)
- Size Multiplier (per-note scaling)
- Position Offset (optional adjustment)

---

## Level Creation

### LevelEditorHelper Workflow
1. Attach component to any GameObject
2. Assign Level Data asset
3. Configure Current Beat and Approach Angle
4. Context menu: "Add Hit Object at Current Beat"
5. Visual preview appears in Scene view

### Beat Time Conversion
```
seconds = beats × (60 / BPM)

Example at 120 BPM:
Beat 0 = 0.00s    Beat 8 = 4.00s
Beat 1 = 0.50s    Beat 16 = 8.00s
Beat 4 = 2.00s    Beat 32 = 16.00s
```

### Angle Reference

| Direction | Degrees |
|-----------|---------|
| Right     | 0°      |
| Up-Right  | 45°     |
| Up        | 90°     |
| Up-Left   | 135°    |
| Left      | 180°    |
| Down-Left | 225°    |
| Down      | 270°    |
| Down-Right| 315°    |

### Pattern Examples

**Cardinal Cross**
```
Beat N+0: 0°, Beat N+1: 90°, Beat N+2: 180°, Beat N+3: 270°
```

**Diagonal Pattern**
```
Beat N+0: 45°, Beat N+1: 135°, Beat N+2: 225°, Beat N+3: 315°
```

**Circle (8 notes)**
```
0°, 45°, 90°, 135°, 180°, 225°, 270°, 315° at 0.5 beat intervals
```

**Alternating**
```
0°, 180°, 0°, 180° at 0.5 beat intervals
```

---

## Component Reference

### TimingSystem
- BPM-based timing using `AudioSettings.dspTime`
- Handles beat/second conversions
- Supports speed multipliers
- Checkpoint offset support

**Key Methods:**
- `Initialize(LevelData, DifficultySettings)`
- `StartTiming()` / `StartTimingFromOffset(float)`
- `BeatsToSeconds(float)` / `SecondsToBeats(float)`

**Properties:**
- `CurrentSongTime` (seconds)
- `CurrentBeat` (calculated from song time)
- `BeatDuration` (seconds per beat)

### ScoringSystem
- Hit evaluation (Perfect/Good/Miss)
- Score and combo tracking
- Accuracy calculation

**Key Methods:**
- `Initialize(DifficultySettings)`
- `EvaluateHit(float timingError)` → HitResult
- `RegisterHit(HitResult)`
- `GetScoreData()` → ScoreData

**Events:**
- `OnHit(HitResult, int score)`
- `OnComboChanged(int combo)`
- `OnScoreChanged(int score)`

### NoteSpawner
- Spawns notes based on timing
- Manages active note pool
- Checkpoint support

**Key Methods:**
- `Initialize(LevelData, TimingSystem)`
- `UpdateSpawner()` (call per frame)
- `ClearAllHitObjects()`
- `ResetToIndex(int)` (for checkpoints)

### GameplayManager
Main orchestrator connecting all systems.

**Required References:**
- Level Data, Difficulty Settings
- Audio Source, Note Spawner
- Player Zone, Cursor Controller
- Practice Manager, Gameplay UI, Results Screen

**Key Methods:**
- `StartLevel(LevelData, DifficultySettings)`
- `RestartLevel()`
- `TogglePause()`

### PracticeManager
Checkpoint system for practice mode.

**Key Methods:**
- `CreateCheckpoint()` (Z key)
- `RemoveLatestCheckpoint()` (X key)
- `GoToPreviousCheckpoint()` / `GoToNextCheckpoint()` ([ / ] keys)
- `RestartFromCheckpoint(CheckpointData)`

**Events:**
- `OnCheckpointCreated(CheckpointData)`
- `OnCheckpointRemoved(int index)`
- `OnCheckpointActivated(CheckpointData)`

---

## Configuration

### GameplayManager Inspector
Critical component - all references must be assigned:

**Level Settings:**
- Level Data (ScriptableObject)
- Difficulty Settings (ScriptableObject)

**System References:**
- Audio Source → MusicPlayer
- Note Spawner → NoteSpawner
- Player Zone → PlayerZone
- Cursor Controller → CursorController
- Practice Manager → PracticeManager
- Gameplay UI → GameplayUI
- Results Screen → ResultsScreenUI

### UI Component Setup

**GameplayUI:**
- Assign all text elements (Score, Combo, Accuracy, Hit Result)
- Progress Bar component reference
- Practice mode UI elements
- Pause menu panel

**ProgressBarUI:**
- Fill Image (Slider's Fill child)
- Checkpoint Container (Panel)
- Checkpoint Marker Prefab

**ResultsScreenUI:**
- All score text fields
- Button references (Retry, Menu)

---

## Controls

| Input | Action |
|-------|--------|
| Mouse Movement | Cursor control |
| Left Click | Hit note (when in player zone) |
| ESC | Pause/Unpause |
| Z | Create checkpoint (practice mode) |
| X | Remove latest checkpoint |
| [ | Previous checkpoint |
| ] | Next checkpoint |

---

## Troubleshooting

### Notes Not Appearing
- Verify HitObject prefab assigned in NoteSpawner
- Check Camera projection (must be Orthographic)
- Confirm spawn distance is within camera view

### Hit Detection Issues
- Ensure Box Collider 2D exists on HitObject
- Verify PlayerZone has collider component
- Check cursor layer mask settings

### Audio Synchronization
- Adjust Song Offset in Level Data
  - Positive values delay the start
  - Negative values advance the start
- Use practice mode to identify timing drift

### Hit Window Feel
- Modify Perfect/Good windows in Difficulty Settings
- Enable "Scale Hit Windows With Speed" for consistent timing across speeds

### Missing TextMeshPro
- Import via Window → TextMeshPro → Import TMP Essential Resources

---

## Architecture

### Data Flow
```
LevelData + DifficultySettings
        ↓
   GameplayManager
        ↓
   ┌────┴────┬────────┬──────────┐
   ↓         ↓        ↓          ↓
Timing   Scoring   Spawner   Practice
   ↓         ↓        ↓          ↓
   └─────────┴────────┴──────────┘
              ↓
         GameplayUI
```

### Event System
- Scoring events trigger UI updates
- Practice mode events update progress bar
- Hit detection uses direct component communication
- Manager orchestrates system lifecycle

### Extension Points

**Visual Effects:**
Subscribe to `ScoringSystem.OnHit` for particle systems or screen effects

**New Note Types:**
Inherit from `HitObject` and override movement/detection methods

**Boss Mechanics:**
Use `PlayerZone.SetZoneSize(Vector2)` for dynamic play area

**Custom Scoring:**
Modify score constants in `ScoringSystem` or extend evaluation logic

**Audio Effects:**
Extend `AudioManager` for hit sounds or music filters

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Data/           # ScriptableObject definitions
│   ├── Core/           # Timing, scoring, hit results
│   ├── Gameplay/       # HitObject, spawner, player zone, cursor
│   ├── Practice/       # Checkpoint system
│   ├── UI/             # Gameplay UI, progress bar, results
│   ├── Managers/       # Main game orchestration
│   ├── Audio/          # Audio control and management
│   └── Utilities/      # Level editor helper, beat calculator
├── Prefabs/            # Reusable GameObjects
├── Music/              # Audio files
├── Levels/             # Level ScriptableObjects
├── Difficulties/       # Difficulty ScriptableObjects
└── Scenes/             # Unity scenes
```

---

## Development Workflow

1. Import audio to Music folder
2. Create Difficulty preset(s)
3. Create Level Data with BPM
4. Use LevelEditorHelper to place notes
5. Test with simple patterns first
6. Adjust song offset if needed
7. Iterate on note placement
8. Create additional difficulties by duplicating settings

---

## Performance Notes

- Note spawning is optimized for single-frame checks
- Active note pool is minimal (only visible notes)
- UI updates are event-driven (not per-frame)
- Audio uses streaming for large files
- Consider object pooling for production builds with many simultaneous notes
