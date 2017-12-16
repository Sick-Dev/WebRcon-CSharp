namespace WebRcon{
    class CloseTabMessage:MessageBase {
        public ushort id { get; private set; }

        internal override MessageType messageType { get {return MessageType.CloseTab;}}

        public CloseTabMessage() { }
        public CloseTabMessage(Tab tab) {
            id = tab.id;
        }

        internal override void FillInData(NetworkMessage message) {
            message.WriteUInt16(id);
        }

        internal override void ProcessData(NetworkMessage message) {
            id = message.ReadUInt16();
        }
    }
}
