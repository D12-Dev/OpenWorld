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
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.HelpCommand
            },
            new Command()
            {
                Word = "settings",
                Description = "Displays the current server settings.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.SettingsCommand
            },
            new Command()
            {
                Word = "modlist",
                Description = "Displays the mods currently enforced, whitelisted and banned on the server.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.ModListCommand
            },
            new Command()
            {
                Word = "reload",
                Description = "Reloads all server settings.",
                Category = Command.CommandCategory.Server_Administration,
                SimpleCommand = SimpleCommands.ReloadCommand
            },
            new Command()
            {
                Word = "status",
                Description = "Shows an overview of the server status.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.StatusCommand
            },
            new Command()
            {
                Word = "eventlist",
                Description = "Displays a list of events to be used with 'invoke' and 'plague'.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.EventListCommand
            },
            new Command()
            {
                Word = "chat",
                Description = "Recalls the cache of chat messages from the server.",
                Category = Command.CommandCategory.Player_Interaction,
                SimpleCommand = SimpleCommands.ChatCommand
            },
            new Command()
            {
                Word = "list",
                Description = "Displays a list of players.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.ListCommand
            },
            new Command()
            {
                Word = "settlements",
                Description = "Displays settlement information.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.SettlementsCommand
            },
            new Command()
            {
                Word = "banlist",
                Description = "Lists all banned players and their IPs.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.BanListCommand
            },
            new Command()
            {
                Word = "adminlist",
                Description = "Lists all admins on the server.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.AdminListCommand
            },
            new Command()
            {
                Word = "whitelist",
                Description = "Lists all whitelisted players on the server.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.WhiteListCommand
            },
            new Command()
            {
                Word = "wipe",
                Description = "Delete all player data from the server, permanently.",
                Category = Command.CommandCategory.Server_Administration,
                SimpleCommand = SimpleCommands.WipeCommand
            },
            new Command()
            {
                Word = "clear",
                Description = "Clears this server console window.",
                Category = Command.CommandCategory.Information,
                SimpleCommand = SimpleCommands.ClearCommand
            },
            new Command()
            {
                Word = "exit",
                Description = "Closes the server.",
                Category = Command.CommandCategory.Server_Administration,
                SimpleCommand = SimpleCommands.ExitCommand
            },
            new Command()
            {
                Word = "say",
                Description = "Sends a chat message to the server.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.SayCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "broadcast",
                Description = "Sends a notification to all connected players.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.BroadcastCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "notify",
                Description = "Sends a notification to a specific player.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.NotifyCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "invoke",
                Description = "Sends an event to a specific player (see 'eventlist').",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.InvokeCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "plague",
                Description = "Sends an event to all connected players (see 'eventlist').",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.PlagueCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "player",
                Description = "Displays all data about a specific player.",
                Category = Command.CommandCategory.Information,
                AdvancedCommand = AdvancedCommands.PlayerDetailsCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "faction",
                Description = "Displays information about a specific faction.",
                Category = Command.CommandCategory.Information,
                AdvancedCommand = AdvancedCommands.FactionDetailsCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "kick",
                Description = "Kicks a specific player from the server.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.KickCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "ban",
                Description = "Bans a specific player from the server (by IP address).",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.BanCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "pardon",
                Description = "Unbans a specific player from the server.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.PardonCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "promote",
                Description = "Promotes a specific player to an administrator.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.PromoteCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "demote",
                Description = "Revokes administrator permissions from a specific player.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.DemoteCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "giveitem",
                Description = "Gives a specific item to a specific player.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.GiveItemCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "giveitemall",
                Description = "Gives a specific item to all connected players.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.GiveItemAllCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "protect",
                Description = "Protects a specific player from events until 'deprotect'ed.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.ProtectCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "deprotect",
                Description = "De'protect's a specific player from events.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.DeprotectCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "immunize",
                Description = "Temporarily protects a specific player from events.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.ImmunizeCommand,
                Parameters = new Dictionary<string, string>()
            },
            new Command()
            {
                Word = "deimmunize",
                Description = "Demoves the temporary immunity granted by 'immunize'.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.DeimmunizeCommand,
                Parameters = new Dictionary<string, string>()
            }
        };
    }
}
