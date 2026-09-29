using System;
using System.Collections.Generic;
using VoxDetroit.Businesses;
using VoxDetroit.Commerce;
using VoxDetroit.Careers;
using VoxDetroit.Core;
using VoxDetroit.Economy;
using VoxDetroit.Events;
using VoxDetroit.Inventory;
using VoxDetroit.Jobs;
using VoxDetroit.Persistence;
using VoxDetroit.Properties;
using VoxDetroit.Reputation;
using VoxDetroit.Story;
using VoxDetroit.Sports;

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
        public CityEventService CityEvents { get; }
        public SportsGameService Sports { get; }
        public CareerService Careers { get; }
        public PropertyService Properties { get; }
        public BusinessService Businesses { get; }
        public ReputationService Reputation { get; }
        public StoryService Story { get; }

        public SimulationSession(
            VoxDetroitSaveData data,
            IEnumerable<JobDefinition> jobDefinitions,
            IEnumerable<SportsTeamDefinition> sportsTeams = null)
        {
            Data = SaveDataNormalizer.Normalize(
                data ??
                throw new ArgumentNullException(nameof(data)));

            Clock = new GameClock(Data.clock);
            Finance = new FinanceService(Data.finance);
            Obligations = new ObligationService(Data.obligations);
            Jobs = new JobService(
                Data.employment,
                jobDefinitions);
            JobTasks = new JobTaskService(Data.jobTasks);
            Inventory = new InventoryService(Data.inventory);
            Stores = new StoreService(Data.stores);
            CityEvents = new CityEventService(Data.cityEvents);
            Sports = new SportsGameService(
                Data.sports,
                sportsTeams ?? PrototypeSportsCatalog.CreateTeams());
            Careers = new CareerService(Data.careers);
            Properties = new PropertyService(Data.properties);
            Businesses = new BusinessService(Data.businesses);
            Reputation = new ReputationService(Data.reputation);
            Story = new StoryService(Data.story);
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
