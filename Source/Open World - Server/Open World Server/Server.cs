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
            if (string.Join(' ', commandArgs).Count(x => x == '"')%2 != 0) ConsoleUtils.LogToConsole("Uneven amount of quotation marks detected, aborting.", ConsoleUtils.ConsoleLogMode.Error);
            else
            {
                // TODO: The last IsNullOrWhiteSpace() is to deal with quotes at the end of command strings. Clean that up so it's not necessary.
                string[] processedCommandArgs = string.Join(' ', commandArgs).Split('"').Select(x => x.Trim()).Select((c, i) => i % 2 == 0 ? c.Split(' ') : new string[1] { c }).SelectMany(x => x).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                try
                {
                    Command invoked = ServerCommands.Where(x => x.Word == commandWord).SingleOrDefault();
                    if (invoked != null) invoked.Execute(processedCommandArgs);
                    else SimpleCommands.UnknownCommand(commandWord);
                }
                catch (Exception ex)
                {
                    ConsoleUtils.LogToConsole($"ERROR: {ex.Message}", ConsoleUtils.ConsoleLogMode.Error);
                }
            }
        }
    }
}