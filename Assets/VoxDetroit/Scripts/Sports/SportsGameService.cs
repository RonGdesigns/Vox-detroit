using System;
using System.Collections.Generic;
using VoxDetroit.Events;

namespace VoxDetroit.Sports
{
    public sealed class SportsGameSnapshot
    {
        public SportsGameStatus status;
        public int homeScore;
        public int awayScore;
        public int minuteIntoEvent;
        public int segment;
        public IReadOnlyList<SportsGameMoment> visibleMoments;
    }

    public sealed class SportsGameService
    {
        private readonly SportsWorldState _state;
        private readonly Dictionary<string, SportsTeamDefinition> _teams;

        public SportsGameService(
            SportsWorldState state,
            IEnumerable<SportsTeamDefinition> teams)
        {
            _state = state ??
                throw new ArgumentNullException(nameof(state));

            _teams =
                new Dictionary<string, SportsTeamDefinition>(
                    StringComparer.Ordinal);

            if (teams != null)
            {
                foreach (SportsTeamDefinition team in teams)
                {
                    if (team != null &&
                        !string.IsNullOrWhiteSpace(team.id))
                    {
                        _teams[team.id] = team;
                    }
                }
            }
        }

        public SportsGameRecord Find(string gameId)
        {
            if (string.IsNullOrWhiteSpace(gameId))
            {
                return null;
            }

            foreach (SportsGameRecord game in _state.games)
            {
                if (game != null &&
                    string.Equals(
                        game.id,
                        gameId,
                        StringComparison.Ordinal))
                {
                    return game;
                }
            }

            return null;
        }

        public IReadOnlyList<SportsGameRecord> GetActive(
            long gameMinute)
        {
            var active = new List<SportsGameRecord>();

            foreach (SportsGameRecord game in _state.games)
            {
                if (IsActive(game, gameMinute))
                {
                    active.Add(game);
                }
            }

            return active;
        }

        public bool IsActive(
            SportsGameRecord game,
            long gameMinute)
        {
            if (game == null ||
                game.status == SportsGameStatus.Cancelled)
            {
                return false;
            }

            long end =
                game.startMinute +
                Math.Max(1, game.scheduledDurationMinutes);

            return gameMinute >= game.startMinute &&
                   gameMinute < end;
        }

        public SportsGameSnapshot GetSnapshot(
            string gameId,
            long gameMinute)
        {
            SportsGameRecord game = Find(gameId);

            if (game == null)
            {
                return null;
            }

            EnsureTimeline(game);

            int elapsed =
                (int)Math.Max(
                    0,
                    gameMinute - game.startMinute);

            int duration =
                Math.Max(
                    1,
                    game.scheduledDurationMinutes);

            var visible =
                new List<SportsGameMoment>();

            int home = 0;
            int away = 0;

            foreach (SportsGameMoment moment in game.moments)
            {
                if (moment.minuteIntoEvent > elapsed)
                {
                    break;
                }

                visible.Add(moment);

                if (moment.type == SportsMomentType.Score)
                {
                    if (moment.teamId == game.homeTeamId)
                    {
                        home += moment.points;
                    }
                    else if (moment.teamId == game.awayTeamId)
                    {
                        away += moment.points;
                    }
                }
            }

            SportsGameStatus status;

            if (game.status == SportsGameStatus.Cancelled)
            {
                status = SportsGameStatus.Cancelled;
            }
            else if (gameMinute < game.startMinute)
            {
                status = SportsGameStatus.Scheduled;
            }
            else if (elapsed >= duration)
            {
                status = SportsGameStatus.Final;
            }
            else
            {
                status = SportsGameStatus.InProgress;
            }

            if (status == SportsGameStatus.Final)
            {
                game.status = status;
                game.homeScore = home;
                game.awayScore = away;
            }

            return new SportsGameSnapshot
            {
                status = status,
                homeScore = home,
                awayScore = away,
                minuteIntoEvent = Math.Min(elapsed, duration),
                segment = ResolveSegment(
                    game.sport,
                    elapsed,
                    duration),
                visibleMoments = visible
            };
        }

