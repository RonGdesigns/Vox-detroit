#!/usr/bin/env python3
import json
import math
import pathlib
import sys

DEFAULT_PATH = pathlib.Path(
    "Assets/VoxDetroit/Data/downtown-core-prototype.json"
)

EXPECTED_ROADS = {
    "Woodward Avenue",
    "Michigan Avenue",
    "Cadillac Square",
}

EXPECTED_BUILDINGS = {
    "Cadillac Tower",
    "One Campus Martius",
    "First National Bank Building",
}

MAX_REASONABLE_BUILDING_HEIGHT_METERS = 300.0
BOUNDS_TOLERANCE_DEGREES = 0.0002


def finite_number(value):
    return isinstance(value, (int, float)) and math.isfinite(value)


def validate_point(point, bounds, label, errors):
    lat = point.get("latitude")
    lon = point.get("longitude")

    if not finite_number(lat) or not finite_number(lon):
        errors.append(f"{label}: non-finite latitude/longitude")
        return

    if not (
        bounds["south"] - BOUNDS_TOLERANCE_DEGREES
        <= lat
        <= bounds["north"] + BOUNDS_TOLERANCE_DEGREES
    ):
        errors.append(f"{label}: latitude {lat} outside prototype bounds")

    if not (
        bounds["west"] - BOUNDS_TOLERANCE_DEGREES
        <= lon
        <= bounds["east"] + BOUNDS_TOLERANCE_DEGREES
    ):
        errors.append(f"{label}: longitude {lon} outside prototype bounds")


def validate(path):
    data = json.loads(path.read_text(encoding="utf-8"))
    errors = []
    warnings = []

    bounds = data.get("bounds") or {}
    for key in ("south", "west", "north", "east"):
        if not finite_number(bounds.get(key)):
            errors.append(f"bounds.{key} missing or invalid")

    if errors:
        return data, errors, warnings, {}

    if bounds["south"] >= bounds["north"]:
        errors.append("south bound must be less than north bound")

    if bounds["west"] >= bounds["east"]:
        errors.append("west bound must be less than east bound")

    roads = data.get("roads") or []
    buildings = data.get("buildings") or []

    if not roads:
        errors.append("dataset contains no roads")

    if not buildings:
        errors.append("dataset contains no buildings")

    ids = set()
    duplicate_ids = set()

    for index, road in enumerate(roads):
        rid = road.get("id")
        if not rid:
            errors.append(f"road[{index}] missing id")
        elif rid in ids:
            duplicate_ids.add(rid)
        else:
            ids.add(rid)

        width = road.get("widthMeters")
        if not finite_number(width) or width <= 0 or width > 60:
            errors.append(f"{rid or index}: invalid road width {width}")

        points = road.get("centerline") or []
        if len(points) < 2:
            errors.append(f"{rid or index}: road has fewer than 2 points")

        for point_index, point in enumerate(points):
            validate_point(
                point,
                bounds,
                f"{rid or index}.centerline[{point_index}]",
                errors,
            )

    max_height = 0.0
    fallback_height_buildings = []

    for index, building in enumerate(buildings):
        bid = building.get("id")
        if not bid:
            errors.append(f"building[{index}] missing id")
        elif bid in ids:
            duplicate_ids.add(bid)
        else:
            ids.add(bid)

        points = building.get("footprint") or []
        if len(points) < 3:
            errors.append(f"{bid or index}: footprint has fewer than 3 points")

        for point_index, point in enumerate(points):
            validate_point(
                point,
                bounds,
                f"{bid or index}.footprint[{point_index}]",
                errors,
            )

        height = building.get("heightMeters") or 0
        levels = building.get("levels") or 0

        if not finite_number(height) or height < 0:
            errors.append(f"{bid or index}: invalid height {height}")
        elif height > MAX_REASONABLE_BUILDING_HEIGHT_METERS:
            errors.append(
                f"{bid or index}: height {height}m exceeds safety limit"
            )
        else:
            max_height = max(max_height, float(height))

        if not isinstance(levels, int) or levels < 0 or levels > 100:
            errors.append(f"{bid or index}: invalid level count {levels}")

        if height <= 0 and levels <= 0:
            fallback_height_buildings.append(
                building.get("name") or bid or str(index)
            )

    if duplicate_ids:
        errors.append(
            "duplicate feature IDs: " + ", ".join(sorted(duplicate_ids))
        )

    named_roads = {r.get("name") for r in roads if r.get("name")}
    named_buildings = {
        b.get("name") for b in buildings if b.get("name")
    }

    missing_roads = sorted(EXPECTED_ROADS - named_roads)
    missing_buildings = sorted(EXPECTED_BUILDINGS - named_buildings)

    if missing_roads:
        warnings.append(
            "expected recognizable roads not found: "
            + ", ".join(missing_roads)
        )

    if missing_buildings:
        warnings.append(
            "expected recognizable buildings not found: "
            + ", ".join(missing_buildings)
        )

    if fallback_height_buildings:
        warnings.append(
            f"{len(fallback_height_buildings)} buildings need "
            "procedural height fallback"
        )

    summary = {
        "areaName": data.get("areaName"),
        "roads": len(roads),
        "buildings": len(buildings),
        "namedRoads": len(named_roads),
        "namedBuildings": len(named_buildings),
        "maxExplicitHeightMeters": max_height,
        "fallbackHeightBuildings": len(fallback_height_buildings),
        "sourceAttribution": data.get("sourceAttribution"),
        "sourceLicense": data.get("sourceLicense"),
    }

    if "OpenStreetMap" not in (data.get("sourceName") or ""):
        warnings.append("sourceName does not identify OpenStreetMap")

    if "OpenStreetMap" not in (data.get("sourceAttribution") or ""):
        errors.append("OpenStreetMap attribution is missing")

    if "ODbL" not in (data.get("sourceLicense") or ""):
        errors.append("ODbL source-license metadata is missing")

    return data, errors, warnings, summary


def main():
    path = pathlib.Path(sys.argv[1]) if len(sys.argv) > 1 else DEFAULT_PATH

    if not path.exists():
        print(f"ERROR: dataset not found: {path}", file=sys.stderr)
        return 2

    _, errors, warnings, summary = validate(path)

    print("Vox Detroit map-data validation")
    print(json.dumps(summary, indent=2))

    for warning in warnings:
        print(f"WARNING: {warning}")

    if errors:
        for error in errors:
            print(f"ERROR: {error}", file=sys.stderr)
        return 1

    print("Dataset validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
