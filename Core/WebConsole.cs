using System;
using System.Linq;
using System.Collections.Generic;
using SickDev.CommandSystem;

namespace WebRcon{
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

        public WebConsole() { }
        public WebConsole(string cKey) {
            this.cKey = cKey;
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
            messageBuffer.RegisterHandler<ErrorMessage>(OnError);
        }

        void Connect() {
            ChangeConnectionStatus(ConnectionStatus.Connecting);
            client.ConnectViaHost(Config.host, Config.port, OnConnectionAttempt);
        }

        void OnConnectionAttempt(bool successful) {
            if (!successful)
                ChangeConnectionStatus(ConnectionStatus.Disconnected, ErrorCode.ConnectionError);
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
            defaultTab.Log("Welcome to WebRcon v."+Config.protocolVersion+" for "+Config.pluginApi+". You are linked and ready to start.");
            CreateCommandsManager();
            ChangeConnectionStatus(ConnectionStatus.Linked);
        }

        void CreateCommandsManager() {
            commandsManager = new CommandsManager();
            commandsManager.onCommandAdded += OnCommandAdded;
            CommandsManager.onExceptionThrown += OnExceptionWasThrown;
            CommandsManager.onMessage += OnCommandSystemMessage;
            commandsManager.Load();
        }

        void OnCommandAdded(CommandBase command) {
            CommandInfoMessage message = new CommandInfoMessage(command.name, command.signature.parameters, command.description);
            NetworkMessage netMessage = message.Build();
            client.Send(netMessage);
        }

        void OnCommand(CommandMessage message) {
            Tab tab = GetContainer<Tab>(message.tabId);
            tab.Log("> " + message.parsedCommand.raw);
            if (onCommand != null)
                onCommand(message);
            else
                ExecuteCommand(message);
        }

        public void ExecuteCommand(CommandMessage message) {
            Tab tab = GetContainer<Tab>(message.tabId);

            try {
                CommandExecuter executer = commandsManager.GetCommandExecuter(message.parsedCommand);
                if (executer.IsValidCommand()) {
                    object result = executer.Execute();
                    if (executer.HasReturnType())
                        tab.Log(result.ToString());
                }
            }
            catch (Exception e) {
                OnExceptionWasThrown(e);
            }
        }

        void OnError(ErrorMessage message) {
            ErrorCode error = message.code;

            switch (error) {
            case ErrorCode.CkeyAlreadyInUse:
            case ErrorCode.CkeyNotFound:
            case ErrorCode.ProtocolVersionMismatch:
                ChangeConnectionStatus(ConnectionStatus.Disconnected, error);
                break;
            default:
                OnError(error);
                break;
            }
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

        void OnError(ErrorCode error) {
            if (onError != null)
                onError(error);
        }

        void OnExceptionThrown(Exception exception) {
            if (onInnerExceptionThrown != null)
                onInnerExceptionThrown(exception);
        }

        public void Close() {
            if (!isLinked)
                return;
            ChangeConnectionStatus(ConnectionStatus.Disconnected, ErrorCode.None);
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

        public T GetContainer<T>(ushort id) where T:Container{
            return containers.Find(x => x.id == id) as T;
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

        void OnCommandSystemMessage(string message) {
            defaultTab.Log(message);
        }
    }
}