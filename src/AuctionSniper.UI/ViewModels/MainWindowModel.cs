using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using agsXMPP;
using AuctionSniper.Domain;
using AuctionSniper.Xmpp;

namespace AuctionSniper.UI.ViewModels {
    public class MainWindowModel :  INotifyPropertyChanged, ISniperListener {
        private readonly SynchronizationContext uiSynchronizationContext;
        public event PropertyChangedEventHandler PropertyChanged;
        private string XMPP_HOST = "localhost";
        private string RESOURCE = "resource";
        private XmppChatClient xmppClient;
        private string sniper_password = "sniper";

        public MainWindowModel(SynchronizationContext uiSynchronizationContext) {
            this.uiSynchronizationContext = uiSynchronizationContext;
            xmppClient = new XmppChatClient(new Jid("sniper", XMPP_HOST, RESOURCE), sniper_password);
            this.joinCommand = new UICommand(() => JoinAuction());
        }

        void JoinAuction() {
                xmppClient.Login();
                var itemJid = new Jid(this.ItemId, XMPP_HOST, RESOURCE);
                IAuction auction = new XmppAuction(xmppClient, itemJid);
                var auctionMessageTranslator = new AuctionMessageTranslator(new Domain.AuctionSniper(this, auction));
                xmppClient.OnChatMessageReceived += (s, msg) => auctionMessageTranslator.Process(msg);
                auction.Join();
            ShowStatus("joining");

        }

        public string ItemId { get; set; }

        public void AuctionLost() {
            ShowStatus("lost");
        }

        private void ShowStatus(string sts) {
            this.uiSynchronizationContext.Send(x => this.Status = sts, null);
        }
        public ICommand JoinCommand {
            get { return this.joinCommand; }
        }
        private string status;
        private ICommand joinCommand;

        public string Status {
            get { return status; }
            set {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        private void NotifyPropertyChanged(string propertyName) {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SniperIsBidding()
        {
            ShowStatus("bidding");
        }

        
    }

    public class UICommand : ICommand {
        private readonly Action action;

        public UICommand(Action action) {
            this.action = action;
        }

        public void Execute(object parameter) {
            action();
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}