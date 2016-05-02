# envedit
A command line tool to edit Windows environment variables

The desire was to use some bash-like syntax to play off of muscle memory
(cd, ls, rm commands) and to provide a simple interface to get/set.

# Help Output
```cmd
envedit: An environment variable editor

Can be used through a one-at-a-time command line interface (CLI),
or interactively through a read evaluate print loop (REPL).

To access REPL, run envedit with no commands,
or alternatively provide the -i flag.

envedit defaults to User environment variables.
To target System environment variables,
use the -s flag with any of the commands.

Available Commands
------------------
ls: list the variables available in the current environment
get name: list the value of variable name
set name value: set the value of variable name
rm name: delete the variable name
append name value: append value to variable name
help: display this help menu

REPL Specific Commands
----------------------
quit: quit (REPL mode)
cd: toggle between user/system environments (REPL mode)
```

# Example Commands
```cmd
envedit help
envedit ls
envedit -s ls
envedit get -s path
envedit set JAVA_HOME C:\Program Files\Java\jdk1.8.0_65
envedit append path ;C:\Users\guest\bin

envedit set a 1
envedit get a
envedit rm a
```
