using System.Threading;

namespace OpenWorld
{
    public static class Threading
    {
        public static void GenerateThreads(int threadID)
        {
            if (threadID == 0)
            {
                Thread networkingThread = new Thread(new ThreadStart(Networking.TryConnectToServer));
                networkingThread.IsBackground = true;
                networkingThread.Name = "Connection Thread";
                networkingThread.Start();
            }

            else if (threadID == 1)
            {
                Thread CheckThread = new Thread(() => Networking.CheckConnection());
                CheckThread.IsBackground = true;
                CheckThread.Name = "Check Thread";
                CheckThread.Start();
            }

            else if (threadID == 2)
            {
                Thread PvPThread = new Thread(() => Networking.PvPChannel());
                PvPThread.IsBackground = true;
                PvPThread.Name = "PvP Thread";
                PvPThread.Start();
            }

            else return;
        }
    }
}
