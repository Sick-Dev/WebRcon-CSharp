using System.Collections.Generic;

namespace WebRcon {
    public class CommandsManager {

        public delegate void OnCommandAdded(CommandBase command);

        List<CommandBase> commands;
        public event OnCommandAdded onCommandAdded;

        public CommandsManager() {
            commands = new List<CommandBase>();
        }

        internal void Load() {
            CommandAttributeLoader loader = new CommandAttributeLoader();
            Add(loader.LoadCommands());
        }

        public void Add(CommandBase[] commands) {
            for (int i = 0; i < commands.Length; i++)
                Add(commands[i]);
        }

        public void Add(CommandBase command) {
            if (!commands.Contains(command)) {
                commands.Add(command);
                if (onCommandAdded != null)
                    onCommandAdded(command);
            }
        }

        public CommandBase[] GetCommands() {
            return commands.ToArray();
        }

        internal CommandExecuter GetCommandExecuter(string text) {
            ParsedCommand parsedCommand = new ParsedCommand(text);
            return GetCommandExecuter(parsedCommand);
        }

        internal CommandExecuter GetCommandExecuter(ParsedCommand parsedCommand) {
            return new CommandExecuter(commands, parsedCommand);            
        }
    }
}
