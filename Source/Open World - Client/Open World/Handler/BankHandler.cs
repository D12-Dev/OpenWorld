using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorld
{
    public static class BankHandler
    {
        public static void ResetBankVariables()
        {
            Main._ParametersCache.depositSilverInt = 0;

            Main._ParametersCache.listToShowInBankMenu.Clear();
        }

        public static void SendSilverToBank()
        {
            if (Main._ParametersCache.depositSilverInt == 0) return;

            string depositedString = "FactionManagement│Bank│Deposit" + "│" + Main._ParametersCache.depositSilverInt;

            Networking.SendData(depositedString);

            ResetBankVariables();
        }
    }
}
