using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenWorldServer
{
    [System.Serializable]
    public static partial class Server
    {
        static void Main()
        {
            ServerUtils.SetPaths();
            ServerUtils.SetCulture();

            ServerUtils.CheckServerVersion();
            ServerUtils.CheckClientVersionRequirement();
            ServerUtils.CheckSettingsFile();

            ModHandler.CheckMods();
            FactionHandler.CheckFactions();
            WorldHandler.CheckWorldFile();
            PlayerUtils.CheckAllAvailablePlayers();

            Threading.GenerateThreads(0);

            while (!exit) ListenForCommands();
        }


        public static void ListenForCommands()
        {
            string[] commandParts = Console.ReadLine().Trim().Split(" "), commandArgs = commandParts.TakeLast(commandParts.Length - 1).ToArray();
            string commandWord = commandParts[0].ToLower();
            try
            {
                Command invoked = ServerCommands.Where(x => x.Word == commandWord).SingleOrDefault();
                if (invoked != null) invoked.Execute(commandArgs);
                else SimpleCommands.UnknownCommand(commandWord);
            }
            catch (Exception ex)
            {
                ConsoleUtils.LogToConsole($"ERROR: {ex.Message}", ConsoleUtils.ConsoleLogMode.Error);
            }
        }
    }
}