        public EventDemandModifiers GetGameDayDemand(
            SportsGameRecord game)
        {
            int attendance =
                game == null
                    ? 0
                    : Math.Max(
                        0,
                        game.expectedAttendance);

            int scale =
                attendance >= 50000 ? 3 :
                attendance >= 20000 ? 2 :
                attendance >= 5000 ? 1 : 0;

            return new EventDemandModifiers
            {
                trafficPercent = 115 + (scale * 15),
                taxiDemandPercent = 125 + (scale * 25),
                deliveryDemandPercent = 110 + (scale * 10),
                foodDemandPercent = 125 + (scale * 20),
                retailDemandPercent = 115 + (scale * 15),
                securityDemandPercent = 140 + (scale * 25),
                hotelDemandPercent = 110 + (scale * 15),
                undergroundOpportunityPercent =
                    105 + (scale * 10)
            };
        }

        public bool MarkPlayerAttended(string gameId)
        {
            SportsGameRecord game = Find(gameId);

            if (game == null)
            {
                return false;
            }

            game.playerAttended = true;
            return true;
        }

        public void EnsureTimeline(SportsGameRecord game)
        {
            if (game == null ||
                game.moments.Count > 0)
            {
                return;
            }

            SportsTeamDefinition home =
                ResolveTeam(game.homeTeamId);

            SportsTeamDefinition away =
                ResolveTeam(game.awayTeamId);

            int homeStrength = home?.strength ?? 50;
            int awayStrength = away?.strength ?? 50;

            var rng =
                new DeterministicRng(
                    game.seed == 0
                        ? StableSeed(game.id)
                        : game.seed);

            AddSegmentMarkers(game);

            switch (game.sport)
            {
                case SportType.Football:
                    GenerateFootball(
                        game,
                        homeStrength,
                        awayStrength,
                        rng);
                    break;

                case SportType.Baseball:
                    GenerateBaseball(
                        game,
                        homeStrength,
                        awayStrength,
                        rng);
                    break;

                case SportType.Basketball:
                    GenerateBasketball(
                        game,
                        homeStrength,
                        awayStrength,
                        rng);
                    break;

                case SportType.Hockey:
                    GenerateHockey(
                        game,
                        homeStrength,
                        awayStrength,
                        rng);
                    break;
            }

            game.moments.Sort(
                (a, b) =>
                    a.minuteIntoEvent.CompareTo(
                        b.minuteIntoEvent));

            game.moments.Add(
                new SportsGameMoment
                {
                    minuteIntoEvent =
                        Math.Max(
                            1,
                            game.scheduledDurationMinutes),
                    type = SportsMomentType.FinalWhistle,
                    label = "Final",
                    excitement = 70
                });
        }

        private SportsTeamDefinition ResolveTeam(
            string teamId)
        {
            return
                !string.IsNullOrWhiteSpace(teamId) &&
                _teams.TryGetValue(
                    teamId,
                    out SportsTeamDefinition team)
                    ? team
                    : null;
        }

        private static void GenerateFootball(
            SportsGameRecord game,
            int homeStrength,
            int awayStrength,
            DeterministicRng rng)
        {
            int scoringPlays =
                4 + rng.Next(0, 6);

            for (int i = 0; i < scoringPlays; i++)
            {
                bool home =
                    WeightedHome(
                        homeStrength,
                        awayStrength,
                        rng);

                int roll = rng.Next(0, 100);
                int points =
                    roll < 60 ? 7 :
                    roll < 92 ? 3 : 2;

                AddScore(
                    game,
                    rng.Next(
                        5,
                        Math.Max(
                            6,
                            game.scheduledDurationMinutes - 4)),
                    home ? game.homeTeamId : game.awayTeamId,
                    points,
                    points == 7
                        ? "Touchdown"
                        : points == 3
                            ? "Field goal"
                            : "Two-point score",
                    75 + rng.Next(0, 26));
            }
        }

        private static void GenerateBaseball(
            SportsGameRecord game,
            int homeStrength,
            int awayStrength,
            DeterministicRng rng)
        {
            int runs =
                3 + rng.Next(0, 9);

            for (int i = 0; i < runs; i++)
            {
                bool home =
                    WeightedHome(
                        homeStrength,
                        awayStrength,
                        rng);

                AddScore(
                    game,
                    rng.Next(
                        3,
                        Math.Max(
                            4,
                            game.scheduledDurationMinutes - 3)),
                    home ? game.homeTeamId : game.awayTeamId,
                    1,
                    rng.Next(0, 5) == 0
                        ? "Home run"
                        : "Run scored",
                    55 + rng.Next(0, 46));
            }
        }

