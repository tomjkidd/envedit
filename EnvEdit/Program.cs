using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace EnvEdit
{
    // TODO: Handle the wrong number of commands for each Action! Create a validateState method.
    class Program
    {
        static void Main(string[] args)
        {
            if (IsInteractiveEdit(args))
            {
                CommandLineInterface.repl();
            }
            else
            {
                CommandLineInterface.run(args);
            }
        }

        public static bool IsInteractiveEdit(IEnumerable<string> args) {
            var argCount = args.ToList().Count;
            return (argCount == 1 && args.First() == "-i") || argCount == 0;
        }
    }
}
