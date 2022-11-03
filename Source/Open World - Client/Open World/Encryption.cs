using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenWorld
{
    public static class Encryption
    {
        public static string EncryptString(string stuff)
        {
            if (string.IsNullOrWhiteSpace(stuff)) return null;

            string encryptedStuff = "";

            foreach (char ch in stuff)
            {
                double unicode = Convert.ToInt16(ch);
                encryptedStuff = encryptedStuff + unicode + ":";
            }

            encryptedStuff = encryptedStuff.Remove(encryptedStuff.Length - 1);

            return encryptedStuff;
        }

        public static string DecryptString(string stuff)
        {
            if (string.IsNullOrWhiteSpace(stuff)) return null;

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
