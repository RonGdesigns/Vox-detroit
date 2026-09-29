# Roadmap

## M0 — Voxel Foundation

### Code complete
- [x] Core scale constants
- [x] Block IDs
- [x] Chunk voxel storage
- [x] Negative-safe chunk coordinate helpers
- [x] Basic exposed-face mesh builder
- [x] Runtime chunk view
- [x] Test chunk bootstrap
- [x] Detroit geographic-to-local-meter conversion
- [x] World data service keyed by chunk coordinate
- [x] Configurable horizontal multi-chunk streaming
- [x] View unloading outside render radius
- [x] Neighbor-aware chunk-boundary face culling
- [x] Deterministic prototype chunk generation

### Requires Unity/editor verification
- [ ] Create/open Unity project around this repository
- [ ] Resolve any API/version compile differences
- [ ] Enter Play Mode and render first chunk
- [ ] Move across a 3×3/5×5 streamed area
- [ ] Verify chunk seams have no duplicate/internal faces
- [ ] Confirm collider behavior
- [ ] Profile representative chunks

### Next optimization
- [ ] Greedy meshing
- [ ] Background mesh generation
- [ ] Save/load prototype
- [ ] Separate simulation and rendering distance

## M1 — First Detroit Block

### Code complete
- [x] Choose fixed Downtown prototype bounds
- [x] Record OpenStreetMap source/licensing requirements
- [x] Define intermediate road/building JSON schema
- [x] Add Overpass editor download/conversion tool
- [x] Convert latitude/longitude to local meters/voxels
- [x] Rasterize road centerlines with approximate width
- [x] Rasterize building footprints
- [x] Extrude simple building wall/roof shells
- [x] Runtime bootstrap for imported JSON

### Requires Unity/editor verification
- [ ] Run the OSM importer
- [ ] Inspect generated intermediate JSON
- [ ] Render the imported Downtown block
- [ ] Compare road/building layout against source geography
- [ ] Tune road width and building-height fallbacks
- [ ] Replace one landmark with a handcrafted version

**Exit condition:** the block is recognizable from its real-world layout.

## M2 — First Playable City Slice

- [ ] Character controller
- [ ] Interaction system
- [ ] Wallet and bank
- [ ] Apartment/home state
- [ ] One store
- [ ] One interactive job
- [ ] NPC schedules
- [ ] One driveable vehicle
- [ ] One property renovation interaction
- [ ] Short branching story sequence

**Exit condition:** 15–30 minutes of coherent play where city, work, money and story interact.

## M3 — Vertical Slice

- [ ] Several connected neighborhoods
- [ ] Multiple job families
- [ ] Property ownership
- [ ] Business prototype
- [ ] Reputation
- [ ] Traffic/crowd simulation tiers
- [ ] City-change events
- [ ] Save migration/versioning
- [ ] Art/audio identity pass

## Scope rule

Do not expand to all of Detroit until M1 proves the import pipeline and M2 proves the gameplay loop.
