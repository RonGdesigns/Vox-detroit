using System;
using System.Collections.Generic;

namespace VoxDetroit.Sports
{
    public enum SportType
    {
        Football,
        Baseball,
        Basketball,
        Hockey
    }

    public enum BrandClearanceStatus
    {
        WorkingName,
        Cleared,
        Licensed
    }

    public enum SportsGameStatus
    {
        Scheduled,
        InProgress,
        Final,
        Cancelled
    }

    public enum SportsMomentType
    {
        PeriodStart,
        Score,
        BigPlay,
        Penalty,
        Timeout,
        Intermission,
        FinalWhistle
    }

    [Serializable]
    public sealed class SportsTeamDefinition
    {
        public string id;
        public string city;
        public string nickname;
        public string abbreviation;
        public SportType sport;
        public string mascotDescription;
        public string paletteId;
        public int strength = 50;
        public BrandClearanceStatus brandStatus =
            BrandClearanceStatus.WorkingName;
    }

    [Serializable]
    public sealed class SportsVenueDefinition
    {
        public string id;
        public string displayName;
        public string districtId;
        public int capacityHint;
        public bool indoor;
        public List<SportType> supportedSports =
            new List<SportType>();
        public BrandClearanceStatus brandStatus =
            BrandClearanceStatus.WorkingName;
    }

    [Serializable]
    public sealed class SportsGameMoment
    {
        public int minuteIntoEvent;
        public SportsMomentType type;
        public string teamId;
        public int points;
        public string label;
        public int excitement;
    }

    [Serializable]
    public sealed class SportsGameRecord
    {
        public string id;
        public SportType sport;
        public string homeTeamId;
        public string awayTeamId;
        public string venueId;
        public long startMinute;
        public int scheduledDurationMinutes;
        public int expectedAttendance;
        public int seed;
        public SportsGameStatus status;
        public int homeScore;
        public int awayScore;
        public bool playerAttended;
        public List<SportsGameMoment> moments =
            new List<SportsGameMoment>();
    }

    [Serializable]
    public sealed class SportsWorldState
    {
        public List<SportsGameRecord> games =
            new List<SportsGameRecord>();
    }
}
