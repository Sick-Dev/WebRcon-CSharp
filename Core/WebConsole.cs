using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SickDev.CommandSystem;

namespace SickDev.WebRcon{
    public delegate void OnInnerExceptionThrownHandler(Exception exception);
    public delegate void OnDisconnectedHandler(ErrorCode error);
    public delegate void OnErrorHandler(ErrorCode error);
    public delegate void OnCommandHandler(CommandMessage message);

    public class WebConsole {
        public static WebConsole initializedInstance { get; private set; }

        Client client;
        List<Container> containers;
        ConnectionStatus status = ConnectionStatus.Disconnected;

        public Tab defaultTab { get; private set; }
        public string cKey { get; set; }
        public CommandsManager commandsManager { get; private set; }
        public bool isLinked { get { return status == ConnectionStatus.Linked; } }
        public bool isInitialized { get { return status != ConnectionStatus.Disconnected; } }
        MessageBuffer messageBuffer { get { return client.buffer; } }
        
        public event OnInnerExceptionThrownHandler onInnerExceptionThrown;
        public event Action onUnlinked;
        public event Action onLinked;
        public event OnDisconnectedHandler onDisconnected;
        public event OnErrorHandler onError;
        public event OnCommandHandler onCommand;
        public event Action<Tab> onTabClosed;

        public WebConsole():this(string.Empty) { }
        public WebConsole(string cKey) : this(cKey, new Configuration()) { }
        public WebConsole(string cKey, Configuration configuration) {
            this.cKey = cKey;
            CreateCommandsManager(configuration);
        }

        void CreateCommandsManager(Configuration configuration) {
            CommandsManager.onExceptionThrown += OnExceptionWasThrown;
            CommandsManager.onMessage += OnCommandSystemMessage;

            configuration.RegisterAssembly("WebRcon.core");
            commandsManager = new CommandsManager(configuration);
            commandsManager.onCommandAdded += OnCommandAdded;
        }

        internal static void OnExceptionWasThrown(Exception exception) {
            if (initializedInstance == null)
                return;
            try {
                if (initializedInstance.defaultTab != null)
                    initializedInstance.defaultTab.Log("Plugin internal exception: " + exception.ToString());
            }
            catch { }

            initializedInstance.OnExceptionThrown(exception);
        }

        void OnExceptionThrown(Exception exception) {
            if (onInnerExceptionThrown != null)
                onInnerExceptionThrown(exception);
        }

        void OnCommandSystemMessage(string message) {
            if (isLinked)
                defaultTab.Log(message);
        }

        void OnCommandAdded(Command command) {
            CommandInfoMessage message = new CommandInfoMessage(command.name, command.signature.parameters, command.description);
            NetworkMessage netMessage = message.Build();
            client.Send(netMessage);
        }

        public void Initialize() {
            if (isInitialized)
                throw new AlreadyInitializedException();
            if (initializedInstance == null)
                initializedInstance = this;
            else if (initializedInstance != this)
                throw new AlreadyInitializedException();

            containers = new List<Container>();
            SetupClient();
            Connect();
        }

        void SetupClient() {
            client = new Client();
            client.onConnectionBroken += OnConnectionBroken;
            messageBuffer.RegisterHandler<WelcomeMessage>(OnWelcome);
            messageBuffer.RegisterHandler<CommandMessage>(OnCommand);
            messageBuffer.RegisterHandler<CloseTabMessage>(OnCloseTab);
            messageBuffer.RegisterHandler<ErrorMessage>(OnError);
        }

        void OnConnectionBroken() {
            ChangeConnectionStatus(ConnectionStatus.Disconnected, ErrorCode.ConnectionError);
        }

        void ChangeConnectionStatus(ConnectionStatus newStatus, ErrorCode error = ErrorCode.None) {
            ConnectionStatus oldStatus = status;
            status = newStatus;
            if (oldStatus == ConnectionStatus.Linked && newStatus == ConnectionStatus.Unlinked)
                OnUnlinked();
            else if (newStatus == ConnectionStatus.Linked)
                OnLinked();
            else if (newStatus == ConnectionStatus.Disconnected)
                OnDisconnected(error);
        }

        void OnUnlinked() {
            if (onUnlinked != null)
                onUnlinked();
        }

        void OnLinked() {
            if (onLinked != null)
                onLinked();
        }

        void OnDisconnected(ErrorCode error) {
            initializedInstance = null;
            if (onDisconnected != null)
                onDisconnected(error);
        }

