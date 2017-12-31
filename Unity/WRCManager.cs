using System;
using System.Collections.Generic;
using UnityEngine;
using SickDev.WebRcon;

namespace SickDev.WebRcon.Unity {
    public sealed class WRCManager : MonoBehaviour {
        static WRCManager _singleton;
        public static WRCManager singleton {
            get {
                if(_singleton == null)
                    _singleton = FindObjectOfType<WRCManager>();
                return _singleton;
            }
        }

        [SerializeField]
        string _cKey;
        [SerializeField]
        bool autoInitialize = true;
        [SerializeField]
        bool _dontDestroyOnLoad = true;
        [SerializeField, EnumFlags]
        LogType _logType = (LogType)(
            (1 << (int)LogType.Error) |
            (1 << (int)LogType.Assert) |
            (1 << (int)LogType.Warning) |
            (1 << (int)LogType.Log) |
            (1 << (int)LogType.Exception)
        );

        public WebConsole console { get; private set; }

        bool justLinked;
        bool justUnlinked;
        ErrorCode? disconnectedError;
        List<Exception> innerExceptionBuffer = new List<Exception>();
        List<ErrorCode> errorBuffer = new List<ErrorCode>();
        List<CommandMessage> commandBuffer = new List<CommandMessage>();

        public event Action onLinked;
        public event Action onUnlinked;
        public event OnDisconnectedHandler onDisconnected;
        public event OnInnerExceptionThrownHandler onInnerExceptionThrown;
        public event OnErrorHandler onError;
        public event OnCommandHandler onCommand;

        public string cKey {
            get { return _cKey; }
            set { _cKey = value; }
        }

        public bool dontDestroyOnLoad {
            get { return _dontDestroyOnLoad; }
            set { _dontDestroyOnLoad = value; }
        }

        public LogType logType {
            get { return _logType; }
            set { _logType = value; }
        }

        static WRCManager() {
            Config.dllsToExclude.AddRange(DllsExcluder.dllsToExclude);
        }

        void Awake() {
            if(singleton != this) {
                Destroy(this);
                return;
            }
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            console = new WebConsole();
            SetupHandlers();

            if(autoInitialize)
                Initialize();
        }

        public void Initialize() {
            if(string.IsNullOrEmpty(cKey))
                throw new ArgumentException("cKey is null or empty. Please, ensure to use a valid cKey");
            console.cKey = cKey;
            ResetBuffers();
            console.Initialize();
        }

        void ResetBuffers() {
            justLinked = false;
            justUnlinked = false;
            disconnectedError = null;
            innerExceptionBuffer = new List<Exception>();
            errorBuffer = new List<ErrorCode>();
            commandBuffer = new List<CommandMessage>();
        }

        void SetupHandlers() {
            console.onLinked += OnLinked;
            console.onUnlinked += OnUnlinked;
            console.onDisconnected += OnDisconnected;
            console.onInnerExceptionThrown += OnInternalExceptionThrown;
            console.onError += OnError;
            console.onCommand += OnCommand;
        }

        void OnLinked() {
            justLinked = true;
        }

        void OnUnlinked() {
            justUnlinked = true;
        }

        void OnDisconnected(ErrorCode error) {
            disconnectedError = error;
        }

        void OnInternalExceptionThrown(Exception exception) {
            Debug.LogError("Inner Exception: " + exception.ToString());
            innerExceptionBuffer.Add(exception);
        }

        void OnError(ErrorCode error) {
            errorBuffer.Add(error);
        }

        void OnCommand(CommandMessage command) {
            commandBuffer.Add(command);
        }

        void Update() {
            ProcessOnLinked();
            ProcessOnUnlinked();
            ProcessOnDisconnected();
            ProcessOnInnerExceptionThrown();
            ProcessOnError();
            ProcessOnCommand();
        }

        void ProcessOnLinked() {
            if(justLinked) {
                Application.logMessageReceivedThreaded += OnLogMessageReceived;
                justLinked = false;
                if(onLinked != null)
                    onLinked();
            }
        }

        void ProcessOnUnlinked() {
            if(justUnlinked) {
                justUnlinked = false;
                if(onUnlinked != null)
                    onUnlinked();
            }
        }

        void ProcessOnDisconnected() {
            if(disconnectedError != null) {
                Application.logMessageReceivedThreaded -= OnLogMessageReceived;
                ErrorCode oldCode = disconnectedError.Value;
                disconnectedError = null;
                if(onDisconnected != null)
                    onDisconnected(oldCode);
            }
        }

        void ProcessOnInnerExceptionThrown() {
            for(int i = 0; i < innerExceptionBuffer.Count; i++) {
                if(onInnerExceptionThrown != null)
                    onInnerExceptionThrown(innerExceptionBuffer[i]);
            }
            innerExceptionBuffer.Clear();
        }

        void ProcessOnError() {
            for(int i = 0; i < errorBuffer.Count; i++) {
                if(onError != null)
                    onError(errorBuffer[i]);
            }
            errorBuffer.Clear();
        }

        void ProcessOnCommand() {
            for(int i = 0; i < commandBuffer.Count; i++) {
                if(onCommand != null)
                    onCommand(commandBuffer[i]);
                console.ExecuteCommand(commandBuffer[i]);
            }
            commandBuffer.Clear();
        }

        void OnLogMessageReceived(string condition, string stackTrace, LogType type) {
            if((logType & type) == type) {
                string message = type.ToString();
                if(type == LogType.Warning)
                    message = "<span style=\"color:yellow;\">" + message + "</span>";
                else if(type == LogType.Error || type == LogType.Exception)
                    message = "<span style=\"color:red;\">" + message + "</span>";
                message += ":\t" + condition;
                message += "<details><summary>StackTrace</summary>" + stackTrace + "</details>";
                console.defaultTab.Log(message);
            }
        }

        public void Close() {
            if(console != null)
                console.Close();
        }

        void OnDestroy() {
            Close();
        }
    }
}