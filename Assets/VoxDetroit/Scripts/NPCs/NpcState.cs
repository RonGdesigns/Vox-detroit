using System;
using System.Collections.Generic;

namespace VoxDetroit.NPCs
{
    [Serializable]
    public sealed class NpcScheduleEntry
    {
        public int dayOfWeek;
        public int startMinute;
        public int endMinute;
        public string locationId;
        public string activity;
    }

    [Serializable]
    public sealed class NpcRecord
    {
        public string id;
        public string displayName;
        public string homeLocationId;
        public string jobId;
        public int relationshipToPlayer;
        public List<NpcScheduleEntry> schedule =
            new List<NpcScheduleEntry>();
        public List<string> tags = new List<string>();
    }

    [Serializable]
    public sealed class NpcWorldState
    {
        public List<NpcRecord> npcs = new List<NpcRecord>();
    }
}
