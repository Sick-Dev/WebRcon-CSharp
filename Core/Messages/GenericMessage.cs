namespace WebRcon {
    internal abstract class GenericMessage:MessageBase {
        internal override void FillInData(NetworkMessage message) {
            WriteHeader(message);
            WriteData(message);
        }

        internal abstract void WriteHeader(NetworkMessage message);
        internal abstract void WriteData(NetworkMessage message);
    }
}
