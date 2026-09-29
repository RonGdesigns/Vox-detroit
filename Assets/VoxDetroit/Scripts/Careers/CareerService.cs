using System;

namespace VoxDetroit.Careers
{
    public sealed class CareerService
    {
        private readonly CareerWorldState _state;

        public CareerService(CareerWorldState state)
        {
            _state = state ??
                throw new ArgumentNullException(nameof(state));
        }

        public IncomePathProgress GetOrCreate(string pathId)
        {
            if (string.IsNullOrWhiteSpace(pathId))
            {
                throw new ArgumentException(
                    "Career path ID is required.",
                    nameof(pathId));
            }

            foreach (IncomePathProgress progress in _state.paths)
            {
                if (progress != null &&
                    string.Equals(
                        progress.pathId,
                        pathId,
                        StringComparison.Ordinal))
                {
                    return progress;
                }
            }

            var created = new IncomePathProgress
            {
                pathId = pathId
            };

            _state.paths.Add(created);
            return created;
        }

        public void Unlock(string pathId)
        {
            GetOrCreate(pathId).unlocked = true;
        }

        public bool IsUnlocked(string pathId)
        {
            return GetOrCreate(pathId).unlocked;
        }

        public void AddExperience(string pathId, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            IncomePathProgress progress =
                GetOrCreate(pathId);

            progress.experience =
                checked(progress.experience + amount);
        }

        public void AddReputation(string pathId, int amount)
        {
            IncomePathProgress progress =
                GetOrCreate(pathId);

            progress.reputation =
                checked(progress.reputation + amount);
        }

        public void AddTrust(string pathId, int amount)
        {
            IncomePathProgress progress =
                GetOrCreate(pathId);

            progress.trust =
                Math.Max(
                    0,
                    checked(progress.trust + amount));
        }

        public void AddHeat(string pathId, int amount)
        {
            IncomePathProgress progress =
                GetOrCreate(pathId);

            progress.heat =
                Math.Max(
                    0,
                    checked(progress.heat + amount));
        }

        public void CoolHeat(string pathId, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            IncomePathProgress progress =
                GetOrCreate(pathId);

            progress.heat =
                Math.Max(0, progress.heat - amount);
        }
    }
}
