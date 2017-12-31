using System;
using System.Collections.Generic;

namespace SickDev.WebRcon {
    internal class MessageBuffer {
        Queue<MessageBase> queue = new Queue<MessageBase>();
        Dictionary<Type, Action<MessageBase>> handlers = new Dictionary<Type, Action<MessageBase>>();
        Dictionary<ErrorCode, Action> errorHandlers = new Dictionary<ErrorCode, Action>();
        Dictionary<Delegate, Action<MessageBase>> intermediates = new Dictionary<Delegate, Action<MessageBase>>();

        public bool isEmpty{
            get { return queue.Count==0; }
        }

        public MessageBuffer() {
            RegisterHandler<ErrorMessage>(DoErrorHandlers);
        }

        public void Queue(MessageBase message) {
            if (HasHandlers(message.GetType()))
                DoHandlers(message);
            else
                queue.Enqueue(message);
        }

        public MessageBase GetNextMessage() {
            return queue.Dequeue();
        }

        public bool HasHandlers(Type type) {
            return handlers.ContainsKey(type)&&handlers[type]!=null;
        }

        void DoHandlers(MessageBase message) {
            Type type = message.GetType();
            handlers[type](message);
        }

        void DoErrorHandlers(ErrorMessage message) {
            if (errorHandlers.ContainsKey(message.code)) {
                if (errorHandlers[message.code]!=null)
                    errorHandlers[message.code]();
            }
        }
        
        public void RegisterHandler<T>(Action<T> callback) where T:MessageBase {
            RegisterHandlerInternal<T>(callback, x => callback((T)x));
        }

        public void RegisterHandler<T>(Action callback) where T : MessageBase {
            RegisterHandlerInternal<T>(callback, x => callback());
        }

        void RegisterHandlerInternal<T>(Delegate callback, Action<MessageBase> intermediate) {
            Type type = typeof(T);
            if (!handlers.ContainsKey(type))
                handlers.Add(type, null);
            
            if (handlers[type] == null)
                handlers[type] = intermediate;
            else
                handlers[type] += intermediate;
            intermediates.Add(callback, intermediate);
        }

        public void UnRegisterHandler<T>(Action<T> callback) where T : MessageBase {
            UnRegisterHandlerInternal<T>(callback);
        }

        public void UnRegisterHandler<T>(Action callback) where T : MessageBase {
            UnRegisterHandlerInternal<T>(callback);
        }

        void UnRegisterHandlerInternal<T>(Delegate callback) {
            Type type = typeof(T);
            try {
                handlers[type] -= intermediates[callback];
                intermediates.Remove(callback);
            }
            catch {
                throw new NonRegisteredHandlerException(type, callback.Method);
            }
        }

        public void RegisterErrorHandler(ErrorCode code, Action callback) {
            if (!errorHandlers.ContainsKey(code))
                errorHandlers.Add(code, null);

            if (errorHandlers[code]==null)
                errorHandlers[code]=callback;
            else
                errorHandlers[code]+=callback;
        }

        public void UnRegisterErrorHandler(ErrorCode code, Action callback) {
            try {
                errorHandlers[code]-=callback;
            }
            catch {
                throw new NonRegisteredErrorHandlerException(code, callback.Method);
            }
        }
    }
}
