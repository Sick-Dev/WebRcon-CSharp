using SickDev.WebRcon;
using System.Threading;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            WebConsole console = new WebConsole("6B3QJ01ZZ9");
            console.onError += Console_onError;
            console.onExceptionThrown += Console_onInnerExceptionThrown;
            console.Initialize();
            console.onConnected += () => {
                console.GetTab(0).Log("<b>Lorem</b> ipsum dolor sit amet, consectetur adipiscing elit. Integer sagittis diam quis neque pretium lobortis. Fusce et arcu in ante vulputate dignissim nec vel mi. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Mauris ut congue lorem, eu ultricies urna. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pretium ipsum vehicula orci mattis, vitae commodo urna tincidunt. Nam quis ultrices enim. Sed ullamcorper nunc finibus egestas accumsan. Duis nec enim faucibus nibh malesuada suscipit. Aliquam eleifend tortor nec ligula consectetur tincidunt. In aliquam blandit magna sit amet efficitur. ");
                console.CreateTab("new Tab").Log("HOLA");
                CommandsBuilder builder = new CommandsBuilder(typeof(Program));
                console.commandsManager.Add(builder.Build());
            };
            while(true)
                Thread.Sleep(1000);
            //console.Close();
        }

        private static void Console_onInnerExceptionThrown(System.Exception exception) {
            System.Console.WriteLine(exception.ToString());
        }

        private static void Console_onError(ErrorCode error) {
            System.Console.WriteLine(error.ToString());
        }

        public static float Max(float a, float b) {
            if(a > b)
                return a;
            else
                return b;
        }

        public static int Max(int a, int b) {
            if(a > b)
                return a;
            else
                return b;
        }
    }
}
