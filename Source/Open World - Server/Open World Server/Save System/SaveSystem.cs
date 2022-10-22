using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace OpenWorldServer
{
    public class SaveSystem
    {
        public static void SaveUserData(ServerClient client)
        {
            string folderPath = Server.playersFolderPath;
            string filePath = folderPath + Path.DirectorySeparatorChar + client.username + ".data";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(filePath, FileMode.Create);

            MainDataHolder data = new MainDataHolder(client);

            formatter.Serialize(stream, data);

            stream.Close();
        }

        public static MainDataHolder LoadUserData(string username)
        {
            string path = Server.playersFolderPath + Path.DirectorySeparatorChar + username + ".data";

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            MainDataHolder data = formatter.Deserialize(stream) as MainDataHolder;

            stream.Close();

            return data;
        }

        public static void SaveBannedIPs(Dictionary<string, string> IPs)
        {
            string folderPath = Server.mainFolderPath;
            string filepath = folderPath + Path.DirectorySeparatorChar + "Banned IPs.data";

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(filepath, FileMode.Create);

            BanDataHolder data = new BanDataHolder(IPs);

            formatter.Serialize(stream, data);

            stream.Close();
        }

        public static BanDataHolder LoadBannedIPs()
        {
            string path = Server.mainFolderPath + Path.DirectorySeparatorChar + "Banned IPs.data";

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            BanDataHolder data = formatter.Deserialize(stream) as BanDataHolder;

            stream.Close();

            return data;
        }
    }
}
