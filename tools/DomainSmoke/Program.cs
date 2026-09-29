using System;
using VoxDetroit.Businesses;
using VoxDetroit.Commerce;
using VoxDetroit.Inventory;
using VoxDetroit.Persistence;
using VoxDetroit.Simulation;
using VoxDetroit.Core;
using VoxDetroit.Detroit;
using VoxDetroit.Economy;
using VoxDetroit.Jobs;
using VoxDetroit.NPCs;
using VoxDetroit.Properties;
using VoxDetroit.Reputation;
using VoxDetroit.Story;
using VoxDetroit.Voxels;
using VoxDetroit.World;

internal static class Program
{
    private static int _assertions;

    private static void Main()
    {
        TestClock();
        TestChunkCoordinates();
        TestFinanceJobsPropertyAndObligations();
        TestStoreTasksAndSimulation();
        TestPrototypeScenario();
        TestBusiness();
        TestNpcSchedule();
        TestStory();
        TestReputation();
        TestVoxelDeltas();
        TestDetroitRasterization();

        Console.WriteLine(
            $"Vox Detroit domain smoke tests passed: {_assertions} assertions.");
    }

    private static void TestClock()
    {
        var state = new GameClockState();
        var clock = new GameClock(state);

        clock.AdvanceMinutes(1500);

        Assert(clock.DayIndex == 1, "clock day");
        Assert(clock.MinuteOfDay == 60, "clock minute");
        Assert(clock.DayOfWeek == 1, "clock weekday");
    }

    private static void TestChunkCoordinates()
    {
        ChunkCoord coord =
            ChunkCoord.FromWorldVoxel(-1, 0, -33);

        Assert(coord.X == -1, "negative chunk x");
        Assert(coord.Z == -2, "negative chunk z");
        Assert(
            ChunkCoord.ToLocalVoxel(-1) == 31,
            "negative local voxel");
    }

    private static void TestFinanceJobsPropertyAndObligations()
    {
        var financeState = new FinanceState();
        var finance = new FinanceService(financeState);

        finance.GetOrCreateAccount(
            AccountIds.PlayerChecking,
            50000);

        finance.GetOrCreateAccount(
            AccountIds.PlayerSavings,
            0);

        bool transferred = finance.TryTransfer(
            AccountIds.PlayerChecking,
            AccountIds.PlayerSavings,
            new Money(10000),
            0,
            "transfer");

        Assert(transferred, "bank transfer");
        Assert(
            finance.GetBalance(AccountIds.PlayerChecking).Cents ==
            40000,
            "checking after transfer");

        var employment = new EmploymentState();
        var jobs = new JobService(
            employment,
            PrototypeJobCatalog.Create());

        Assert(
            jobs.TryAcceptJob("job.delivery.entry"),
            "accept delivery job");

        Assert(jobs.TryStartShift(100), "start shift");

        Money pay = jobs.EndShiftAndPay(
            220,
            finance);

        Assert(pay.Cents == 3700, "two hour wage");

        var propertyState = new PropertyWorldState();
        propertyState.properties.Add(
            new PropertyRecord
            {
                id = "property.test",
                addressLabel = "Test Property",
                purchasePriceCents = 20000,
                marketValueCents = 30000,
                condition = 40
            });

        var properties = new PropertyService(propertyState);

        Assert(
            properties.TryPurchase(
                "property.test",
                "player",
                AccountIds.PlayerChecking,
                finance,
                230),
            "purchase property");

        var renovation = new RenovationDefinition
        {
            id = "upgrade.windows",
            displayName = "Replace Windows",
            costCents = 5000,
            conditionGain = 20,
            valueGainPercent = 10
        };

        Assert(
            properties.TryRenovate(
                "property.test",
                renovation,
                AccountIds.PlayerChecking,
                finance,
                240),
            "renovate property");

        PropertyRecord property =
            properties.Find("property.test");

        Assert(property.condition == 60, "renovation condition");
        Assert(
            property.marketValueCents == 33000,
            "renovation value");

        var obligationState = new ObligationState();
        obligationState.obligations.Add(
            new ObligationRecord
            {
                id = "rent.test",
                displayName = "Prototype Rent",
                type = ObligationType.Rent,
                payerAccountId = AccountIds.PlayerChecking,
                amountCents = 30000,
                intervalMinutes = 43200,
                nextDueMinute = 300
            });

        var obligations =
            new ObligationService(obligationState);

        var missed =
            obligations.ProcessDue(300, finance);

        Assert(missed.Count == 1, "missed unaffordable rent");
        Assert(
            obligationState.obligations[0].missedPayments == 1,
            "missed payment count");
    }


