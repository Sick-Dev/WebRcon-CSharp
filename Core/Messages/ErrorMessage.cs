namespace WebRcon{
	class ErrorMessage : MessageBase {

        public ErrorCode code { get; private set; }

        internal override void ProcessData(NetworkMessage message) {
            code = (ErrorCode)message.ReadUInt16();
		}
	}
}

