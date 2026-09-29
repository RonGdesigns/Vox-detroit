using System;

namespace VoxDetroit.NPCs
{
    public sealed class NpcScheduleService
    {
        public NpcScheduleEntry Resolve(
            NpcRecord npc,
            int dayOfWeek,
            int minuteOfDay)
        {
            if (npc == null)
            {
                throw new ArgumentNullException(nameof(npc));
            }

            int day = PositiveMod(dayOfWeek, 7);
            int minute = PositiveMod(minuteOfDay, 1440);

            foreach (NpcScheduleEntry entry in npc.schedule)
            {
                if (entry == null ||
                    PositiveMod(entry.dayOfWeek, 7) != day)
                {
                    continue;
                }

                if (ContainsMinute(entry, minute))
                {
                    return entry;
                }
            }

            return new NpcScheduleEntry
            {
                dayOfWeek = day,
                startMinute = 0,
                endMinute = 1440,
                locationId = npc.homeLocationId,
                activity = "home"
            };
        }

        public bool Validate(NpcRecord npc, out string error)
        {
            if (npc == null)
            {
                error = "NPC is null.";
                return false;
            }

            for (int i = 0; i < npc.schedule.Count; i++)
            {
                NpcScheduleEntry a = npc.schedule[i];

                if (a == null ||
                    a.startMinute < 0 ||
                    a.startMinute >= 1440 ||
                    a.endMinute < 0 ||
                    a.endMinute > 1440 ||
                    a.startMinute == a.endMinute)
                {
                    error = $"Invalid schedule entry {i} for {npc.id}.";
                    return false;
                }

                for (int j = i + 1; j < npc.schedule.Count; j++)
                {
                    NpcScheduleEntry b = npc.schedule[j];

                    if (b == null ||
                        PositiveMod(a.dayOfWeek, 7) !=
                        PositiveMod(b.dayOfWeek, 7))
                    {
                        continue;
                    }

                    if (Overlaps(a, b))
                    {
                        error =
                            $"Schedule entries {i} and {j} overlap for {npc.id}.";
                        return false;
                    }
                }
            }

            error = null;
            return true;
        }

        private static bool ContainsMinute(
            NpcScheduleEntry entry,
            int minute)
        {
            if (entry.startMinute < entry.endMinute)
            {
                return minute >= entry.startMinute &&
                       minute < entry.endMinute;
            }

            return minute >= entry.startMinute ||
                   minute < entry.endMinute;
        }

        private static bool Overlaps(
            NpcScheduleEntry a,
            NpcScheduleEntry b)
        {
            for (int minute = 0; minute < 1440; minute++)
            {
                if (ContainsMinute(a, minute) &&
                    ContainsMinute(b, minute))
                {
                    return true;
                }
            }

            return false;
        }

        private static int PositiveMod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }
}
