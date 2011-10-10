using NUnit.Framework;

namespace AuctionSniper.Acceptance.Tests {
    [TestFixture]
    public class AuctionSniperEndToEndTests {
        private readonly FakeAuctionServer auction = new FakeAuctionServer("item-54321");
        private readonly ApplicationRunner _application = new ApplicationRunner(@"..\..\..\..\AuctionSniper.UI\bin\Debug\AuctionSniper.UI.exe");


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