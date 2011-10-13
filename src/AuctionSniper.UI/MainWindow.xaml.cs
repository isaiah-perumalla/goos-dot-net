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
using AuctionSniper.Xmpp;

namespace AuctionSniper.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string XMPP_HOST = "localhost";
        private string RESOURCE = "resource";
        private XmppChatClient xmppClient;
        private string sniper_password = "sniper";

        public MainWindow()
        {
            InitializeComponent();
            xmppClient = new XmppChatClient(new Jid("sniper", XMPP_HOST, RESOURCE));
            xmppClient.OnMessageReceived += (s, msg) => {
                                                if(msg.Type != MessageType.chat) return;
                                                Action action = () => statusLbl.Content = "lost"; 
                                                this.statusLbl.Dispatcher.Invoke(action);
                                            };
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
    }
}
