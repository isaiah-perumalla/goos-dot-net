using System;
using System.Diagnostics;
using NUnit.Framework;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems.Finders;
using Label = White.Core.UIItems.Label;

namespace AuctionSniper.Acceptance.Tests {
    public class ApplicationRunner {
        private readonly Application applicationInstance;
        private string LOST_AUCTION = "lost";

        public ApplicationRunner(string exePath) {
            
            applicationInstance = Application.Launch(exePath);
            
        }

        public string XmppID { get; private set; }

        public void StartBiddingIn(FakeAuctionServer auction) {

         
        }

        public void ShowsSniperHasLostAuction() {
            var mainwindow = applicationInstance.GetWindow(SearchCriteria.ByAutomationId("mainwindow"), InitializeOption.NoCache);
            var status = mainwindow.Get<Label>(SearchCriteria.ByAutomationId("auction_status"));
            Assert.That(status.Text, Is.EqualTo(LOST_AUCTION));
        }

        public void Dispose() {
            applicationInstance.Dispose();
        }
    }
}