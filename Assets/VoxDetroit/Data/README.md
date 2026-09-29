# Detroit map-data workspace

This folder is the destination for converted geospatial source data used by the prototype.

## M1 prototype area

**Area:** Downtown Detroit core near Campus Martius

**Bounds:**
- South: 42.3306
- West: -83.0475
- North: 42.3323
- East: -83.0444

The area is deliberately small. It is intended to prove the pipeline before expanding to larger Detroit districts.

## OpenStreetMap source

The editor importer at:

`Assets/VoxDetroit/Editor/DetroitOsmImportWindow.cs`

queries OpenStreetMap-derived data through the Overpass API and converts roads/building footprints into Vox Detroit's intermediate JSON format.

When OSM data is used, preserve the generated source metadata and provide the required attribution in the shipped product:

**© OpenStreetMap contributors**

OpenStreetMap data is available under the **Open Database License (ODbL) 1.0**.

Reference:
https://www.openstreetmap.org/copyright

Do not use or trace proprietary map imagery as a substitute for properly licensed source data.
