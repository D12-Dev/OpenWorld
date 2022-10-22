using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenWorldServer
{
    public static class WorldUtils
    {
        public static void AddSettlement(ServerClient? client, string data)
        {
            string[] dataSplit = data.Split(' ');

            if (client != null)
            {
                client.homeTileID = dataSplit[0];

                foreach(ServerClient sc in Server.savedClients)
                {
                    if (sc.username == client.username)
                    {
                        sc.homeTileID = client.homeTileID;
                        break;
                    }
                }

                SaveSystem.SaveUserData(client);
            }

            string dataString = "AddSettlement│" + dataSplit[0] + "│" + dataSplit[1];

            foreach (ServerClient sc in Networking.connectedClients)
            {
                if (client != null)
                {
                    if (sc.username == client.username) continue;
                }

                Networking.SendData(sc, dataString);
            }

            Server.savedSettlements.Add(client.homeTileID, new List<string> { client.username });

            ConsoleUtils.LogToConsole("Settlement With ID [" + dataSplit[0] + "] And Owner [" + dataSplit[1] + "] Has Been Added");
        }

        public static void RemoveSettlement(ServerClient? client, string tile)
        {
            if (client != null)
            {
                client.homeTileID = null;

                foreach (ServerClient sc in Server.savedClients)
                {
                    if (sc.username == client.username)
                    {
                        sc.homeTileID = null;
                        break;
                    }
                }

                SaveSystem.SaveUserData(client);
            }

            if (!string.IsNullOrWhiteSpace(tile))
            {
                string dataString = "RemoveSettlement│" + tile;

                foreach (ServerClient sc in Networking.connectedClients)
                {
                    if (client != null)
                    {
                        if (sc.username == client.username) continue;
                    }

                    Networking.SendData(sc, dataString);
                }

                Server.savedSettlements.Remove(tile);

                ConsoleUtils.LogToConsole("Settlement With ID [" + tile + "] Has Been Deleted");
            }
        }

        public static void CheckForTileDisponibility(ServerClient client, string tileID)
        {
            foreach (ServerClient savedClient in Server.savedClients)
            {
                if (savedClient.username == client.username)
                {
                    if (savedClient.homeTileID == tileID) return;

                    else
                    {
                        foreach (KeyValuePair<string, List<string>> pair in Server.savedSettlements)
                        {
                            if (pair.Value[0] == client.username)
                            {
                                RemoveSettlement(client, pair.Key);
                                Thread.Sleep(500);
                                break;
                            }
                        }

                        break;
                    }
                }

                else
                {
                    if (savedClient.homeTileID == tileID)
                    {
                        Networking.SendData(client, "Disconnect│Corrupted");

                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleUtils.LogToConsole("Player [" + client.username + "] Tried To Claim Used Tile! [" + tileID + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            AddSettlement(client, tileID + " " + client.username);
        }
    }
}
