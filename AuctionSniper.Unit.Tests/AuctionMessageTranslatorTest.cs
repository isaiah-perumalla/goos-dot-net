using System;
using agsXMPP.protocol.client;
using NUnit.Framework;

namespace AuctionSniper.Unit.Tests {

    [TestFixture]
    public class AuctionMessageTranslatorTest {

        [Test]
        public void NotifiesAuctionClosedWhenAuctionClosedMessageReceived() {

            var auctionMessageTranslator = new AuctionMessageTranslator();

            string closeMessage = @"";
            Message auctionClosedMessage = new Message(){Body = closeMessage};
            auctionMessageTranslator.Process(auctionClosedMessage);
        }
        
    }

    public class AuctionMessageTranslator {
        public void Process(Message auctionClosedMessage) {
            throw new NotImplementedException();
        }
    }
}