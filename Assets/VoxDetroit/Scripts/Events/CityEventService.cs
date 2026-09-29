using System;
using System.Collections.Generic;

namespace VoxDetroit.Events
{
    public sealed class CityEventService
    {
        private readonly CityEventWorldState _state;

        public CityEventService(CityEventWorldState state)
        {
            _state = state ??
                throw new ArgumentNullException(nameof(state));
        }

        public IReadOnlyList<ScheduledCityEvent> GetActive(
            long gameMinute)
        {
            var active = new List<ScheduledCityEvent>();

            foreach (ScheduledCityEvent cityEvent in _state.events)
            {
                if (IsActive(cityEvent, gameMinute))
                {
                    active.Add(cityEvent);
                }
            }

            return active;
        }

        public bool IsActive(
            ScheduledCityEvent cityEvent,
            long gameMinute)
        {
            return cityEvent != null &&
                   !cityEvent.cancelled &&
                   cityEvent.endMinute > cityEvent.startMinute &&
                   gameMinute >= cityEvent.startMinute &&
                   gameMinute < cityEvent.endMinute;
        }

        public CrowdTier GetCrowdTier(
            ScheduledCityEvent cityEvent)
        {
            if (cityEvent == null)
            {
                return CrowdTier.Sparse;
            }

            int attendance =
                Math.Max(0, cityEvent.expectedAttendance);

            if (attendance >= 50000)
            {
                return CrowdTier.Massive;
            }

            if (attendance >= 15000)
            {
                return CrowdTier.Packed;
            }

            if (attendance >= 5000)
            {
                return CrowdTier.Busy;
            }

            if (attendance >= 1000)
            {
                return CrowdTier.Light;
            }

            return CrowdTier.Sparse;
        }

        public EventDemandModifiers GetCombinedDemand(
            long gameMinute)
        {
            var result = new EventDemandModifiers();

            foreach (ScheduledCityEvent cityEvent in _state.events)
            {
                if (!IsActive(cityEvent, gameMinute))
                {
                    continue;
                }

                ApplyMaximum(
                    result,
                    cityEvent.demand);
            }

            return result;
        }

        public ScheduledCityEvent Find(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                return null;
            }

            foreach (ScheduledCityEvent cityEvent in _state.events)
            {
                if (cityEvent != null &&
                    string.Equals(
                        cityEvent.id,
                        eventId,
                        StringComparison.Ordinal))
                {
                    return cityEvent;
                }
            }

            return null;
        }

        public bool MarkPlayerAttended(string eventId)
        {
            ScheduledCityEvent cityEvent = Find(eventId);

            if (cityEvent == null)
            {
                return false;
            }

            cityEvent.playerAttended = true;
            return true;
        }

        private static void ApplyMaximum(
            EventDemandModifiers target,
            EventDemandModifiers source)
        {
            if (source == null)
            {
                return;
            }

            target.trafficPercent =
                Math.Max(
                    target.trafficPercent,
                    source.trafficPercent);

            target.taxiDemandPercent =
                Math.Max(
                    target.taxiDemandPercent,
                    source.taxiDemandPercent);

            target.deliveryDemandPercent =
                Math.Max(
                    target.deliveryDemandPercent,
                    source.deliveryDemandPercent);

            target.foodDemandPercent =
                Math.Max(
                    target.foodDemandPercent,
                    source.foodDemandPercent);

            target.retailDemandPercent =
                Math.Max(
                    target.retailDemandPercent,
                    source.retailDemandPercent);

            target.securityDemandPercent =
                Math.Max(
                    target.securityDemandPercent,
                    source.securityDemandPercent);

            target.hotelDemandPercent =
                Math.Max(
                    target.hotelDemandPercent,
                    source.hotelDemandPercent);

            target.undergroundOpportunityPercent =
                Math.Max(
                    target.undergroundOpportunityPercent,
                    source.undergroundOpportunityPercent);
        }
    }
}
