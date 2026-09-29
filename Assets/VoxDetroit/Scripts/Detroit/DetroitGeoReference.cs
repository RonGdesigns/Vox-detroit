using System;

namespace VoxDetroit.Detroit
{
    public readonly struct LocalMeters
    {
        public readonly double East;
        public readonly double North;

        public LocalMeters(double east, double north)
        {
            East = east;
            North = north;
        }

        public override string ToString()
        {
            return $"E {East:F2}m, N {North:F2}m";
        }
    }

    public static class DetroitGeoReference
    {
        // Downtown reference used as a stable local origin for the prototype.
        public const double AnchorLatitude = 42.3314;
        public const double AnchorLongitude = -83.0458;

        private const double EarthRadiusMeters = 6378137.0;
        private const double DegToRad = Math.PI / 180.0;

        public static LocalMeters ToLocalMeters(double latitude, double longitude)
        {
            double anchorLatRad = AnchorLatitude * DegToRad;
            double deltaLat = (latitude - AnchorLatitude) * DegToRad;
            double deltaLon = (longitude - AnchorLongitude) * DegToRad;

            double north = deltaLat * EarthRadiusMeters;
            double east = deltaLon * EarthRadiusMeters * Math.Cos(anchorLatRad);

            return new LocalMeters(east, north);
        }

        public static (int x, int z) ToWorldVoxel(double latitude, double longitude, double voxelSizeMeters)
        {
            if (voxelSizeMeters <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(voxelSizeMeters));
            }

            LocalMeters meters = ToLocalMeters(latitude, longitude);
            int x = (int)Math.Floor(meters.East / voxelSizeMeters);
            int z = (int)Math.Floor(meters.North / voxelSizeMeters);
            return (x, z);
        }
    }
}
