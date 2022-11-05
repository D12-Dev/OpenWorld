using System;
using System.Collections.Generic;
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

        };
    }
    public struct Parameter
    {
        

        public string Name { get; set; }
        public string Description { get; set; }
        public List<ParameterValidation.Rule> Rules { get; set; }
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
            if (SimpleCommand != null) SimpleCommand();
            else if (AdvancedCommand != null && arguments != null) AdvancedCommand(arguments);
            else throw new Exception($"The execution of {Word} failed due to an invalid command structure. Ensure a command method is mapped, and that arguments are provided if required.");
        }

    }
}
