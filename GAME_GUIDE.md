# Beatbreak - Game Guide

A mouse-based rhythm game where players hit approaching notes in sync with music.

---

## Quick Start

1. Create sprites (white squares)
2. Create prefabs (HitObject, CheckpointMarker)
3. Build the Unity scene (see hierarchy below)
4. Create difficulty and level assets
5. Connect references in GameplayManager
6. Press Play!

---

## Unity Scene Hierarchy

```
Main Camera (Orthographic, Size: 10, Position: 0,0,-10)

PlayerZone
├─ Sprite Renderer (semi-transparent white)
├─ Box Collider 2D
└─ PlayerZone script

HitObjectContainer (empty GameObject)

CursorController
└─ CursorController script

NoteSpawner
└─ NoteSpawner script

PracticeManager
└─ PracticeManager script

MusicPlayer
└─ AudioSource (Play On Awake: OFF, Loop: OFF)

GameplayManager ⭐ MAIN - Connect ALL references here!
└─ GameplayManager script

GameplayCanvas
├─ GameplayUI (script here - connect all child UI elements)
├─ ScoreText (Top-Left)
├─ ComboText (Center, initially disabled)
├─ AccuracyText (Top-Right)
├─ HitResultText (Center, initially disabled)
├─ ProgressBar (Slider at top)
│   ├─ Fill Image
│   └─ CheckpointContainer (Panel, no Image)
├─ PracticeModeText (Top-Center, yellow)
├─ PracticeControlsPanel (Bottom-Right with controls text)
└─ PauseMenuPanel (Full screen, initially disabled)

ResultsCanvas (Initially DISABLED)
└─ ResultsScreenUI (script here)
    ├─ Score/Accuracy/Combo texts
    └─ Retry/Menu buttons
```

---

## Required Prefabs

### HitObject Prefab (`Assets/Prefabs/`)
- Sprite Renderer (white square)
- Box Collider 2D
- HitObject script (assign SpriteRenderer and BoxCollider)
  - Normal Color: White
  - Hover Color: Yellow
  - Hit Color: Green
  - Miss Color: Red

### CheckpointMarker Prefab (`Assets/Prefabs/`)
- UI Image (yellow, size 4x30)

---

## Creating Assets

### Difficulty Settings
Right-click in `Assets/Difficulties/` → Create → Beatbreak → Difficulty Settings

**Recommended Presets:**
- **Easy**: Speed 0.75x, Perfect 0.08s, Good 0.15s
- **Normal**: Speed 1.0x, Perfect 0.05s, Good 0.10s
- **Hard**: Speed 1.5x, Perfect 0.04s, Good 0.08s
- **Expert**: Speed 2.0x, Perfect 0.03s, Good 0.06s

### Level Data
Right-click in `Assets/Levels/` → Create → Beatbreak → Level Data

Set:
- Song (AudioClip from `Assets/Music/`)
- BPM (beats per minute of the song)
- Default Approach Time: 1.0
- Hit Objects (see below)

---

## Creating Levels

### Using LevelEditorHelper (Recommended)
1. Attach `LevelEditorHelper` to any GameObject (e.g., PlayerZone)
2. Assign your Level Data asset
3. Set Current Beat (when to hit the note)
4. Set Approach Angle (where it comes from)
5. Right-click script → "Add Hit Object at Current Beat"
6. Preview appears in Scene view

### Manual Entry
Expand Hit Objects list in Level Data, add entries with:
- **Beat Time**: When to hit (in beats)
- **Approach Angle**: Direction (0°=Right, 90°=Up, 180°=Left, 270°=Down)
- **Speed Multiplier**: 1.0 (default)
- **Size Multiplier**: 1.0 (default)

### Beat Calculations (120 BPM example)
```
Beat 0 = 0.00s    Beat 4 = 2.00s    Beat 8 = 4.00s
Beat 1 = 0.50s    Beat 5 = 2.50s    Beat 16 = 8.00s
Beat 2 = 1.00s    Beat 6 = 3.00s

Formula: seconds = beats × (60 / BPM)
```

### Simple Test Pattern (120 BPM)
```
Beat 0, Angle 0° (Right)      Beat 8, Angle 45° (Up-Right)
Beat 1, Angle 90° (Up)        Beat 9, Angle 135° (Up-Left)
Beat 2, Angle 180° (Left)     Beat 10, Angle 225° (Down-Left)
Beat 3, Angle 270° (Down)     Beat 11, Angle 315° (Down-Right)
Beat 4, Angle 0° (Right)      Beat 12, Angle 45° (Up-Right)
Beat 5, Angle 90° (Up)        Beat 13, Angle 135° (Up-Left)
Beat 6, Angle 180° (Left)     Beat 14, Angle 225° (Down-Left)
Beat 7, Angle 270° (Down)     Beat 15, Angle 315° (Down-Right)
```

---

## Gameplay Controls

