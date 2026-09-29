using System;
using System.Collections.Generic;

namespace VoxDetroit.Vehicles
{
    [Serializable]
    public sealed class VehicleRecord
    {
        public string id;
        public string modelId;
        public string ownerId;
        public string parkedLocationId;
        public int condition = 100;
        public int fuelPercent = 100;
        public long odometerMeters;
        public long marketValueCents;
        public bool insured;
    }

    [Serializable]
    public sealed class VehicleWorldState
    {
        public List<VehicleRecord> vehicles =
            new List<VehicleRecord>();
    }
}
