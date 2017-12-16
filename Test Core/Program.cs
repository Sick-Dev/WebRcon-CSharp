using WebRcon;
using System.Threading;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            WebConsole console = new WebConsole("J10XI5DRTK");
            console.commandsManager.AddAssemblyWithCommands("");
            console.Initialize();
            console.onLinked += () => {
                Tab tab = console.CreateTab("1234567890.1234567890..1234567890...1234567890");
                //console.CloseTab(tab);
                console.onTabClosed += x => System.Console.WriteLine("Tab "+tab.id+" closed");
            };
            while(true)
                Thread.Sleep(1000);
            //console.Close();
        }
    }
}