    private static void TestStoreTasksAndSimulation()
    {
        VoxDetroitSaveData data =
            NewGameFactory.Create(
                new NewGameSettings
                {
                    startingCashCents = 0,
                    startingCheckingCents = 50000,
                    worldSeed = 123
                });

        var session = new SimulationSession(
            data,
            PrototypeJobCatalog.Create());

        session.Finance.GetOrCreateAccount("store.hardware", 0);

        var store = new StoreRecord
        {
            id = "store.test",
            displayName = "Prototype Hardware",
            financeAccountId = "store.hardware"
        };

        store.catalog.Add(
            new StoreCatalogEntry
            {
                itemId = "material.brick",
                unitPriceCents = 250,
                stock = 10
            });

        data.stores.stores.Add(store);

        Assert(
            session.Stores.TryBuy(
                "store.test",
                "material.brick",
                3,
                AccountIds.PlayerChecking,
                session.Finance,
                session.Inventory,
                session.Clock.TotalMinutes),
            "buy construction materials");

        Assert(
            session.Inventory.Count("material.brick") == 3,
            "inventory received materials");

        Assert(
            session.Finance.GetBalance("store.hardware").Cents == 750,
            "store receives purchase");

        Assert(
            session.Jobs.TryAcceptJob("job.delivery.entry"),
            "session accepts job");

        data.jobTasks.tasks.Add(
            new JobTaskRecord
            {
                id = "task.delivery.test",
                jobId = "job.delivery.entry",
                title = "Prototype Delivery",
                type = JobTaskType.Delivery,
                status = JobTaskStatus.Offered,
                requiredProgress = 1,
                completionPayCents = 1200,
                reputationReward = 2,
                deadlineMinute = 180
            });

        Assert(
            session.JobTasks.TryAccept(
                "task.delivery.test",
                data.employment.currentJobId,
                session.Clock.TotalMinutes),
            "accept job task");

        long beforeTask =
            session.Finance
                .GetBalance(AccountIds.PlayerChecking)
                .Cents;

        Assert(
            session.JobTasks.TryAddProgress(
                "task.delivery.test",
                1,
                session.Clock.TotalMinutes,
                session.Jobs,
                session.Finance),
            "complete task progress");

        Assert(
            data.jobTasks.tasks[0].status ==
            JobTaskStatus.Completed,
            "job task completed");

        Assert(
            session.Finance
                .GetBalance(AccountIds.PlayerChecking)
                .Cents ==
            beforeTask + 1200,
            "job task paid");

        data.obligations.obligations.Add(
            new ObligationRecord
            {
                id = "utility.test",
                displayName = "Prototype Utility Bill",
                payerAccountId = AccountIds.PlayerChecking,
                amountCents = 100,
                intervalMinutes = 43200,
                nextDueMinute = 10
            });

        SimulationAdvanceResult advance =
            session.AdvanceMinutes(20);

        Assert(
            advance.currentMinute == 20,
            "simulation advances time");

        Assert(
            advance.missedObligations.Count == 0,
            "simulation processes payable bill");
    }


