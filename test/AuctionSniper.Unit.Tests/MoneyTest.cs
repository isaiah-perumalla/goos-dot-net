using NUnit.Framework;
using AuctionSniper.Domain;

namespace AuctionSniper.Unit.Tests {
    [TestFixture]
    public class MoneyTest {
        
        [Test]
        public void  AddMoneterayValueWithSameCurrency() {
            
            Assert.That(100.Gbp() + 250.50m.Gbp(), Is.EqualTo(350.50m.Gbp()));
        }
    }
}