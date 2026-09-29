using System;
using System.Collections.Generic;
using VoxDetroit.Businesses;
using VoxDetroit.Commerce;
using VoxDetroit.Core;
using VoxDetroit.Economy;
using VoxDetroit.Inventory;
using VoxDetroit.Jobs;
using VoxDetroit.Persistence;
using VoxDetroit.Properties;
using VoxDetroit.Reputation;
using VoxDetroit.Story;

namespace VoxDetroit.Simulation
{
    public sealed class SimulationAdvanceResult
    {
        public long previousMinute;
        public long currentMinute;
        public IReadOnlyList<ObligationRecord> missedObligations;
    }

    public sealed class SimulationSession
    {
        public VoxDetroitSaveData Data { get; }

        public GameClock Clock { get; }
        public FinanceService Finance { get; }
        public ObligationService Obligations { get; }
        public JobService Jobs { get; }
        public JobTaskService JobTasks { get; }
        public InventoryService Inventory { get; }
        public StoreService Stores { get; }
        public PropertyService Properties { get; }
        public BusinessService Businesses { get; }
        public ReputationService Reputation { get; }
        public StoryService Story { get; }

        public SimulationSession(
            VoxDetroitSaveData data,
            IEnumerable<JobDefinition> jobDefinitions)
        {
            Data = data ??
                throw new ArgumentNullException(nameof(data));

            Clock = new GameClock(data.clock);
            Finance = new FinanceService(data.finance);
            Obligations = new ObligationService(data.obligations);
            Jobs = new JobService(
                data.employment,
                jobDefinitions);
            JobTasks = new JobTaskService(data.jobTasks);
            Inventory = new InventoryService(data.inventory);
            Stores = new StoreService(data.stores);
            Properties = new PropertyService(data.properties);
            Businesses = new BusinessService(data.businesses);
            Reputation = new ReputationService(data.reputation);
            Story = new StoryService(data.story);
        }

        public SimulationAdvanceResult AdvanceMinutes(int minutes)
        {
            if (minutes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes));
            }

            long previous = Clock.TotalMinutes;
            Clock.AdvanceMinutes(minutes);

            JobTasks.ExpireOverdue(Clock.TotalMinutes);

            IReadOnlyList<ObligationRecord> missed =
                Obligations.ProcessDue(
                    Clock.TotalMinutes,
                    Finance);

            return new SimulationAdvanceResult
            {
                previousMinute = previous,
                currentMinute = Clock.TotalMinutes,
                missedObligations = missed
            };
        }
    }
}
