using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;
using Verse.Profile;
using System.IO;

namespace OpenWorld
{
    public class Dialog_MPParameters : Window
    {
        public static Dialog_MPParameters __instance;

        private string windowTitle = "Multiplayer Menu";
        private string ipEntryText = "Server IP";
        private string portEntryText = "Server Port";
        private string usernameEntryText = "Username";
        private string passwordEntryText = "Password";
        private string passwordHideText = "";
        private string passwordConfirmEntryText = "Confirm Password";
        private string passwordConfirmHideText = "";

        public string passwordText;
        public string passwordConfirmText;

        private int startAcceptingInputAtFrame;

        private float listButtonX = 30f;
        private float listButtonY = 30f;

        private float buttonX = 150f;
        private float buttonY = 38f;

        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;
        private bool hidingPassword = true;

        public override Vector2 InitialSize => new Vector2(400f, 543f);

        public Dialog_MPParameters()
        {
            try
            {
                MainDataHolder data = SaveSystem.LoadData();
                {
                    Main._ParametersCache.ipText = data.ipText;
                    Main._ParametersCache.portText = data.portText;
                    Main._ParametersCache.usernameText = data.usernameText;
                }
            }

            catch { }

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float ipEntryTextDif = Text.CalcSize(ipEntryText).y + StandardMargin;
            float ipEntryDif = ipEntryTextDif + 30f;

            float portEntryTextDif = ipEntryDif + Text.CalcSize(portEntryText).y + StandardMargin * 2;
            float portEntryDif = portEntryTextDif + 30f;

            float usernameEntryTextDif = portEntryDif + Text.CalcSize(usernameEntryText).y + StandardMargin * 2;
            float userEntryDif = usernameEntryTextDif + 30f;

            float passwordEntryTextDif = userEntryDif + Text.CalcSize(passwordEntryText).y + StandardMargin * 2;
            float passwordEntryDif = passwordEntryTextDif + 30f;

            float passwordConfirmEntryTextDif = passwordEntryDif + Text.CalcSize(passwordConfirmEntryText).y + StandardMargin * 2;
            float passwordConfirmEntryDif = passwordConfirmEntryTextDif + 30f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Text.Font = GameFont.Small;

            if (Widgets.ButtonText(new Rect(rect.width - listButtonX, rect.y, listButtonX, listButtonY), "L"))
            {
                Find.WindowStack.Add(new Dialog_MPServerList());
            }

            Widgets.Label(new Rect(centeredX - Text.CalcSize(ipEntryText).x / 2, ipEntryTextDif, Text.CalcSize(ipEntryText).x, Text.CalcSize(ipEntryText).y), ipEntryText);
            string a = Widgets.TextField(new Rect(centeredX - (200f / 2), ipEntryDif, 200f, 30f), Main._ParametersCache.ipText);
            if (AcceptsInput && a.Length <= 32) Main._ParametersCache.ipText = a;

            Widgets.Label(new Rect(centeredX - Text.CalcSize(portEntryText).x / 2, portEntryTextDif, Text.CalcSize(portEntryText).x, Text.CalcSize(portEntryText).y), portEntryText);
            string b = Widgets.TextField(new Rect(centeredX - (200f / 2), portEntryDif, 200f, 30f), Main._ParametersCache.portText);
            if (AcceptsInput && b.Length <= 5 && b.All(character => Char.IsDigit(character))) Main._ParametersCache.portText = b;

            Widgets.Label(new Rect(centeredX - Text.CalcSize(usernameEntryText).x / 2, usernameEntryTextDif, Text.CalcSize(usernameEntryText).x, Text.CalcSize(usernameEntryText).y), usernameEntryText);
            string c = Widgets.TextField(new Rect(centeredX - (200f / 2), userEntryDif, 200f, 30f), Main._ParametersCache.usernameText);
            if (AcceptsInput && c.Length <= 24 && c.All(character => Char.IsLetterOrDigit(character) || character == '_' || character == '-')) Main._ParametersCache.usernameText = c;

            Widgets.Label(new Rect(centeredX - Text.CalcSize(passwordEntryText).x / 2, passwordEntryTextDif, Text.CalcSize(passwordEntryText).x, Text.CalcSize(passwordEntryText).y), passwordEntryText);
            string d = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordEntryDif, 200f, 30f), passwordText);
            if (AcceptsInput && d.Length <= 24 && !d.Contains('│') && !d.Contains('»') && !d.Contains('┼'))
            {
                Text.Font = GameFont.Medium;
                if (hidingPassword) passwordHideText = new string('█', d.Length);
                else passwordHideText = "";
                Text.Font = GameFont.Small;

                passwordText = d;
            }
            string e = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordEntryDif, 200f, 30f), passwordHideText);

            Widgets.Label(new Rect(centeredX - Text.CalcSize(passwordConfirmEntryText).x / 2, passwordConfirmEntryTextDif, Text.CalcSize(passwordConfirmEntryText).x, Text.CalcSize(passwordConfirmEntryText).y), passwordConfirmEntryText);
            string f = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordConfirmEntryDif, 200f, 30f), passwordConfirmText);
            if (AcceptsInput && f.Length <= 24 && !f.Contains('│') && !f.Contains('»') && !f.Contains('┼'))
            {
                Text.Font = GameFont.Medium;
                if (hidingPassword) passwordConfirmHideText = new string('█', f.Length);
                else passwordConfirmHideText = "";
                Text.Font = GameFont.Small;

                passwordConfirmText = f;
            }
            string g = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordConfirmEntryDif, 200f, 30f), passwordConfirmHideText);

            if (Widgets.ButtonText(new Rect(rect.width - listButtonX, passwordConfirmEntryDif, listButtonX, listButtonY), "S"))
            {
                if (hidingPassword) hidingPassword = false;
                else hidingPassword = true;
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Join"))
            {
                if (Networking.isTryingToConnect) return;

                bool isMissingInfo = false;

                if (string.IsNullOrWhiteSpace(Main._ParametersCache.ipText)) isMissingInfo = true;
                if (string.IsNullOrWhiteSpace(Main._ParametersCache.portText)) isMissingInfo = true;
                if (string.IsNullOrWhiteSpace(Main._ParametersCache.usernameText)) isMissingInfo = true;
                if (string.IsNullOrWhiteSpace(passwordText)) isMissingInfo = true;
                if (string.IsNullOrWhiteSpace(passwordConfirmText)) isMissingInfo = true;
                if (passwordText != passwordConfirmText) isMissingInfo = true;

                if (isMissingInfo)
                {
                    Find.WindowStack.Add(new OW_ErrorDialog("Login details are missing or incorrect"));
                    return;
                }

                else
                {
                    Networking.ip = Main._ParametersCache.ipText;
                    Networking.port = Main._ParametersCache.portText;
                    Networking.username = Main._ParametersCache.usernameText;
                    Networking.password = passwordText;

                    SaveSystem.SaveData(Main._ParametersCache);

                    Threading.GenerateThreads(0);

                    __instance = this;
                }
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                this.Close();
            }
        }
    }
}