using System.Threading;

namespace Open_World_Server
{
    public class Threading
    {
        public void GenerateThreads(int threadID)
        {
            //Open up server for clients
            if (threadID == 0)
            {
                Thread NetworkingThread = new Thread(new ThreadStart(MainProgram._Networking.ReadyServer));
                NetworkingThread.IsBackground = true;
                NetworkingThread.Name = "Networking Thread";
                NetworkingThread.Start();
            }

            else if (threadID == 1)
            {
                Thread CheckThread = new Thread(() => MainProgram._Networking.CheckClientsConnection());
                CheckThread.IsBackground = true;
                CheckThread.Name = "Check Thread";
                CheckThread.Start();
            }

            else return;
        }

        public void GenerateClientThread(ServerClient client)
        {
            Thread ClientThread = new Thread(() => MainProgram._Networking.ListenToClient(client));
            ClientThread.IsBackground = true;
            ClientThread.Name = "User Thread " + client.username;
            ClientThread.Start();
        }
    }
}
