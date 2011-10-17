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
            var auctionSniper = new Domain.AuctionSniper(sniperListener.MockObject);
            auctionSniper.AuctionClosed();

        }
    }

    
}