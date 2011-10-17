namespace AuctionSniper.Domain {
    public interface ISniperListener
    {
        void AuctionLost();
        void SniperIsBidding();
    }
}