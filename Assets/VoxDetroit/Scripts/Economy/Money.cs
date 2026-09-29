using System;
using System.Globalization;

namespace VoxDetroit.Economy
{
    public readonly struct Money : IEquatable<Money>, IComparable<Money>
    {
        public long Cents { get; }

        public Money(long cents)
        {
            Cents = cents;
        }

        public static Money Zero => new Money(0);

        public static Money FromDollars(decimal dollars)
        {
            return new Money(
                checked((long)decimal.Round(
                    dollars * 100m,
                    0,
                    MidpointRounding.AwayFromZero)));
        }

        public decimal ToDollars()
        {
            return Cents / 100m;
        }

        public int CompareTo(Money other)
        {
            return Cents.CompareTo(other.Cents);
        }

        public bool Equals(Money other)
        {
            return Cents == other.Cents;
        }

        public override bool Equals(object obj)
        {
            return obj is Money other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Cents.GetHashCode();
        }

        public override string ToString()
        {
            return ToDollars().ToString("C", CultureInfo.CurrentCulture);
        }

        public static Money operator +(Money a, Money b)
        {
            return new Money(checked(a.Cents + b.Cents));
        }

        public static Money operator -(Money a, Money b)
        {
            return new Money(checked(a.Cents - b.Cents));
        }

        public static bool operator >=(Money a, Money b)
        {
            return a.Cents >= b.Cents;
        }

        public static bool operator <=(Money a, Money b)
        {
            return a.Cents <= b.Cents;
        }

        public static bool operator >(Money a, Money b)
        {
            return a.Cents > b.Cents;
        }

        public static bool operator <(Money a, Money b)
        {
            return a.Cents < b.Cents;
        }
    }
}
