using System;
using System.Collections.Generic;

namespace VoxDetroit.Events
{
    public enum MusicGenre
    {
        Techno,
        House,
        Jazz,
        HipHop,
        RnB,
        Soul,
        Rock,
        Pop,
        Gospel,
        Electronic,
        Alternative,
        World,
        Fusion
    }

    [Serializable]
    public sealed class PerformanceTrackDefinition
    {
        public string id;
        public string title;
        public MusicGenre genre;
        public int bpm;
        public int durationSeconds;
        public string audioAssetKey;
        public bool originalForGame = true;
    }

    [Serializable]
    public sealed class PerformerDefinition
    {
        public string id;
        public string stageName;
        public MusicGenre primaryGenre;
        public int fame;
        public string homeRegion;
        public List<string> traits = new List<string>();
        public List<PerformanceTrackDefinition> tracks =
            new List<PerformanceTrackDefinition>();
    }

    [Serializable]
    public sealed class PerformerWorldState
    {
        public List<string> discoveredPerformerIds =
            new List<string>();
        public List<string> seenLivePerformerIds =
            new List<string>();
    }
}
