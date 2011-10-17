using System;

namespace AuctionSniper.Domain
{
    public interface IAuctionEventListener
    {
        void AuctionClosed();
        void CurrentPrice(Money currentPrice, Money increment);
    }

    public struct Money {
        private readonly decimal amount;
        private readonly string currencyCode;

        public Money(decimal amount, string currencyCode) {
            this.amount = amount;
            this.currencyCode = currencyCode;
        }

        public decimal Amount {
            get { return this.amount; }
        }

        public  static Money operator + (Money amt1, Money amt2) {
            return new Money(amt1.Amount+amt1.Amount, amt1.currencyCode);
        }
        public override string ToString() {
            return string.Format("Amount: {0}{1}", amount, currencyCode);
        }
    }

    public static class MoneyExtension {
        public static Money Gbp(this decimal amount) {
            return new Money(amount, "GBP");
        }


        public static Money AsMoney(this string amount, string currency) {
            return new Money(Decimal.Parse(amount), currency);
        }

        public static Money Gbp(this int amount)
        {
            return new Money(amount, "GBP");
        }
    }
}