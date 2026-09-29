using System;

namespace VoxDetroit.Detroit
{
    [Serializable]
    public sealed class DetroitImportDocument
    {
        public string schemaVersion = "1.0";
        public string sourceName;
        public string sourceAttribution;
        public string sourceLicense;
        public string sourceUrl;
        public string retrievedUtc;
        public string areaName;
        public GeoBounds bounds;
        public RoadFeature[] roads;
        public BuildingFeature[] buildings;
    }

    [Serializable]
    public sealed class GeoBounds
    {
        public double south;
        public double west;
        public double north;
        public double east;
    }

    [Serializable]
    public sealed class GeoPoint
    {
        public double latitude;
        public double longitude;

        public GeoPoint()
        {
        }

        public GeoPoint(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }

    [Serializable]
    public sealed class RoadFeature
    {
        public string id;
        public string name;
        public string roadClass;
        public float widthMeters;
        public GeoPoint[] centerline;
    }

    [Serializable]
    public sealed class BuildingFeature
    {
        public string id;
        public string name;
        public string buildingType;
        public float heightMeters;
        public int levels;
        public GeoPoint[] footprint;
    }
}
