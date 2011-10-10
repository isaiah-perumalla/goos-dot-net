using System;

namespace AuctionSniper.Xmpp
{
    public class XmppException : Exception
    {
        public XmppException(string message)
            : base(message)
        {

        }
    }
}