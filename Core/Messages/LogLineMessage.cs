using System.Text;

namespace WebRcon{
    internal class LogLineMessage : UpdateContainerMessage	{
		string text;

        internal override MessageType messageType{
            get{
                return MessageType.LogLine;
            }
        }

        public LogLineMessage(ushort id, string timeStamp, string text):base(id, timeStamp){
			this.text = text;
		}

        internal override void WriteData(NetworkMessage message) {
            message.WriteString(Encoding.UTF32, text);
        }
    }
}

