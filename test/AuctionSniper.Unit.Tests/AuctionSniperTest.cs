using AuctionSniper.Domain;
using NMock;
using NUnit.Framework;

namespace AuctionSniper.Unit.Tests {

    [TestFixture]
    public class AuctionSniperTest {

        private MockFactory mockery;
        private Mock<ISniperListener> sniperListener;

        [SetUp]
        public void Before() {
            mockery = new MockFactory();
            sniperListener = mockery.CreateMock<ISniperListener>();
        }

        [TearDown]
        public void After()
        {
            mockery.VerifyAllExpectationsHaveBeenMet();
        }



        [Test]
        public void NotifiesAuctionLostWhenAuctionCloses() {

            sniperListener.Expects.One.Method(x => x.AuctionLost());
            IAuction nullAuction = null;
            var auctionSniper = new Domain.AuctionSniper(sniperListener.MockObject, nullAuction);
            auctionSniper.AuctionClosed();

        }

        [Test]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives() {
            var auction = mockery.CreateMock<IAuction>();
            var currentPrice = 98.Gbp();
            var increment = 20.Gbp();
            auction.Expects.One.Method(x => x.Bid(currentPrice + increment))
                               .With(currentPrice+increment);
            sniperListener.Expects.One.Method(x => x.SniperIsBidding());
            var auctionSniper = new Domain.AuctionSniper(sniperListener.MockObject, auction.MockObject);
            auctionSniper.CurrentPrice(currentPrice, increment);

        }

    }

}