using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace OpenWorldServer
{
    public static class AdvancedCommands
    {
        public static string commandData;

        //Communication

        public static void SayCommand()
        {
            if (string.IsNullOrWhiteSpace(commandData))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                string messageForConsole = "Chat - [Console] " + commandData;

                ConsoleUtils.LogToConsole(messageForConsole);

                Server.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient sc in clients)
                {
                    Networking.SendData(sc, "ChatMessage│SERVER│" + commandData);
                }
            }
        }

        public static void BroadcastCommand()
        {
            if (string.IsNullOrWhiteSpace(commandData))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient sc in clients)
                {
                    Networking.SendData(sc, "Notification│" + commandData);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Letter Sent To Every Connected Player");
                ConsoleUtils.LogToConsole("");
            }
        }

        public static void NotifyCommand()
        {
            bool isMissingParameters = false;

            string clientID = commandData.Split(' ')[0];
            string text = commandData.Replace(clientID + " ", "");

            if (string.IsNullOrWhiteSpace(clientID)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(text)) isMissingParameters = true;

            if (isMissingParameters)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] not found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    Networking.SendData(targetClient, "Notification│" + text);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Sent Letter To [" + targetClient.username + "]");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        //Items

        public static void GiveItemCommand()
        {
            Console.Clear();

            bool isMissingParameters = false;

            string clientID = commandData.Split(' ')[0];
            string itemID = commandData.Split(' ')[1];
            string itemQuantity = commandData.Split(' ')[2];
            string itemQuality = commandData.Split(' ')[3];

            if (string.IsNullOrWhiteSpace(clientID)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(itemID)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(itemQuantity)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(itemQuality)) isMissingParameters = true;

            if (isMissingParameters)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("Usage: Giveitem [username] [itemID] [itemQuantity] [itemQuality]");
                ConsoleUtils.LogToConsole("");
            }
            
            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    Networking.SendData(targetClient, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Item Has Neen Gifted To Player [" + targetClient.username + "]");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        public static void GiveItemAllCommand()
        {
            Console.Clear();

            bool isMissingParameters = false;

            string itemID = commandData.Split(' ')[0];
            string itemQuantity = commandData.Split(' ')[1];
            string itemQuality = commandData.Split(' ')[2];

            if (string.IsNullOrWhiteSpace(itemID)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(itemQuantity)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(itemQuality)) isMissingParameters = true;

            if (isMissingParameters)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient client in clients)
                {
                    Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Item Has Neen Gifted To All Players");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        //Anti-PvP

        public static void ImmunizeCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    targetClient.isImmunized = true;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = true;
                    PlayerUtils.SavePlayer(targetClient);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Inmmunized");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        public static void DeimmunizeCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    targetClient.isImmunized = false;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = false;
                    PlayerUtils.SavePlayer(targetClient);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Deinmmunized");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        public static void ProtectCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    targetClient.eventShielded = true;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = true;

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Protected");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        public static void DeprotectCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    targetClient.eventShielded = false;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = false;

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Deprotected");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        //Events

        public static void InvokeCommand()
        {
            Console.Clear();

            bool isMissingParameters = false;

            string clientID = commandData.Split(' ')[0];
            string eventID = commandData.Split(' ')[1];

            if (string.IsNullOrWhiteSpace(clientID)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(eventID)) isMissingParameters = true;

            if (isMissingParameters)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    Networking.SendData(targetClient, "ForcedEvent│" + eventID);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Sent Event [" + eventID + "] to [" + targetClient.username + "]");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        public static void PlagueCommand()
        {
            Console.Clear();

            string eventID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(eventID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            ServerClient[] clients = Networking.connectedClients.ToArray();
            foreach (ServerClient client in clients)
            {
                Networking.SendData(client, "ForcedEvent│" + eventID);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Sent Event [" + eventID + "] To Every Player");
            ConsoleUtils.LogToConsole("");
        }

        //Administration

        public static void PromoteCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    if (targetClient.isAdmin == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Was Already An Administrator");
                        ConsoleUtils.LogToConsole(Environment.NewLine);
                    }

                    else
                    {
                        targetClient.isAdmin = true;
                        Server.savedClients.Find(fetch => fetch.username == clientID).isAdmin = true;
                        PlayerUtils.SavePlayer(targetClient);

                        Networking.SendData(targetClient, "Admin│Promote");

                        Console.ForegroundColor = ConsoleColor.Green;
                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Promoted");
                        ConsoleUtils.LogToConsole(Environment.NewLine);
                    }
                }
            }
        }

        public static void DemoteCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    if (!targetClient.isAdmin)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Is Not An Administrator");
                        ConsoleUtils.LogToConsole(Environment.NewLine);
                    }

                    else
                    {
                        targetClient.isAdmin = false;
                        Server.savedClients.Find(fetch => fetch.username == targetClient.username).isAdmin = false;
                        PlayerUtils.SavePlayer(targetClient);

                        Networking.SendData(targetClient, "Admin│Demote");

                        Console.ForegroundColor = ConsoleColor.Green;
                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Demoted");
                        ConsoleUtils.LogToConsole(Environment.NewLine);
                    }
                }
            }
        }

        public static void PlayerDetailsCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];
            ServerClient clientToInvestigate = null;

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Server.savedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    bool isConnected = false;
                    string ip = "None";

                    if (Networking.connectedClients.Find(fetch => fetch.username == targetClient.username) != null)
                    {
                        clientToInvestigate = Networking.connectedClients.Find(fetch => fetch.username == targetClient.username);
                        isConnected = true;
                        ip = ((IPEndPoint)clientToInvestigate.tcp.Client.RemoteEndPoint).Address.ToString();
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player Details: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.LogToConsole("Username: [" + targetClient.username + "]");
                    ConsoleUtils.LogToConsole("Password: [" + targetClient.password + "]");
                    ConsoleUtils.LogToConsole("");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Security: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.LogToConsole("Connection IP: [" + ip + "]");
                    ConsoleUtils.LogToConsole("Admin: [" + targetClient.isAdmin + "]");
                    ConsoleUtils.LogToConsole("");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Status: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.LogToConsole("Online: [" + isConnected + "]");
                    ConsoleUtils.LogToConsole("Immunized: [" + targetClient.isImmunized + "]");
                    ConsoleUtils.LogToConsole("Event Shielded: [" + targetClient.eventShielded + "]");
                    ConsoleUtils.LogToConsole("In RTSE: [" + targetClient.inRTSE + "]");
                    ConsoleUtils.LogToConsole("");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Wealth: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.LogToConsole("Stored Gifts: [" + targetClient.giftString.Count + "]");
                    ConsoleUtils.LogToConsole("Stored Trades: [" + targetClient.tradeString.Count + "]");
                    ConsoleUtils.LogToConsole("Wealth Value: [" + targetClient.wealth + "]");
                    ConsoleUtils.LogToConsole("Pawn Count: [" + targetClient.pawnCount + "]");
                    ConsoleUtils.LogToConsole("");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Details: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.LogToConsole("Home Tile ID: [" + targetClient.homeTileID + "]");
                    ConsoleUtils.LogToConsole("Faction: [" + (targetClient.faction == null ? "None" : targetClient.faction.name)  + "]");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        public static void FactionDetailsCommand()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;

            string factionID = commandData.Split(' ')[0];
            if (string.IsNullOrWhiteSpace(factionID))
            {
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                Faction factionToSearch = Server.savedFactions.Find(fetch => fetch.name == commandData);

                if (factionToSearch == null)
                {
                    ConsoleUtils.LogToConsole("Faction " + commandData + " Was Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    ConsoleUtils.LogToConsole("Faction Details Of [" + factionToSearch.name + "]:");
                    ConsoleUtils.LogToConsole("");

                    ConsoleUtils.LogToConsole("Members:");
                    Console.ForegroundColor = ConsoleColor.White;

                    foreach (KeyValuePair<ServerClient, FactionHandler.MemberRank> member in factionToSearch.members)
                    {
                        ConsoleUtils.LogToConsole("[" + member.Value + "]" + " - " + member.Key.username);
                    }

                    ConsoleUtils.LogToConsole("");
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Wealth:");
                    Console.ForegroundColor = ConsoleColor.White;

                    ConsoleUtils.LogToConsole(factionToSearch.wealth.ToString());
                    ConsoleUtils.LogToConsole("");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Structures:");
                    Console.ForegroundColor = ConsoleColor.White;

                    if (factionToSearch.factionStructures.Count == 0) ConsoleUtils.LogToConsole("No Structures");
                    else
                    {
                        FactionStructure[] structures = factionToSearch.factionStructures.ToArray();
                        foreach (FactionStructure structure in structures)
                        {
                            ConsoleUtils.LogToConsole("[" + structure.structureTile + "]" + " - " + structure.structureName);
                        }
                    }

                    ConsoleUtils.LogToConsole("");
                }
            }
        }

        //Security

        public static void BanCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    Server.bannedIPs.Add(((IPEndPoint)targetClient.tcp.Client.RemoteEndPoint).Address.ToString(), targetClient.username);
                    targetClient.disconnectFlag = true;

                    SaveSystem.SaveBannedIPs(Server.bannedIPs);
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Banned");
                    ConsoleUtils.LogToConsole(Environment.NewLine);
                }
            }
        }

        public static void PardonCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                foreach (KeyValuePair<string, string> pair in Server.bannedIPs)
                {
                    if (pair.Value == clientID)
                    {
                        Server.bannedIPs.Remove(pair.Key);
                        SaveSystem.SaveBannedIPs(Server.bannedIPs);
                        Console.ForegroundColor = ConsoleColor.Green;
                        ConsoleUtils.LogToConsole("Player [" + pair.Value + "] Has Been Unbanned");
                        Console.ForegroundColor = ConsoleColor.White;
                        ConsoleUtils.LogToConsole(Environment.NewLine);
                        return;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                ConsoleUtils.LogToConsole("");
            }    
        }

        public static void KickCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.LogToConsole("Missing Parameters");
                ConsoleUtils.LogToConsole("");
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found");
                    ConsoleUtils.LogToConsole("");
                }

                else
                {
                    targetClient.disconnectFlag = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Kicked");
                    ConsoleUtils.LogToConsole("");
                }
            }
        }
    }
}