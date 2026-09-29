using System;
using System.Collections.Generic;

namespace VoxDetroit.Events
{
    public enum CityEventType
    {
        MusicFestival,
        Concert,
        CulturalFestival,
        PrideFestival,
        FoodFestival,
        StreetMarket,
        Parade,
        Fireworks,
        Motorsport,
        CommunityEvent,
        WatchParty,
        NightlifeEvent
    }

    public enum CrowdTier
    {
        Sparse,
        Light,
        Busy,
        Packed,
        Massive
    }

    [Serializable]
    public sealed class EventDemandModifiers
    {
        public int trafficPercent = 100;
        public int taxiDemandPercent = 100;
        public int deliveryDemandPercent = 100;
        public int foodDemandPercent = 100;
        public int retailDemandPercent = 100;
        public int securityDemandPercent = 100;
        public int hotelDemandPercent = 100;
        public int undergroundOpportunityPercent = 100;
    }

    [Serializable]
    public sealed class EventVenueDefinition
    {
        public string id;
        public string displayName;
        public string districtId;
        public int crowdCapacityHint;
        public bool outdoor;
        public int stageCount;
        public List<string> tags = new List<string>();
    }

    [Serializable]
    public sealed class ScheduledCityEvent
    {
        public string id;
        public string displayName;
        public CityEventType type;
        public string venueId;
        public long startMinute;
        public long endMinute;
        public int expectedAttendance;
        public EventDemandModifiers demand =
            new EventDemandModifiers();
        public List<string> performerIds =
            new List<string>();
        public List<string> activityTags =
            new List<string>();
        public bool cancelled;
        public bool playerAttended;
    }

    [Serializable]
    public sealed class CityEventWorldState
    {
        public List<ScheduledCityEvent> events =
            new List<ScheduledCityEvent>();
    }
}
