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
            var auctionEvent = AuctionEvent.From(message.Body);
            var eventType = auctionEvent.EventType;
            if("CLOSE".Equals(eventType))
                auctionEventListener.AuctionClosed();
            else if("PRICE".Equals(eventType))
            {
                var currentPrice = auctionEvent.CurrentPrice;
                var increment = auctionEvent.Increment;
                auctionEventListener.CurrentPrice(currentPrice, increment);

            }
        }

        class AuctionEvent {
            private readonly Dictionary<string, string> eventValues;

            AuctionEvent(string message) {
                this.eventValues = Unpack(message);
            }

            public string EventType { get { return eventValues["Event"]; }}

            public Money CurrentPrice {
                get { return eventValues["CurrentPrice"].AsMoney(defaultCurrency);
                }
            }
            public Money Increment {
                get
                {
                    return eventValues["Increment"].AsMoney(defaultCurrency); ;
                }
            }

            private static Dictionary<string, string> Unpack(string body)
            {
                var fields = FieldsIn(body);
                var auctionEventMap = fields.Select(KeyValuePair)
                                            .Where(pair => pair.Length == 2)
                                            .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
                return auctionEventMap;
            }

            private static string[] KeyValuePair(string field) {
                return field.Split(new[] {':'});
            }

            private static IEnumerable<string> FieldsIn(string body) {
                return body.Split(new[] { ';' });
            }

            public static AuctionEvent From(string message) {
                return new AuctionEvent(message);
            }
        }

    }
}