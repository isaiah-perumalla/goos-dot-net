using System;
using System.Threading;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using NUnit.Framework;
using agsXMPP;
using Xmpp;

namespace AuctionSniper.Integration.Tests.Xmpp
{
    [TestFixture]
    public class XmppConnectionTests
    {
        const int default_xmpp_port = 5222;
        private const string XMPP_HOST = "localhost";

        [Test]
        [ExpectedException(typeof(XmppException))]
        public void ThrowsExceptionWhenConnectionCannotBeMade() {
            string INVALID_HOST = "invalidhost";
            var connection = new XConnection(INVALID_HOST);
            connection.Login("testuser", "pass", "auction");
          

        } 
        
        [Test]
        public void LoinToValidXMPPServer() {
            var connection = new XConnection(XMPP_HOST);
            connection.Login("testuser", "pass", "auction");
        }

        [Test]
        public void ConnectsToAnXmppServer() {
            XmppClientConnection xmppConnection = createXmppConnection(XMPP_HOST);
            var hasLoggedIn = new ManualResetEvent(false);
            xmppConnection.OnLogin += e => hasLoggedIn.Set();
            xmppConnection.Open("auction-item1", "auction", "auction");
            
            Assert.That(hasLoggedIn.WaitOne(1.Seconds()), "did not log in, timeout");


        }

        
        [Test]
        public void canSendReceiveMessages()
        {
            const string xmppHost = "localhost"         ;
            XmppClientConnection xmppConnection = createXmppConnection(xmppHost);
            var receivedMessageEvent = new ManualResetEvent(false);
           
            xmppConnection.OnMessage += (s, m) => {
                                            Console.WriteLine(m.ToString());
                                            receivedMessageEvent.Set();
                                        };
           
            xmppConnection.Open("auction-item1", "auction", "auction");

           

            XmppClientConnection xmppConnection1 = createXmppConnection(xmppHost);
            var loggedInEvent = new ManualResetEvent(false);
            xmppConnection1.OnLogin += o => loggedInEvent.Set();
            xmppConnection1.Open("testuser", "pass", "auction");
            Assert.That(loggedInEvent.WaitOne(2.Seconds()), "Could not login");


            var msg = new Message
            {
                Type = MessageType.chat,
                To = new Jid("auction-item1", "localhost", "auction"),
                Body = "hello"
                
            };
            xmppConnection1.Send(msg);


            Assert.That(receivedMessageEvent.WaitOne(3.Seconds()), "did not receive message, timeout");


        }

        void xmppConnection_OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            throw new NotImplementedException();
        }

        private XmppClientConnection createXmppConnection(string xmppHost) {
            var xmppConnection = new XmppClientConnection {
                                                              Server = "localhost",
                                                              ConnectServer = xmppHost,
                                                              AutoResolveConnectServer = false,
                                                              Port = default_xmpp_port,
                                                          
                                                          };
            xmppConnection.OnClose += XmppCon_OnClose;
            xmppConnection.OnSocketError += XmppCon_OnSocketError;
            xmppConnection.OnStreamError += XmppCon_OnStreamError;
         
            
                        

            xmppConnection.OnAuthError += (o, e) => Console.WriteLine("loging failed {0}", e.ToString());
            return xmppConnection;
        }

        private static void XmppCon_OnStreamError(object sender, Element e)
        {
            Console.WriteLine("stream error \n {0}", e);
        }

        private static void XmppCon_OnSocketError(object sender, Exception ex)
        {
            Console.WriteLine("socket error \n {0}", ex.Message);
        }

        private static void XmppCon_OnClose(object sender)
        {
            Console.WriteLine("Connection closed");
        }
    }

    public class XConnection {
        private readonly XmppClientConnection conn;
        private XmppException error;
        private readonly ManualResetEvent hasLoggedIn = new ManualResetEvent(false);

        public XConnection(string xmppHost) {
            conn = CreateXmppConnection(xmppHost);
        }

        private XmppClientConnection CreateXmppConnection(string xmppHost) {
            var connection  = new XmppClientConnection {
                                                              Server = xmppHost,
                                                              ConnectServer = xmppHost,
                                                              AutoResolveConnectServer = false,
                                                        };
            connection.OnLogin += o => hasLoggedIn.Set();
            connection.OnAuthError += (o, e) => error =  new XmppException(e.Value);
            connection.OnSocketError += (o,e) => error = new XmppException(e.Message);
            connection.OnStreamError += (o,e) => error = new XmppException(e.Value);
            return connection;

        }

        public void Login(string userid, string password, string resource) {
            conn.Open(userid, password, resource);
            if (!hasLoggedIn.WaitOne(1.Seconds())) throw error;
        }
    }

    public class XmppException : Exception {
        public XmppException(string message):base(message) {
           
        }
    }
}
