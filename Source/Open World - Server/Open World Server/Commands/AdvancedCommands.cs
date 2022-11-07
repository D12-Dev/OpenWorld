using System;
using System.Linq;
using System.Net;

namespace OpenWorldServer
{
    public static class AdvancedCommands
    {
        public static void SayCommand(string[] arguments)
        {
            ConsoleUtils.LogToConsole($"Chat - [Console] {arguments[0]}");
            Server.chatCache.Add($"[{DateTime.Now}] │ {arguments[0]}");
            foreach (ServerClient sc in Networking.connectedClients) Networking.SendData(sc, $"ChatMessage│SERVER│{arguments[0]}");
        }
        public static void BroadcastCommand(string[] arguments)
        {
            foreach (ServerClient sc in Networking.connectedClients) Networking.SendData(sc, $"Notification│{arguments[0]}");
            ConsoleUtils.LogToConsole("Letter Sent To Every Connected Player", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void NotifyCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            Networking.SendData(targetClient, $"Notification│{arguments[1]}");
            ConsoleUtils.LogToConsole($"Sent Letter To {targetClient.username}", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void GiveItemCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            Networking.SendData(targetClient, $"GiftedItems│{arguments[1]}┼{arguments[2]}┼{arguments[3]}┼");
            ConsoleUtils.LogToConsole($"Item Has Neen Gifted To Player {targetClient.username}", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void GiveItemAllCommand(string[] arguments)
        {
            foreach (ServerClient client in Networking.connectedClients) Networking.SendData(client, $"GiftedItems│{arguments[0]}┼{arguments[1]}┼{arguments[2]}┼");
            ConsoleUtils.LogToConsole("Item Has Neen Gifted To All Players", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void ImmunizeCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            targetClient.isImmunized = true;
            Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = true;
            PlayerUtils.SavePlayer(targetClient);
            ConsoleUtils.LogToConsole($"Player {targetClient.username} Has Been Immunized", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void DeimmunizeCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            targetClient.isImmunized = false;
            Server.savedClients.Find(fetch => fetch.username == targetClient.username).isImmunized = false;
            PlayerUtils.SavePlayer(targetClient);
            ConsoleUtils.LogToConsole($"Player {targetClient.username} Has Been Deimmunized", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void ProtectCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            targetClient.eventShielded = true;
            Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = true;
            PlayerUtils.SavePlayer(targetClient);
            ConsoleUtils.LogToConsole($"Player {targetClient.username} Has Been Protected", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void DeprotectCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            targetClient.eventShielded = false;
            Server.savedClients.Find(fetch => fetch.username == targetClient.username).eventShielded = false;
            PlayerUtils.SavePlayer(targetClient);
            ConsoleUtils.LogToConsole($"Player {targetClient.username} Has Been Protected", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void InvokeCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            Networking.SendData(targetClient, "ForcedEvent│" + arguments[1]);
            ConsoleUtils.LogToConsole($"Sent Event {arguments[1]} to {targetClient.username}", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void PlagueCommand(string[] arguments)
        {
            foreach (ServerClient client in Networking.connectedClients) Networking.SendData(client, "ForcedEvent│" + arguments[0]);
            ConsoleUtils.LogToConsole($"Sent Event {arguments[0]} To Every Player", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void PromoteCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            if (targetClient.isAdmin == true) ConsoleUtils.LogToConsole($"Player {targetClient.username} Was Already An Administrator", ConsoleUtils.ConsoleLogMode.Info);
            else
            {
                targetClient.isAdmin = true;
                Server.savedClients.Find(fetch => fetch.username == arguments[0]).isAdmin = true;
                PlayerUtils.SavePlayer(targetClient);
                Networking.SendData(targetClient, "Admin│Promote");
                ConsoleUtils.LogToConsole($"Player {targetClient.username} Has Been Promoted", ConsoleUtils.ConsoleLogMode.Info);
            }
        }
        public static void DemoteCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            if (!targetClient.isAdmin) ConsoleUtils.LogToConsole($"Player {targetClient.username} Is Not An Administrator", ConsoleUtils.ConsoleLogMode.Info);
            else
            {
                targetClient.isAdmin = false;
                Server.savedClients.Find(fetch => fetch.username == targetClient.username).isAdmin = false;
                PlayerUtils.SavePlayer(targetClient);
                Networking.SendData(targetClient, "Admin│Demote");
                ConsoleUtils.LogToConsole($"Player {targetClient.username} Has Been Demoted", ConsoleUtils.ConsoleLogMode.Info);
            }
        }
        public static void PlayerDetailsCommand(string[] arguments)
        {
            ServerClient liveClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]), savedClient = Server.savedClients.Find(fetch => fetch.username == arguments[0]);
            bool isConnected = liveClient!=null;
            string ip = liveClient == null?"N/A - Offline":((IPEndPoint)liveClient.tcp.Client.RemoteEndPoint).Address.ToString();
            ConsoleUtils.LogToConsole("Player Details", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole($"Username: {savedClient.username}\nPassword: {savedClient.password}\n");
            ConsoleUtils.LogToConsole("Role", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole($"Admin: {savedClient.isAdmin}");
            ConsoleUtils.LogToConsole("Status", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole($"Online: {isConnected}\nConnection IP: {ip}\nImmunized: {savedClient.isImmunized}\nEvent Shielded: {savedClient.eventShielded}\nIn RTSE: {savedClient.inRTSE}");
            ConsoleUtils.LogToConsole("Wealth", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole($"Stored Gifts: {savedClient.giftString.Count}\nStored Trades: {savedClient.tradeString.Count}\nWealth Value: {savedClient.wealth}\nPawn Count: {savedClient.pawnCount}");
            ConsoleUtils.LogToConsole("Details", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole($"Home Tile ID: {savedClient.homeTileID}\nFaction: {(savedClient.faction == null ? "None" : savedClient.faction.name)}");
        }
        public static void FactionDetailsCommand(string[] arguments)
        {
                Faction factionToSearch = Server.savedFactions.Find(fetch => fetch.name == arguments[0]);
                if (factionToSearch == null) ConsoleUtils.LogToConsole($"Faction {arguments[0]} Was Not Found", ConsoleUtils.ConsoleLogMode.Info);
                else
                {
                    ConsoleUtils.LogToConsole($"Faction Details Of {factionToSearch.name}", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole("Members", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole(string.Join('\n', factionToSearch.members.Select(x => $"[{x.Value}] - {x.Key.username}")));
                    ConsoleUtils.LogToConsole("Wealth", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole(factionToSearch.wealth.ToString());
                    ConsoleUtils.LogToConsole("Structures", ConsoleUtils.ConsoleLogMode.Heading);
                    ConsoleUtils.LogToConsole(factionToSearch.factionStructures.Count == 0 ? "No Structures" : string.Join('\n', factionToSearch.factionStructures.Select(x => $"[{x.structureTile}] - {x.structureName}")));
                }
        }
        public static void BanCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            Server.bannedIPs.Add(((IPEndPoint)targetClient.tcp.Client.RemoteEndPoint).Address.ToString(), targetClient.username);
            targetClient.disconnectFlag = true;
            SaveSystem.SaveBannedIPs(Server.bannedIPs);
            ConsoleUtils.LogToConsole("Player {targetClient.username} Has Been Banned", ConsoleUtils.ConsoleLogMode.Info);
            
        }
        public static void PardonCommand(string[] arguments)
        {
            try
            {
                Server.bannedIPs.Remove(Server.bannedIPs.Where(x => x.Value == arguments[0]).Single().Key);
                SaveSystem.SaveBannedIPs(Server.bannedIPs);
                ConsoleUtils.LogToConsole($"Player {arguments[0]} Has Been Unbanned", ConsoleUtils.ConsoleLogMode.Info);
            }
            catch
            {
                ConsoleUtils.LogToConsole($"Player {arguments[0]} Not Found", ConsoleUtils.ConsoleLogMode.Info);
            }
        }
        public static void KickCommand(string[] arguments)
        {
            ServerClient targetClient = Networking.connectedClients.Find(fetch => fetch.username == arguments[0]);
            targetClient.disconnectFlag = true;
            ConsoleUtils.LogToConsole("Player {targetClient.username} Has Been Kicked", ConsoleUtils.ConsoleLogMode.Info);
        }
    }
}