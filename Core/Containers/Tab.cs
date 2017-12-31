using System;

namespace SickDev.WebRcon{
	public class Tab:Container{

		internal Tab(Client client, string name):base(client, name){}

		public void Log(string text) {
            string timeStamp = DateTime.Now.ToLongTimeString();
            MessageBase message = new LogLineMessage(id, timeStamp, text);
			NetworkMessage netMessage = message.Build();
			client.Send(netMessage);
		}

		internal override MessageBase GetMessage(){
			return new NewTabMessage(this);
		}
	}
}

