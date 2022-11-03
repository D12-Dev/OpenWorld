using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenWorldServer
{
    public static class WorldHandler
    {
        public static void CheckWorldFile()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("World Check:");
            Console.ForegroundColor = ConsoleColor.White;

            if (File.Exists(Server.worldSettingsPath))
            {
                string[] settings = File.ReadAllLines(Server.worldSettingsPath);

                foreach (string setting in settings)
                {
                    if (setting.StartsWith("Globe Coverage (0.3, 0.5, 1.0): "))
                    {
                        string splitString = setting.Replace("Globe Coverage (0.3, 0.5, 1.0): ", "");
                        Server.globeCoverage = float.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Seed: "))
                    {
                        string splitString = setting.Replace("Seed: ", "");
                        Server.seed = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Overall Rainfall (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Rainfall (0-6): ", "");
                        Server.overallRainfall = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Temperature (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Temperature (0-6): ", "");
                        Server.overallTemperature = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Population (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Population (0-6): ", "");
                        Server.overallPopulation = int.Parse(splitString);
                        continue;
                    }
                }

                ConsoleUtils.LogToConsole("Loaded World File");
                Console.WriteLine("");
            }

            else
            {
                string[] settingsPreset = new string[]
{
                    "- World Settings -",
                    "Globe Coverage (0.3, 0.5, 1.0): 0.3",
                    "Seed: Seed",
                    "Overall Rainfall (0-6): 3",
                    "Overall Temperature (0-6): 3",
                    "Overall Population (0-6): 3"
                };

                File.WriteAllLines(Server.worldSettingsPath, settingsPreset);

                ConsoleUtils.LogToConsole("Generating World File");

                CheckWorldFile();
            }
        }
    }
}
