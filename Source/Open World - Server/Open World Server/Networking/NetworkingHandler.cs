using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OpenWorldServer
{
    public static class NetworkingHandler
    {
        public static void ConnectHandle(ServerClient client, string data)
        {
            JoiningsUtils.LoginProcedures(client, data);
        }

        public static void ChatMessageHandle(ServerClient client, string data)
        {
            ServerUtils.SendChatMessage(client, data);
        }

        public static void UserSettlementHandle(ServerClient client, string data)
        {
            if (data.StartsWith("UserSettlement│NewSettlementID│"))
            {
                try
                {
                    client.wealth = float.Parse(data.Split('│')[3]);
                    client.pawnCount = int.Parse(data.Split('│')[4]);

                    PlayerUtils.CheckForPlayerWealth(client);
                }
                catch { }

                WorldUtils.CheckForTileDisponibility(client, data.Split('│')[2]);
            }

            else if (data.StartsWith("UserSettlement│AbandonSettlementID│"))
            {
                if (client.homeTileID != data.Split('│')[2] || string.IsNullOrWhiteSpace(client.homeTileID)) return;
                else WorldUtils.RemoveSettlement(client, data.Split('│')[2]);
            }

            else if (data == "UserSettlement│NoSettlementInLoad")
            {
                if (string.IsNullOrWhiteSpace(client.homeTileID)) return;
                else WorldUtils.RemoveSettlement(client, client.homeTileID);
            }
        }

        public static void ForceEventHandle(ServerClient client, string data)
        {
            string dataToSend = "";

            if (PlayerUtils.CheckForConnectedPlayers(data.Split('│')[2]))
            {
                if (PlayerUtils.CheckForPlayerShield(data.Split('│')[2]))
                {
                    dataToSend = "│SentEvent│Confirm│";

                    PlayerUtils.SendEventToPlayer(client, data);
                }

                else dataToSend = "│SentEvent│Deny│";
            }
            else dataToSend = "│SentEvent│Deny│";

            Networking.SendData(client, dataToSend);
        }

        public static void SendGiftHandle(ServerClient client, string data)
        {
            PlayerUtils.SendGiftToPlayer(client, data);
        }

        public static void SendTradeHandle(ServerClient client, string data)
        {
            string dataToSend = "";

            if (PlayerUtils.CheckForConnectedPlayers(data.Split('│')[1]))
            {
                dataToSend = "│SentTrade│Confirm│";

                PlayerUtils.SendTradeRequestToPlayer(client, data);
            }
            else dataToSend = "│SentTrade│Deny│";

            Networking.SendData(client, dataToSend);
        }

        public static void SendBarterHandle(ServerClient client, string data)
        {
            string dataToSend = "";

            if (PlayerUtils.CheckForConnectedPlayers(data.Split('│')[1]))
            {
                dataToSend = "│SentBarter│Confirm│";

                PlayerUtils.SendBarterRequestToPlayer(client, data);
            }
            else dataToSend = "│SentBarter│Deny│";

            Networking.SendData(client, dataToSend);
        }

        public static void TradeStatusHandle(ServerClient client, string data)
        {
            string username = data.Split('│')[2];
            ServerClient target = null;

            foreach (ServerClient sc in Networking.connectedClients)
            {
                if (sc.username == username)
                {
                    target = sc;
                    break;
                }
            }

            if (target == null) return;

            if (data.StartsWith("TradeStatus│Deal│"))
            {
                Networking.SendData(target, "│SentTrade│Deal│");

                ConsoleUtils.LogToConsole("Trade Done Between [" + target.username + "] And [" + client.username + "]");
            }

            else if (data.StartsWith("TradeStatus│Reject│"))
            {
                Networking.SendData(target, "│SentTrade│Reject│");
            }
        }

        public static void BarterStatusHandle(ServerClient client, string data)
        {
            string user = data.Split('│')[2];
            ServerClient target = null;

            foreach (ServerClient sc in Networking.connectedClients)
            {
                if (sc.username == user)
                {
                    target = sc;
                    break;
                }

                if (sc.homeTileID == user)
                {
                    target = sc;
                    break;
                }
            }

            if (target == null) return;

            if (data.StartsWith("BarterStatus│Deal│"))
            {
                Networking.SendData(target, "│SentBarter│Deal│");

                ConsoleUtils.LogToConsole("Barter Done Between [" + target.username + "] And [" + client.username + "]");
            }

            else if (data.StartsWith("BarterStatus│Reject│"))
            {
                Networking.SendData(target, "│SentBarter│Reject│");
            }

            else if (data.StartsWith("BarterStatus│Rebarter│"))
            {
                Networking.SendData(target, "│SentBarter│Rebarter│" + client.username + "│" + data.Split('│')[3]);
            }
        }

        public static void SpyInfoHandle(ServerClient client, string data)
        {
            string dataToSend = "";

            if (PlayerUtils.CheckForConnectedPlayers(data.Split('│')[1]))
            {
                dataToSend = "│SentSpy│Confirm│" + PlayerUtils.GetSpyData(data.Split('│')[1], client);
            }
            else dataToSend = "│SentSpy│Deny│";

            Networking.SendData(client, dataToSend);
        }

        public static void FactionManagementHandle(ServerClient client, string data)
        {
            if (data == "FactionManagement│Refresh")
            {
                if (client.faction == null) return;
                else Networking.SendData(client, FactionHandler.GetFactionDetails(client));
            }

            else if (data.StartsWith("FactionManagement│Create│"))
            {
                if (client.faction != null) return;

                string factionName = data.Split('│')[2];

                if (string.IsNullOrWhiteSpace(factionName)) return;

                Faction factionToFetch = Server.factionList.Find(fetch => fetch.name == factionName);
                if (factionToFetch == null) FactionHandler.CreateFaction(factionName, client);
                else Networking.SendData(client, "FactionManagement│NameInUse");
            }

            else if (data == "FactionManagement│Disband")
            {
                if (client.faction == null) return;

                if (FactionHandler.GetMemberPowers(client.faction, client) != FactionHandler.MemberRank.Leader)
                {
                    Networking.SendData(client, "FactionManagement│NoPowers");
                    return;
                }

                Faction factionToCheck = client.faction;
                FactionHandler.PurgeFaction(factionToCheck);
            }

            else if (data == "FactionManagement│Leave")
            {
                if (client.faction == null) return;

                FactionHandler.RemoveMember(client.faction, client);
            }

            else if (data.StartsWith("FactionManagement│Join│"))
            {
                string factionString = data.Split('│')[2];

                Faction factionToJoin = Server.factionList.Find(fetch => fetch.name == factionString);

                if (factionToJoin == null) return;
                else FactionHandler.AddMember(factionToJoin, client);
            }

            else if (data.StartsWith("FactionManagement│AddMember"))
            {
                if (client.faction == null) return;

                if (FactionHandler.GetMemberPowers(client.faction, client) == FactionHandler.MemberRank.Member)
                {
                    Networking.SendData(client, "FactionManagement│NoPowers");
                    return;
                }

                string tileID = data.Split('│')[2];
                if (!PlayerUtils.CheckForConnectedPlayers(tileID)) Networking.SendData(client, "PlayerNotConnected│");
                else
                {
                    ServerClient memberToAdd = PlayerUtils.GetPlayerFromTile(tileID);
                    if (memberToAdd.faction != null) Networking.SendData(client, "FactionManagement│AlreadyInFaction");
                    else Networking.SendData(memberToAdd, "FactionManagement│Invite│" + client.faction.name);
                }
            }

            else if (data.StartsWith("FactionManagement│RemoveMember"))
            {
                if (client.faction == null) return;

                if (FactionHandler.GetMemberPowers(client.faction, client) == FactionHandler.MemberRank.Member)
                {
                    Networking.SendData(client, "FactionManagement│NoPowers");
                    return;
                }

                string tileID = data.Split('│')[2];
                if (!PlayerUtils.CheckForConnectedPlayers(tileID))
                {
                    Faction factionToCheck = Server.factionList.Find(fetch => fetch.name == client.faction.name);
                    ServerClient memberToRemove = Server.savedClients.Find(fetch => fetch.homeTileID == tileID);

                    if (memberToRemove.faction == null) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else if (memberToRemove.faction.name != factionToCheck.name) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else FactionHandler.RemoveMember(factionToCheck, memberToRemove);
                }

                else
                {
                    ServerClient memberToRemove = PlayerUtils.GetPlayerFromTile(tileID);

                    if (memberToRemove.faction == null) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else if (memberToRemove.faction != client.faction) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else FactionHandler.RemoveMember(client.faction, memberToRemove);
                }
            }

            else if (data.StartsWith("FactionManagement│PromoteMember"))
            {
                if (client.faction == null) return;

                if (FactionHandler.GetMemberPowers(client.faction, client) != FactionHandler.MemberRank.Leader)
                {
                    Networking.SendData(client, "FactionManagement│NoPowers");
                    return;
                }

                string tileID = data.Split('│')[2];

                if (!PlayerUtils.CheckForConnectedPlayers(tileID))
                {
                    Faction factionToCheck = Server.factionList.Find(fetch => fetch.name == client.faction.name);
                    ServerClient memberToPromote = Server.savedClients.Find(fetch => fetch.homeTileID == tileID);

                    if (memberToPromote.faction == null) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else if (memberToPromote.faction.name != factionToCheck.name) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else FactionHandler.PromoteMember(factionToCheck, memberToPromote);
                }

                else
                {
                    ServerClient memberToPromote = PlayerUtils.GetPlayerFromTile(tileID);

                    if (memberToPromote.faction == null) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else if (memberToPromote.faction != client.faction) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else FactionHandler.PromoteMember(client.faction, memberToPromote);
                }
            }

            else if (data.StartsWith("FactionManagement│DemoteMember"))
            {
                if (client.faction == null) return;

                if (FactionHandler.GetMemberPowers(client.faction, client) != FactionHandler.MemberRank.Leader)
                {
                    Networking.SendData(client, "FactionManagement│NoPowers");
                    return;
                }

                string tileID = data.Split('│')[2];

                if (!PlayerUtils.CheckForConnectedPlayers(tileID))
                {
                    Faction factionToCheck = Server.factionList.Find(fetch => fetch.name == client.faction.name);
                    ServerClient memberToDemote = Server.savedClients.Find(fetch => fetch.homeTileID == tileID);

                    if (memberToDemote.faction == null) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else if (memberToDemote.faction.name != factionToCheck.name) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else FactionHandler.DemoteMember(factionToCheck, memberToDemote);
                }

                else
                {
                    ServerClient memberToDemote = PlayerUtils.GetPlayerFromTile(tileID);

                    if (memberToDemote.faction == null) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else if (memberToDemote.faction != client.faction) Networking.SendData(client, "FactionManagement│NotInFaction");
                    else FactionHandler.DemoteMember(client.faction, memberToDemote);
                }
            }
        }
    }
}