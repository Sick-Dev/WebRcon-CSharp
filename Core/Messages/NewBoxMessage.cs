namespace WebRcon {
    internal class NewBoxMessage : NewContainerMessage {

        public NewBoxMessage(Box box):base(box) {}

        internal override MessageType messageType{
            get{
                return MessageType.NewBox;
            }
        }
    }
}

