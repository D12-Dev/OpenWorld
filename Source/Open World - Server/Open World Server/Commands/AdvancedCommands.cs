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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                string messageForConsole = "Chat - [Console] " + commandData;

                ConsoleUtils.LogToConsole(messageForConsole);

                Server.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

                foreach (ServerClient sc in Networking.connectedClients)
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                foreach (ServerClient sc in Networking.connectedClients)
                {
                    Networking.SendData(sc, "Notification│" + commandData);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.WriteWithTime("Letter Sent To Every Connected Player");
                Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] not found");
                    Console.WriteLine();
                }

                else
                {
                    Networking.SendData(targetClient, "Notification│" + text);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Sent Letter To [" + targetClient.username + "]");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                ConsoleUtils.WriteWithTime("Usage: Giveitem [username] [itemID] [itemQuantity] [itemQuality]");
                Console.WriteLine();
            }
            
            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
                }

                else
                {
                    Networking.SendData(targetClient, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Item Has Neen Gifted To Player [" + targetClient.username + "]");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                ConsoleUtils.WriteWithTime("Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]");
                Console.WriteLine();
            }

            else
            {
                foreach (ServerClient client in Networking.connectedClients)
                {
                    Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Item Has Neen Gifted To All Players");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
                }

                else
                {
                    targetClient.isImmunized = true;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = true;
                    SaveSystem.SaveUserData(targetClient);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + targetClient.username + "] Has Been Inmmunized");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
                }

                else
                {
                    targetClient.isImmunized = false;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = false;
                    SaveSystem.SaveUserData(targetClient);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + targetClient.username + "] Has Been Deinmmunized");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
                }

                else
                {
                    targetClient.eventShielded = true;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = true;

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + targetClient.username + "] Has Been Protected");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
                }

                else
                {
                    targetClient.eventShielded = false;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = false;

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + targetClient.username + "] Has Been Deprotected");
                    Console.WriteLine();
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
            ServerClient target = null;

            if (string.IsNullOrWhiteSpace(clientID)) isMissingParameters = true;
            if (string.IsNullOrWhiteSpace(eventID)) isMissingParameters = true;

            if (isMissingParameters)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
                }

                else
                {
                    Networking.SendData(target, "ForcedEvent│" + eventID);

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Sent Event [" + eventID + "] to [" + targetClient.username + "]");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            foreach (ServerClient client in Networking.connectedClients)
            {
                Networking.SendData(client, "ForcedEvent│" + eventID);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Sent Event [" + eventID + "] To Every Player");
            Console.WriteLine();
        }

        //Administration

        public static void PromoteCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
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
                        SaveSystem.SaveUserData(targetClient);

                        Networking.SendData(targetClient, "│Promote│");

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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
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
                        SaveSystem.SaveUserData(targetClient);

                        Networking.SendData(targetClient, "│Demote│");

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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Server.savedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
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
                    ConsoleUtils.WriteWithTime("Player Details: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.WriteWithTime("Username: [" + targetClient.username + "]");
                    ConsoleUtils.WriteWithTime("Password: [" + targetClient.password + "]");
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Security: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.WriteWithTime("Connection IP: [" + ip + "]");
                    ConsoleUtils.WriteWithTime("Admin: [" + targetClient.isAdmin + "]");
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Status: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.WriteWithTime("Online: [" + isConnected + "]");
                    ConsoleUtils.WriteWithTime("Immunized: [" + targetClient.isImmunized + "]");
                    ConsoleUtils.WriteWithTime("Event Shielded: [" + targetClient.eventShielded + "]");
                    ConsoleUtils.WriteWithTime("In RTSE: [" + targetClient.inRTSE + "]");
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Wealth: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.WriteWithTime("Stored Gifts: [" + targetClient.giftString.Count + "]");
                    ConsoleUtils.WriteWithTime("Stored Trades: [" + targetClient.tradeString.Count + "]");
                    ConsoleUtils.WriteWithTime("Wealth Value: [" + targetClient.wealth + "]");
                    ConsoleUtils.WriteWithTime("Pawn Count: [" + targetClient.pawnCount + "]");
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Details: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleUtils.WriteWithTime("Home Tile ID: [" + targetClient.homeTileID + "]");
                    ConsoleUtils.WriteWithTime("Faction: [" + (targetClient.faction == null ? "None" : targetClient.faction.name)  + "]");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                Faction factionToSearch = Server.factionList.Find(fetch => fetch.name == commandData);

                if (factionToSearch == null)
                {
                    ConsoleUtils.WriteWithTime("Faction " + commandData + " Was Not Found");
                    Console.WriteLine();
                }

                else
                {
                    ConsoleUtils.WriteWithTime("Faction Details Of [" + factionToSearch.name + "]:");
                    Console.WriteLine();

                    ConsoleUtils.WriteWithTime("Members:");
                    Console.ForegroundColor = ConsoleColor.White;

                    foreach (KeyValuePair<ServerClient, FactionHandler.MemberRank> member in factionToSearch.members)
                    {
                        ConsoleUtils.WriteWithTime(member.Key.username + " - " + member.Value);
                    }

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Wealth:");
                    Console.ForegroundColor = ConsoleColor.White;

                    ConsoleUtils.WriteWithTime(factionToSearch.wealth.ToString());
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Structures:");
                    Console.ForegroundColor = ConsoleColor.White;

                    ConsoleUtils.WriteWithTime("TODO");

                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
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
                ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                Console.WriteLine();
            }    
        }

        public static void KickCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleUtils.WriteWithTime("Missing Parameters");
                Console.WriteLine();
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + clientID + "] Not Found");
                    Console.WriteLine();
                }

                else
                {
                    targetClient.disconnectFlag = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleUtils.WriteWithTime("Player [" + targetClient.username + "] Has Been Kicked");
                    Console.WriteLine();
                }
            }
        }
    }
}