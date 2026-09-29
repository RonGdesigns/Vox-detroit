# Detroit World Pipeline

## Goal

Turn properly licensed public geospatial data into a recognizable, performant voxel city while reserving manual art time for important locations.

## Pipeline

```text
OpenStreetMap / other approved GIS sources
        ↓
Raw retrieval
        ↓
Vox Detroit intermediate JSON
        ↓
Coordinate normalization
        ↓
Road + building feature extraction
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

## M1 prototype area

The first automated import uses a deliberately small Downtown Detroit core:

- South: 42.3306
- West: -83.0475
- North: 42.3323
- East: -83.0444

This area is large enough to prove recognizable street/building geometry while remaining cheap to rasterize and inspect.

## Retrieval paths

There are two supported prototype retrieval paths:

1. **GitHub/Python:** `tools/osm/fetch_downtown.py`
2. **Unity Editor:** `Vox Detroit > Detroit OSM Importer`

The Python path is used to commit a reproducible intermediate JSON dataset before the Unity editor is available.

## Intermediate data

The runtime does not depend directly on Overpass or OSM XML/JSON.

The committed intermediate schema stores only what the prototype needs:
- road ID/name/class/estimated width/centerline,
- building ID/name/type/height/levels/footprint,
- source and license metadata,
- import bounds and retrieval timestamp.

This creates a stable boundary between changing map-source formats and gameplay/world generation code.

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

For the current OSM prototype:
- attribution: **© OpenStreetMap contributors**
- license: **Open Database License (ODbL) 1.0**
- reference: https://www.openstreetmap.org/copyright

Do not use or trace proprietary map imagery as a substitute for properly licensed source data.

## Accuracy target

“Identical” should be treated as a layered target:
- geographic layout can approach 1:1 scale,
- important landmarks can be highly recognizable,
- ordinary structures may be procedurally approximated,
- interiors exist primarily where gameplay needs them.

That keeps the city believable without making every structure a handcrafted replica.
