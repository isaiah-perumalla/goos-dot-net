using agsXMPP;
using AuctionSniper.Domain;

namespace AuctionSniper.Xmpp {
    public class XmppAuction : IAuction {
        private const string JOIN_COMMAND = "SOLVersion: 1.1; Command: JOIN;";
        private const string BID_COMMAND = "SOLVersion: 1.1; Command: BID; Price: {0};";
        private readonly XmppChatClient xmppClient;
        private readonly Jid itemJid;

        public XmppAuction(XmppChatClient xmppClient, Jid itemJid) {
            this.xmppClient = xmppClient;
            this.itemJid = itemJid;

        }

        public void Bid(Money bidAmount) {
            xmppClient.SendMessageTo(itemJid, BidFor(bidAmount));
        }

        public void Join() {
            xmppClient.SendMessageTo(itemJid, JOIN_COMMAND);
        }

        private static string BidFor(Money money) {
            return string.Format(BID_COMMAND, money.Amount);
        }
    }
}