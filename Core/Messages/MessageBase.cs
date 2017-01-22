using System;
namespace WebRcon {
    public abstract class MessageBase {

        internal virtual MessageType messageType{
            get{
                throw new NotImplementedException("Did you forgot to override \"messageType\" on a derived class?");
            }
        }

        internal NetworkMessage Build() {
            NetworkMessage message = NetworkMessage.CreateOutgoing(messageType);
            FillInData(message);
            message.Assemble();
            return message;
        }

        internal virtual void Execute() { }
        internal virtual void ProcessData(NetworkMessage message) { }
        internal virtual void FillInData(NetworkMessage message) { }
    }
}

