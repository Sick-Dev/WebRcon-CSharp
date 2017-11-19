using System.Collections.Generic;

namespace WebRcon{
	public static partial class Config	{
        internal const string host =
#if DEBUG
        "test.webrcon.com";
#else
        "webrcon.com";
#endif

        internal const int port = 8004;
        public static int maxPingTimeout = 5000;
		public static int readInterval = 200;
        public static int processInterval = 200;
        internal const string pluginApi = ".Net/Mono C#";
		internal const string protocolVersion = "alpha";        
        public static List<string> dllsToExclude {
            get { return _dllsToExclude; }
        }

        static List<string> _dllsToExclude = new List<string> {
           "mscorlib.dll",
           "Microsoft.VisualStudio.HostingProcess.Utilities.dll",
           "System.Windows.Forms.dll",
           "System.dll",
           "System.Drawing.dll",
           "Microsoft.VisualStudio.HostingProcess.Utilities.Sync.dll",
           "Microsoft.VisualStudio.Debugger.Runtime.dll",
           "mscorlib.dll",
           "System.Core.dll",
           "System.Configuration.dll",
           "System.Xml.dll"
        };
	}
}