namespace AuctionSniper.Domain {
    public interface IAuction
    {
        void Bid(Money bidAmount);
    }
}