using System;

namespace VoxDetroit.Core
{
    [Serializable]
    public sealed class GameClockState
    {
        public long totalMinutes;
    }

    public sealed class GameClock
    {
        public const int MinutesPerDay = 1440;
        public const int DaysPerWeek = 7;

        private readonly GameClockState _state;

        public GameClock(GameClockState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public long TotalMinutes => _state.totalMinutes;
        public int MinuteOfDay => PositiveMod(_state.totalMinutes, MinutesPerDay);
        public int DayIndex => (int)(_state.totalMinutes / MinutesPerDay);
        public int DayOfWeek => PositiveMod(DayIndex, DaysPerWeek);

        public void AdvanceMinutes(int minutes)
        {
            if (minutes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes));
            }

            _state.totalMinutes = checked(_state.totalMinutes + minutes);
        }

        public static int ToMinuteOfDay(int hour, int minute)
        {
            if (hour < 0 || hour > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hour));
            }

            if (minute < 0 || minute > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(minute));
            }

            return (hour * 60) + minute;
        }

        private static int PositiveMod(long value, int divisor)
        {
            long result = value % divisor;
            return (int)(result < 0 ? result + divisor : result);
        }

        private static int PositiveMod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }
}
