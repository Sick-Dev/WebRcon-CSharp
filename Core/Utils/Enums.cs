namespace SickDev.WebRcon{
    public enum ConnectionStatus {
        Disconnected = 0,
        Connecting,
        Connected
    }

    public enum ErrorCode : ushort{
        None = 0,
		ConnectionError = 1,
		CkeyNotFound = 100,
		CkeyAlreadyInUse = 101,
		TabIdAlreadyInUse = 102,
		TabIdDoesntExist = 103,
		ProtocolVersionMismatch = 104,
		BoxIdAlreadyInUse = 105,
		BoxIdDoesntExist = 106,
        GuestAccountExpired = 107
	}

    internal enum MessageType : byte {
        Welcome = 0x00,
        Login = 0x01,
        Error = 0x02,
        LoginOk = 0x03,
        NewTab = 0x04,
        Ping = 0x05,
        LogLine = 0x06,
        NewBox = 0x07,
        UpdateBox = 0x08,
        Command = 0x09,
        CommandInfo = 0x0A2,
        CloseTab = 0xA3
    }
}