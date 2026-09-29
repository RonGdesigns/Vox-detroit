using System;
using System.Collections.Generic;

namespace VoxDetroit.Jobs
{
    [Serializable]
    public sealed class JobProgressState
    {
        public string jobId;
        public int reputation;
        public int completedTasks;
        public long totalMinutesWorked;
        public long totalEarningsCents;
    }

    [Serializable]
    public sealed class EmploymentState
    {
        public string currentJobId;
        public bool shiftActive;
        public long activeShiftStartMinute;
        public List<JobProgressState> progress =
            new List<JobProgressState>();
    }
}
