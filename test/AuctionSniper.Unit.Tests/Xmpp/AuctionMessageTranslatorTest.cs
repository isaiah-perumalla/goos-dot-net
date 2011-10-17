using agsXMPP.protocol.client;
using AuctionSniper.Domain;
using AuctionSniper.Xmpp;
using NMock;
using NMock;
using NUnit.Framework;

namespace AuctionSniper.Unit.Tests {

    [TestFixture]
    public class AuctionMessageTranslatorTest {
        private MockFactory mockery;
        private Mock<IAuctionEventListener> auctionEventListener;
        private AuctionMessageTranslator auctionMessageTranslator;

        [SetUp]
        public void Before() {
            mockery = new MockFactory();
            auctionEventListener = mockery.CreateMock<IAuctionEventListener>();
            auctionMessageTranslator = new AuctionMessageTranslator(auctionEventListener.MockObject);
        }

        [TearDown]
        public void After() {
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void NotifiesAuctionClosedWhenAuctionClosedMessageReceived() {

            
            auctionEventListener.Expects.One.Method(x => x.AuctionClosed());

            Message auctionClosedMessage = Message(@"SOLVersion: 1.1; Event: CLOSE;");
            auctionMessageTranslator.Process(auctionClosedMessage);
        }

        private static Message Message(string messageBody) {
            return new Message {Body = messageBody};
        }

        [Test]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceived() {
            auctionEventListener.Expects.One.Method(x => x.CurrentPrice(192.Gbp(), 7.Gbp()))
                                            .With(192.Gbp(), 7.Gbp());
                                            

            var auctionPriceMessage =
                Message(@"SOLVersion: 1.1; Event: PRICE; CurrentPrice: 192; Increment: 7; Bidder: Someone else;");
            auctionMessageTranslator.Process(auctionPriceMessage);

        }

    }
}