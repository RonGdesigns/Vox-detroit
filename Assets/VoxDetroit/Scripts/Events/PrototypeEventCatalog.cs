using System.Collections.Generic;
using VoxDetroit.Core;

namespace VoxDetroit.Events
{
    public static class PrototypeVenueCatalog
    {
        public static List<EventVenueDefinition> Create()
        {
            return new List<EventVenueDefinition>
            {
                new EventVenueDefinition
                {
                    id = "venue.hart-plaza",
                    displayName = "Hart Plaza",
                    districtId = "district.downtown",
                    crowdCapacityHint = 40000,
                    outdoor = true,
                    stageCount = 3,
                    tags = new List<string>
                    {
                        "riverfront",
                        "festival",
                        "large-crowd",
                        "outdoor"
                    }
                },
                new EventVenueDefinition
                {
                    id = "venue.campus-martius",
                    displayName = "Campus Martius",
                    districtId = "district.downtown",
                    crowdCapacityHint = 8000,
                    outdoor = true,
                    stageCount = 1,
                    tags = new List<string>
                    {
                        "downtown",
                        "public-space",
                        "seasonal"
                    }
                },
                new EventVenueDefinition
                {
                    id = "venue.eastern-market",
                    displayName = "Eastern Market",
                    districtId = "district.eastern-market",
                    crowdCapacityHint = 12000,
                    outdoor = true,
                    stageCount = 2,
                    tags = new List<string>
                    {
                        "market",
                        "food",
                        "music",
                        "community"
                    }
                }
            };
        }
    }

    public static class PrototypePerformerCatalog
    {
        public static List<PerformerDefinition> Create()
        {
            return new List<PerformerDefinition>
            {
                Performer(
                    "artist.nova-ash",
                    "Nova Ash",
                    MusicGenre.RnB,
                    88,
                    "Detroit",
                    Track(
                        "track.nova.afterglow",
                        "Afterglow on Woodward",
                        MusicGenre.RnB,
                        94,
                        218),
                    Track(
                        "track.nova.midnight",
                        "Midnight River",
                        MusicGenre.Soul,
                        90,
                        241)),

                Performer(
                    "artist.static-saint",
                    "Static Saint",
                    MusicGenre.Techno,
                    93,
                    "Detroit",
                    Track(
                        "track.static.machine-light",
                        "Machine Light",
                        MusicGenre.Techno,
                        132,
                        318),
                    Track(
                        "track.static.after-hours",
                        "After Hours Grid",
                        MusicGenre.Techno,
                        136,
                        294)),

                Performer(
                    "artist.jae-meridian",
                    "Jae Meridian",
                    MusicGenre.HipHop,
                    91,
                    "Midwest",
                    Track(
                        "track.jae.cityline",
                        "Cityline",
                        MusicGenre.HipHop,
                        88,
                        205),
                    Track(
                        "track.jae.river-side",
                        "River Side",
                        MusicGenre.HipHop,
                        92,
                        224)),

                Performer(
                    "artist.blue-orbit",
                    "Blue Orbit Quartet",
                    MusicGenre.Jazz,
                    78,
                    "Detroit",
                    Track(
                        "track.blue.cadillac-nocturne",
                        "Cadillac Nocturne",
                        MusicGenre.Jazz,
                        118,
                        386)),

                Performer(
                    "artist.velvet-engine",
                    "Velvet Engine",
                    MusicGenre.Fusion,
                    84,
                    "Great Lakes",
                    Track(
                        "track.velvet.foundry",
                        "Foundry Summer",
                        MusicGenre.Fusion,
                        112,
                        267)),

                Performer(
                    "artist.sunday-signal",
                    "Sunday Signal",
                    MusicGenre.Gospel,
                    76,
                    "Detroit",
                    Track(
                        "track.sunday.open-sky",
                        "Open Sky",
                        MusicGenre.Gospel,
                        102,
                        252))
            };
        }

        private static PerformerDefinition Performer(
            string id,
            string stageName,
            MusicGenre genre,
            int fame,
            string homeRegion,
            params PerformanceTrackDefinition[] tracks)
        {
            var performer = new PerformerDefinition
            {
                id = id,
                stageName = stageName,
                primaryGenre = genre,
                fame = fame,
                homeRegion = homeRegion
            };

            performer.tracks.AddRange(tracks);
            return performer;
        }

