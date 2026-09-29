using System;

namespace VoxDetroit.Events
{
    [Serializable]
    public sealed class CrowdRepresentationPlan
    {
        public int fullNpcCount;
        public int lightweightAgentCount;
        public int visualCrowdCount;
        public int queueAgentBudget;
        public int vendorAgentBudget;
    }

    public static class CrowdRepresentationPlanner
    {
        public static CrowdRepresentationPlan Build(
            int expectedAttendance,
            int performanceBudgetPercent = 100)
        {
            int attendance =
                Math.Max(0, expectedAttendance);

            int budget =
                Math.Max(
                    25,
                    Math.Min(
                        150,
                        performanceBudgetPercent));

            int full =
                Math.Min(
                    attendance,
                    (120 * budget) / 100);

            int lightweight =
                Math.Min(
                    Math.Max(
                        0,
                        attendance - full),
                    (900 * budget) / 100);

            int visual =
                Math.Max(
                    0,
                    attendance - full - lightweight);

            return new CrowdRepresentationPlan
            {
                fullNpcCount = full,
                lightweightAgentCount = lightweight,
                visualCrowdCount = visual,
                queueAgentBudget =
                    Math.Min(
                        80,
                        Math.Max(8, full / 2)),
                vendorAgentBudget =
                    Math.Min(
                        30,
                        Math.Max(4, full / 6))
            };
        }
    }

    [Serializable]
    public sealed class EventOverlayPlan
    {
        public int stageCount;
        public int vendorBooths;
        public int securityCheckpoints;
        public int temporaryBarrierSegments;
        public int portableLightRigs;
        public bool tailgateZone;
        public bool temporaryParkingControl;
    }

    public static class EventOverlayPlanner
    {
        public static EventOverlayPlan ForFestival(
            CrowdTier crowdTier,
            int stageCount)
        {
            int scale =
                (int)crowdTier + 1;

            return new EventOverlayPlan
            {
                stageCount =
                    Math.Max(1, stageCount),
                vendorBooths = 8 * scale,
                securityCheckpoints =
                    Math.Max(2, 2 * scale),
                temporaryBarrierSegments =
                    20 * scale,
                portableLightRigs =
                    4 * scale,
                tailgateZone = false,
                temporaryParkingControl = true
            };
        }

        public static EventOverlayPlan ForSportsGame(
            int expectedAttendance)
        {
            int scale =
                expectedAttendance >= 50000 ? 5 :
                expectedAttendance >= 20000 ? 4 :
                expectedAttendance >= 10000 ? 3 :
                2;

            return new EventOverlayPlan
            {
                stageCount = 0,
                vendorBooths = 10 * scale,
                securityCheckpoints = 3 * scale,
                temporaryBarrierSegments = 18 * scale,
                portableLightRigs = 2 * scale,
                tailgateZone = expectedAttendance >= 20000,
                temporaryParkingControl = true
            };
        }
    }
}