| Action | Key/Input |
|--------|-----------|
| Move Cursor | Mouse |
| Hit Note | Left Click |
| Pause | ESC |
| Create Checkpoint | Z |
| Remove Checkpoint | X |
| Previous Checkpoint | [ |
| Next Checkpoint | ] |

---

## GameplayManager Setup (CRITICAL!)

This is the main component - must assign ALL references:

**Level Settings:**
- Level Data → your Level asset
- Difficulty Settings → your Difficulty asset

**References:**
- Audio Source → MusicPlayer
- Note Spawner → NoteSpawner
- Player Zone → PlayerZone
- Cursor Controller → CursorController
- Practice Manager → PracticeManager
- Gameplay UI → GameplayUI (on Canvas)
- Results Screen → ResultsScreenUI (on ResultsCanvas)

**Settings:**
- Pause Key: Escape

---

## UI Setup Details

### GameplayUI Script (on Canvas/GameplayUI)
Assign all child text elements:
- Score Text, Combo Text, Accuracy Text, Hit Result Text
- Progress Bar (ProgressBarUI component)
- Practice Mode Text, Practice Controls Panel, Pause Menu Panel

### ProgressBarUI Script (on ProgressBar)
- Fill Image → Drag the Fill child
- Checkpoint Container → Drag CheckpointContainer child
- Checkpoint Marker Prefab → Drag your prefab

### ResultsScreenUI Script (on ResultsCanvas)
- Assign all score text fields
- Assign Retry and Menu buttons

---

## Troubleshooting

**Notes don't appear:**
- Check HitObject prefab assigned to NoteSpawner
- Verify Camera is Orthographic
- Check spawn distance (default: 10)

**Can't hit notes:**
- Ensure Box Collider 2D is on HitObject prefab
- Verify PlayerZone has collider

**Audio out of sync:**
- Adjust "Song Offset" in Level Data
- Positive offset = delay start
- Negative offset = start early

**Hit timing feels off:**
- Adjust Perfect/Good windows in Difficulty Settings
- Enable "Scale Hit Windows With Speed" for consistency

**TextMeshPro missing:**
- Window → TextMeshPro → Import TMP Essential Resources

---

## Code Architecture

### Core Systems
- **TimingSystem**: BPM-based timing using `AudioSettings.dspTime`
- **ScoringSystem**: Hit evaluation (Perfect/Good/Miss) and score tracking
- **NoteSpawner**: Spawns notes based on beat timing
- **PracticeManager**: Checkpoint system for practice mode

### Data (ScriptableObjects)
- **LevelData**: Song, BPM, hit objects
- **DifficultySettings**: Speed multiplier, hit windows
- **HitObjectData**: Individual note properties

### Gameplay Components
- **HitObject**: Note movement and hit detection
- **PlayerZone**: Center hit area (boss mechanics ready via `SetZoneSize()`)
- **CursorController**: Mouse tracking and hover detection
- **GameplayManager**: Main orchestrator

---

## Pattern Ideas

**Cardinal Cross:**
0°, 90°, 180°, 270° (one beat apart)

**Diagonal X:**
45°, 135°, 225°, 315° (one beat apart)

**Circle (8 notes):**
0°, 45°, 90°, 135°, 180°, 225°, 270°, 315° (0.5 beats apart)

**Alternating:**
0°, 180°, 0°, 180° (0.5 beats apart)

**Spiral:**
0°, 90°, 180°, 270° (0.25 beats apart)

---

## Extending the Game

### Add Visual Effects
Subscribe to `ScoringSystem.OnHit` event for particles/screen shake

### Create New Note Types
Inherit from `HitObject` and override methods

### Boss Mechanics
Use `PlayerZone.SetZoneSize(newSize)` to dynamically change play area

### Custom Scoring
Modify constants in `ScoringSystem` (PERFECT_SCORE, GOOD_SCORE, etc.)

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Data/           (ScriptableObjects)
│   ├── Core/           (Timing, Scoring)
│   ├── Gameplay/       (HitObject, Spawner, PlayerZone, Cursor)
│   ├── Practice/       (PracticeManager)
│   ├── UI/             (UI components)
│   ├── Managers/       (GameplayManager)
│   ├── Audio/          (AudioManager)
│   └── Utilities/      (Helpers)
├── Prefabs/
├── Music/
├── Levels/
├── Difficulties/
└── Scenes/
```

---

## Common Angles Reference

| Direction | Angle |
|-----------|-------|
| Right     | 0°    |
| Up-Right  | 45°   |
| Up        | 90°   |
| Up-Left   | 135°  |
| Left      | 180°  |
| Down-Left | 225°  |
| Down      | 270°  |
| Down-Right| 315°  |

---

## Tips

- Start with 120 BPM songs (easy math)
- Use Practice Mode (Z key) when testing
- Place notes on whole beats for easy levels
- Use half beats (0.5) for medium difficulty
- Use quarter beats (0.25) for hard sections
- Test with a simple 10-20 note level first
- Adjust song offset if timing feels consistently off
- Enable Practice Mode initially for easier testing

---

That's it! Build the scene, assign references, and press Play. 🎮
