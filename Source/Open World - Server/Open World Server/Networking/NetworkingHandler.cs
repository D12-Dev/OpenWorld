using System;
using System.Collections.Generic;
using System.Text;

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
            string dataToSend = "";

            if (data == "FactionManagement│Refresh")
            {
                dataToSend = "FactionManagement│Refresh";

                Networking.SendData(client, dataToSend);
            }

            else if (data == "FactionManagement│Create")
            {

            }

            else if (data == "FactionManagement│Disband")
            {

            }

            else if (data == "FactionManagement│AddMember")
            {

            }

            else if (data == "FactionManagement│RemoveMember")
            {

            }

            else if (data == "FactionManagement│ChangeMemberRank")
            {

            }
        }
    }
}
