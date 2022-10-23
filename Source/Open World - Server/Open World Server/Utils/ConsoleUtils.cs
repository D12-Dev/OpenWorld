using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenWorldServer
{
    public static class ConsoleUtils
    {
        public static void UpdateTitle()
        {
            Console.Title = Server.serverName + " " + Server.serverVersion + " / " + Networking.localAddress.ToString() + " / " + Networking.connectedClients.Count + " Of " + Server.maxPlayers + " Connected Players";
        }

        public static void LogToConsole(string data)
        {
            string dataToLog = "";
            if (data != Environment.NewLine) dataToLog = "[" + DateTime.Now + "]" + " │ " + data;
            else dataToLog = "";

            Console.WriteLine(dataToLog);

            if (data.StartsWith("Chat - [")) WriteToLog(dataToLog, "Chat");
            else if (data.StartsWith("Gift Done Between")) WriteToLog(dataToLog, "Gift");
            else if (data.StartsWith("Trade Done Between")) WriteToLog(dataToLog, "Trade");
            else if (data.StartsWith("Barter Done Between")) WriteToLog(dataToLog, "Barter");
            else if (data.StartsWith("Spy Done Between")) WriteToLog(dataToLog, "Spy");
            else if (data.StartsWith("PvP Done Between")) WriteToLog(dataToLog, "PvP");
            else if (data.StartsWith("Visit Done Between")) WriteToLog(dataToLog, "Visit");
            else WriteToLog(dataToLog, "Normal");
        }

        public static void WriteWithTime(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) Console.WriteLine();
            else Console.WriteLine("[" + DateTime.Now + "] | " + str);
        }

        public static void WriteToLog(string data, string logMode)
        {
            string pathToday = Server.logFolderPath + Path.DirectorySeparatorChar + DateTime.Today.Month + "-" + DateTime.Today.Day + "-" + DateTime.Today.Year;
            if (!Directory.Exists(pathToday)) Directory.CreateDirectory(pathToday);

            string logName;
            if (logMode == "Chat") logName = "Chat.txt";
            else if (logMode == "Gift") logName = "Gift.txt";
            else if (logMode == "Trade") logName = "Trade.txt";
            else if (logMode == "Barter") logName = "Barter.txt";
            else if (logMode == "Spy") logName = "Spy.txt";
            else if (logMode == "PvP") logName = "PvP.txt";
            else if (logMode == "Visit") logName = "Visit.txt";
            else logName = "Log.txt";

            try { File.AppendAllText(pathToday + Path.DirectorySeparatorChar + logName, data + Environment.NewLine); }
            catch { }
        }

        public static void DisplayNetworkStatus()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            LogToConsole("Network Check:");

            Console.ForegroundColor = ConsoleColor.White;
            LogToConsole("Server Started");
            LogToConsole("Type 'Help' To See Available Commands");
            LogToConsole("Network Line Started");
            Console.WriteLine("");
        }
    }
}
