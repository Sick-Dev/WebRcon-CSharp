using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SickDev.WebRcon{
	internal class Client{
        public delegate void OnConnectionAttempt(bool succesful);

		TcpClient client;
        NetworkStream stream;
        NetworkReader reader;
		Thread pingThread;
		Thread readThread;
        Thread processMessagesThread;
        bool forceClose;
        DateTime lastPing = DateTime.Now;

        public MessageBuffer buffer { get; private set; }
		public bool isConnected{ get{return client.Connected;}}

        public event Action onConnectionBroken;

		public Client() { 
			client = new TcpClient();
            buffer=new MessageBuffer();
            buffer.RegisterHandler<PingMessage>(OnPing);
		}

        void OnPing(PingMessage message) {
            lastPing = DateTime.Now;
            Send(new PongMessage().Build());
        }

		public void ConnectViaIP(IPAddress ip, int port, OnConnectionAttempt callback){
			client.BeginConnect(ip, port, OnConnected, callback);
		}

		public void ConnectViaHost(string host, int port, OnConnectionAttempt callback) {
			client.BeginConnect(host, port, OnConnected, callback);
		}

		public void Close() {
            forceClose = true;
			if (client != null)
				client.Close();
            if (stream != null)
                stream.Close();
		}

		void OnConnected(IAsyncResult result){
            bool successful = true;
            forceClose = false;
			try{
				client.EndConnect(result);
                stream=client.GetStream();
				reader = new NetworkReader(stream);

                pingThread = new Thread(CheckPing);
                pingThread.Start();
				readThread = new Thread(ReadLoop);
				readThread.Start();
                processMessagesThread=new Thread(ProcessMessages);
                processMessagesThread.Start();
			}
			catch(Exception e){
                successful = false;
                WebConsole.OnExceptionWasThrown(e);
            }
            ((OnConnectionAttempt)result.AsyncState)(successful);
		}

        void CheckPing() {
            while (true) {
                if ((DateTime.Now - lastPing).TotalMilliseconds > Config.maxPingTimeout) {
                    if (onConnectionBroken != null)
                        onConnectionBroken();
                    forceClose = true;
                }
                Thread.Sleep(Config.maxPingTimeout);
                if (forceClose)
                    break;
            }
        }

		void ReadLoop(){
			while (true){
                try {
                    reader.Read();
                }
                catch (Exception e) {
                    if (forceClose)
                        break;
                    WebConsole.OnExceptionWasThrown(e);
                }
                Thread.Sleep(Config.readInterval);
            }
		}

		void ProcessMessages() {
            while (true) {
                try {
                    while (reader.hasDataAvailable) {
                        byte[] raw = reader.GetLastPacket();
                        NetworkMessage netMessage = NetworkMessage.CreateIncoming(raw);
                        MessageBase message = TransformMessage(netMessage.type);
                        message.ProcessData(netMessage);
                        buffer.Queue(message);
                    }
                }
                catch (Exception e) {
                    WebConsole.OnExceptionWasThrown(e);
                }
                Thread.Sleep(Config.processInterval);
                if (forceClose)
                    break;
            }
		}

        public static MessageBase TransformMessage(MessageType messageType) {
            try {
                Type type = Type.GetType("WebRcon."+messageType.ToString()+"Message");
                return (MessageBase)Activator.CreateInstance(type);
            }
            catch {
                throw new UnknownMessageTypeException((byte)messageType);
            }
        }

        public void Send(NetworkMessage message){
            try {
                stream.Write(message.raw, 0, message.raw.Length);
            }
            catch (Exception e) {
                WebConsole.OnExceptionWasThrown(e);
            }
        }
    }
}