    private static void TestPrototypeScenario()
    {
        VoxDetroitSaveData data =
            PrototypeScenarioFactory.Create();

        var session = new SimulationSession(
            data,
            PrototypeJobCatalog.Create());

        Assert(
            session.Finance
                .GetBalance(AccountIds.PlayerCash)
                .Cents == 6000,
            "prototype starting cash");

        Assert(
            session.Finance
                .GetBalance(AccountIds.PlayerChecking)
                .Cents == 40000,
            "prototype starting checking");

        Assert(
            data.player.residencePropertyId ==
            PrototypeContentIds.StartingApartment,
            "prototype residence assigned");

        Assert(
            session.Properties.Find(
                PrototypeContentIds.StartingApartment) != null,
            "prototype apartment exists");

        Assert(
            data.obligations.obligations.Count == 2,
            "prototype bills seeded");

        Assert(
            data.npcs.npcs.Count == 2,
            "prototype NPC hooks seeded");

        Assert(
            session.Story.GetFlag("intro.arrived"),
            "prototype intro story state");

        Assert(
            session.Jobs.TryAcceptJob("job.delivery.entry"),
            "prototype delivery job accepted");

        Assert(
            session.JobTasks.TryAccept(
                PrototypeContentIds.FirstDeliveryTask,
                data.employment.currentJobId,
                session.Clock.TotalMinutes),
            "prototype first delivery accepted");

        long balanceBefore =
            session.Finance
                .GetBalance(AccountIds.PlayerChecking)
                .Cents;

        Assert(
            session.JobTasks.TryAddProgress(
                PrototypeContentIds.FirstDeliveryTask,
                1,
                session.Clock.TotalMinutes,
                session.Jobs,
                session.Finance),
            "prototype first delivery completed");

        Assert(
            session.Finance
                .GetBalance(AccountIds.PlayerChecking)
                .Cents ==
            balanceBefore + 1800,
            "prototype delivery pays player");

        Assert(
            session.Stores.TryBuy(
                PrototypeContentIds.HardwareStore,
                "material.brick",
                1,
                AccountIds.PlayerChecking,
                session.Finance,
                session.Inventory,
                session.Clock.TotalMinutes),
            "prototype store purchase");

        Assert(
            session.Inventory.Count("material.brick") == 1,
            "prototype inventory receives purchase");

        var sparse = new VoxDetroitSaveData
        {
            finance = null,
            inventory = null,
            stores = null,
            jobTasks = null,
            npcs = null
        };

        SaveDataNormalizer.Normalize(sparse);

        Assert(
            sparse.finance != null &&
            sparse.inventory != null &&
            sparse.stores != null &&
            sparse.jobTasks != null &&
            sparse.npcs != null,
            "save normalizer repairs missing sections");
    }

    private static void TestBusiness()
    {
        var financeState = new FinanceState();
        var finance = new FinanceService(financeState);

        finance.GetOrCreateAccount("business.test", 10000);

        var state = new BusinessWorldState();
        state.businesses.Add(
            new BusinessRecord
            {
                id = "biz.test",
                displayName = "Test Shop",
                cashAccountId = "business.test",
                open = true,
                employeeCount = 3,
                reputation = 10,
                baseDailyRevenueCents = 20000,
                baseDailyExpenseCents = 5000
            });

        var service = new BusinessService(state);
        Money net = service.SimulateDay(
            "biz.test",
            finance,
            1440,
            100);

        Assert(net.Cents > 0, "business positive net");
        Assert(
            state.businesses[0].lifetimeRevenueCents > 0,
            "business revenue recorded");
    }

    private static void TestNpcSchedule()
    {
        var npc = new NpcRecord
        {
            id = "npc.test",
            homeLocationId = "property.home"
        };

        npc.schedule.Add(
            new NpcScheduleEntry
            {
                dayOfWeek = 1,
                startMinute = 480,
                endMinute = 1020,
                locationId = "business.work",
                activity = "work"
            });

        var schedules = new NpcScheduleService();

        Assert(
            schedules.Validate(npc, out string error),
            error ?? "npc schedule validation");

        NpcScheduleEntry work =
            schedules.Resolve(npc, 1, 600);

        Assert(
            work.locationId == "business.work",
            "npc at work");

        NpcScheduleEntry home =
            schedules.Resolve(npc, 1, 1200);

        Assert(
            home.locationId == "property.home",
            "npc at home");
    }

