using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace OpenWorld
{
    public class SaveSystem
    {
        public static void SaveData(ParametersCache parameterData)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + Path.DirectorySeparatorChar + "Ludeon Studios" + Path.DirectorySeparatorChar + "RimWorld by Ludeon Studios" + Path.DirectorySeparatorChar + "Open World" + Path.DirectorySeparatorChar + "Data";
            string filePath = folderPath + Path.DirectorySeparatorChar + "Login Data.data";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(filePath, FileMode.Create);

            MainDataHolder data = new MainDataHolder(parameterData);

            formatter.Serialize(stream, data);

            stream.Close();
        }

        public static MainDataHolder LoadData()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + Path.DirectorySeparatorChar + "Ludeon Studios" + Path.DirectorySeparatorChar + "RimWorld by Ludeon Studios" + Path.DirectorySeparatorChar + "Open World" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "Login Data.data";

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            MainDataHolder data = formatter.Deserialize(stream) as MainDataHolder;

            stream.Close();

            return data;
        }
    }
}
