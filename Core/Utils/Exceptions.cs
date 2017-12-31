using System;
using System.Reflection;

namespace SickDev.WebRcon{
    public class WebRconException : Exception { }

	public class AlreadyInitializedException : WebRconException { 
		public override string Message{
			get{
				return typeof(WebConsole).Name+" has already been initialized";
			}
		}
	}

	public class NonInitializedException : WebRconException { 
		public override string Message{
			get{
				return typeof(WebConsole).Name + " is not yet initialized";
			}
		}
	}

	internal class UnknownMessageTypeException : WebRconException {

        byte typeCode;

        public UnknownMessageTypeException(byte typeCode) {
            this.typeCode=typeCode;
        }

		public override string Message{
			get{
				return "Unknown network message type: "+typeCode;
			}
		}
	}

    internal class HandlerException : WebRconException {

        MethodBase method;

        public HandlerException(MethodBase method) {
            this.method=method;
        }

        public override string Message{
            get{
                return "No such handler ("+method.Name+") is registered "+GetMessageTail();
            }
        }

        protected virtual string GetMessageTail() {
            return string.Empty;
        }
    }

    internal class NonRegisteredHandlerException : HandlerException {

        Type type;

        public NonRegisteredHandlerException(Type type, MethodBase method) : base(method) {
            this.type=type;
        }

        protected override string GetMessageTail(){
            return "for type "+type.ToString();
        }
    }

    internal class NonRegisteredErrorHandlerException : HandlerException {

        ErrorCode error;

        public NonRegisteredErrorHandlerException(ErrorCode error, MethodBase method):base(method) {
            this.error=error;
        }

        protected override string GetMessageTail() {
            return "for type "+error.ToString();
        }
    }
}

