using System;
using System.Collections.Generic;

namespace WebRcon{
	public abstract class Container{
		static Dictionary<Type, ushort> lastIds = new Dictionary<Type, ushort>();

		public ushort id { get; private set; }
		internal Client client { get; private set;}
		internal string name { get; private set; }

		internal Container(Client client, string name){
			id = GetNextID();
			this.client = client;
			this.name = name;
		}

        internal void SendNewContainerMessage() {
            MessageBase message = GetMessage();
			NetworkMessage netMessage = message.Build();
			client.Send(netMessage);
		}

		internal abstract MessageBase GetMessage();

		ushort GetNextID(){
			Type type = this.GetType();
			if (!lastIds.ContainsKey(type))
			    lastIds.Add(type, 0);
			return lastIds[type]++;
		}
	}
}

