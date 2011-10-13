using NUnit.Framework;

namespace AuctionSniper.Acceptance.Tests {
    [TestFixture]
    public class AuctionSniperEndToEndTests {
        private const string AUCTION_SNIPER_EXE = @"..\..\..\..\src\AuctionSniper.UI\bin\Debug\AuctionSniper.UI.exe";
        private readonly FakeAuctionServer auction = new FakeAuctionServer("item-54321");
        private readonly ApplicationRunner _application = new ApplicationRunner(AUCTION_SNIPER_EXE);


        [TestFixtureSetUp]
        public void BeforeFixture() {
            
        }
        [TestFixtureTearDown]
        public void AfterFixture() {
            _application.Dispose();
            auction.Dispose();
        }

        [Test]
        public void SniperJoinsAuctionUntilAuctionCloses() {
            auction.StartSellingItem();
            _application.StartBiddingIn(auction);
            auction.HasReceivedJoinRequestFrom(_application.XmppID);
            auction.announceClosed();
            _application.ShowsSniperHasLostAuction();
        }
    }
}