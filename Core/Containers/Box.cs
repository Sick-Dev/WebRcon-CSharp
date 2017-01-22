using System;

namespace WebRcon{
    internal class Box:Container{

		internal Box(Client client, string name):base(client, name){}

		public void Update(string text){
			string timeStamp = DateTime.Now.ToShortTimeString();
            UpdateBoxMessage message = new UpdateBoxMessage(id, timeStamp, text);
			NetworkMessage netMessage = message.Build();
			client.Send(netMessage);
		}

		internal override MessageBase GetMessage(){
            return new NewBoxMessage(this);
		}
	}
}


