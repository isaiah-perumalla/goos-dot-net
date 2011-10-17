using System;
using System.Collections.Generic;
using System.Linq;
using agsXMPP.protocol.client;
using AuctionSniper.Domain;

namespace AuctionSniper.Xmpp
{
    public class AuctionMessageTranslator
    {
        private const string defaultCurrency = "GBP";
        private readonly IAuctionEventListener auctionEventListener;

        public AuctionMessageTranslator(IAuctionEventListener auctionEventListener)
        {
            this.auctionEventListener = auctionEventListener;
        }

        public void Process(Message message) {
            var msgMap = Unpack(message.Body);
            var auctionEvent = msgMap["Event"];
            if("CLOSE".Equals(auctionEvent))
                auctionEventListener.AuctionClosed();
            else if("PRICE".Equals(auctionEvent))
            {
                var currentPrice = msgMap["CurrentPrice"].AsMoney(defaultCurrency);
                var increment = msgMap["Increment"].AsMoney(defaultCurrency);
                auctionEventListener.CurrentPrice(currentPrice, increment);

            }
        }

        private static Dictionary<string , string> Unpack(string body) {
            var messageComponents = body.Split(new[] {';'});
            var auctionEventMap = messageComponents.Select(msgComponent => msgComponent.Split(new[] {':'}))
                                                   .Where(pair => pair.Length == 2)
                                                   .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
            return auctionEventMap;
        }
    }
}