using System;
using VoxDetroit.Businesses;
using VoxDetroit.Careers;
using VoxDetroit.Commerce;
using VoxDetroit.Core;
using VoxDetroit.Economy;
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
    public static class SaveSchema
    {
        public const int CurrentVersion = 1;
    }

    [Serializable]
    public sealed class VoxDetroitSaveData
    {
        public int schemaVersion = SaveSchema.CurrentVersion;
        public string saveId;
        public string createdUtc;
        public string updatedUtc;
        public int worldSeed;

        public GameClockState clock = new GameClockState();
        public PlayerState player = new PlayerState();
        public FinanceState finance = new FinanceState();
        public ObligationState obligations = new ObligationState();
        public EmploymentState employment = new EmploymentState();
        public JobTaskWorldState jobTasks = new JobTaskWorldState();
        public InventoryState inventory = new InventoryState();
        public StoreWorldState stores = new StoreWorldState();
        public CareerWorldState careers = new CareerWorldState();
        public SubstanceMarketState substanceMarkets =
            new SubstanceMarketState();
        public PropertyWorldState properties = new PropertyWorldState();
        public BusinessWorldState businesses = new BusinessWorldState();
        public VehicleWorldState vehicles = new VehicleWorldState();
        public NpcWorldState npcs = new NpcWorldState();
        public ReputationState reputation = new ReputationState();
        public StoryState story = new StoryState();
        public VoxelDeltaState voxelChanges = new VoxelDeltaState();
    }
}
