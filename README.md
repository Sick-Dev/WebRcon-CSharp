**WebRcon-CSharp** is the official .Net/Mono/C# plugin to work with [WebRcon](http://www.webrcon.com/).

1. [Requirements](#requirements)
2. [Installation](#installation)
3. [Usage](#usage)
4. [FAQ](#faq)
5. [License](#license)
6. [About](#about)

# Requirements
In order to use this plugin, you first need to:
- A valid WebRcon ckey
- .NET 3.5 framework / Mono 3.0 or superior installed

# Installation
To use this plugin, you can either download the **precompiled libraries** or compile them from source code yourself.

Once compiled, add references "WebRCon.Core.dll" and "CommandSystem.dll" to your C# project.

# Usage
## Initialization
```C#
string cKey = "0M6EQVX8PI"; //Your cKey should go here
WebConsole console = new WebConsole(cKey);
console.Initialize();
console.onLinked += () => {
    //Your code
};
```
1. First, create a "WebConsole" object and assign your cKey.
2. Call "Initialize" method.
3. Once the communication is stablished and the plugin is linked to the server, the "onLinked" event will fire.

## Logging messages to WebConsole
```C#
console.defaultTab.Log("Hello World!");
```
To show messages on the web console, call the "Log" method on any "Tab" object.
If no other tabs have been created explicitely, you can use the "defaultTab".

To create and send message to another tab, simply call the method "CreateTab".
```C#
Tab newTab = console.CreateTab("Tab name");
newTab.Log("This message is sent to the new tab");
```

## Registering Commands
```C#
public static bool IsNumberEven(int number) {
    return (number % 2) == 0
}
...
Command command = new FuncCommand<int, bool>(IsNumberEven);
console.commandsManager.Add(command);
```
Once a command is registered, it can be called from the WebConsole.

Commands management is implemented by the [CommandSystem](https://github.com/Cobo3/CommandSystem), which allows to parse strings into ready-to-use commands.
The [CommandSystem](https://github.com/Cobo3/CommandSystem) already contains a full [in-depth guide](https://github.com/Cobo3/CommandSystem) of how it works, so feel free to read it  and familiarize yourself with it.

## Closing the connection
```C#
console.Close();
```
It is recommended to manually close the connection at the end of the execution of your application.

## Events
- "onLinked" : Called when the connection status becomes **linked**.
- "onUnlinked" : Called when the connection status becomes **unlinked**.
- "onDisconnected" : Called when the connection status becomes **disconnected**. Returns the reason as an ErrorCode.
- "onError" : Called from the server when something is wrong. Returns the specific ErrorCode.
- "onExceptionThrown" : Called when an asynchronous operation throws an exception. The exception can come from either plugin source code or from the execution of a custom command.
- "onCommand" : Raised when a command is called from the WebConsole. Use it to manually manage the execution of commands. If no delegate is assigned to this event, the [CommandSystem](https://github.com/Cobo3/CommandSystem) will execute the command automatically.

# FAQ
### What is WebRcon?
Visit [WebRcon Website](http://www.webrcon.com) to obtain all needed info.

### What is the cKey?
The cKey is the generated code that will link your application to a WebConsole.

### Can I use this plugin on Unity?
You can find the Unity plugin for WebRcon on [Github](https://github.com/Sick-Dev/WebRcon-Unity).

### How do I register additional assemblies to use with [CommandSystem](https://github.com/Cobo3/CommandSystem)?
```C#
WebConsole console = new WebConsole(cKey, "AssemblyName1", "AssemblyName2");
```
When creating a new "WebConsole" object, you can specify assemblies to register via its constructor.

# License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

# About
Created by SickDev.
- **Bubexel** - [GitHub](https://github.com/serk7) - [WebPage](http://www.bubexel.com)
- **Cobo** - [GitHub](https://github.com/Cobo3) - [WebPage](https://coboantonio.wordpress.com/)
