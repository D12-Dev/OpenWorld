using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            if (string.IsNullOrWhiteSpace(commandData)) ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            

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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient sc in clients)
                {
                    Networking.SendData(sc, "Notification│" + commandData);
                }

                ConsoleUtils.LogToConsole("Letter Sent To Every Connected Player", ConsoleUtils.ConsoleLogMode.Info);
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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null) ConsoleUtils.LogToConsole("Player [" + clientID + "] not found", ConsoleUtils.ConsoleLogMode.Info);
                

                else
                {
                    Networking.SendData(targetClient, "Notification│" + text);

                    ConsoleUtils.LogToConsole("Sent Letter To [" + targetClient.username + "]", ConsoleUtils.ConsoleLogMode.Info);
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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
                ConsoleUtils.LogToConsole("Usage: Giveitem [username] [itemID] [itemQuantity] [itemQuality]");
            }
            
            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);
                }

                else
                {
                    Networking.SendData(targetClient, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    ConsoleUtils.LogToConsole("Item Has Neen Gifted To Player [" + targetClient.username + "]", ConsoleUtils.ConsoleLogMode.Info);
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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
                ConsoleUtils.LogToConsole("Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]");
            }

            else
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient client in clients)
                {
                    Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    ConsoleUtils.LogToConsole("Item Has Neen Gifted To All Players", ConsoleUtils.ConsoleLogMode.Info);
                }
            }
        }

        //Anti-PvP

        public static void ImmunizeCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID)) ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null) ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);
                

                else
                {
                    targetClient.isImmunized = true;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = true;
                    PlayerUtils.SavePlayer(targetClient);

                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Inmmunized", ConsoleUtils.ConsoleLogMode.Info);
                }
            }
        }

        public static void DeimmunizeCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null) ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);

                

                else
                {
                    targetClient.isImmunized = false;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = false;
                    PlayerUtils.SavePlayer(targetClient);

                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Deinmmunized", ConsoleUtils.ConsoleLogMode.Info]
                        );
                }
            }
        }

        public static void ProtectCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID)) ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null) ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);

               

                else
                {
                    targetClient.eventShielded = true;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = true;


                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Protected", ConsoleUtils.ConsoleLogMode.Info);

                }
            }
        }

        public static void DeprotectCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);

            

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);


                else
                {
                    targetClient.eventShielded = false;
                    Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = false;

                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Deprotected", ConsoleUtils.ConsoleLogMode.Info);
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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
   
            

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);
           
                

                else
                {
                    Networking.SendData(targetClient, "ForcedEvent│" + eventID);

     
                    ConsoleUtils.LogToConsole("Sent Event [" + eventID + "] to [" + targetClient.username + "]", ConsoleUtils.ConsoleLogMode.Info);
          
                }
            }
        }

        public static void PlagueCommand()
        {
            Console.Clear();

            string eventID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(eventID))
            
  
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);

            

            ServerClient[] clients = Networking.connectedClients.ToArray();
            foreach (ServerClient client in clients)
            {
                Networking.SendData(client, "ForcedEvent│" + eventID);
            }


            ConsoleUtils.LogToConsole("Sent Event [" + eventID + "] To Every Player", ConsoleUtils.ConsoleLogMode.Info);

        }

        //Administration

        public static void PromoteCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            
 
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
   
            

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
  
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);

                }

                else
                {
                    if (targetClient.isAdmin == true)
                    {
             
                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Was Already An Administrator", ConsoleUtils.ConsoleLogMode.Info);
 
                    }

                    else
                    {
                        targetClient.isAdmin = true;
                        Server.savedClients.Find(fetch => fetch.username == clientID).isAdmin = true;
                        PlayerUtils.SavePlayer(targetClient);

                        Networking.SendData(targetClient, "Admin│Promote");

                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Promoted", ConsoleUtils.ConsoleLogMode.Info);
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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);
                }

                else
                {
                    if (!targetClient.isAdmin)
                    {
                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Is Not An Administrator", ConsoleUtils.ConsoleLogMode.Info);
                    }

                    else
                    {
                        targetClient.isAdmin = false;
                        Server.savedClients.Find(fetch => fetch.username == targetClient.username).isAdmin = false;
                        PlayerUtils.SavePlayer(targetClient);

                        Networking.SendData(targetClient, "Admin│Demote");

                        ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Demoted", ConsoleUtils.ConsoleLogMode.Info);
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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                ServerClient targetClient = Server.savedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Warning);

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

                    ConsoleUtils.LogToConsole("Player Details", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole("Username: [" + targetClient.username + "]\nPassword: [" + targetClient.password + "]");
                    ConsoleUtils.LogToConsole("Security", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole("Connection IP: [" + ip + "]\nAdmin: [" + targetClient.isAdmin + "]");
                    ConsoleUtils.LogToConsole("Status", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole("Online: [" + isConnected + "]\nImmunized: [" + targetClient.isImmunized + "]\nEvent Shielded: [" + targetClient.eventShielded + "]\nIn RTSE: [" + targetClient.inRTSE + "]");
                    ConsoleUtils.LogToConsole("Wealth",ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole("Stored Gifts: [" + targetClient.giftString.Count + "]\nStored Trades: [" + targetClient.tradeString.Count + "]\nWealth Value: [" + targetClient.wealth + "]\nPawn Count: [" + targetClient.pawnCount + "]");
                    ConsoleUtils.LogToConsole("Details",ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole("Home Tile ID: [" + targetClient.homeTileID + "]\nFaction: [" + (targetClient.faction == null ? "None" : targetClient.faction.name) + "]");
                }
            }
        }

        public static void FactionDetailsCommand()
        {
            Console.Clear();

            string factionID = commandData.Split(' ')[0];
            if (string.IsNullOrWhiteSpace(factionID))
            {
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                Faction factionToSearch = Server.savedFactions.Find(fetch => fetch.name == commandData);

                if (factionToSearch == null)
                {
                    ConsoleUtils.LogToConsole("Faction " + commandData + " Was Not Found", ConsoleUtils.ConsoleLogMode.Info);
                }

                else
                {
                    ConsoleUtils.LogToConsole("Faction Details Of [" + factionToSearch.name + "]", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole("Members",ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole(string.Join('\n', factionToSearch.members.Select(x => $"[{x.Value}] - {x.Key.username}")));
                    ConsoleUtils.LogToConsole("Wealth", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole(factionToSearch.wealth.ToString());
                    ConsoleUtils.LogToConsole("Structures", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole(factionToSearch.factionStructures.Count == 0 ? "No Structures" : string.Join('\n', factionToSearch.factionStructures.Select(x => $"[{x.structureTile}] - {x.structureName}")));
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
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);
                }

                else
                {
                    Server.bannedIPs.Add(((IPEndPoint)targetClient.tcp.Client.RemoteEndPoint).Address.ToString(), targetClient.username);
                    targetClient.disconnectFlag = true;

                    SaveSystem.SaveBannedIPs(Server.bannedIPs);
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Banned", ConsoleUtils.ConsoleLogMode.Info);
                }
            }
        }

        public static void PardonCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                foreach (KeyValuePair<string, string> pair in Server.bannedIPs)
                {
                    if (pair.Value == clientID)
                    {
                        Server.bannedIPs.Remove(pair.Key);
                        SaveSystem.SaveBannedIPs(Server.bannedIPs);
                        ConsoleUtils.LogToConsole("Player [" + pair.Value + "] Has Been Unbanned", ConsoleUtils.ConsoleLogMode.Info);
                        // TODO: This is unstructured.
                        return;
                    }
                }

                ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);
            }    
        }

        public static void KickCommand()
        {
            Console.Clear();

            string clientID = commandData.Split(' ')[0];

            if (string.IsNullOrWhiteSpace(clientID))
            {
                ConsoleUtils.LogToConsole("Missing Parameters", ConsoleUtils.ConsoleLogMode.Warning);
            }

            else
            {
                ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == clientID);

                if (targetClient == null)
                {
                    ConsoleUtils.LogToConsole("Player [" + clientID + "] Not Found", ConsoleUtils.ConsoleLogMode.Info);
                }

                else
                {
                    targetClient.disconnectFlag = true;
                    ConsoleUtils.LogToConsole("Player [" + targetClient.username + "] Has Been Kicked", ConsoleUtils.ConsoleLogMode.Info);
                }
            }
        }
    }
}