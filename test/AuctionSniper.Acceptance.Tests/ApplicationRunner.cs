using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using White.Core.UIItems.WindowItems;
using Label = White.Core.UIItems.Label;
using AuctionSniper.Utils;

namespace AuctionSniper.Acceptance.Tests {
    public class ApplicationRunner {
        private readonly Application applicationInstance;
        private const string LOST_AUCTION = "lost";
        private Window _mainwindow;
        public const string SniperXmppID = "sniper";
        private const string JOINING = "joining";

        public ApplicationRunner(string exePath) {
            
            applicationInstance = Application.Launch(exePath);
            _mainwindow = applicationInstance.GetWindow(SearchCriteria.ByAutomationId("mainwindow"),
                                                        InitializeOption.NoCache);

        }


        public void StartBiddingIn(FakeAuctionServer auction) {
            var joinAuctionButton = Get<Button>("join_button");
            var auctionIdTextbox = _mainwindow.Get<TextBox>(SearchCriteria.ByAutomationId("auctionid_txtbox"));
            auctionIdTextbox.Text = auction.AuctionId;
            joinAuctionButton.Click();
            WaitUntil(()=> Status.Text, Is.EqualTo(JOINING), 3.Seconds(), "Auction status");

        }

        private void WaitUntil(Func<object > getValue, Constraint constraint, TimeSpan timeout, string message) {
            var start = DateTime.Now;
            while(DateTime.Now - start <= timeout)
                if(constraint.Matches(getValue())) return;
            Assert.That(getValue(), constraint, message );
        }

        private T Get<T>(string automationId) where T : UIItem {
            var uiItem = _mainwindow.Get<T>(SearchCriteria.ByAutomationId(automationId));
            Assert.NotNull(uiItem, String.Format("could not find ui element with automation id '{0}' on main window", automationId));
            return uiItem;
        }

        protected Label Status {
            get { return Get<Label>("auction_status"); }
        }

        public void ShowsSniperHasLostAuction() {
            WaitUntil(() => Status.Text, Is.EqualTo(LOST_AUCTION), 3.Seconds(), "Auction status lost");
        }

        public void Dispose() {
            applicationInstance.Dispose();
        }

        public void HasShownSniperIsBidding() {
            
        }
    }
}