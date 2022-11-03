using System.Net;
using UnityEngine;
using Verse;

namespace OpenWorld
{
    public class ModStuff : Mod
    {
        ModConfigs settings;

        public ModStuff(ModContentPack content) : base(content)
        {
            settings = GetSettings<ModConfigs>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("Multiplayer Parameters");
            listingStandard.CheckboxLabeled("Auto Deny Trades", ref settings.tradeBool, "Deny trades automatically when someone attempts to");
            listingStandard.CheckboxLabeled("Auto Deny Visits", ref settings.visitBool, "Deny visits automatically when someone attempts to");
            listingStandard.CheckboxLabeled("Online Raids", ref settings.onlineRaidBool, "Participate in online raiding");
            listingStandard.CheckboxLabeled("Offline Raids", ref settings.offlineRaidBool, "Participate in offline raiding");
            listingStandard.CheckboxLabeled("Spy Warning", ref settings.spyWarningBool, "Ignore spy warnings when being spied on");
            listingStandard.CheckboxLabeled("Very Secret Stuff", ref settings.secretBool, "Secret");

            listingStandard.GapLine();
            listingStandard.Label("External Sources");
            if (listingStandard.ButtonTextLabeled("Check Latest Mod Changelogs", "Open"))
            {
                try { System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/changelog/2768146099"); } catch { }
            }
            if (listingStandard.ButtonTextLabeled("Join Open World's Discord Community", "Open"))
            {
                try { System.Diagnostics.Process.Start("https://discord.gg/SNtVjR6bqU"); } catch { }
            }
            if (listingStandard.ButtonTextLabeled("Access Open World's GitHub Repository", "Open"))
            {
                try { System.Diagnostics.Process.Start("https://github.com/TastyLollipop/OpenWorld"); } catch { }
            }
            if (listingStandard.ButtonTextLabeled("Get To Tasty Lollipop's Patreon", "Open"))
            {
                try { System.Diagnostics.Process.Start("https://www.patreon.com/user?u=81586827"); } catch { }
            }

            listingStandard.GapLine();
            listingStandard.Label("Mod Details");
            listingStandard.Label("Running Version: " + Main._ParametersCache.versionCode);

            WebClient wc = new WebClient();
            string latestVersion = wc.DownloadString("https://raw.githubusercontent.com/TastyLollipop/OpenWorld/main/Latest%20Versions%20Cache");
            latestVersion = latestVersion.Split('│')[2].Replace("- Latest Client Version: ", "");
            latestVersion = latestVersion.Remove(0, 1);
            listingStandard.Label("Latest Version Available: " + latestVersion);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() { return "Open World"; }
    }
}
