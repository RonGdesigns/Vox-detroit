# Downtown Prototype Data Report

The first committed Downtown Detroit map slice is now treated as a testable project asset rather than an opaque download.

## Current dataset

Area: **Downtown Detroit Core Prototype**

Bounds:
- south 42.3306
- west -83.0475
- north 42.3323
- east -83.0444

Current imported content:
- **127 road features**
- **20 building footprints**
- **8 unique named roads**
- **15 named buildings**
- **15 buildings with explicit heights**
- **14 buildings with explicit level counts**

No road has fewer than two points and no building has fewer than three footprint points in the current dataset.

## Recognizable streets present

The import currently includes:
- Woodward Avenue
- Michigan Avenue
- Fort Street West
- Monroe Street
- Griswold Street
- Cadillac Square
- Bates Street
- East Congress Street

## Recognizable buildings present

Examples include:
- Cadillac Tower — ~133.4 m / 40 levels in source data
- First National Bank Building — ~104 m / 26 levels
- 1001 Woodward — ~103 m / 25 levels
- One Kennedy Square — ~73.2 m / 10 levels
- One Campus Martius — ~51.2 m / 16 levels
- The Qube — ~44.8 m / 14 levels

## Corrections made before Unity rendering

The original prototype safety cap limited building extrusion to 192 voxels. At a 0.5 m voxel size, that would clip buildings above **96 m**.

That cap has been replaced with a bad-data safety limit of **300 m**, which preserves the real heights in this prototype while still preventing a malformed source value from creating a pathological tower.

Imported-world streaming also no longer assumes a fixed number of vertical chunks. Inside the horizontal render radius, it loads every populated vertical chunk already present in the imported world data. Tall buildings therefore remain complete without requiring the player to move upward.

## Automatic validation

`tools/osm/validate_downtown.py` now verifies:
- valid bounds,
- valid road widths,
- valid road and building geometry,
- coordinates near the selected prototype bounds,
- unique feature IDs,
- sensible building heights/levels,
- OpenStreetMap attribution,
- ODbL metadata.

The normal Domain Smoke GitHub Action runs this check after the C# domain tests.

## Known limitations

The source map gives us geometry and useful tags, not finished game art.

The first Unity render will still require:
- materials/colors that distinguish road, grass, brick, glass and concrete,
- better roof/facade rules,
- special handling for nonstandard structures such as canopies/fountains,
- landmark-specific modeling,
- terrain/elevation data,
- road markings, sidewalks, trees, lights and street furniture.

The objective of M1 is recognizable geography first, visual authenticity second.


## Overpass geometry behavior

The query bounds select features that intersect the prototype area. Overpass can return the complete geometry of an intersecting way, so some road/building points extend beyond the exact query rectangle. The validator intentionally permits this within a small Downtown safety envelope and reports the count instead of treating those points as corrupt data.
