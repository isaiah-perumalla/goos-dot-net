using agsXMPP;
using AuctionSniper.Domain;

namespace AuctionSniper.Xmpp {

    public class XmppAuction : IAuction {
        const string JOIN_COMMAND = "SOLVersion: 1.1; Command: JOIN;";
        const string BID_COMMAND = "SOLVersion: 1.1; Command: BID; Price: {0};";
        readonly XmppChatClient xmppClient;
        readonly Jid itemJid;

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

        static string BidFor(Money money) {
            return string.Format(BID_COMMAND, money.Amount);
        }
    }
}