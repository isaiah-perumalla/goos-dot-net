using AuctionSniper.Domain;
using NUnit.Framework;

namespace AuctionSniper.Acceptance.Tests {
    [TestFixture]
    public class AuctionSniperEndToEndTests {
        private const string AUCTION_SNIPER_EXE = @"..\..\..\..\src\AuctionSniper.UI\bin\Debug\AuctionSniper.UI.exe";
        private readonly FakeAuctionServer auction = new FakeAuctionServer("item-54321");
        private readonly ApplicationRunner application = new ApplicationRunner(AUCTION_SNIPER_EXE);


        [TestFixtureSetUp]
        public void BeforeFixture() {
            
        }
        [TestFixtureTearDown]
        public void AfterFixture() {
            application.Dispose();
            auction.Dispose();
        }

        [Test]
        public void SniperJoinsAuctionUntilAuctionCloses() {
            auction.StartSellingItem();
            application.StartBiddingIn(auction);
            auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppID);
            auction.AnnounceClosed();
            application.ShowsSniperHasLostAuction();
        }

        [Test]
        public void SniperMakeHigherBidButLoses() {
            auction.StartSellingItem();
            application.StartBiddingIn(auction);
            auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppID);

            auction.ReportPrice(1000.Gbp(), 98.Gbp(), "other bidder");
            application.HasShownSniperIsBidding();
            auction.HasReceivedBid(1098, ApplicationRunner.SniperXmppID);

            auction.AnnounceClosed();

            application.ShowsSniperHasLostAuction();

        }
    }
}