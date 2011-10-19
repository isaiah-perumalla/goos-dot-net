using AuctionSniper.Domain;
using NUnit.Framework;

namespace AuctionSniper.Acceptance.Tests {
    [TestFixture]
    public class AuctionSniperEndToEndTests {
        private const string AUCTION_SNIPER_EXE = @"..\..\..\..\src\AuctionSniper.UI\bin\Debug\AuctionSniper.UI.exe";
        private readonly FakeAuctionServer auction = new FakeAuctionServer("item-54321");
        private ApplicationRunner application ;


        [SetUp]
        public void BeforeTest() {
            application = new ApplicationRunner(AUCTION_SNIPER_EXE);
        }
        [TearDown]
        public void AfterTest() {
            application.Dispose();
            
        }

        [TestFixtureTearDown]
        public void AfterFixture() {
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
            auction.HasReceivedBid(1098.Gbp(), ApplicationRunner.SniperXmppID);
            
            
            auction.AnnounceClosed();
            application.ShowsSniperHasLostAuction();

        }
        
        [Test]
        public void SniperWinsAnAuctionByMakeHigherBid() {
            auction.StartSellingItem();
            application.StartBiddingIn(auction);
            auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppID);

            auction.ReportPrice(1000.Gbp(), 98.Gbp(), "other bidder");
            application.HasShownSniperIsBidding();
            auction.HasReceivedBid(1098.Gbp(), ApplicationRunner.SniperXmppID);
            auction.ReportPrice(1098.Gbp(), 97.Gbp(), ApplicationRunner.SniperXmppID);

            application.HasShownSniperIsWinning();
            auction.AnnounceClosed();
            application.ShowsSniperHasWonAuction();

        }

       
    }
}