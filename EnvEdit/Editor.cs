using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvEdit
{
    enum Action
    {
        Noop,
        Help,
        ParseError,
        ChangeTarget,
        List,
        Get,
        Set,
        Delete,
        Append,
        Quit
    }

    class EditorState
    {
        public List<string> Args { get; set; }
        public List<string> Flags { get; set; }
        public EnvironmentVariableTarget Target { get; set; }
        public Action Action { get; set; }
        public string Result { get; set; }

        public EditorState(EnvironmentVariableTarget target, Action action, List<string> args)
        {
            Target = target; // User/System selection
            Action = action; // Keeps track of which command is being requested
            Args = args; // Arguments provided with the command
            Result = ""; // Result of action
        }

        public EditorState(EnvironmentVariableTarget target, Action action, List<string> args, List<string> flags)
        {
            Target = target; // User/System selection
            Action = action; // Keeps track of which command is being requested
            Args = args; // Arguments provided with the command
            Flags = flags; // Flags provided with the command
            Result = ""; // Result of action
        }
    }

    class Editor
    {
        public static bool containsFlag(IEnumerable<string> flags, string flag)
        {
            return flags.Where(arg => arg == flag).FirstOrDefault() != null;
        }

        public static EditorState performAction(EditorState state)
        {
            switch (state.Action)
            {
                case Action.ChangeTarget:
                    state.Target = state.Target.Equals(EnvironmentVariableTarget.User) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;
                    state.Args = new List<string>();
                    state.Result = null;
                    break;
                case Action.List:
                    state.Result = list(state);
                    break;
                case Action.Get:
                    state.Result = get(state);
                    break;
                case Action.Set:
                    state.Result = set(state);
                    break;
                case Action.Append:
                    state.Result = append(state, containsFlag(state.Flags, "-t"));
                    break;
                case Action.Delete:
                    var key = state.Args.First();
                    string deleteArg = null; // Passing null will delete env var
                    state.Args = new List<string> { key, deleteArg };
                    state.Result = set(state);
                    break;
                case Action.Quit:
                    break;
                case Action.Help:
                    state.Args = new List<string>();
                    state.Result = help();
                    break;
                case Action.Noop:
                case Action.ParseError:
                    state.Args = new List<string>();
                    state.Result = null;
                    break;
            }

            return state;
        }

        public static string help()
        {
            var strs = new List<string> {
                "envedit: An environment variable editor",
                "",
                "Can be used through a one-at-a-time command line interface (CLI),",
                "or interactively through a read evaluate print loop (REPL).",
                "",
                "To access REPL, run envedit with no commands,",
                "or alternatively provide the -i flag.",
                "",
                "envedit defaults to User environment variables.",
                "To target System environment variables,",
                "use the -s flag with any of the commands.",
                "",
                "Available Commands",
                "------------------",
                "help: display this help menu",
                "ls: list the variables available in the current environment",
                "get name: list the value of variable name",
                "set name value: set the value of variable name",
                "rm name: delete the variable name",
                "append name value: append value to variable name",
                "",
                "REPL Specific Commands",
                "----------------------",
                "quit: quit (REPL mode)",
                "cd: toggle between user/system environments (REPL mode)",
                
            };
            return String.Join("\n", strs);
        }

        public static string list(EditorState state)
        {
            var evs = Environment.GetEnvironmentVariables(state.Target);
            return String.Join("\n", evs.Keys.OfType<String>().OrderBy(ev => ev).ToList());
        }

        public static string get(EditorState state)
        {
            var key = state.Args.First();
            return Environment.GetEnvironmentVariable(key, state.Target);
        }

        public static string set(EditorState state)
        {
            var key = state.Args.First();
            var value = String.Join(" ", state.Args.Skip(1));
            Environment.SetEnvironmentVariable(key, value, state.Target);
            return Environment.GetEnvironmentVariable(key, state.Target);
        }

        public static string append(EditorState state, bool test)
        {
            var key = state.Args.First();
            var value = String.Join(" ", state.Args.Skip(1));
            var old = Environment.GetEnvironmentVariable(key, state.Target);
            var newValue = String.Concat(old, value);
            if (test)
            {
                return newValue;
            }
            else
            {
                Environment.SetEnvironmentVariable(key, newValue, state.Target);
                return Environment.GetEnvironmentVariable(key, state.Target);
            }
        }
    }
}
