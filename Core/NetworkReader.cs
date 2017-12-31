using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace SickDev.WebRcon {
    internal class NetworkReader {

        const int bytesReserverForSize = 2;
        enum Step { ReadSize, ReadData }

        NetworkStream stream;
        Step step;
        Queue<byte[]> queue = new Queue<byte[]>();
        byte[] incomingPacket;
        int position;
        bool readAgain;

        public bool hasDataAvailable {
            get { return queue.Count>0; }
        }

        public NetworkReader(NetworkStream stream) {
            this.stream=stream;
            PrepareForNextPacket();
        }

        public void Read() {
            do {
                readAgain = false;
                if (step == Step.ReadSize)
                    ReadSize();
                else if (step == Step.ReadData)
                    ReadData();
            } while (readAgain);
        }

        void ReadSize() {
            ReadFromStream(bytesReserverForSize-position);
            if (position==bytesReserverForSize) {
                PrepareIncomingPacket();
                step=Step.ReadData;
                ReadData();
            }
        }

        void ReadFromStream(int size) {
            if (stream.DataAvailable) {
                int bytesRead = stream.Read(incomingPacket, position, size);
                position += bytesRead;
            }
        }

        void PrepareIncomingPacket() {
            ushort size = BitConverter.ToUInt16(incomingPacket, 0);
            byte[] temp = new byte[size+bytesReserverForSize];
            Array.Copy(incomingPacket, temp, bytesReserverForSize);
            incomingPacket=temp;
        }

        void ReadData() {
            ReadFromStream(incomingPacket.Length-position);            
            if (position==incomingPacket.Length) {
                RemoveSize();
                queue.Enqueue(incomingPacket);
                PrepareForNextPacket();
                readAgain = true;
            }
        }

        void RemoveSize() {
            byte[] temp = new byte[incomingPacket.Length-bytesReserverForSize];
            Array.Copy(incomingPacket, bytesReserverForSize, temp, 0, temp.Length);
            incomingPacket=temp;
        }

        void PrepareForNextPacket() {
            position=0;
            incomingPacket=new byte[bytesReserverForSize];
            step=Step.ReadSize;
        }

        public byte[] GetLastPacket() {
            return queue.Dequeue();
        }
    }
}
