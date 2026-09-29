using VoxDetroit.Commerce;
using VoxDetroit.Core;
using VoxDetroit.Economy;
using VoxDetroit.Inventory;
using VoxDetroit.Jobs;
using VoxDetroit.NPCs;
using VoxDetroit.Persistence;
using VoxDetroit.Properties;
using VoxDetroit.Story;

namespace VoxDetroit.Simulation
{
    public static class PrototypeContentIds
    {
        public const string StartingApartment =
            "property.prototype.starting-apartment";

        public const string HardwareStore =
            "store.prototype.hardware";

        public const string HardwareStoreAccount =
            "business.prototype.hardware";

        public const string FirstDeliveryTask =
            "task.prototype.first-delivery";

        public const string NeighborNpc =
            "npc.prototype.neighbor";

        public const string DispatcherNpc =
            "npc.prototype.dispatcher";
    }

    public static class PrototypeScenarioFactory
    {
        private const long MinutesPerThirtyDays =
            30L * GameClock.MinutesPerDay;

        public static VoxDetroitSaveData Create()
        {
            VoxDetroitSaveData data =
                NewGameFactory.Create(
                    new NewGameSettings
                    {
                        startingCashCents = 6000,
                        startingCheckingCents = 40000,
                        worldSeed = 42701
                    });

            SeedResidence(data);
            SeedBills(data);
            SeedInventory(data);
            SeedStores(data);
            SeedJobTasks(data);
            SeedNpcs(data);
            SeedStory(data);

            return SaveDataNormalizer.Normalize(data);
        }

        private static void SeedResidence(
            VoxDetroitSaveData data)
        {
            data.properties.properties.Add(
                new PropertyRecord
                {
                    id =
                        PrototypeContentIds.StartingApartment,
                    buildingId =
                        "prototype.building.starting-apartment",
                    addressLabel =
                        "Downtown Starter Apartment",
                    use = PropertyUse.Residential,
                    ownerId =
                        "owner.prototype.landlord",
                    marketValueCents = 16500000,
                    monthlyRentCents = 95000,
                    condition = 62,
                    enterable = true
                });

            data.player.residencePropertyId =
                PrototypeContentIds.StartingApartment;
        }

        private static void SeedBills(
            VoxDetroitSaveData data)
        {
            data.obligations.obligations.Add(
                new ObligationRecord
                {
                    id = "obligation.prototype.rent",
                    displayName = "Apartment Rent",
                    type = ObligationType.Rent,
                    payerAccountId =
                        AccountIds.PlayerChecking,
                    amountCents = 95000,
                    intervalMinutes = MinutesPerThirtyDays,
                    nextDueMinute = MinutesPerThirtyDays,
                    relatedEntityId =
                        PrototypeContentIds.StartingApartment
                });

            data.obligations.obligations.Add(
                new ObligationRecord
                {
                    id = "obligation.prototype.utilities",
                    displayName = "Utilities",
                    type = ObligationType.Utilities,
                    payerAccountId =
                        AccountIds.PlayerChecking,
                    amountCents = 12500,
                    intervalMinutes = MinutesPerThirtyDays,
                    nextDueMinute = MinutesPerThirtyDays,
                    relatedEntityId =
                        PrototypeContentIds.StartingApartment
                });
        }

        private static void SeedInventory(
            VoxDetroitSaveData data)
        {
            data.inventory.stacks.Add(
                new ItemStack
                {
                    itemId = "tool.phone",
                    quantity = 1
                });
        }

        private static void SeedStores(
            VoxDetroitSaveData data)
        {
            data.finance.accounts.Add(
                new AccountState
                {
                    id =
                        PrototypeContentIds.HardwareStoreAccount,
                    balanceCents = 250000
                });

            var hardware = new StoreRecord
            {
                id = PrototypeContentIds.HardwareStore,
                displayName = "Prototype Hardware",
                financeAccountId =
                    PrototypeContentIds.HardwareStoreAccount
            };

            hardware.catalog.Add(
                new StoreCatalogEntry
                {
                    itemId = "material.lumber",
                    unitPriceCents = 375,
                    stock = 80
                });

            hardware.catalog.Add(
                new StoreCatalogEntry
                {
                    itemId = "material.brick",
                    unitPriceCents = 250,
                    stock = 120
                });

            hardware.catalog.Add(
                new StoreCatalogEntry
                {
                    itemId = "material.glass",
                    unitPriceCents = 525,
                    stock = 40
                });

            hardware.catalog.Add(
                new StoreCatalogEntry
                {
                    itemId = "tool.basic-kit",
                    unitPriceCents = 3500,
                    stock = 8
                });

            data.stores.stores.Add(hardware);
        }

        private static void SeedJobTasks(
            VoxDetroitSaveData data)
        {
            data.jobTasks.tasks.Add(
                new JobTaskRecord
                {
                    id =
                        PrototypeContentIds.FirstDeliveryTask,
                    jobId = "job.delivery.entry",
                    title = "First Downtown Delivery",
                    type = JobTaskType.Delivery,
                    status = JobTaskStatus.Offered,
                    offeredMinute = 0,
                    deadlineMinute =
                        GameClock.ToMinuteOfDay(18, 0),
                    originLocationId =
                        "location.prototype.pickup",
                    destinationLocationId =
                        "location.prototype.delivery",
                    requiredProgress = 1,
                    completionPayCents = 1800,
                    reputationReward = 2
                });
        }

        private static void SeedNpcs(
            VoxDetroitSaveData data)
        {
            var neighbor = new NpcRecord
            {
                id = PrototypeContentIds.NeighborNpc,
                displayName = "Neighbor",
                homeLocationId =
                    PrototypeContentIds.StartingApartment
            };

            data.npcs.npcs.Add(neighbor);

            var dispatcher = new NpcRecord
            {
                id = PrototypeContentIds.DispatcherNpc,
                displayName = "Delivery Dispatcher",
                homeLocationId =
                    "location.prototype.dispatcher-home",
                jobId = "job.delivery.entry"
            };

            for (int day = 0; day < 5; day++)
            {
                dispatcher.schedule.Add(
                    new NpcScheduleEntry
                    {
                        dayOfWeek = day,
                        startMinute =
                            GameClock.ToMinuteOfDay(8, 0),
                        endMinute =
                            GameClock.ToMinuteOfDay(17, 0),
                        locationId =
                            "location.prototype.delivery-office",
                        activity = "work"
                    });
            }

            data.npcs.npcs.Add(dispatcher);
        }

        private static void SeedStory(
            VoxDetroitSaveData data)
        {
            data.story.flags.Add(
                new StoryFlag
                {
                    key = "intro.arrived",
                    value = true
                });

            data.story.variables.Add(
                new StoryVariable
                {
                    key = "life.stability",
                    value = 0
                });
        }
    }
}
