namespace SickDev.WebRcon{
    internal class NewTabMessage : NewContainerMessage{
		public NewTabMessage(Tab tab):base(tab){}

        internal override MessageType messageType{
			get{
				return MessageType.NewTab;
			}
		}
    }
}

