using System.Collections.Generic;
using VoxDetroit.Businesses;
using VoxDetroit.Commerce;
using VoxDetroit.Careers;
using VoxDetroit.Core;
using VoxDetroit.Economy;
using VoxDetroit.Events;
using VoxDetroit.Inventory;
using VoxDetroit.Jobs;
using VoxDetroit.NPCs;
using VoxDetroit.Player;
using VoxDetroit.Properties;
using VoxDetroit.Reputation;
using VoxDetroit.Story;
using VoxDetroit.Vehicles;
using VoxDetroit.World;

namespace VoxDetroit.Persistence
{
    public static class SaveDataNormalizer
    {
        public static VoxDetroitSaveData Normalize(
            VoxDetroitSaveData data)
        {
            if (data == null)
            {
                data = new VoxDetroitSaveData();
            }

            if (data.clock == null)
            {
                data.clock = new GameClockState();
            }

            if (data.player == null)
            {
                data.player = new PlayerState();
            }

            if (data.finance == null)
            {
                data.finance = new FinanceState();
            }

            if (data.obligations == null)
            {
                data.obligations = new ObligationState();
            }

            if (data.employment == null)
            {
                data.employment = new EmploymentState();
            }

            if (data.jobTasks == null)
            {
                data.jobTasks = new JobTaskWorldState();
            }

            if (data.inventory == null)
            {
                data.inventory = new InventoryState();
            }

            if (data.stores == null)
            {
                data.stores = new StoreWorldState();
            }

            if (data.cityEvents == null)
            {
                data.cityEvents = new CityEventWorldState();
            }

            if (data.performers == null)
            {
                data.performers = new PerformerWorldState();
            }

            if (data.careers == null)
            {
                data.careers = new CareerWorldState();
            }

            if (data.substanceMarkets == null)
            {
                data.substanceMarkets =
                    new SubstanceMarketState();
            }

            if (data.properties == null)
            {
                data.properties = new PropertyWorldState();
            }

            if (data.businesses == null)
            {
                data.businesses = new BusinessWorldState();
            }

            if (data.vehicles == null)
            {
                data.vehicles = new VehicleWorldState();
            }

            if (data.npcs == null)
            {
                data.npcs = new NpcWorldState();
            }

            if (data.reputation == null)
            {
                data.reputation = new ReputationState();
            }

            if (data.story == null)
            {
                data.story = new StoryState();
            }

            if (data.voxelChanges == null)
            {
                data.voxelChanges = new VoxelDeltaState();
            }

            data.player.ownedVehicleIds =
                data.player.ownedVehicleIds ??
                new List<string>();

            data.player.ownedBusinessIds =
                data.player.ownedBusinessIds ??
                new List<string>();

            data.finance.accounts =
                data.finance.accounts ??
                new List<AccountState>();

            data.finance.transactions =
                data.finance.transactions ??
                new List<TransactionRecord>();

            data.obligations.obligations =
                data.obligations.obligations ??
                new List<ObligationRecord>();

            data.employment.progress =
                data.employment.progress ??
                new List<JobProgressState>();

            data.jobTasks.tasks =
                data.jobTasks.tasks ??
                new List<JobTaskRecord>();

            data.inventory.stacks =
                data.inventory.stacks ??
                new List<ItemStack>();

            data.stores.stores =
                data.stores.stores ??
                new List<StoreRecord>();

            data.cityEvents.events =
                data.cityEvents.events ??
                new List<ScheduledCityEvent>();

            data.performers.discoveredPerformerIds =
                data.performers.discoveredPerformerIds ??
                new List<string>();

            data.performers.seenLivePerformerIds =
                data.performers.seenLivePerformerIds ??
                new List<string>();

            data.careers.paths =
                data.careers.paths ??
                new List<IncomePathProgress>();

            data.substanceMarkets.categories =
                data.substanceMarkets.categories ??
                new List<SubstanceMarketProgress>();

            data.properties.properties =
                data.properties.properties ??
                new List<PropertyRecord>();

            data.businesses.businesses =
                data.businesses.businesses ??
                new List<BusinessRecord>();

            data.vehicles.vehicles =
                data.vehicles.vehicles ??
                new List<VehicleRecord>();

            data.npcs.npcs =
                data.npcs.npcs ??
                new List<NpcRecord>();

            data.reputation.records =
                data.reputation.records ??
                new List<ReputationRecord>();

            data.story.flags =
                data.story.flags ??
                new List<StoryFlag>();

            data.story.variables =
                data.story.variables ??
                new List<StoryVariable>();

            data.story.triggeredEventIds =
                data.story.triggeredEventIds ??
                new List<string>();

            data.voxelChanges.changes =
                data.voxelChanges.changes ??
                new List<VoxelDeltaRecord>();

            return data;
        }
    }
}
