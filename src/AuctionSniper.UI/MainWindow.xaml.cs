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
            var auctionMessageTranslator = new AuctionMessageTranslator(new Domain.AuctionSniper(this, null));
            xmppClient.OnChatMessageReceived += (s, msg) => auctionMessageTranslator.Process(msg);
            xmppClient.Login(sniper_password);
            
        }

        private void join_auction_cliked(object sender, RoutedEventArgs e) {
            try
            {
                
                xmppClient.SendMessageTo(new Jid(auctionIdTxt.Text, XMPP_HOST, RESOURCE), string.Empty);
                this.statusLbl.Content = "joining";
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
            
        }
    }
}
