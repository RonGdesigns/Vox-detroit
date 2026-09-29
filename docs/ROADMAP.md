# Roadmap

## M0 — Voxel Foundation

### Code now
- [x] Core scale constants
- [x] Block IDs
- [x] Chunk voxel storage
- [x] Negative-safe chunk coordinate helpers
- [x] Basic exposed-face mesh builder
- [x] Runtime chunk view
- [x] Test chunk bootstrap
- [x] Detroit geographic-to-local-meter conversion

### Requires Unity/editor verification
- [ ] Create/open Unity project around this repository
- [ ] Resolve any API/version compile differences
- [ ] Enter Play Mode and render first chunk
- [ ] Confirm collider behavior
- [ ] Profile one chunk

### Next engineering
- [ ] Multi-chunk world service
- [ ] Neighbor-aware meshing
- [ ] Chunk streaming radius
- [ ] Greedy meshing
- [ ] Save/load prototype

## M1 — First Detroit Block

- [ ] Choose exact Downtown test bounds
- [ ] Define import JSON schema
- [ ] Build map-data ingestion tool
- [ ] Convert lat/lon to local meters
- [ ] Rasterize road surfaces
- [ ] Rasterize building footprints
- [ ] Add simple procedural walls/roofs
- [ ] Compare generated block against source map data
- [ ] Replace one landmark with handcrafted version

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
