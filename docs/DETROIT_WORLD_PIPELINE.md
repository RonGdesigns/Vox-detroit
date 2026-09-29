# Detroit World Pipeline

## Goal

Turn public geospatial data into a recognizable, performant voxel city while reserving manual art time for important locations.

## Pipeline

```text
Source GIS / OpenStreetMap-style data
        ↓
Raw import cache
        ↓
Coordinate normalization
        ↓
Road + parcel/building feature extraction
        ↓
Local meter coordinates
        ↓
Voxel rasterization
        ↓
Procedural architecture rules
        ↓
Chunk data
        ↓
Unity mesh generation
        ↓
Handcrafted hero-location overrides
```

## Geographic anchor

The initial code uses a Downtown Detroit reference near 42.3314, -83.0458.

This anchor is only an origin for local coordinate math. It does not imply the first playable block must be centered exactly on that point.

## First import target

Do not import the whole city first.

M1 should select one small Downtown test area and prove:
- road centerlines align plausibly,
- road widths can be represented,
- building footprints land in the correct relative positions,
- building heights can be supplied or inferred,
- the resulting block is visually recognizable.

## Feature hierarchy

### Data-driven
- streets,
- building footprints,
- water boundary,
- rail lines where relevant,
- parks/large open areas,
- coarse elevation.

### Procedurally styled
- ordinary houses,
- garages,
- warehouses,
- low-rise commercial buildings,
- parking lots,
- fences/alleys,
- generic interiors where needed.

### Handcrafted
- major landmarks,
- story-critical businesses,
- signature interiors,
- locations where procedural output would damage recognizability.

## Procedural building inputs

A building generator may consume:
- footprint polygon,
- height or story estimate,
- land-use/building type,
- neighborhood style tag,
- frontage direction,
- construction condition,
- seed.

It should output a deterministic structure so saves do not change when a chunk is reloaded.

## Data licensing

Every imported dataset must record:
- source,
- license/attribution requirement,
- retrieval date,
- transformation steps.

No map-provider imagery or proprietary game assets should be copied into the project without appropriate rights.

## Accuracy target

“Identical” should be treated as a layered target:
- geographic layout can approach 1:1 scale,
- important landmarks can be highly recognizable,
- ordinary structures may be procedurally approximated,
- interiors exist primarily where gameplay needs them.

That keeps the city believable without making every structure a handcrafted replica.
