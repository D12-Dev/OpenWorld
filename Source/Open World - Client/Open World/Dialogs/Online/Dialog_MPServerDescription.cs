using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorld
{
    public class Dialog_MPServerDescription : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 543f);

        private string windowTitle = "Server Information";
        private string name;
        private string description;
        private string ip;
        private string port;
        private string discord = "Discord";
        private string modList = "Mod List";
        private string whitelistedModList = "Whitelisted Mods List";

        private string discordURL;
        private string modListURL;
        private string whitelistedModListURL;

        private float buttonX = 150f;
        private float buttonY = 38f;

        private List<string> serverDetails;

        private Dialog_MPServerList MPServerList;

        public Dialog_MPServerDescription(List<string> details, Dialog_MPServerList MPServerList)
        {
            serverDetails = details;
            this.MPServerList = MPServerList;

            name = serverDetails[1];
            name = name.Remove(name.Count() - 1, 1);

            description = serverDetails[2].Replace("Description: ", "");
            description = description.Remove(0, 1);
            description = description.Remove(description.Count() - 1, 1);

            ip = serverDetails[3];
            ip = ip.Remove(ip.Count() - 1, 1);

            port = serverDetails[4];
            port = port.Remove(port.Count() - 1, 1);

            discordURL = serverDetails[5].Replace("Discord: ", "");
            discordURL = discordURL.Remove(0, 1);

            modListURL = serverDetails[6].Replace("Modlist: ", "");
            modListURL = modListURL.Remove(0, 1);

            whitelistedModListURL = serverDetails[7].Replace("Whitelisted Modlist: ", "");
            whitelistedModListURL = whitelistedModListURL.Remove(0, 1);

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float horizontalLineDif = Text.CalcSize(windowTitle).y + (StandardMargin / 4);
            float nameDif = Text.CalcSize(windowTitle).y + (StandardMargin / 2);
            float ipDif = nameDif + (StandardMargin * 2);
            float portDif = ipDif + (StandardMargin * 2);
            float discordDif = portDif + (StandardMargin * 2);
            float modsDif = discordDif + (StandardMargin * 2) + buttonY;
            float whitelistedModsDif = modsDif + (StandardMargin * 2) + buttonY;
            float descriptionDif = whitelistedModsDif + (StandardMargin * 2) + buttonY + 6f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Widgets.Label(new Rect(centeredX - Text.CalcSize(name).x / 2, nameDif, Text.CalcSize(name).x, Text.CalcSize(name).y), name);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(ip).x / 2, ipDif, Text.CalcSize(ip).x, Text.CalcSize(ip).y), ip);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(port).x / 2, portDif, Text.CalcSize(port).x, Text.CalcSize(port).y), port);

            Widgets.Label(new Rect(centeredX - Text.CalcSize(discord).x / 2, discordDif, Text.CalcSize(discord).x, Text.CalcSize(discord).y), discord);
            if (Widgets.ButtonText(new Rect(rect.x, discordDif + buttonY, rect.width, Text.CalcSize(discord).y), "Check"))
            {
                if (discordURL.StartsWith("Null") || string.IsNullOrWhiteSpace(discordURL)) Find.WindowStack.Add(new OW_ErrorDialog("The requested URL is unavailable"));
                else try { System.Diagnostics.Process.Start(discordURL); } catch { }
            }

            Widgets.Label(new Rect(centeredX - Text.CalcSize(modList).x / 2, modsDif, Text.CalcSize(modList).x, Text.CalcSize(modList).y), modList);
            if (Widgets.ButtonText(new Rect(rect.x, modsDif + buttonY, rect.width, Text.CalcSize(modList).y), "Check"))
            {
                if (modListURL.StartsWith("Null") || string.IsNullOrWhiteSpace(modListURL)) Find.WindowStack.Add(new OW_ErrorDialog("The requested URL is unavailable"));
                else try { System.Diagnostics.Process.Start(modListURL); } catch { }
            }

            Widgets.Label(new Rect(centeredX - Text.CalcSize(whitelistedModList).x / 2, whitelistedModsDif, Text.CalcSize(whitelistedModList).x, Text.CalcSize(whitelistedModList).y), whitelistedModList);
            if (Widgets.ButtonText(new Rect(rect.x, whitelistedModsDif + buttonY, rect.width, Text.CalcSize(whitelistedModList).y), "Check"))
            {
                if (whitelistedModListURL.StartsWith("Null") || string.IsNullOrWhiteSpace(whitelistedModListURL)) Find.WindowStack.Add(new OW_ErrorDialog("The requested URL is unavailable"));
                else try { System.Diagnostics.Process.Start(whitelistedModListURL); } catch { }
            }

            Text.Font = GameFont.Small;
            Widgets.TextArea(new Rect(rect.x, descriptionDif, rect.width, 78f), description);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Connect"))
            {
                string ipToJoin = ip.Replace("IP: ", "");
                ipToJoin = ipToJoin.Remove(0, 1);

                string portToJoin = port.Replace("Port: ", "");
                portToJoin = portToJoin.Remove(0, 1);

                Main._ParametersCache.ipText = ipToJoin;
                Main._ParametersCache.portText = portToJoin;
                MPServerList.Close();
                Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Return"))
            {
                Close();
            }
        }
    }
}
