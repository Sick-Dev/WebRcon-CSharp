using System.Text;

namespace WebRcon {
    abstract class UpdateContainerMessage:GenericMessage {

        ushort id;
        string timeStamp;

        public UpdateContainerMessage(ushort id, string timeStamp) {
            this.id=id;
            this.timeStamp=timeStamp;
        }

        internal override void WriteHeader(NetworkMessage message) {
            message.WriteUInt16(id);
            message.WriteString(Encoding.UTF32, timeStamp);
        }
    }
}
