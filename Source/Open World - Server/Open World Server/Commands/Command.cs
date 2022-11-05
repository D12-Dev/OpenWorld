using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenWorldServer
{
    public static class ParameterValidation
    {
        public enum Rule
        {
            PlayerOnline,
            ValidEvent
        }
        public static readonly Dictionary<Rule, Func<string, bool>> Validation = new Dictionary<Rule, Func<string, bool>>()
        {
            {Rule.PlayerOnline, (arg) => Networking.connectedClients.Any(x => x.username == arg) },
            {Rule.ValidEvent, (arg) => SimpleCommands.EventList.Contains(arg) }
            // TODO: ValidFaction, ValidItem, ValidItemQuantity, ValidItemQuality, PlayerBanned, PlayerAdmin
        };
    }
    public struct Parameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public HashSet<ParameterValidation.Rule> Rules { get; set; } 
    }
    public class Command
    {
        public enum CommandCategory
        {
            Player_Interaction,
            Server_Administration,
            Information
        }
        public string Word { get; set; }
        public string Description { get; set; }
        public CommandCategory Category { get; set; }
        public HashSet<Parameter> Parameters { get; set; }

        private Action _simpleCommand;
        public Action SimpleCommand
        {
            get => _simpleCommand; 
            set
            {
                _advancedCommand = null;
                _simpleCommand = value;
            }
        }
        private Action<string[]> _advancedCommand;
        
        public Action<string[]> AdvancedCommand
        {
            get => _advancedCommand;
            set
            {
                _simpleCommand = null;
                _advancedCommand = value;
            }
        }


        public void Execute(string[] arguments = null)
        {
            // TODO: Itemized error messages.
            if (SimpleCommand != null) SimpleCommand();
            else if (AdvancedCommand != null && arguments.Length == Parameters.Count && Parameters.SelectMany((x, i) => x.Rules.Select(y => ParameterValidation.Validation[y](arguments[i]))).All(x=>x)) AdvancedCommand(arguments);
            else throw new Exception($"The execution of {Word} failed due to an invalid command structure. Ensure a command method is mapped, and that the correct number of arguments are provided if required. Expected {Parameters.Count} parameters and recieved {arguments.Length}. If the correct number of arguments were sent, ensure they are valid for the context.");
        }

    }
}
