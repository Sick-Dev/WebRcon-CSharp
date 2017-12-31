using SickDev.WebRcon;
using System.Threading;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            WebConsole console = new WebConsole("2FFNTV104J");
            console.commandsManager.AddAssemblyWithCommands("");
            console.onError += Console_onError;
            console.onInnerExceptionThrown += Console_onInnerExceptionThrown;
            console.Initialize();
            console.onLinked += () => {
                console.GetContainer<Tab>(0).Log("<b>Lorem</b> ipsum dolor sit amet, consectetur adipiscing elit. Integer sagittis diam quis neque pretium lobortis. Fusce et arcu in ante vulputate dignissim nec vel mi. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Mauris ut congue lorem, eu ultricies urna. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pretium ipsum vehicula orci mattis, vitae commodo urna tincidunt. Nam quis ultrices enim. Sed ullamcorper nunc finibus egestas accumsan. Duis nec enim faucibus nibh malesuada suscipit. Aliquam eleifend tortor nec ligula consectetur tincidunt. In aliquam blandit magna sit amet efficitur. ");
                for(int i = 0; i < 20; i++)
                    console.CreateTab(i.ToString());
                //console.CloseTab(tab);
                console.onTabClosed += x => System.Console.WriteLine("Tab "+x.id+" closed");
            };
            while(true)
                Thread.Sleep(1000);
            //console.Close();
        }

        private static void Console_onInnerExceptionThrown(System.Exception exception) {
            System.Console.WriteLine(exception.Message);
        }

        private static void Console_onError(ErrorCode error) {
            System.Console.WriteLine(error.ToString());
        }
    }
}
