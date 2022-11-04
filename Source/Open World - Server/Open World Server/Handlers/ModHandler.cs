using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenWorldServer
{
    public static class ModHandler
    {
        public static void CheckMods(bool newLine)
        {
            if (newLine) ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Mods Check:");
            Console.ForegroundColor = ConsoleColor.White;

            CheckEnforcedMods();
            CheckWhitelistedMods();
            CheckBlacklistedMods();
        }

        public static void CheckEnforcedMods()
        {
            if (!Directory.Exists(Server.enforcedModsFolderPath))
            {
                Directory.CreateDirectory(Server.enforcedModsFolderPath);
                ConsoleUtils.LogToConsole("No Enforced Mods Folder Found, Generating");
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(Server.enforcedModsFolderPath);

                if (modFolders.Length == 0)
                {
                    ConsoleUtils.LogToConsole("No Enforced Mods Found, Ignoring");
                    return;
                }

                else LoadMods(modFolders, 0);
            }
        }

        public static void CheckWhitelistedMods()
        {
            if (!Directory.Exists(Server.whitelistedModsFolderPath))
            {
                Directory.CreateDirectory(Server.whitelistedModsFolderPath);
                ConsoleUtils.LogToConsole("No Whitelisted Mods Folder Found, Generating");
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(Server.whitelistedModsFolderPath);

                if (modFolders.Length == 0) ConsoleUtils.LogToConsole("No Whitelisted Mods Found, Ignoring");

                else LoadMods(modFolders, 1);
            }
        }

        public static void CheckBlacklistedMods()
        {
            if (!Directory.Exists(Server.blacklistedModsFolderPath))
            {
                Directory.CreateDirectory(Server.blacklistedModsFolderPath);
                ConsoleUtils.LogToConsole("No Blacklisted Mods Folder Found, Generating");
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(Server.blacklistedModsFolderPath);

                if (modFolders.Length == 0) ConsoleUtils.LogToConsole("No Blacklisted Mods Found, Ignoring");

                else LoadMods(modFolders, 2);
            }
        }

        private static void LoadMods(string[] modFolders, int modType)
        {
            int failedToLoadMods = 0;
            List<string> modList = new List<string>();

            if (modType == 0) Server.enforcedMods.Clear();
            else if (modType == 1) Server.whitelistedMods.Clear();
            else if (modType == 2) Server.blacklistedMods.Clear();

            foreach (string modFolder in modFolders)
            {
                try
                {
                    string aboutFilePath = modFolder + Path.DirectorySeparatorChar + "About" + Path.DirectorySeparatorChar + "About.xml";
                    string[] aboutLines = File.ReadAllLines(aboutFilePath);

                    foreach (string line in aboutLines)
                    {
                        if (line.Contains("<name>") && line.Contains("</name>"))
                        {
                            string modName = line;

                            string purgeString = modName.Split('<')[0];
                            modName = modName.Remove(0, purgeString.Count());

                            modName = modName.Replace("<name>", "");
                            modName = modName.Replace("</name>", "");

                            if (modName.Contains("")) modName = modName.Replace("&amp", "&");
                            if (modName.Contains("")) modName = modName.Replace("&quot", "&");
                            if (modName.Contains("")) modName = modName.Replace("&lt", "&");

                            modList.Add(modName);
                            break;
                        }
                    }
                }

                catch { failedToLoadMods++; }
            }

            modList.Sort();
            if (modType == 0)
            {
                Server.enforcedMods = modList;
                ConsoleUtils.LogToConsole("Loaded [" + Server.enforcedMods.Count() + "] Enforced Mods");
            }
            else if (modType == 1)
            {
                Server.whitelistedMods = modList;
                ConsoleUtils.LogToConsole("Loaded [" + Server.whitelistedMods.Count() + "] Whitelisted Mods");
            }
            else if (modType == 2)
            {
                Server.blacklistedMods = modList;
                ConsoleUtils.LogToConsole("Loaded [" + Server.blacklistedMods.Count() + "] Blacklisted Mods");
            }

            if (failedToLoadMods > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleUtils.LogToConsole("Failed to load [" + failedToLoadMods + "] Mods");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
