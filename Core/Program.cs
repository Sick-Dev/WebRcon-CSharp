using System;
using System.Threading;
using WebRcon;

class Program {
    static WebConsole console;

    static void Main() {
        console = new WebConsole();
        console.onInnerExceptionThrown += OnExceptionThrown;
        console.onLinked += () => {
            console.CreateTab("tab 2");
        };
        console.onDisconnected += (error)=>Console.WriteLine("Error: "+error);
        console.cKey = "JEOT2W5RO3";
        console.Initialize();
        Thread.Sleep(5000);
    }

    static void OnExceptionThrown(Exception e) {
        Console.WriteLine(e.Message + "\n" + e.StackTrace);
    }

    [Command(description ="que echoes el mensaje leñe")]
    static string echo(string text) {
        return "<font color='red'><i>"+text+"</i></font>";
    }

    [Command(description ="te gustan las galletas?")]
    static string DoYouLikeCookies() {
        return "Do you like cookies? <button onclick='SendCommand(0, \"AnswerCookies true\");'>Yes</button> or <button onclick='SendCommand(0, \"AnswerCookies false\");'>No</button>";
    }

    [Command]
    static string AnswerCookies(bool answer) {
        if (answer)
            return "Can we be friends?";
        else
            return "Closing connection...";
    }
}