        private static PerformanceTrackDefinition Track(
            string id,
            string title,
            MusicGenre genre,
            int bpm,
            int durationSeconds)
        {
            return new PerformanceTrackDefinition
            {
                id = id,
                title = title,
                genre = genre,
                bpm = bpm,
                durationSeconds = durationSeconds,
                audioAssetKey = $"music/{id}",
                originalForGame = true
            };
        }
    }

    public static class PrototypeCityEventCatalog
    {
        public static List<ScheduledCityEvent> Create()
        {
            long day = GameClock.MinutesPerDay;

            return new List<ScheduledCityEvent>
            {
                new ScheduledCityEvent
                {
                    id = "event.river-pulse",
                    displayName = "River Pulse Weekend",
                    type = CityEventType.MusicFestival,
                    venueId = "venue.hart-plaza",
                    startMinute =
                        (2 * day) +
                        GameClock.ToMinuteOfDay(14, 0),
                    endMinute =
                        (2 * day) +
                        GameClock.ToMinuteOfDay(23, 30),
                    expectedAttendance = 32000,
                    performerIds = new List<string>
                    {
                        "artist.static-saint",
                        "artist.jae-meridian",
                        "artist.nova-ash"
                    },
                    activityTags = new List<string>
                    {
                        "music",
                        "food-vendors",
                        "nightlife",
                        "temporary-jobs"
                    },
                    demand = FestivalDemand(
                        150,
                        190,
                        135,
                        175,
                        150,
                        210,
                        155,
                        145)
                },

                new ScheduledCityEvent
                {
                    id = "event.jazz-river",
                    displayName = "Jazz on the River",
                    type = CityEventType.MusicFestival,
                    venueId = "venue.hart-plaza",
                    startMinute =
                        (6 * day) +
                        GameClock.ToMinuteOfDay(13, 0),
                    endMinute =
                        (6 * day) +
                        GameClock.ToMinuteOfDay(22, 0),
                    expectedAttendance = 18000,
                    performerIds = new List<string>
                    {
                        "artist.blue-orbit",
                        "artist.velvet-engine"
                    },
                    activityTags = new List<string>
                    {
                        "music",
                        "family",
                        "food-vendors",
                        "photography"
                    },
                    demand = FestivalDemand(
                        135,
                        160,
                        120,
                        155,
                        130,
                        175,
                        140,
                        110)
                },

                new ScheduledCityEvent
                {
                    id = "event.world-roots",
                    displayName = "World Roots Celebration",
                    type = CityEventType.CulturalFestival,
                    venueId = "venue.hart-plaza",
                    startMinute =
                        (10 * day) +
                        GameClock.ToMinuteOfDay(12, 0),
                    endMinute =
                        (10 * day) +
                        GameClock.ToMinuteOfDay(21, 30),
                    expectedAttendance = 14000,
                    performerIds = new List<string>
                    {
                        "artist.sunday-signal",
                        "artist.velvet-engine"
                    },
                    activityTags = new List<string>
                    {
                        "culture",
                        "dance",
                        "food-vendors",
                        "community"
                    },
                    demand = FestivalDemand(
                        125,
                        145,
                        120,
                        170,
                        145,
                        150,
                        125,
                        105)
                },

                new ScheduledCityEvent
                {
                    id = "event.market-night",
                    displayName = "Market Night Live",
                    type = CityEventType.NightlifeEvent,
                    venueId = "venue.eastern-market",
                    startMinute =
                        (4 * day) +
                        GameClock.ToMinuteOfDay(18, 0),
                    endMinute =
                        (4 * day) +
                        GameClock.ToMinuteOfDay(23, 59),
                    expectedAttendance = 6500,
                    performerIds = new List<string>
                    {
                        "artist.nova-ash"
                    },
                    activityTags = new List<string>
                    {
                        "nightlife",
                        "food",
                        "vendors",
                        "local-business"
                    },
                    demand = FestivalDemand(
                        120,
                        145,
                        125,
                        160,
                        150,
                        130,
                        110,
                        130)
                }
            };
        }

        private static EventDemandModifiers FestivalDemand(
            int traffic,
            int taxi,
            int delivery,
            int food,
            int retail,
            int security,
            int hotel,
            int underground)
        {
            return new EventDemandModifiers
            {
                trafficPercent = traffic,
                taxiDemandPercent = taxi,
                deliveryDemandPercent = delivery,
                foodDemandPercent = food,
                retailDemandPercent = retail,
                securityDemandPercent = security,
                hotelDemandPercent = hotel,
                undergroundOpportunityPercent = underground
            };
        }
    }
}
