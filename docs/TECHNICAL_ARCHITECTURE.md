# Technical Architecture

## Engine boundary

Unity provides rendering, input, audio, physics, platform builds and editor tooling.

Vox Detroit owns:
- voxel data and meshing,
- chunk streaming,
- world persistence,
- geospatial import,
- procedural city generation,
- economy/jobs/property systems,
- NPC simulation,
- story state.

This keeps the game independent from Minecraft while avoiding the cost of writing a complete graphics/game engine.

## World scale

- 1 Unity unit = 1 meter
- Initial voxel edge = 0.5 meters
- Chunk = 32 × 32 × 32 voxels
- Physical chunk size = 16 × 16 × 16 meters
- World coordinates are expressed in voxel integers for storage and meters for rendering

The voxel size is a starting decision, not a permanent promise. Performance and art tests can justify changing it before production content locks.

## Coordinate spaces

1. **Geographic** — latitude/longitude from GIS data
2. **Local meters** — east/north offsets from a Detroit anchor
3. **World voxels** — integer voxel coordinates
4. **Chunk/local voxels** — chunk coordinate plus index inside the chunk
5. **Unity world space** — meters used for rendering/physics

Conversion should happen at explicit boundaries. Gameplay code should not carry latitude/longitude everywhere.

## Chunk data

A chunk currently stores one block ID per voxel. This is intentionally simple for M0.

Future optimization candidates:
- palette compression,
- run-length encoding,
- sparse storage for empty chunks,
- region files,
- background serialization,
- dirty-chunk tracking.

Do not optimize storage before profiling a representative city slice.

## Meshing

M0 uses basic exposed-face meshing so correctness is easy to verify.

Next steps:
1. neighbor-aware face culling across chunk boundaries,
2. greedy meshing,
3. material/submesh batching,
4. asynchronous mesh generation,
5. LOD/impostor strategy for distant city geometry.

## Streaming

Target architecture:
- a world service owns chunk data,
- a streamer decides which chunks should be resident,
- generation/import runs off the main thread where safe,
- Unity object creation and Mesh assignment happen on the main thread,
- simulation distance is independent from render distance.

## Simulation tiers

Not every NPC needs the same cost.

- **Tier A:** nearby/important NPCs — full movement and interaction
- **Tier B:** neighborhood NPCs — schedule/state simulation without full physics
- **Tier C:** distant population — aggregate simulation/events

The same idea can later apply to traffic and businesses.

## Persistence

Save files should ultimately store changes relative to generated/imported base data rather than serializing the entire city every save.

Candidate layers:
- base city seed/version,
- player state,
- story flags,
- changed voxels/chunks,
- property/business state,
- persistent NPC state.

## Current source folders

```text
Assets/VoxDetroit/Scripts/
  Core/       constants and shared primitives
  Voxels/     block IDs, chunk data and meshing
  World/      coordinates, chunk views and streaming
  Detroit/    geographic conversion/import pipeline
  Economy/    money and business systems
  Jobs/       interactive career systems
```

## Immediate technical risk list

1. 0.5m voxels can create high geometry/memory density.
2. A 1:1 metro-scale map cannot stay fully loaded.
3. Imported GIS data will be noisy/incomplete.
4. Roads and buildings need stylization rules, not literal raw-data extrusion.
5. Vehicles need smooth collision over a voxel-authored world.
6. Save data must survive generator revisions.

M0 and M1 exist to answer these risks before large content production.
