using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorld
{
    public class MPChat
    {
        public string cacheInputText;
        public List<string> cacheChatText = new List<string>() { "Welcome to the chat!", "Please, keep anything you post appropriate and respect other users.", "Type '/help' to see available commands." };
        public string[] profanityFilter = new string[]
        {
            "Piss off",
            "Fuck you",
            "Dick head",
            "Asshole",
            "Son of a bitch",
            "Bastard",
            "Bitch",
            "Bitches",
            "Nigga",
            "Nigger",
            "Cunt",
            "Motherfucker",
            "Shitter",
            "Pussy",
            "Eat shit",
            "Fuck off",
            "Slut",
            "Shut the fuck up",
            "Shut the hell up",
            "Nig nog",
            "Niggi",
            "Twat",
            "Zipperhead",
            "Spic",
            "Tranny",
            "Trany",
            "Troon",
            "Retard",
            "Aborted",
            "Cotton picker",
            "Goat fucker",
            "Coomer",
            "Faggot",
            "Faggie"
        };

        public void SendMessage()
        {
            string message = DateTime.Now.ToString("h:mm tt") + " | " + "[" + Networking.username + "]" + ": " + cacheInputText;

            if (cacheInputText.StartsWith("/"))
            {
                string command = cacheInputText.Remove(0, 1);
                string username = "LOCAL";
                string text = "";

                if (command == "Help" || command == "help")
                {
                    text = "Available Commands:";
                    text += "\n\n- Ping: Checks connection with the server.";
                    text += "\n\n- Msg (WIP): Sends a private msg to X player.";
                    text += "\n\n- Easter: Shows easter eggs.";
                }
                else if (command == "Up Up Down Down Left Right Left Right B A START")
                {
                    text = "Violet was here! Such a good person!";
                }
                else
                {
                    cacheInputText = "";
                    return;
                }

                message = DateTime.Now.ToString("h:mm tt") + " | " + "[" + username + "]" + ": " + text;
                Main._MPChat.cacheChatText.Insert(0, message);
                cacheInputText = "";
                return;
            }

            if (Main._ParametersCache.chatMode != 1)
            {
                Main._MPChat.cacheInputText = "";
                Find.WindowStack.Add(new Dialog_MPDisabledChat());
                return;
            }

            else
            {
                foreach (string str in Main._MPChat.profanityFilter)
                {
                    if (Main._MPChat.cacheInputText.Contains(str) || Main._MPChat.cacheInputText.Contains(str.ToLowerInvariant()) || Main._MPChat.cacheInputText.Contains(str.ToUpperInvariant()))
                    {
                        Main._MPChat.cacheInputText = "";
                        return;
                    }
                }

                Main._MPChat.cacheChatText.Insert(0, message);

                Networking.SendData("ChatMessage│" + Networking.username + "│" + cacheInputText);

                cacheInputText = "";
            }
        }

        public void ReceiveMessage(string data)
        {
            try
            {
                string username = data.Split('│')[1];
                string text = data.Split('│')[2];

                string message = DateTime.Now.ToString("h:mm tt") + " | " + "[" + username + "]" + ": " + text;
                Main._MPChat.cacheChatText.Insert(0, message);
            }

            catch { }
        }
    }
}
