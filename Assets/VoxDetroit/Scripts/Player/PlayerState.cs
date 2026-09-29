using System;
using System.Collections.Generic;

namespace VoxDetroit.Player
{
    [Serializable]
    public sealed class PlayerState
    {
        public string playerId = "player";
        public string displayName = "Player";
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationY;
        public string residencePropertyId;
        public List<string> ownedVehicleIds = new List<string>();
        public List<string> ownedBusinessIds = new List<string>();
    }
}
