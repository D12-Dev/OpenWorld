using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace OpenWorldServer
{
    public static class FactionHandler
    {
        private const string membersFileName = "Members.txt";
        private const string ranksFileName = "Ranks.txt";
        private const string wealthFileName = "Wealth.txt";

        public enum MemberRank { Member, Moderator, Leader }

        public static void CheckFactions()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Factions Check:");
            Console.ForegroundColor = ConsoleColor.White;

            if (!Directory.Exists(Server.factionsFolderPath))
            {
                Directory.CreateDirectory(Server.factionsFolderPath);
                ConsoleUtils.LogToConsole("No Factions Folder Found, Generating");
                Console.WriteLine("");
            }

            else
            {
                string[] factionFiles = Directory.GetFiles(Server.factionsFolderPath);

                if (factionFiles.Length == 0)
                {
                    ConsoleUtils.LogToConsole("No Factions Found, Ignoring");
                    Console.WriteLine("");
                }

                else LoadFactions(factionFiles);
            }
        }

        public static void CreateFaction(string factionName, ServerClient factionLeader)
        {
            Faction newFaction = new Faction();
            newFaction.name = factionName;
            newFaction.wealth = 0;
            newFaction.members.Add(factionLeader, MemberRank.Leader);
            SaveFaction(newFaction);

            factionLeader.faction = newFaction;

            ServerClient clientToSave = Server.savedClients.Find(fetch => fetch.username == factionLeader.username);
            clientToSave.faction = newFaction;
            SaveSystem.SaveUserData(clientToSave);

            Networking.SendData(factionLeader, "FactionManagement│Created");

            Thread.Sleep(100);

            Networking.SendData(factionLeader, GetFactionDetails(factionLeader));
        }

        public static void SaveFaction(Faction factionToSave)
        {
            string factionSavePath = Server.factionsFolderPath + Path.DirectorySeparatorChar + factionToSave.name + ".bin";

            if (factionToSave.members.Count() > 1)
            {
                //Order faction members dictionary to order
            }
            
            Stream s = File.OpenWrite(factionSavePath);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(s, factionToSave);

            s.Flush();
            s.Close();
            s.Dispose();

            if (!Server.factionList.Contains(factionToSave)) Server.factionList.Add(factionToSave);
        }

        public static void LoadFactions(string[] factionFiles)
        {
            int failedToLoadFactions = 0;

            foreach (string faction in factionFiles)
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream s = File.Open(faction, FileMode.Open);
                    object obj = formatter.Deserialize(s);
                    Faction factionToLoad = (Faction)obj;

                    s.Flush();
                    s.Close();
                    s.Dispose();

                    if (factionToLoad.members.Count < 1)
                    {
                        DisbandFaction(factionToLoad);
                        continue;
                    }

                    if (!Server.factionList.Contains(factionToLoad)) Server.factionList.Add(factionToLoad);
                }
                catch { failedToLoadFactions++; }
            }

            ConsoleUtils.LogToConsole("Loaded [" + Server.factionList.Count() + "] Factions");

            if (failedToLoadFactions > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleUtils.LogToConsole("Failed to load [" + failedToLoadFactions + "] Factions");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("");
        }

        public static void DisbandFaction(Faction factionToDisband)
        {
            Server.factionList.Remove(factionToDisband);

            string factionSavePath = Server.factionsFolderPath + Path.DirectorySeparatorChar + factionToDisband.name + ".bin";
            File.Delete(factionSavePath);
        }

        public static string GetFactionName(Faction factionToLoad)
        {
            string factionFolder = factionToLoad.folderPath;

            return Path.GetDirectoryName(factionFolder);
        }

        public static Dictionary<ServerClient, MemberRank> GetFactionMembers(Faction factionToLoad)
        {
            string factionFolder = factionToLoad.folderPath;
            string membersFilePath = factionFolder + Path.DirectorySeparatorChar + membersFileName;

            string[] allMembers = File.ReadAllLines(membersFilePath);
            Dictionary<ServerClient, MemberRank> factionMembers = new Dictionary<ServerClient, MemberRank>();

            foreach (string str in allMembers)
            {
                ServerClient clientToFind = Server.savedClients.Find(fetch => fetch.username == str);
                MemberRank clientRank = (MemberRank)GetMemberRank(factionToLoad, clientToFind);

                if (clientToFind != null) factionMembers.Add(clientToFind, MemberRank.Member);
                else continue;
            }

            return factionMembers;
        }

        public static MemberRank GetMemberRank(Faction factionToLoad, ServerClient memberToCheck)
        {
            string factionFolder = factionToLoad.folderPath;
            string rankFilePath = factionFolder + Path.DirectorySeparatorChar + ranksFileName;
            string membersFilePath = factionFolder + Path.DirectorySeparatorChar + membersFileName;

            int memberIndex = File.ReadAllLines(membersFilePath).ToList().IndexOf(memberToCheck.username);
            return (MemberRank)int.Parse(File.ReadAllLines(rankFilePath)[memberIndex]);
        }

        public static string GetFactionDetails(ServerClient client)
        {
            string dataToSend = "FactionManagement│Details│";

            if (client.faction == null) return dataToSend;

            else
            {
                dataToSend += client.faction.name + "│";

                foreach (KeyValuePair<ServerClient, MemberRank> member in client.faction.members)
                {
                    dataToSend += member.Key.username + ":" + (int)member.Value + "»";
                }

                return dataToSend;
            }
        }

        public static int GetFactionWealth(Faction factionToLoad)
        {
            string factionFolder = factionToLoad.folderPath;
            string wealthFilePath = factionFolder + Path.DirectorySeparatorChar + wealthFileName;

            return int.Parse(File.ReadAllLines(wealthFilePath)[0]);
        }

        public static void AddMember(Faction faction, ServerClient memberToAdd)
        {
            faction.members.Add(memberToAdd, MemberRank.Member);
            SaveFaction(faction);

            memberToAdd.faction = faction;

            ServerClient clientToSave = Server.savedClients.Find(fetch => fetch.username == memberToAdd.username);
            clientToSave.faction = faction;
            SaveSystem.SaveUserData(clientToSave);

            UpdateAllPlayersInFaction(faction);
        }

        public static void RemoveMember(Faction faction, ServerClient memberToRemove)
        {
            faction.members.Remove(memberToRemove);
            if (faction.members.Count > 0) SaveFaction(faction);
            else DisbandFaction(faction);

            memberToRemove.faction = null;

            ServerClient clientToSave = Server.savedClients.Find(fetch => fetch.username == memberToRemove.username);
            clientToSave.faction = null;
            SaveSystem.SaveUserData(clientToSave);

            Networking.SendData(memberToRemove, GetFactionDetails(memberToRemove));

            UpdateAllPlayersInFaction(faction);
        }

        public static void UpdateAllPlayersInFaction(Faction faction)
        {
            foreach (ServerClient member in Networking.connectedClients)
            {
                if (member.faction == faction) Networking.SendData(member, GetFactionDetails(member));
            }
        }

        public static void ChangeMemberRank(Faction faction, ServerClient memberToChange, MemberRank newRank)
        {
            faction.members[memberToChange] = newRank;

            SaveFaction(faction);
        }

        public static ServerClient GetFactionLeader(Faction faction)
        {
            foreach (KeyValuePair<ServerClient, FactionHandler.MemberRank> member in faction.members)
            {
                if (member.Value == MemberRank.Leader) return member.Key;
            }

            return null;
        }
    }
}