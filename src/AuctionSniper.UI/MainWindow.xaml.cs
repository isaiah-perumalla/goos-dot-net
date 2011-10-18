using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using agsXMPP;
using agsXMPP.protocol.client;
using AuctionSniper.Domain;
using AuctionSniper.Xmpp;

namespace AuctionSniper.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ISniperListener {
        private string XMPP_HOST = "localhost";
        private string RESOURCE = "resource";
        private XmppChatClient xmppClient;
        private string sniper_password = "sniper";

        public MainWindow()
        {
            InitializeComponent();
            xmppClient = new XmppChatClient(new Jid("sniper", XMPP_HOST, RESOURCE));
            
            
        }

        private void join_auction_cliked(object sender, RoutedEventArgs e) {
            try
            {

               /* var fake = new XmppChatClient(new Jid("item-54321", "localhost", "auction"));
                fake.Login("auction");*/
                xmppClient.Login(sniper_password);
                var itemJid = new Jid(auctionIdTxt.Text, XMPP_HOST, RESOURCE);
                IAuction auction = new XmppAuction(xmppClient, itemJid);
                var auctionMessageTranslator = new AuctionMessageTranslator(new Domain.AuctionSniper(this, auction));
                xmppClient.OnChatMessageReceived += (s, msg) => auctionMessageTranslator.Process(msg);

                xmppClient.SendMessageTo(itemJid, "SOLVersion: 1.1; Command: JOIN;");
                this.statusLbl.Content = "joining";
//                var message = string.Format(@"SOLVersion: 1.1; Event: PRICE; CurrentPrice: {0}; Increment: {1}; Bidder: {2};",
//                                            100m, 20m, "some one else");
                
//                fake.SendMessageTo(new Jid("sniper", XMPP_HOST, RESOURCE), message);
            }
            catch(XmppException ex)
            {
                statusLbl.Content = ex.Message;
            }
        }

        
        public void AuctionLost() {
            Action action = () => statusLbl.Content = "lost";
            this.statusLbl.Dispatcher.Invoke(action);
        }

        public void SniperIsBidding() {
            Action action = () => statusLbl.Content = "bidding";
            this.statusLbl.Dispatcher.Invoke(action);
        }
    }

    public class XmppAuction : IAuction {
        private readonly XmppChatClient xmppClient;
        private readonly Jid itemJid;

        public XmppAuction(XmppChatClient xmppClient, Jid itemJid) {
            this.xmppClient = xmppClient;
            this.itemJid = itemJid;

        }

        public void Bid(Money bidAmount) {
            xmppClient.SendMessageTo(itemJid, BidFor(bidAmount));
        }

        private string BidFor(Money money) {
            return string.Format("SOLVersion: 1.1; Command: BID; Price: {0};", money.Amount);
        }
    }
}
