﻿using System.Threading;

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

            else return;
        }
    }
}
