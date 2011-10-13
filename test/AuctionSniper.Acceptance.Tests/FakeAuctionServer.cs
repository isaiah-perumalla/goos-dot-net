using System;
using System.Collections.Concurrent;
using agsXMPP;
using agsXMPP.protocol.client;
using AuctionSniper.Utils;
using AuctionSniper.Xmpp;
using NUnit.Framework;

namespace AuctionSniper.Acceptance.Tests {
    public class FakeAuctionServer {
        private readonly string _auctionItemId;
        private XmppChatClient auctionChat;
        private string AUCTION_PASSWORD = "auction";
        private SingleMessageListener singleMessageListener = new SingleMessageListener();
        private const string RESOURCE = "auction";
        private const string XMPP_HOST = "localhost";

        public FakeAuctionServer(string auctionItemId) {
            _auctionItemId = auctionItemId;

            auctionChat = new XmppChatClient(new Jid(_auctionItemId, XMPP_HOST, RESOURCE));
            auctionChat.OnMessageReceived += (s, msg) => singleMessageListener.ProcessMessage(msg);
        }

        public string AuctionId {
            get {
                return _auctionItemId;
            }
        }

        public void StartSellingItem() {
            auctionChat.Login(AUCTION_PASSWORD);
        }

        public void HasReceivedJoinRequestFrom(string xmppId) {
            singleMessageListener.ReceivesAMessageFrom(xmppId);
        }

        public void announceClosed() {
         
            auctionChat.SendMessageTo(singleMessageListener.SniperJid, string.Empty);
        }

        public void Dispose() {
            auctionChat.Dispose();
        }
    }

    internal class SingleMessageListener {
        private readonly BlockingCollection<Message> messages = new BlockingCollection<Message>(1);

        public Jid SniperJid { get; private set; }

        public void ProcessMessage(Message msg) {
            messages.Add(msg);
        }

        public void ReceivesAMessageFrom(string xmppId) {
            Message msg;
            TimeSpan timeout = 2.Seconds();
            Assert.That(messages.TryTake(out msg, timeout), String.Format("did not receive message from sniper within {0} seconds", timeout));
            SniperJid = msg.From;
        }
    }
}