        private static void GenerateBasketball(
            SportsGameRecord game,
            int homeStrength,
            int awayStrength,
            DeterministicRng rng)
        {
            int scoringPlays =
                55 + rng.Next(0, 25);

            for (int i = 0; i < scoringPlays; i++)
            {
                bool home =
                    WeightedHome(
                        homeStrength,
                        awayStrength,
                        rng);

                int roll = rng.Next(0, 100);
                int points =
                    roll < 15 ? 1 :
                    roll < 75 ? 2 : 3;

                AddScore(
                    game,
                    rng.Next(
                        1,
                        Math.Max(
                            2,
                            game.scheduledDurationMinutes - 1)),
                    home ? game.homeTeamId : game.awayTeamId,
                    points,
                    points == 3
                        ? "Three-pointer"
                        : points == 2
                            ? "Field goal"
                            : "Free throw",
                    35 + rng.Next(0, 56));
            }
        }

        private static void GenerateHockey(
            SportsGameRecord game,
            int homeStrength,
            int awayStrength,
            DeterministicRng rng)
        {
            int goals =
                3 + rng.Next(0, 7);

            for (int i = 0; i < goals; i++)
            {
                bool home =
                    WeightedHome(
                        homeStrength,
                        awayStrength,
                        rng);

                AddScore(
                    game,
                    rng.Next(
                        2,
                        Math.Max(
                            3,
                            game.scheduledDurationMinutes - 2)),
                    home ? game.homeTeamId : game.awayTeamId,
                    1,
                    "Goal",
                    70 + rng.Next(0, 31));
            }
        }

        private static bool WeightedHome(
            int homeStrength,
            int awayStrength,
            DeterministicRng rng)
        {
            int homeWeight =
                Math.Max(1, homeStrength + 5);

            int awayWeight =
                Math.Max(1, awayStrength);

            return rng.Next(
                       0,
                       homeWeight + awayWeight) <
                   homeWeight;
        }

        private static void AddScore(
            SportsGameRecord game,
            int minute,
            string teamId,
            int points,
            string label,
            int excitement)
        {
            game.moments.Add(
                new SportsGameMoment
                {
                    minuteIntoEvent = minute,
                    type = SportsMomentType.Score,
                    teamId = teamId,
                    points = points,
                    label = label,
                    excitement = excitement
                });
        }

        private static void AddSegmentMarkers(
            SportsGameRecord game)
        {
            int segments =
                game.sport == SportType.Baseball
                    ? 9
                    : game.sport == SportType.Hockey
                        ? 3
                        : 4;

            int duration =
                Math.Max(
                    segments,
                    game.scheduledDurationMinutes);

            for (int i = 0; i < segments; i++)
            {
                game.moments.Add(
                    new SportsGameMoment
                    {
                        minuteIntoEvent =
                            (duration * i) / segments,
                        type = SportsMomentType.PeriodStart,
                        label =
                            game.sport == SportType.Baseball
                                ? $"Inning {i + 1}"
                                : $"Period {i + 1}",
                        excitement = 20
                    });
            }
        }

        private static int ResolveSegment(
            SportType sport,
            int elapsed,
            int duration)
        {
            int segments =
                sport == SportType.Baseball
                    ? 9
                    : sport == SportType.Hockey
                        ? 3
                        : 4;

            int clamped =
                Math.Min(
                    Math.Max(elapsed, 0),
                    Math.Max(duration - 1, 0));

            return Math.Min(
                segments,
                1 +
                (clamped * segments) /
                Math.Max(1, duration));
        }

        private static int StableSeed(string value)
        {
            unchecked
            {
                int hash = 17;

                if (value != null)
                {
                    foreach (char c in value)
                    {
                        hash = (hash * 31) + c;
                    }
                }

                return hash;
            }
        }

        private sealed class DeterministicRng
        {
            private uint _state;

            public DeterministicRng(int seed)
            {
                _state =
                    seed == 0
                        ? 2463534242u
                        : unchecked((uint)seed);
            }

            public int Next(int minInclusive, int maxExclusive)
            {
                if (maxExclusive <= minInclusive)
                {
                    return minInclusive;
                }

                _state ^= _state << 13;
                _state ^= _state >> 17;
                _state ^= _state << 5;

                uint range =
                    (uint)(maxExclusive - minInclusive);

                return
                    minInclusive +
                    (int)(_state % range);
            }
        }
    }
}
