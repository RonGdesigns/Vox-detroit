using System;
using System.Collections.Generic;

namespace VoxDetroit.Jobs
{
    public enum JobTaskType
    {
        Pickup,
        Delivery,
        Repair,
        Build,
        Photograph,
        Guard,
        TransportPassenger,
        ServiceOrder
    }

    public enum JobTaskStatus
    {
        Offered,
        Active,
        Completed,
        Failed,
        Cancelled
    }

    [Serializable]
    public sealed class JobTaskRecord
    {
        public string id;
        public string jobId;
        public string title;
        public JobTaskType type;
        public JobTaskStatus status;
        public long offeredMinute;
        public long acceptedMinute;
        public long deadlineMinute;
        public string originLocationId;
        public string destinationLocationId;
        public string targetEntityId;
        public int requiredProgress = 1;
        public int currentProgress;
        public long completionPayCents;
        public int reputationReward = 1;
    }

    [Serializable]
    public sealed class JobTaskWorldState
    {
        public List<JobTaskRecord> tasks =
            new List<JobTaskRecord>();
    }
}
