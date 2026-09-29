# Vox Detroit

**Working title:** a standalone voxel open-world life RPG set in a geographically recognizable Detroit.

This is **not a Minecraft mod**. The project uses an original voxel/block world system in Unity so the city, economy, jobs, NPC simulation, vehicles, property systems and story can be designed without depending on Minecraft.

## Current milestone — M0: Voxel Foundation

The first technical goal is intentionally small:

> Render a chunk of original voxel geometry, then prove that real Detroit latitude/longitude coordinates can be mapped into local game-space meters.

### Starting decisions

- Engine: Unity
- Target: PC first, with a future commercial Steam release in mind
- World scale: 1 Unity unit = 1 meter
- Starting voxel size: 0.5 meters
- Starting chunk size: 32 × 32 × 32 voxels (16m × 16m × 16m)
- Geographic anchor: Downtown Detroit
- World strategy: public GIS/map data + procedural ordinary structures + handcrafted landmarks
- Gameplay pillars: work, money, housing, property, business, NPC schedules, reputation, story and a city that can physically change

## Repository layout

```text
Assets/
  VoxDetroit/
    Scripts/
      Core/
      Voxels/
      World/
      Detroit/
      Economy/
      Jobs/
docs/
```

## Milestones

### M0 — Voxel foundation
- [x] Scale/constants
- [x] Block IDs
- [x] Chunk data storage
- [x] Negative-safe chunk coordinate math
- [x] Basic exposed-face mesh builder
- [x] Runtime chunk component
- [x] Test-world bootstrap
- [x] Detroit lat/lon → local meters conversion
- [ ] Compile in Unity
- [ ] Render first chunk in Play Mode
- [ ] Multi-chunk streaming
- [ ] Greedy meshing
- [ ] Save/load

### M1 — First Detroit block
- [ ] GIS import format
- [ ] Road geometry
- [ ] Building footprint ingestion
- [ ] Footprint rasterization
- [ ] Procedural wall/roof generation
- [ ] Generate one recognizable Downtown Detroit block

### M2 — First playable city slice
- [ ] Player controller
- [ ] Wallet/bank
- [ ] Apartment
- [ ] First interactive job
- [ ] Store
- [ ] NPC schedules
- [ ] Basic vehicle
- [ ] Opening story slice

## Art/IP rule

All code, textures, UI, audio, characters and branding should be original or properly licensed. The project can use voxel/block technology without copying Minecraft assets, interface, sounds or branding.
