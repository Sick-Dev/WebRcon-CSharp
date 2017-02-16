using System.Text;
using SickDev.CommandSystem;

namespace WebRcon {
    public class CommandMessage : MessageBase {
        public ushort tabId { get; private set; }
        public ParsedCommand parsedCommand { get; private set; }

        internal override void ProcessData(NetworkMessage message) {
            tabId = message.ReadUInt16();
            string raw = message.ReadString(Encoding.UTF32);
            parsedCommand = new ParsedCommand(raw);
        }
    }
}
