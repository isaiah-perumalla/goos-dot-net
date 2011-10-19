using System;
using System.Collections.Concurrent;
using agsXMPP;
using agsXMPP.protocol.client;
using AuctionSniper.Domain;
using AuctionSniper.Utils;
using AuctionSniper.Xmpp;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace AuctionSniper.Acceptance.Tests {
    public class FakeAuctionServer {
        private readonly string _auctionItemId;
        private XmppChatClient auctionChat;
        private string AUCTION_PASSWORD = "auction";
        private SingleMessageListener singleMessageListener = new SingleMessageListener();
        private const string BID_COMMAND = "SOLVersion: 1.1; Command: BID; Price: {0};";
        private const string JOIN_COMMAND = "SOLVersion: 1.1; Command: JOIN;";
        private const string RESOURCE = "auction";
        private const string XMPP_HOST = "localhost";

        public FakeAuctionServer(string auctionItemId) {
            _auctionItemId = auctionItemId;

            auctionChat = new XmppChatClient(new Jid(_auctionItemId, XMPP_HOST, RESOURCE), AUCTION_PASSWORD);
            auctionChat.OnChatMessageReceived += (s, msg) => singleMessageListener.ProcessMessage(msg);
        }

        public string AuctionId {
            get {
                return _auctionItemId;
            }
        }

        public void StartSellingItem() {
            
            auctionChat.Login();
        }

        public void HasReceivedJoinRequestFrom(string xmppId) {
            object join_message;
            singleMessageListener.ReceivesAMessageFrom(xmppId, Is.EqualTo(JOIN_COMMAND), "did not receive join message");
        }

        public void AnnounceClosed() {
         
            auctionChat.SendMessageTo(singleMessageListener.SniperJid, @"SOLVersion: 1.1; Event: CLOSE;");
            auctionChat.Close();
        }

        public void Dispose() {
            auctionChat.Dispose();
        }

        public void ReportPrice(Money price, Money increment, string bidderId) {
            var priceMsg =
                string.Format(@"SOLVersion: 1.1; Event: PRICE; CurrentPrice: {0}; Increment: {1}; Bidder: {2};",
                              price.Amount, increment.Amount, bidderId);
            auctionChat.SendMessageTo(singleMessageListener.SniperJid, priceMsg);
        }

        public void HasReceivedBid(Money price, string sniperId) {
            string bid_message = string.Format(BID_COMMAND, price.Amount);
            singleMessageListener.ReceivesAMessageFrom(sniperId, Is.EqualTo(bid_message), "did not receive bid message");
        }
    }

    internal class SingleMessageListener {
        private readonly BlockingCollection<Message> messages = new BlockingCollection<Message>(1);

        public Jid SniperJid { get; private set; }

        public void ProcessMessage(Message msg) {
            messages.Add(msg);
        }

        public void ReceivesAMessageFrom(string sniperXmppId, Constraint constraint, string message) {
            Message msg;
            TimeSpan timeout = 4.Seconds();
            Assert.That(messages.TryTake(out msg, timeout), String.Format("did not receive message from sniper within {0} seconds", timeout));
            Assert.That(msg.From.User, Is.EqualTo(sniperXmppId), "message was not from {0}", sniperXmppId);
            Assert.That(msg.Body, constraint, message);
            SniperJid = msg.From;
        }
    }
}