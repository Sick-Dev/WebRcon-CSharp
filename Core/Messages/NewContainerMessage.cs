using System.Text;

namespace WebRcon{
    internal abstract class NewContainerMessage : GenericMessage{
		protected Container container;

		public NewContainerMessage(Container container){
			this.container = container;
		}

        internal override void WriteHeader(NetworkMessage message) {
            message.WriteUInt16(container.id);
            message.WriteString(Encoding.UTF32, container.name);
        }

        internal override void WriteData(NetworkMessage message) {}
    }
}