        void OnWelcome() {
            messageBuffer.UnRegisterHandler<WelcomeMessage>(OnWelcome);
            ChangeConnectionStatus(ConnectionStatus.Unlinked);
            messageBuffer.RegisterHandler<LoginOkMessage>(OnLoginOk);

            MessageBase login = new LoginMessage(cKey);
            NetworkMessage netMessage = login.Build();
            client.Send(netMessage);
        }

        void OnLoginOk() {
            messageBuffer.UnRegisterHandler<LoginOkMessage>(OnLoginOk);
            defaultTab = CreateTab("Default");
            defaultTab.Log("Welcome to WebRcon v." + Config.protocolVersion + " for " + Config.pluginApi + ". You are linked and ready to start.");
            ChangeConnectionStatus(ConnectionStatus.Linked);
            commandsManager.LoadCommands();
        }

        void OnCommand(CommandMessage message) {
            Tab tab = GetContainer<Tab>(message.tabId);
            tab.Log("> " + message.parsedCommand.raw);
            if (onCommand != null)
                onCommand(message);
            else
                ExecuteCommand(message);
        }

        void OnCloseTab(CloseTabMessage message) {
            Tab tab = GetContainer<Tab>(message.id);
            containers.Remove(tab);
            if (onTabClosed != null)
                onTabClosed(tab);
        }

        void OnError(ErrorMessage message) {
            ErrorCode error = message.code;

            switch (error) {
                case ErrorCode.ProtocolVersionMismatch:
                case ErrorCode.CkeyNotFound:
                case ErrorCode.CkeyAlreadyInUse:
                case ErrorCode.GuestAccountExpired:
                    ChangeConnectionStatus(ConnectionStatus.Disconnected, error);
                    break;
            }
            OnError(error);
        }

        void OnError(ErrorCode error) {
            if (onError != null)
                onError(error);
        }

        void Connect() {
            ChangeConnectionStatus(ConnectionStatus.Connecting);
            client.ConnectViaHost(Config.host, Config.port, OnConnectionAttempt);
        }

        void OnConnectionAttempt(bool successful) {
            if (!successful)
                ChangeConnectionStatus(ConnectionStatus.Disconnected, ErrorCode.ConnectionError);
        }

        public void ExecuteCommand(CommandMessage message) {
            Tab tab = GetContainer<Tab>(message.tabId);
            CommandExecuter executer = commandsManager.GetCommandExecuter(message.parsedCommand);
            if(executer.isValidCommand) {
                try {
                    object result = executer.Execute();
                    if(executer.hasReturnValue) {
                        string resultString = ConvertCommandResultToString(result);
                        tab.Log(resultString);
                    }
                }
                catch (CommandSystemException exception) {
                    OnExceptionWasThrown(exception);
                }
            }
        }

        string ConvertCommandResultToString(object result) {
            if(result == null)
                return "null";
            else if(result is Array) {
                Array resultArray = (Array)result;
                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < resultArray.Length; i++) {
                    builder.Append(resultArray.GetValue(i).ToString());
                    if(i < resultArray.Length - 1)
                        builder.Append(", ");
                }
                return builder.ToString();
            }
            else
                return result.ToString();
        }

        public void Close() {
            if (!isInitialized)
                return;
            ChangeConnectionStatus(ConnectionStatus.Disconnected, ErrorCode.None);
            if (isLinked)
                defaultTab.Log("Disconnecting");
            client.Close();
        }

        public Tab CreateTab(string name) {
            Tab tab = new Tab(client, name);
            CreateContainer(tab);
            return tab;
        }

        Box CreateBox(string name) {
            Box box = new Box(client, name);
            CreateContainer(box);
            return box;
        }

        void CreateContainer(Container container) {
            if (!isInitialized)
                throw new NonInitializedException();
            containers.Add(container);
            container.SendNewContainerMessage();
        }

        public Tab[] GetAllTabs() {
            return containers.Where(x => x is Tab).Cast<Tab>().ToArray();
        }

        public Tab GetTab(ushort id) {
            return GetContainer<Tab>(id);
        }

        public T GetContainer<T>(ushort id) where T:Container{
            return containers.Find(x => x.id == id) as T;
        }

        public void CloseTab(ushort id) {
            CloseTab(GetContainer<Tab>(id));
        }

        public void CloseTab(Tab tab) {
            MessageBase message = new CloseTabMessage(tab);
            NetworkMessage netMessage = message.Build();
            client.Send(netMessage);
        }
    }
}