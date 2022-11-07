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
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Message",
                        Description = "The message you would like to send in chat.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    }
                }
            },
            new Command()
            {
                Word = "broadcast",
                Description = "Sends a notification to all connected players.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.BroadcastCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Message",
                        Description = "The message you would like to broadcast to all players.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    }
                }
            },
            new Command()
            {
                Word = "notify",
                Description = "Sends a notification to a specific player.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.NotifyCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player to whom you would like to send the notification.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        { 
                            ParameterValidation.Rule.PlayerOnline
                        }
                    },
                    new Parameter()
                    {
                        Name = "Message",
                        Description = "The message you would like to send in chat.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    }
                }
            },
            new Command()
            {
                Word = "invoke",
                Description = "Sends an event to a specific player (see 'eventlist').",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.InvokeCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player to whom you would like to send the notification.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    },
                    new Parameter()
                    {
                        Name = "Event",
                        Description = "The event you would like to send to the player.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.ValidEvent
                        }
                    }
                }
            },
            new Command()
            {
                Word = "plague",
                Description = "Sends an event to all connected players (see 'eventlist').",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.PlagueCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Event",
                        Description = "The event you would like to send to the player.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.ValidEvent
                        }
                    }
                }

            },
            new Command()
            {
                Word = "player",
                Description = "Displays all data about a specific player.",
                Category = Command.CommandCategory.Information,
                AdvancedCommand = AdvancedCommands.PlayerDetailsCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player of whom you would like to see information.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    } 
                }
            },
            new Command()
            {
                Word = "faction",
                Description = "Displays information about a specific faction.",
                Category = Command.CommandCategory.Information,
                AdvancedCommand = AdvancedCommands.FactionDetailsCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Faction",
                        Description = "The faction of which you would like to see information.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    }
                }
            },
            new Command()
            {
                Word = "kick",
                Description = "Kicks a specific player from the server.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.KickCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to kick.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            },
            new Command()
            {
                Word = "ban",
                Description = "Bans a specific player from the server (by IP address).",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.BanCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to ban.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            },
            new Command()
            {
                Word = "pardon",
                Description = "Unbans a specific player from the server.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.PardonCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to pardon.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    }
                }
            },
            new Command()
            {
                Word = "promote",
                Description = "Promotes a specific player to an administrator.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.PromoteCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to promote.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            },
            new Command()
            {
                Word = "demote",
                Description = "Revokes administrator permissions from a specific player.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.DemoteCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to demote.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            },
            new Command()
            {
                Word = "giveitem",
                Description = "Gives a specific item to a specific player.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.GiveItemCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to give the item to.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    },
                    new Parameter()
                    {
                        Name = "Item",
                        Description = "The item which you would like to give to the player.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    },
                    new Parameter()
                    {
                        Name = "Quantity",
                        Description = "The quantity of the item which you would like to give to the player.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    },
                    new Parameter()
                    {
                        Name = "Quality",
                        Description = "The quality of the item which you would like to give to the player.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    }
                }
            },
            new Command()
            {
                Word = "giveitemall",
                Description = "Gives a specific item to all connected players.",
                Category = Command.CommandCategory.Player_Interaction,
                AdvancedCommand = AdvancedCommands.GiveItemAllCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Item",
                        Description = "The item which you would like to give to all players.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    },
                    new Parameter()
                    {
                        Name = "Quantity",
                        Description = "The quantity of the item which you would like to give to all players.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    },
                    new Parameter()
                    {
                        Name = "Quality",
                        Description = "The quality of the item which you would like to give to all players.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                    }
                }
            },
            new Command()
            {
                Word = "protect",
                Description = "Protects a specific player from events until 'deprotect'ed.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.ProtectCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to protect.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            },
            new Command()
            {
                Word = "deprotect",
                Description = "De'protect's a specific player from events.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.DeprotectCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to deprotect.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            },
            new Command()
            {
                Word = "immunize",
                Description = "Temporarily protects a specific player from events.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.ImmunizeCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to immunize.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            },
            new Command()
            {
                Word = "deimmunize",
                Description = "Demoves the temporary immunity granted by 'immunize'.",
                Category = Command.CommandCategory.Server_Administration,
                AdvancedCommand = AdvancedCommands.DeimmunizeCommand,
                Parameters = new HashSet<Parameter>()
                {
                    new Parameter()
                    {
                        Name = "Player",
                        Description = "The player who you would like to deimmunize.",
                        Rules = new HashSet<ParameterValidation.Rule>()
                        {
                            ParameterValidation.Rule.PlayerOnline
                        }
                    }
                }
            }
        };
    }
}
