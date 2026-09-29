using System;
using System.Collections.Generic;
using VoxDetroit.Economy;

namespace VoxDetroit.Jobs
{
    public sealed class JobService
    {
        private readonly EmploymentState _state;
        private readonly Dictionary<string, JobDefinition> _definitions;

        public JobService(
            EmploymentState state,
            IEnumerable<JobDefinition> definitions)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _definitions =
                new Dictionary<string, JobDefinition>(
                    StringComparer.Ordinal);

            if (definitions == null)
            {
                return;
            }

            foreach (JobDefinition definition in definitions)
            {
                if (definition == null ||
                    string.IsNullOrWhiteSpace(definition.id))
                {
                    continue;
                }

                _definitions[definition.id] = definition;
            }
        }

        public bool CanAcceptJob(string jobId)
        {
            if (!_definitions.TryGetValue(
                    jobId,
                    out JobDefinition definition))
            {
                return false;
            }

            JobProgressState progress = GetOrCreateProgress(jobId);
            return progress.reputation >= definition.reputationRequired;
        }

        public bool TryAcceptJob(string jobId)
        {
            if (_state.shiftActive || !CanAcceptJob(jobId))
            {
                return false;
            }

            _state.currentJobId = jobId;
            return true;
        }

        public bool TryStartShift(long gameMinute)
        {
            if (_state.shiftActive ||
                string.IsNullOrWhiteSpace(_state.currentJobId) ||
                !_definitions.ContainsKey(_state.currentJobId))
            {
                return false;
            }

            _state.shiftActive = true;
            _state.activeShiftStartMinute = gameMinute;
            return true;
        }

        public Money EndShiftAndPay(
            long gameMinute,
            FinanceService finance,
            string targetAccountId = AccountIds.PlayerChecking)
        {
            if (!_state.shiftActive)
            {
                return Money.Zero;
            }

            if (!_definitions.TryGetValue(
                    _state.currentJobId,
                    out JobDefinition definition))
            {
                _state.shiftActive = false;
                return Money.Zero;
            }

            long workedMinutes =
                Math.Max(0, gameMinute - _state.activeShiftStartMinute);

            long payCents = checked(
                (definition.hourlyPayCents * workedMinutes) / 60L);

            JobProgressState progress =
                GetOrCreateProgress(definition.id);

            progress.totalMinutesWorked =
                checked(progress.totalMinutesWorked + workedMinutes);

            progress.totalEarningsCents =
                checked(progress.totalEarningsCents + payCents);

            _state.shiftActive = false;
            _state.activeShiftStartMinute = 0;

            var pay = new Money(payCents);

            if (pay.Cents > 0)
            {
                finance.Credit(
                    targetAccountId,
                    pay,
                    gameMinute,
                    "wages",
                    $"{definition.displayName} shift");
            }

            return pay;
        }

        public void CompleteTask(
            string jobId,
            int reputationGain = 1)
        {
            JobProgressState progress = GetOrCreateProgress(jobId);
            progress.completedTasks =
                checked(progress.completedTasks + 1);
            progress.reputation =
                checked(progress.reputation +
                        Math.Max(0, reputationGain));
        }

        public JobProgressState GetProgress(string jobId)
        {
            foreach (JobProgressState progress in _state.progress)
            {
                if (progress != null &&
                    string.Equals(
                        progress.jobId,
                        jobId,
                        StringComparison.Ordinal))
                {
                    return progress;
                }
            }

            return null;
        }

        private JobProgressState GetOrCreateProgress(string jobId)
        {
            JobProgressState existing = GetProgress(jobId);
            if (existing != null)
            {
                return existing;
            }

            var created = new JobProgressState
            {
                jobId = jobId
            };

            _state.progress.Add(created);
            return created;
        }
    }
}
