using System.Collections.Generic;
using VoxDetroit.Core;

namespace VoxDetroit.Sports
{
    public static class PrototypeSportsCatalog
    {
        public static List<SportsTeamDefinition> CreateTeams()
        {
            return new List<SportsTeamDefinition>
            {
                Team(
                    "team.detroit.chow-dogs",
                    "Detroit",
                    "Chow Dogs",
                    "DCD",
                    SportType.Football,
                    "A powerful chow-chow dog mascot; not a lion.",
                    "charcoal-copper-cream",
                    58),

                Team(
                    "team.detroit.cheetahs",
                    "Detroit",
                    "Cheetahs",
                    "DCH",
                    SportType.Baseball,
                    "A fast cheetah mascot with an original silhouette.",
                    "teal-gold-charcoal",
                    54),

                Team(
                    "team.detroit.stallions",
                    "Detroit",
                    "Stallions",
                    "DST",
                    SportType.Basketball,
                    "A stylized stallion mascot.",
                    "burgundy-gold-cream",
                    56),

                Team(
                    "team.detroit.blue-wings",
                    "Detroit",
                    "Blue Wings",
                    "DBW",
                    SportType.Hockey,
                    "Working-name winged winter-bird identity; legal review required.",
                    "cobalt-ice-charcoal",
                    57),

                Team("team.lakecity.iron","Lake City","Iron","LCI",SportType.Football,"Industrial ram mascot.","iron-rust-cream",52),
                Team("team.rivercity.comets","River City","Comets","RCC",SportType.Baseball,"Comet mascot.","navy-mint-white",51),
                Team("team.greatlakes.falcons","Great Lakes","Falcons","GLF",SportType.Basketball,"Falcon mascot.","forest-silver-white",53),
                Team("team.northshore.frost","North Shore","Frost","NSF",SportType.Hockey,"Arctic fox mascot.","violet-ice-white",52)
            };
        }

        public static List<SportsVenueDefinition> CreateVenues()
        {
            return new List<SportsVenueDefinition>
            {
                new SportsVenueDefinition
                {
                    id = "sportsvenue.brush-stadium",
                    displayName = "Brush Street Stadium",
                    districtId = "district.downtown",
                    capacityHint = 65000,
                    indoor = true,
                    supportedSports =
                        new List<SportType>
                        {
                            SportType.Football
                        }
                },
                new SportsVenueDefinition
                {
                    id = "sportsvenue.motor-ballpark",
                    displayName = "Motor City Ballpark",
                    districtId = "district.downtown",
                    capacityHint = 40000,
                    indoor = false,
                    supportedSports =
                        new List<SportType>
                        {
                            SportType.Baseball
                        }
                },
                new SportsVenueDefinition
                {
                    id = "sportsvenue.woodward-arena",
                    displayName = "Woodward Arena",
                    districtId = "district.midtown-edge",
                    capacityHint = 20000,
                    indoor = true,
                    supportedSports =
                        new List<SportType>
                        {
                            SportType.Basketball,
                            SportType.Hockey
                        }
                }
            };
        }

        public static List<SportsGameRecord> CreateSchedule()
        {
            long day = GameClock.MinutesPerDay;

            return new List<SportsGameRecord>
            {
                Game(
                    "game.football.home.01",
                    SportType.Football,
                    "team.detroit.chow-dogs",
                    "team.lakecity.iron",
                    "sportsvenue.brush-stadium",
                    (3 * day) +
                    GameClock.ToMinuteOfDay(13, 0),
                    210,
                    61000,
                    1101),

                Game(
                    "game.baseball.home.01",
                    SportType.Baseball,
                    "team.detroit.cheetahs",
                    "team.rivercity.comets",
                    "sportsvenue.motor-ballpark",
                    (5 * day) +
                    GameClock.ToMinuteOfDay(19, 10),
                    190,
                    33000,
                    2202),

                Game(
                    "game.basketball.home.01",
                    SportType.Basketball,
                    "team.detroit.stallions",
                    "team.greatlakes.falcons",
                    "sportsvenue.woodward-arena",
                    (7 * day) +
                    GameClock.ToMinuteOfDay(19, 30),
                    150,
                    18800,
                    3303),

                Game(
                    "game.hockey.home.01",
                    SportType.Hockey,
                    "team.detroit.blue-wings",
                    "team.northshore.frost",
                    "sportsvenue.woodward-arena",
                    (9 * day) +
                    GameClock.ToMinuteOfDay(19, 0),
                    165,
                    19200,
                    4404)
            };
        }

        private static SportsTeamDefinition Team(
            string id,
            string city,
            string nickname,
            string abbreviation,
            SportType sport,
            string mascot,
            string palette,
            int strength)
        {
            return new SportsTeamDefinition
            {
                id = id,
                city = city,
                nickname = nickname,
                abbreviation = abbreviation,
                sport = sport,
                mascotDescription = mascot,
                paletteId = palette,
                strength = strength,
                brandStatus =
                    BrandClearanceStatus.WorkingName
            };
        }

        private static SportsGameRecord Game(
            string id,
            SportType sport,
            string home,
            string away,
            string venue,
            long start,
            int duration,
            int attendance,
            int seed)
        {
            return new SportsGameRecord
            {
                id = id,
                sport = sport,
                homeTeamId = home,
                awayTeamId = away,
                venueId = venue,
                startMinute = start,
                scheduledDurationMinutes = duration,
                expectedAttendance = attendance,
                seed = seed,
                status = SportsGameStatus.Scheduled
            };
        }
    }
}
