using System;
using System.Diagnostics;

static class ConsoleUtility {
    [Conditional("DEBUG")]
    public static void Log(string text, ConsoleColor color = ConsoleColor.Gray) {
        Console.ForegroundColor=color;
        Console.WriteLine(text);
        Console.ForegroundColor=ConsoleColor.Gray;
    }

    [Conditional("DEBUG")]
    public static void Log(object obj, ConsoleColor color = ConsoleColor.Gray) {
        Log(obj.ToString(), color);
    }
}
