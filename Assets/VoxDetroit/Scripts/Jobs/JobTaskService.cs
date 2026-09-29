using System;
using VoxDetroit.Economy;

namespace VoxDetroit.Jobs
{
    public sealed class JobTaskService
    {
        private readonly JobTaskWorldState _state;

        public JobTaskService(JobTaskWorldState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public JobTaskRecord Find(string taskId)
        {
            foreach (JobTaskRecord task in _state.tasks)
            {
                if (task != null &&
                    string.Equals(
                        task.id,
                        taskId,
                        StringComparison.Ordinal))
                {
                    return task;
                }
            }

            return null;
        }

        public bool TryAccept(
            string taskId,
            string activeJobId,
            long gameMinute)
        {
            JobTaskRecord task = Find(taskId);

            if (task == null ||
                task.status != JobTaskStatus.Offered ||
                !string.Equals(
                    task.jobId,
                    activeJobId,
                    StringComparison.Ordinal))
            {
                return false;
            }

            if (task.deadlineMinute > 0 &&
                gameMinute > task.deadlineMinute)
            {
                task.status = JobTaskStatus.Failed;
                return false;
            }

            task.status = JobTaskStatus.Active;
            task.acceptedMinute = gameMinute;
            return true;
        }

        public bool TryAddProgress(
            string taskId,
            int amount,
            long gameMinute,
            JobService jobs,
            FinanceService finance,
            string payAccountId = AccountIds.PlayerChecking)
        {
            if (amount <= 0)
            {
                return false;
            }

            JobTaskRecord task = Find(taskId);

            if (task == null ||
                task.status != JobTaskStatus.Active)
            {
                return false;
            }

            if (task.deadlineMinute > 0 &&
                gameMinute > task.deadlineMinute)
            {
                task.status = JobTaskStatus.Failed;
                return false;
            }

            task.currentProgress = checked(
                task.currentProgress + amount);

            if (task.currentProgress <
                Math.Max(1, task.requiredProgress))
            {
                return true;
            }

            Complete(
                task,
                gameMinute,
                jobs,
                finance,
                payAccountId);

            return true;
        }

        public void ExpireOverdue(long gameMinute)
        {
            foreach (JobTaskRecord task in _state.tasks)
            {
                if (task == null ||
                    (task.status != JobTaskStatus.Offered &&
                     task.status != JobTaskStatus.Active))
                {
                    continue;
                }

                if (task.deadlineMinute > 0 &&
                    gameMinute > task.deadlineMinute)
                {
                    task.status = JobTaskStatus.Failed;
                }
            }
        }

        private static void Complete(
            JobTaskRecord task,
            long gameMinute,
            JobService jobs,
            FinanceService finance,
            string payAccountId)
        {
            task.currentProgress =
                Math.Max(
                    task.currentProgress,
                    Math.Max(1, task.requiredProgress));

            task.status = JobTaskStatus.Completed;

            if (task.completionPayCents > 0 &&
                finance != null)
            {
                finance.Credit(
                    payAccountId,
                    new Money(task.completionPayCents),
                    gameMinute,
                    "job-task",
                    task.title);
            }

            jobs?.CompleteTask(
                task.jobId,
                Math.Max(0, task.reputationReward));
        }
    }
}
