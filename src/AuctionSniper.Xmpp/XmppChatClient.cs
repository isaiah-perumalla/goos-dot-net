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
        private readonly Jid jid;
        private readonly string password;
        public event MessageHandler OnChatMessageReceived ;

        public XmppChatClient(Jid jid, string password) {
            conn = CreateXmppConnection(jid.Server);
            OnChatMessageReceived += delegate { };
            this.jid = jid;
            this.password = password;
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
                                                        if (MessageType.error.Equals(msg.Type))
                                                            OnErrorMessagedReceived(sender, msg);
                                                      };
            connection.OnAuthError += (o, e) => error =  ExceptionWithMsg(string.Format("Auth error {0}",e.ToString()));
            connection.OnSocketError += (o,e) => error = ExceptionWithMsg(string.Format("socket error {0}",e.ToString()));
            connection.OnStreamError += (o,e) => error = ExceptionWithMsg(string.Format("stream error {0}",e.ToString()));
            return connection;

        }

        private void OnErrorMessagedReceived(object sender, Message msg) {
            Console.WriteLine(msg.ToString());
        }

        private XmppException ExceptionWithMsg(string message) {
            Console.WriteLine(message.ToString());
            return new XmppException(message);
        }

        public void Login() {

            if (conn.XmppConnectionState == XmppConnectionState.Disconnected)
            {
                conn.Open(jid.User, this.password, jid.Resource);
            }
            if (!hasLoggedIn.WaitOne(5.Seconds())) throw error;
        }

        public void SendMessageTo(Jid to, string message) {
            conn.Send(new Message(to, this.jid, MessageType.chat, message));
        }
        public void Close() {
            conn.Close();
            hasLoggedIn.Reset();
        }
        public void Dispose() {
            hasLoggedIn.Dispose();
            conn.Close();
        }
    }
}