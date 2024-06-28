using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using VanillaServer.Net.IO;

namespace VanillaServer
{
    public class Program
    {
        private static readonly List<Client> _users = new();
        private static readonly TcpListener _listener = new(IPAddress.Parse("127.0.0.1"), 7891);
        static void Main(string[] args)
        {
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);
                SendUserList(client);
                BroadcastConnection(client);
            }
        }
        private static void SendUserList(Client client)
        {
            foreach (var usr in _users)
            {
                var userListPacket = new PacketBuilder();
                userListPacket.WriteOpCode(1);
                userListPacket.WriteMessage(usr.Username);
                userListPacket.WriteMessage(usr.UID.ToString());
                client.ClientSocket.Client.Send(userListPacket.GetPacketBytes());
            }
        }
        private static void BroadcastConnection(Client newClient)
        {
            foreach (var client in _users)
            {
                if (client == newClient) continue; // 이미 신규 클라이언트에게는 보냈으므로 생략
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(1);
                broadcastPacket.WriteMessage(newClient.Username);
                broadcastPacket.WriteMessage(newClient.UID.ToString());
                client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }

        public static void BroadcastMessage(string message) 
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.FirstOrDefault(x => x.UID.ToString() == uid);
            if (disconnectedUser != null)
            {
                _users.Remove(disconnectedUser);

                foreach (var user in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(10);
                    broadcastPacket.WriteMessage(uid);
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }

                BroadcastMessage($"[{disconnectedUser.Username}] Disconnected!");
            }
        }
    }
}