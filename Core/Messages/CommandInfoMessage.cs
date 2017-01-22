using System.Text;
using System.Reflection;

namespace WebRcon {
    public class CommandInfoMessage : MessageBase {
        string command;
        ParameterInfo[] args;
        string description;

        internal override MessageType messageType {
            get { return MessageType.CommandInfo; }
        }

        public CommandInfoMessage(string command, ParameterInfo[] args, string description) {
            this.command = command;
            this.args = args;
            this.description = description??string.Empty;
        }

        internal override void FillInData(NetworkMessage message) {
            message.WriteString(Encoding.UTF32, command);
            message.WriteUInt16((ushort)args.Length);
            for (int i = 0; i < args.Length; i++) {
                message.WriteString(Encoding.UTF32, args[i].ParameterType.Name);
                message.WriteString(Encoding.UTF32, args[i].Name);
            }
            message.WriteString(Encoding.UTF32, description);
        }
    }
}
