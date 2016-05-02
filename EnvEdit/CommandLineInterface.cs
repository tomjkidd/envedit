using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvEdit
{
    class CommandLineInterface
    {
        public static Action stringToAction(string str)
        {
            switch (str)
            {
                case "cd":
                    return Action.ChangeTarget;
                case "ls":
                    return Action.List;
                case "get":
                    return Action.Get;
                case "set":
                    return Action.Set;
                case "append":
                    return Action.Append;
                case "rm":
                    return Action.Delete;
                case "q":
                case "quit":
                    return Action.Quit;
                case "h":
                case "help":
                    return Action.Help;
                default:
                    return Action.ParseError;
            }
        }

        public static string actionToString(Action action) {
            switch (action)
            {
                case Action.ChangeTarget:
                    return "cd";
                case Action.List:
                    return "ls";
                case Action.Get:
                    return "get";
                case Action.Set:
                    return "set";
                case Action.Append:
                    return "append";
                case Action.Delete:
                    return "rm";
                case Action.Quit:
                    return "q";
                case Action.Help:
                    return "help";
                default:
                    return "noop";
            }
        }

        public static EditorState argsToState(IEnumerable<string> args)
        {
            var flags = args.ToList().Where(arg => arg.StartsWith("-")).ToList();

            var target = Editor.containsFlag(flags, "-s") ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;

            var cleanArgs = args.ToList().Where(arg => !arg.StartsWith("-"));
            var cmd = cleanArgs.FirstOrDefault();
            var cmdArgs = cleanArgs.ToList().Skip(1).ToList();

            var action = stringToAction(cmd);

            return new EditorState(target, action, cmdArgs, flags);
        }

        public static string wrongNumberOfArgs(Action action) {
            return String.Format("Wrong number of arguments provided for {0} command.", actionToString(action));
        }

        public static List<string> validateState(EditorState state)
        {
            var errors = new List<string>();
            switch (state.Action)
            {
                case Action.List:
                case Action.ChangeTarget:
                    if (state.Args.Count > 0)
                    {
                        errors.Add(wrongNumberOfArgs(state.Action));
                    }
                    break;
                case Action.Get:
                case Action.Delete:
                    if (state.Args.Count != 1)
                    {
                        errors.Add(wrongNumberOfArgs(state.Action));
                    }
                    break;
                case Action.Set:
                    if (state.Args.Count < 2)
                    {
                        errors.Add(wrongNumberOfArgs(state.Action));
                    }
                    break;
                case Action.Append:
                    if (state.Args.Count < 2)
                    {
                        errors.Add(wrongNumberOfArgs(state.Action));
                    }
                    break;
                case Action.ParseError:
                    errors.Add("Unable to parse command.");
                    break;
            }
            return errors;
        }

        public static EditorState run(IEnumerable<string> args) {
            var state = argsToState(args);
            return runCommand(state);
        }

        public static EditorState runCommand(EditorState state)
        {
            var errors = validateState(state);
            if (errors.Count != 0)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(String.Format("Invalid command: {0}", String.Join("\n", errors)));
                errorWriter.Write("Use 'help' command to get available commands.");
                Environment.Exit(-1);
            }
            var newState = Editor.performAction(state);
            Console.WriteLine(state.Result);
            return newState;
        }

        public static EditorState getMenuChoice(EditorState previousState)
        {
            if (previousState.Action.Equals(Action.Noop))
            {
                Console.WriteLine(Editor.help());
            }

            Console.Write(String.Format("Current Target: {0}>", previousState.Target.Equals(EnvironmentVariableTarget.User) ? "User" : "System"));

            var input = Console.ReadLine().Split(' ');
            var newState = argsToState(input);

            // NOTE: In repl, don't terminate on a bad command.
            if (newState.Action.Equals(Action.ParseError))
            {
                newState.Action = Action.Noop;
                Console.WriteLine("Unrecognized command, performing noop.");
            }

            // NOTE: In interactive mode, cd is used to change target, not the -s flag!
            newState.Target = previousState.Target;

            return newState;
        }

        public static void repl() {
            // Create a mini bash-like loop
            var state = new EditorState(EnvironmentVariableTarget.User, Action.Noop, new List<string>());

            while (state.Action != Action.Quit)
            {
                var newState = runCommand(state);
                state = getMenuChoice(newState);
            }
        }
    }
}
