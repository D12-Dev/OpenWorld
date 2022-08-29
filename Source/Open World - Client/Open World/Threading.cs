using System.Threading;

namespace OpenWorld
{
    public class Threading
    {
        public void GenerateThreads(int threadID)
        {
            if (threadID == 0)
            {
                Thread networkingThread = new Thread(new ThreadStart(Main._Networking.TryConnectToServer));
                networkingThread.IsBackground = true;
                networkingThread.Name = "Connection Thread";
                networkingThread.Start();
            }

            else if (threadID == 1)
            {
                Thread CheckThread = new Thread(() => Main._Networking.CheckConnection());
                CheckThread.IsBackground = true;
                CheckThread.Name = "Check Thread";
                CheckThread.Start();
            }

            else if (threadID == 2)
            {
                Thread PvPThread = new Thread(() => Main._Networking.PvPChannel());
                PvPThread.IsBackground = true;
                PvPThread.Name = "PvP Thread";
                PvPThread.Start();
            }

            else return;
        }
    }
}
