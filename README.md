# QikConsole

### TODO

- Document how to add to PATH (Linux and Windows)

## Description

Generate documents from fragments and input symbols.

## Usage

- The default generate command takes an `xml` file configuration which includes the necessary parameters.
- These `xml` parameters can be overridden using the `-i` flag.
- This configuration allows for the addition of one or more projects in a single configuration file.
- Get help for existing functionality `qikconsole --help`

```bash

# Generate default project type (project collection)
qikconsole gen -f ./projects.xml -p nt
qikconsole gen -f ./projects.xml -p nt  "-i", "IssueId=34567;IssueType=Story;PsReleaseDate=0000:00:00"
```

- Legacy generate is supported however there is a break in compatibility from the initial command.
- The old command used to be:

```bash
qikconsole gen simple  -f ./project.json "-i", "IssueId=34567;IssueType=Story;PsReleaseDate=0000:00:00"`
```
- This command is now:

```bash
# Generate legacyjsonproject
qikconsole gen -t legacyjsonproject -f ./project.json
qikconsole gen -t legacyjsonproject -f ./project.json "-i", "IssueId=34567;IssueType=Story;PsReleaseDate=0000:00:00"
```

- The format of the `json` project file remains the same.

## Debug Configuration

- To run via the debugger use the following `launch.json` configuration.
- Debug to see how the example configuration files work. There are multiple launch configurations that apply multiple argument combinations.

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            // Use IntelliSense to find out which attributes exist for C# debugging
            // Use hover for the description of the existing attributes
            // For further information visit https://github.com/dotnet/vscode-csharp/blob/main/debugger-launchjson.md
            "name": "Generate (Default, Verbose Parameters)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            // If you have changed target frameworks, make sure to update the program path.
            "program": "${workspaceFolder}/QikConsole/bin/Debug/net7.0/QikConsole.dll",
            "args": [
                "gen", "--file", "${workspaceFolder}\\Debugging\\Simulations\\xmlprojects\\projects.xml", "--projectKey", "ns"
            ],
            "cwd": "${workspaceFolder}/QikConsole",
            // For more information about the 'console' field, see https://aka.ms/VSCode-CS-LaunchJson-Console
            "console": "internalConsole",
            "stopAtEntry": true
        },
        {
            "name": "Generate (Default, Shorthand Parameters)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/QikConsole/bin/Debug/net7.0/QikConsole.dll",
            "args": [
                "gen", "-f", "${workspaceFolder}\\Debugging\\Simulations\\xmlprojects\\projects.xml", "-p", "ns"
            ],
            "cwd": "${workspaceFolder}/QikConsole",
            "console": "internalConsole",
            "stopAtEntry": true
        },
        {
            "name": "Generate (LegacyJsonProject, Verbose Parameters)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/QikConsole/bin/Debug/net7.0/QikConsole.dll",
            "args": [
                "gen", "--type", "legacyjsonproject", "--file", "${workspaceFolder}\\Debugging\\Simulations\\xmlprojects\\legacyproject.json", "--projectKey", "ns"
            ],
            "cwd": "${workspaceFolder}/QikConsole",
            "console": "internalConsole",
            "stopAtEntry": true
        },
        {
            "name": "Generate (LegacyJsonProject, Shorthand Parameters)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/QikConsole/bin/Debug/net7.0/QikConsole.dll",
            "args": [
                "gen", "-t", "legacyjsonproject", "-f", "${workspaceFolder}\\Debugging\\Simulations\\xmlprojects\\legacyproject.json", "-p", "ns", "-i", "IssueId=34567;IssueType=Story;PsReleaseDate=0000:00:00"
            ],
            "cwd": "${workspaceFolder}/QikConsole",
            "console": "internalConsole",
            "stopAtEntry": true
        },
        {
            "name": ".NET Core Attach",
            "type": "coreclr",
            "request": "attach"
        }
    ]
}
```

## Development

### Build

**Linux Release**

```bash
dotnet publish -r linux-x64 --self-contained true
```

### Debugging

- Important that the `externalTerminal` (Windows) is set for the `console` setting in your `launch.json`. Otherwise you'll run the program in your `internalConsole` and it will break. 
- For more information about the 'console' field, see https://aka.ms/VSCode-CS-LaunchJson-Console

```
"console": "externalTerminal",
```

## System.Commandline

There was concern that the project for this has stalled, however it seems to have picked up again and [here is a code review of Phase 1](https://www.youtube.com/watch?v=yDQGsZSEDOk)

- [System.CommandLine (Nuget)](https://www.nuget.org/packages/System.CommandLine)
- [System.CommandLine (Github)](https://github.com/dotnet/command-line-api/blob/master/docs/Your-first-app-with-System-CommandLine.md)
- [Dependency Injection and Settings](https://espressocoder.com/2018/12/03/build-a-console-app-in-net-core-like-a-pro/)
- [Getting Started with System.CommandLine](https://dotnetdevaddict.co.za/2020/09/25/getting-started-with-system-commandline/)
- Commandline Option commands have a ParseArguments parameter:
  - [ParseArguments Example 1](https://csharp.hotexamples.com/examples/CommandLine/Parser/ParseArguments/php-parser-parsearguments-method-examples.html)
  - [ParseArguments Example 2](https://csharp.hotexamples.com/examples/CommandLine/CommandLineParser/ParseArguments/php-commandlineparser-parsearguments-method-examples.html)


## System Logging

- [NLog Tutorial - The essential guide for logging from C#](https://blog.elmah.io/nlog-tutorial-the-essential-guide-for-logging-from-csharp/)
- [NLog on Github](https://github.com/NLog/NLog)