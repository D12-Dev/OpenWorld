using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Open_World_Server
{
    public class SaveSystem
    {
        public static void SaveUserData(ServerClient client)
        {
            string folderPath = OWServer.playersFolderPath;
            string filePath = folderPath + Path.DirectorySeparatorChar + client.username + ".data";

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(filePath, FileMode.Create);

            MainDataHolder data = new MainDataHolder(client);

            formatter.Serialize(stream, data);

            stream.Close();
        }

        public static MainDataHolder LoadUserData(string username)
        {
            string path = OWServer.playersFolderPath + Path.DirectorySeparatorChar + username + ".data";

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            MainDataHolder data = formatter.Deserialize(stream) as MainDataHolder;

            stream.Close();

            return data;
        }

        public static void SaveBannedIPs(Dictionary<string, string> IPs)
        {
            string folderPath = OWServer.mainFolderPath;
            string filepath = folderPath + Path.DirectorySeparatorChar + "Banned IPs.data";

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(filepath, FileMode.Create);

            BanDataHolder data = new BanDataHolder(IPs);

            formatter.Serialize(stream, data);

            stream.Close();
        }

        public static BanDataHolder LoadBannedIPs()
        {
            string path = OWServer.mainFolderPath + Path.DirectorySeparatorChar + "Banned IPs.data";

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            BanDataHolder data = formatter.Deserialize(stream) as BanDataHolder;

            stream.Close();

            return data;
        }
    }
}
