using System;
using System.Threading;
using agsXMPP;
using agsXMPP.protocol.client;
using AuctionSniper.Utils;

namespace AuctionSniper.Xmpp {
    public class XmppChatClient {
        private readonly XmppClientConnection conn;
        private XmppException error = new XmppException("timed out");
        private readonly ManualResetEvent hasLoggedIn = new ManualResetEvent(false);
        private Jid jid;
        public event MessageHandler OnChatMessageReceived ;

        public XmppChatClient(Jid jid) {
            conn = CreateXmppConnection(jid.Server);
            OnChatMessageReceived += delegate { };
            this.jid = jid;
            
        }

        private XmppClientConnection CreateXmppConnection(string xmppHost) {
            var connection  = new XmppClientConnection {
                                                           Server = xmppHost,
                                                           ConnectServer = xmppHost,
                                                           AutoResolveConnectServer = false,
                                                       };
            connection.OnLogin += o => hasLoggedIn.Set();
            connection.OnMessage += (sender, msg) => { 
                                                        if( MessageType.chat.Equals(msg.Type)) 
                                                            OnChatMessageReceived(sender, msg); 
                                                      };
            connection.OnAuthError += (o, e) => error =  ExceptionWithMsg(string.Format("Auth error {0}",e.ToString()));
            connection.OnSocketError += (o,e) => error = ExceptionWithMsg(string.Format("socket error {0}",e.ToString()));
            connection.OnStreamError += (o,e) => error = ExceptionWithMsg(string.Format("stream error {0}",e.ToString()));
            return connection;

        }

        private XmppException ExceptionWithMsg(string message) {
            return new XmppException(message);
        }

        public void Login(string password) {
          
            conn.Open(jid.User, password, jid.Resource);
            if (!hasLoggedIn.WaitOne(3.Seconds())) throw error;
        }

        public void SendMessageTo(Jid to, string message) {
            conn.Send(new Message(to, this.jid, MessageType.chat, message));
        }

        public void Dispose() {
            hasLoggedIn.Dispose();
            conn.Close();
        }
    }
}