using System;
using System.Collections.Generic;
using System.Linq;

namespace Open_World_Server
{
    public class Encryption
    {
        public string EncryptString(string stuff)
        {
            string encryptedStuff = "";

            foreach (char ch in stuff)
            {
                double unicode = Convert.ToInt16(ch);
                encryptedStuff = encryptedStuff + unicode + ":";
            }

            encryptedStuff = encryptedStuff.Remove(encryptedStuff.Length - 1);

            return encryptedStuff;
        }

        public string DecryptString(string stuff)
        {
            List<string> encryptedList = stuff.Split(':').ToList();

            string decryptedStuff = "";

            foreach (string encrypted in encryptedList)
            {
                int convertedCh = int.Parse(encrypted);

                decryptedStuff = decryptedStuff + (char)convertedCh;
            }

            return decryptedStuff;
        }
    }
}
