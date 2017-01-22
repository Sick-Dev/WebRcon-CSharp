using System.Text;

namespace WebRcon {
    internal class UpdateBoxMessage :UpdateContainerMessage {

        string value;

        internal override MessageType messageType{
            get{
                return MessageType.UpdateBox;
            }
        }

        public UpdateBoxMessage(ushort id, string timeStamp, string value):base(id, timeStamp){
            this.value=value;
        }

        internal override void WriteData(NetworkMessage message) {
            message.WriteString(Encoding.UTF32, value);
        }
    }
}