    private static void TestStory()
    {
        var state = new StoryState();
        var story = new StoryService(state);

        story.SetFlag("intro.met_neighbor", true);
        story.SetVariable("downtown.rep", 6);

        var evt = new StoryEventDefinition
        {
            id = "story.test"
        };

        evt.conditions.Add(
            new StoryCondition
            {
                type = StoryConditionType.FlagEquals,
                key = "intro.met_neighbor",
                boolValue = true
            });

        evt.conditions.Add(
            new StoryCondition
            {
                type = StoryConditionType.VariableAtLeast,
                key = "downtown.rep",
                intValue = 5
            });

        evt.actions.Add(
            new StoryAction
            {
                type = StoryActionType.SetFlag,
                key = "story.test.unlocked",
                boolValue = true
            });

        Assert(story.TryTrigger(evt), "story event triggers");
        Assert(
            story.GetFlag("story.test.unlocked"),
            "story action applies");
        Assert(
            !story.TryTrigger(evt),
            "one-shot story event");
    }

    private static void TestReputation()
    {
        var state = new ReputationState();
        var service = new ReputationService(state);

        service.Add("neighborhood:downtown", 5);
        service.Add("neighborhood:downtown", 3);

        Assert(
            service.Get("neighborhood:downtown") == 8,
            "context reputation");
    }

    private static void TestVoxelDeltas()
    {
        var world = new VoxelWorldData();
        world.SetBlock(-1, 0, -1, BlockId.Brick);

        Assert(
            world.GetBlockOrAir(-1, 0, -1) == BlockId.Brick,
            "world negative block");

        var deltaState = new VoxelDeltaState();
        var deltas = new VoxelDeltaService(deltaState);

        deltas.Record(-1, 0, -1, BlockId.Glass);
        deltas.Record(65, 2, 65, BlockId.Wood);

        var restored = new VoxelWorldData();
        deltas.ApplyTo(restored);

        Assert(
            restored.GetBlockOrAir(-1, 0, -1) == BlockId.Glass,
            "restore delta one");

        Assert(
            restored.GetBlockOrAir(65, 2, 65) == BlockId.Wood,
            "restore delta two");
    }

    private static void TestDetroitRasterization()
    {
        double lat = DetroitGeoReference.AnchorLatitude;
        double lon = DetroitGeoReference.AnchorLongitude;

        var document = new DetroitImportDocument
        {
            areaName = "Synthetic Test",
            roads = new[]
            {
                new RoadFeature
                {
                    id = "road.test",
                    widthMeters = 6f,
                    centerline = new[]
                    {
                        new GeoPoint(lat, lon),
                        new GeoPoint(lat, lon + 0.00015)
                    }
                }
            },
            buildings = new[]
            {
                new BuildingFeature
                {
                    id = "building.test",
                    levels = 2,
                    footprint = new[]
                    {
                        new GeoPoint(lat + 0.00004, lon + 0.00004),
                        new GeoPoint(lat + 0.00004, lon + 0.00010),
                        new GeoPoint(lat + 0.00010, lon + 0.00010),
                        new GeoPoint(lat + 0.00010, lon + 0.00004)
                    }
                }
            }
        };

        var world = new VoxelWorldData();
        DetroitFeatureRasterizer.Rasterize(document, world);

        Assert(world.ChunkCount > 0, "detroit raster chunks");

        (int x, int z) =
            DetroitGeoReference.ToWorldVoxel(
                lat,
                lon,
                VoxDetroitConstants.VoxelSizeMeters);

        Assert(
            world.GetBlockOrAir(x, 0, z) == BlockId.Asphalt,
            "detroit road raster");
    }

    private static void Assert(bool condition, string name)
    {
        _assertions++;

        if (!condition)
        {
            throw new InvalidOperationException(
                $"Smoke assertion failed: {name}");
        }
    }
}
