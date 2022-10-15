using System;
using System.Collections.Generic;
using System.Threading;

namespace Open_World_Server
{
    public class WorldUtils
    {
        public void AddSettlement(ServerClient? client, string data)
        {
            string[] dataSplit = data.Split(' ');

            if (client != null)
            {
                client.homeTileID = dataSplit[0];

                foreach(ServerClient sc in OWServer.savedClients)
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

            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                if (client != null)
                {
                    if (sc.username == client.username) continue;
                }

                OWServer._Networking.SendData(sc, dataString);
            }

            OWServer.savedSettlements.Add(client.homeTileID, new List<string> { client.username });

            ServerUtils.WriteServerLog("Settlement With ID [" + dataSplit[0] + "] And Owner [" + dataSplit[1] + "] Has Been Added");
        }

        public void RemoveSettlement(ServerClient? client, string tile)
        {
            if (client != null)
            {
                client.homeTileID = null;

                foreach (ServerClient sc in OWServer.savedClients)
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

                foreach (ServerClient sc in OWServer._Networking.connectedClients)
                {
                    if (client != null)
                    {
                        if (sc.username == client.username) continue;
                    }

                    OWServer._Networking.SendData(sc, dataString);
                }

                OWServer.savedSettlements.Remove(tile);

                ServerUtils.WriteServerLog("Settlement With ID [" + tile + "] Has Been Deleted");
            }
        }

        public void CheckForTileDisponibility(ServerClient client, string tileID)
        {
            foreach (ServerClient savedClient in OWServer.savedClients)
            {
                if (savedClient.username == client.username)
                {
                    if (savedClient.homeTileID == tileID) return;

                    else
                    {
                        foreach (KeyValuePair<string, List<string>> pair in OWServer.savedSettlements)
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
                        OWServer._Networking.SendData(client, "Disconnect│Corrupted");

                        Console.ForegroundColor = ConsoleColor.Red;
                        ServerUtils.WriteServerLog("Player [" + client.username + "] Tried To Claim Used Tile! [" + tileID + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            AddSettlement(client, tileID + " " + client.username);
        }
    }
}
