using WebRcon;
using System.Threading;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            WebConsole console = new WebConsole("JEOT2W5RO3");
            console.commandsManager.AddAssemblyWithCommands("");
            console.Initialize();
            while(true)
                Thread.Sleep(1000);
            //console.Close();
        }
    }
}
