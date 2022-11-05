using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    public static partial class Server
    {
        public static List<Command> ServerCommands = new List<Command>()
        {
            new Command()
            {
                Word = "help",
                Description = "Display all available commands.",
                SimpleCommand = SimpleCommands.HelpCommand
            },
            new Command()
            {
                Word = "settings",
                Description = "Displays the current server settings.",
                SimpleCommand = SimpleCommands.SettingsCommand
            },
            new Command()
            {
                Word = "modlist",
                Description = "Displays the mods currently enforced, whitelisted and banned on the server.",
                SimpleCommand = SimpleCommands.ModListCommand
            },
            new Command()
            {
                Word = "reload",
                Description = "Reloads all server settings.",
                SimpleCommand = SimpleCommands.ReloadCommand
            },
            new Command()
            {
                Word = "status",
                Description = "Shows an overview of the server status.",
                SimpleCommand = SimpleCommands.StatusCommand
            },
            new Command()
            {
                Word = "eventlist",
                Description = "Displays a list of events to be used with 'invoke' and 'plague'.",
                SimpleCommand = SimpleCommands.EventListCommand
            },
            new Command()
            {
                Word = "chat",
                Description = "Recalls the cache of chat messages from the server.",
                SimpleCommand = SimpleCommands.ChatCommand
            },
            new Command()
            {
                Word = "list",
                Description = "Displays a list of players.",
                SimpleCommand = SimpleCommands.ListCommand
            },
            new Command()
            {
                Word = "settlements",
                Description = "Displays settlement information.",
                SimpleCommand = SimpleCommands.SettlementsCommand
            },
            new Command()
            {
                Word = "banlist",
                Description = "Lists all banned players and their IPs.",
                SimpleCommand = SimpleCommands.BanListCommand
            },
            new Command()
            {
                Word = "adminlist",
                Description = "Lists all admins on the server.",
                SimpleCommand = SimpleCommands.AdminListCommand
            },
            new Command()
            {
                Word = "whitelist",
                Description = "Lists all whitelisted players on the server.",
                SimpleCommand = SimpleCommands.WhiteListCommand
            },
            new Command()
            {
                Word = "wipe",
                Description = "Delete all player data from the server, permanently.",
                SimpleCommand = SimpleCommands.WipeCommand
            },
            new Command()
            {
                Word = "clear",
                Description = "Clears this server console window.",
                SimpleCommand = SimpleCommands.ClearCommand
            },
            new Command()
            {
                Word = "exit",
                Description = "Closes the server.",
                SimpleCommand = SimpleCommands.ExitCommand
            },
            new Command()
            {
                Word = "say",
                Description = "Sends a chat message to the server.",
                AdvancedCommand = AdvancedCommands.SayCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "broadcast",
                Description = "Sends a notification to all connected players.",
                AdvancedCommand = AdvancedCommands.BroadcastCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "notify",
                Description = "Sends a notification to a specific player.",
                AdvancedCommand = AdvancedCommands.NotifyCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "invoke",
                Description = "Sends an event to a specific player (see 'eventlist').",
                AdvancedCommand = AdvancedCommands.InvokeCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "plague",
                Description = "Sends an event to all connected players (see 'eventlist').",
                AdvancedCommand = AdvancedCommands.PlagueCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "player",
                Description = "Displays all data about a specific player.",
                AdvancedCommand = AdvancedCommands.PlayerDetailsCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "faction",
                Description = "Displays information about a specific faction.",
                AdvancedCommand = AdvancedCommands.FactionDetailsCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "kick",
                Description = "Kicks a specific player from the server.",
                AdvancedCommand = AdvancedCommands.KickCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "ban",
                Description = "Bans a specific player from the server (by IP address).",
                AdvancedCommand = AdvancedCommands.BanCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "pardon",
                Description = "Unbans a specific player from the server.",
                AdvancedCommand = AdvancedCommands.PardonCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "promote",
                Description = "Promotes a specific player to an administrator.",
                AdvancedCommand = AdvancedCommands.PromoteCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "demote",
                Description = "Revokes administrator permissions from a specific player.",
                AdvancedCommand = AdvancedCommands.DemoteCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "giveitem",
                Description = "Gives a specific item to a specific player.",
                AdvancedCommand = AdvancedCommands.GiveItemCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "giveitemall",
                Description = "Gives a specific item to all connected players.",
                AdvancedCommand = AdvancedCommands.GiveItemAllCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "protect",
                Description = "Protects a specific player from events until 'deprotect'ed.",
                AdvancedCommand = AdvancedCommands.ProtectCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "deprotect",
                Description = "De'protect's a specific player from events.",
                AdvancedCommand = AdvancedCommands.DeprotectCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "immunize",
                Description = "Temporarily protects a specific player from events.",
                AdvancedCommand = AdvancedCommands.ImmunizeCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "deimmunize",
                Description = "Demoves the temporary immunity granted by 'immunize'.",
                AdvancedCommand = AdvancedCommands.DeimmunizeCommand,
                Parameters = new Dictionary<string, string>()
            }
        };
    }
}
