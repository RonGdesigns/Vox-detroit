#!/usr/bin/env python3
import json
import math
import re
import sys
import time
import urllib.parse
import urllib.request
from datetime import datetime, timezone
from pathlib import Path

SOUTH = 42.3306
WEST = -83.0475
NORTH = 42.3323
EAST = -83.0444

ENDPOINTS = [
    "https://overpass-api.de/api/interpreter",
    "https://overpass.kumi.systems/api/interpreter",
]

ROAD_WIDTHS = {
    "motorway": 18.0,
    "motorway_link": 18.0,
    "trunk": 12.0,
    "primary": 12.0,
    "secondary": 10.0,
    "tertiary": 8.0,
    "residential": 7.0,
    "service": 5.0,
    "footway": 2.5,
    "path": 2.5,
    "pedestrian": 2.5,
}


def build_query():
    bbox = f"{SOUTH},{WEST},{NORTH},{EAST}"
    return (
        "[out:json][timeout:45];"
        "("
        f'way["highway"]({bbox});'
        f'way["building"]({bbox});'
        ");"
        "out geom tags;"
    )


def fetch_overpass():
    payload = urllib.parse.urlencode({"data": build_query()}).encode("utf-8")
    last_error = None

    for endpoint in ENDPOINTS:
        for attempt in range(3):
            request = urllib.request.Request(
                endpoint,
                data=payload,
                method="POST",
                headers={
                    "User-Agent": "VoxDetroit-GitHub-Importer/0.1",
                    "Content-Type": "application/x-www-form-urlencoded",
                    "Accept": "application/json",
                },
            )

            try:
                with urllib.request.urlopen(request, timeout=90) as response:
                    return json.loads(response.read().decode("utf-8"))
            except Exception as exc:
                last_error = exc
                if attempt < 2:
                    time.sleep(2 ** attempt)

    raise RuntimeError(f"Overpass download failed: {last_error}")


def parse_number(text):
    if not text:
        return None

    match = re.search(r"-?\d+(?:\.\d+)?", str(text))
    return float(match.group(0)) if match else None


def parse_meters(value):
    if not value:
        return None

    text = str(value).strip().lower()
    number = parse_number(text)
    if number is None:
        return None

    if "ft" in text or "feet" in text or "'" in text:
        return number * 0.3048

    return number


def road_width(tags):
    explicit = parse_meters(tags.get("width"))
    if explicit and explicit > 0:
        return round(explicit, 2)

    lanes = parse_number(tags.get("lanes"))
    highway = tags.get("highway", "")

    if lanes and lanes > 0:
        estimated = lanes * 3.4
        shoulder = 1.0 if highway in {"motorway", "trunk", "primary"} else 0.0
        return round(max(2.5, estimated + shoulder), 2)

    return ROAD_WIDTHS.get(highway, 6.0)


def building_levels(tags):
    value = parse_number(tags.get("building:levels"))
    if not value or value <= 0:
        return 0
    return max(1, int(round(value)))


def building_height(tags):
    explicit = parse_meters(tags.get("height"))
    if explicit and explicit > 0:
        return round(explicit, 2)

    levels = building_levels(tags)
    return round(levels * 3.2, 2) if levels else 0.0


def geometry_points(element):
    points = []
    for point in element.get("geometry", []):
        if "lat" not in point or "lon" not in point:
            continue
        points.append(
            {
                "latitude": float(point["lat"]),
                "longitude": float(point["lon"]),
            }
        )

    if (
        len(points) > 1
        and math.isclose(points[0]["latitude"], points[-1]["latitude"])
        and math.isclose(points[0]["longitude"], points[-1]["longitude"])
    ):
        points.pop()

    return points


def convert(source):
    roads = []
    buildings = []

    for element in sorted(
        source.get("elements", []),
        key=lambda item: (item.get("type", ""), item.get("id", 0)),
    ):
        if element.get("type") != "way":
            continue

        tags = element.get("tags", {})
        geometry = geometry_points(element)

        if len(geometry) < 2:
            continue

        feature_id = f'way/{element.get("id")}'

        if "highway" in tags:
            roads.append(
                {
                    "id": feature_id,
                    "name": tags.get("name", ""),
                    "roadClass": tags.get("highway", ""),
                    "widthMeters": road_width(tags),
                    "centerline": geometry,
                }
            )

        if "building" in tags and len(geometry) >= 3:
            buildings.append(
                {
                    "id": feature_id,
                    "name": tags.get("name", ""),
                    "buildingType": tags.get("building", ""),
                    "heightMeters": building_height(tags),
                    "levels": building_levels(tags),
                    "footprint": geometry,
                }
            )

    return {
        "schemaVersion": "1.0",
        "sourceName": "OpenStreetMap",
        "sourceAttribution": "© OpenStreetMap contributors",
        "sourceLicense": "Open Database License (ODbL) 1.0",
        "sourceUrl": "https://www.openstreetmap.org/copyright",
        "retrievedUtc": datetime.now(timezone.utc).isoformat().replace("+00:00", "Z"),
        "areaName": "Downtown Detroit Core Prototype",
        "bounds": {
            "south": SOUTH,
            "west": WEST,
            "north": NORTH,
            "east": EAST,
        },
        "roads": roads,
        "buildings": buildings,
    }


def validate(document):
    if not document["roads"]:
        raise RuntimeError("Import returned zero roads.")
    if not document["buildings"]:
        raise RuntimeError("Import returned zero buildings.")

    for road in document["roads"]:
        if len(road["centerline"]) < 2:
            raise RuntimeError(f'Invalid road geometry: {road["id"]}')

    for building in document["buildings"]:
        if len(building["footprint"]) < 3:
            raise RuntimeError(f'Invalid building geometry: {building["id"]}')


def main():
    output = (
        Path(sys.argv[1])
        if len(sys.argv) > 1
        else Path("Assets/VoxDetroit/Data/downtown-core-prototype.json")
    )

    source = fetch_overpass()
    document = convert(source)
    validate(document)

    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(
        json.dumps(document, indent=2, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    print(
        f'Wrote {len(document["roads"])} roads and '
        f'{len(document["buildings"])} buildings to {output}'
    )


if __name__ == "__main__":
    main()
