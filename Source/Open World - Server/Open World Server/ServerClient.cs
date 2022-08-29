using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Open_World_Server
{
    [System.Serializable]
    public class ServerClient
    {
        [NonSerialized] public TcpClient tcp;
        public string username;
        public string password;
        public string homeTileID;
        public int pawnCount;
        public float wealth;
        public List<string> giftString = new List<string>();
        public List<string> tradeString = new List<string>();

        public bool isAdmin = false;
        public bool toWipe = false;
        public bool isImmunized = false;
        [NonSerialized] public bool disconnectFlag;
        [NonSerialized] public bool eventShielded;
        [NonSerialized] public bool inRTSE;
        [NonSerialized] public ServerClient inRtsActionWith;

        public ServerClient(TcpClient userSocket)
        {
            tcp = userSocket;
        }
    }
}
