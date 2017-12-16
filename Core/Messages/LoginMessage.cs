using System.Text;

namespace WebRcon {
    internal class LoginMessage : MessageBase {
        string cKey;

        internal override MessageType messageType{get{return MessageType.Login;}}

        public LoginMessage(string cKey) {
            this.cKey=cKey;
        }

        internal override void FillInData(NetworkMessage message) {
            message.WriteString(Encoding.UTF8, cKey);
            message.WriteString(Encoding.UTF8, Config.pluginApi);
            message.WriteString(Encoding.UTF8, Config.protocolVersion);
        }
    }
}

