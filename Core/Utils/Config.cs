namespace SickDev.WebRcon{
	public static partial class Config	{
        internal const string host =
#if DEBUG
        "test.webrcon.com";
#elif CONNECT_LOCALHOST
        "localhost";
#else
        "webrcon.com";
#endif
        internal const int port = 8004;
        public static int maxPingTimeout = 5000;
		public static int readInterval = 200;
        public static int processInterval = 200;
        internal const string pluginApi = ".Net/Mono C#";
		internal const string protocolVersion = "alpha";
	}
}