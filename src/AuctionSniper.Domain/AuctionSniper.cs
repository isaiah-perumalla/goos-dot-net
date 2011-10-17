using System;

namespace AuctionSniper.Domain
{
    public class AuctionSniper : IAuctionEventListener
    {
        private readonly ISniperListener sniperListener;

        public AuctionSniper(ISniperListener sniperListener)
        {
            this.sniperListener = sniperListener;
        }

        public void AuctionClosed()
        {
            sniperListener.AuctionLost();
        }

        public void CurrentPrice(Money currentPrice, Money increment)
        {
            throw new NotImplementedException();
        }
    }
}