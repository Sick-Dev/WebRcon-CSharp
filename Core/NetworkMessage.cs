using System;
using System.Text;

namespace SickDev.WebRcon{
	internal class NetworkMessage {

        const int headerSize = 3;
        const int maxPayloadSize = ushort.MaxValue;

		public MessageType type{get; protected set;}
		public byte[] raw{get; protected set;}
        public byte[] header { get; private set; }
		public byte[] payload{get; protected set;}
		protected int position;

        NetworkMessage(byte[] payload, MessageType type) {
            this.payload=payload;
            this.type=type;
        }

        public static NetworkMessage CreateIncoming(byte[] raw) {
            byte[] payload = new byte[raw.Length-1];
            Array.Copy(raw, 1, payload, 0, payload.Length);
            NetworkMessage message = new NetworkMessage(payload, (MessageType)raw[0]);
            return message;
        }

        public static NetworkMessage CreateOutgoing(MessageType type) {
            NetworkMessage message = new NetworkMessage(new byte[maxPayloadSize], type);
            return message;
        }

        public void Assemble() {
            AddHeader();
            raw=new byte[position+headerSize];
            Array.Copy(header, 0, raw, 0, headerSize);
            Array.Copy(payload, 0, raw, headerSize, position);
        }

        void AddHeader() {
            byte[] size = BitConverter.GetBytes(position+1);
            header=new byte[headerSize];
            header[0]=size[0];
            header[1]=size[1];
            header[2]=(byte)type;
        }

        /*******************************
        READ
        *******************************/
        public byte ReadUInt8() {
            byte result = payload[position];
            position++;
            return result;
        }

        public ushort ReadUInt16() {
            ushort result = BitConverter.ToUInt16(payload, position);
            position=position+2;
            return result;
        }

        public string ReadString(Encoding encoding) {
            ushort size = ReadUInt16();
            byte[] cadena = new byte[size];
            Array.Copy(payload, position, cadena, 0, size);
            position = position + size;
            return encoding.GetString(cadena);
        }

        /*******************************
        WRITE
        *******************************/
        public void WriteUInt16(ushort value) {
            byte[] bValue = BitConverter.GetBytes(value);
            Array.Copy(bValue, 0, payload, position, 2);
            position+=2;
        }

        public void WriteString(Encoding encoding, string text) {
            byte[] bText = encoding.GetBytes(text);
            ushort size = (ushort)bText.Length;
            WriteUInt16(size);
            Array.Copy(bText, 0, payload, position, size);
            position+=size;
        }        
    }
}

