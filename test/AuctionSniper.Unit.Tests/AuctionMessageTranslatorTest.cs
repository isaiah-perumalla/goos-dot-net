using agsXMPP.protocol.client;
using NMock2;
using NUnit.Framework;

namespace AuctionSniper.Unit.Tests {

    [TestFixture]
    public class AuctionMessageTranslatorTest {

        [Test]
        public void NotifiesAuctionClosedWhenAuctionClosedMessageReceived() {

            var mockery = new Mockery();
            var auctionEventListener = mockery.NewMock<IAuctionEventListener>();
            var auctionMessageTranslator = new AuctionMessageTranslator();
            Expect.Once.On(auctionEventListener).Method("AuctionClosed");

            const string closeMessage = @"SOLVersion: 1.1; Event: CLOSE;";
            var auctionClosedMessage = new Message {Body = closeMessage};
            auctionMessageTranslator.Process(auctionClosedMessage);
        }
        
    }

    public interface IAuctionEventListener {
    }

    public class AuctionMessageTranslator {
        public void Process(Message auctionClosedMessage) {
            
        }
    }
}