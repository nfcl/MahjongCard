using Mirror;
using System;
using System.Net;

namespace Network
{
    public class ServerResponse : NetworkMessage
    {
        public long serverId;
        public string hostName;
        public int playerNum;
        public int playerCapacity;
        public Uri uri;
        public IPEndPoint endPoint { get; set; }
    }
}