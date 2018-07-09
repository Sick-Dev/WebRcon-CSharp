using System.Text;

namespace SickDev.WebRcon {
    internal class LoginMessage : MessageBase {
        string cKey;
        string protocolVersion;
        string pluginApi;

        internal override MessageType messageType{get{return MessageType.Login;}}

        public LoginMessage(string cKey, string protocolVersion, string pluginApi) {
            this.cKey = cKey;
            this.protocolVersion = protocolVersion;
            this.pluginApi = pluginApi;
        }

        internal override void FillInData(NetworkMessage message) {
            message.WriteString(Encoding.UTF8, cKey);
            message.WriteString(Encoding.UTF8, pluginApi);
            message.WriteString(Encoding.UTF8, protocolVersion);
        }
    }
}

