using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace OpenWorldServer
{
    [System.Serializable]
    public class ServerClient
    {
        //Security
        [NonSerialized] public TcpClient tcp;
        public string username;
        public string password;
        public bool isAdmin = false;
        public bool toWipe = false;
        [NonSerialized] public bool disconnectFlag;

        //Relevant Data
        public string homeTileID;
        public List<string> giftString = new List<string>();
        public List<string> tradeString = new List<string>();
        [NonSerialized] public Faction faction;

        //Wealth Data
        public int pawnCount;
        public float wealth;

        //Variables Data
        public bool isImmunized = false;
        [NonSerialized] public bool eventShielded;
        [NonSerialized] public bool inRTSE;
        [NonSerialized] public ServerClient inRtsActionWith;

        public ServerClient(TcpClient userSocket)
        {
            tcp = userSocket;
        }
    }
}
