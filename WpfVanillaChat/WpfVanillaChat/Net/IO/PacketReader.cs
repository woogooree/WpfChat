using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfVanillaChat.Net.IO
{
    public class PacketReader : BinaryReader
    {
        private readonly NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }
        public string ReadMessage()
        {
            var length = ReadInt32();
            var msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length);
            return Encoding.ASCII.GetString(msgBuffer);
        }
        
    }
}
