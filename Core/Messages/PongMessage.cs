namespace WebRcon {
    internal class PongMessage : MessageBase {
        internal override MessageType messageType {
            get {
                return MessageType.Ping;
            }
        }
    }
}
