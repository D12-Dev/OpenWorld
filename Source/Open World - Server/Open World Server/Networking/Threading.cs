using System.Threading;

namespace OpenWorldServer
{
    public static class Threading
    {
        public static void GenerateThreads(int threadID)
        {
            if (threadID == 0)
            {
                Thread NetworkingThread = new Thread(new ThreadStart(Networking.ReadyServer));
                NetworkingThread.IsBackground = true;
                NetworkingThread.Name = "Networking Thread";
                NetworkingThread.Start();
            }

            else if (threadID == 1)
            {
                Thread CheckThread = new Thread(() => Networking.CheckClientsConnection());
                CheckThread.IsBackground = true;
                CheckThread.Name = "Check Thread";
                CheckThread.Start();
            }

            else if (threadID == 2)
            {
                Thread CheckThread = new Thread(() => FactionProductionSiteHandler.TickProduction());
                CheckThread.IsBackground = true;
                CheckThread.Name = "Factions Production Site Thread";
                CheckThread.Start();
            }

            else return;
        }

        public static void GenerateClientThread(ServerClient client)
        {
            Thread ClientThread = new Thread(() => Networking.ListenToClient(client));
            ClientThread.IsBackground = true;
            ClientThread.Name = "User Thread " + client.username;
            ClientThread.Start();
        }
    }
}
