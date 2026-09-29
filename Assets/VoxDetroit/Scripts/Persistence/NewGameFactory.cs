using System;
using VoxDetroit.Economy;

namespace VoxDetroit.Persistence
{
    [Serializable]
    public sealed class NewGameSettings
    {
        public long startingCashCents = 10000;
        public long startingCheckingCents = 50000;
        public int worldSeed = 42701;
    }

    public static class NewGameFactory
    {
        public static VoxDetroitSaveData Create(
            NewGameSettings settings = null)
        {
            settings = settings ?? new NewGameSettings();

            string now = DateTime.UtcNow.ToString("O");

            var data = new VoxDetroitSaveData
            {
                saveId = Guid.NewGuid().ToString("N"),
                createdUtc = now,
                updatedUtc = now,
                worldSeed = settings.worldSeed
            };

            data.finance.accounts.Add(
                new AccountState
                {
                    id = AccountIds.PlayerCash,
                    balanceCents = Math.Max(
                        0,
                        settings.startingCashCents)
                });

            data.finance.accounts.Add(
                new AccountState
                {
                    id = AccountIds.PlayerChecking,
                    balanceCents = Math.Max(
                        0,
                        settings.startingCheckingCents)
                });

            data.finance.accounts.Add(
                new AccountState
                {
                    id = AccountIds.PlayerSavings,
                    balanceCents = 0
                });

            return data;
        }
    }
}
