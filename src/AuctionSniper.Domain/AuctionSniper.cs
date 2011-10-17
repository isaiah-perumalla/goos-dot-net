using System;

namespace AuctionSniper.Domain
{
    public class AuctionSniper : IAuctionEventListener
    {
        private readonly ISniperListener sniperListener;
        private readonly IAuction auction;

        public AuctionSniper(ISniperListener sniperListener, IAuction auction) {
            this.sniperListener = sniperListener;
            this.auction = auction;
        }

        public void AuctionClosed()
        {
            sniperListener.AuctionLost();
        }

        public void CurrentPrice(Money currentPrice, Money increment)
        {
            auction.Bid(currentPrice+increment);
            sniperListener.SniperIsBidding();
        }
    }
}