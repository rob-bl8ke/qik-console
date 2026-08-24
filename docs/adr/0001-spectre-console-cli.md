# Use Spectre.Console.Cli over System.CommandLine

System.CommandLine has been in beta since 2020 with no stable release. We switched to Spectre.Console.Cli, which is stable, actively maintained, and bundles command parsing with rich console output (tables, colour markup, panels) in a single package — eliminating the need for a separate output library.

## Considered Options

- **System.CommandLine** — the "official" .NET CLI library, already in use. Rejected because it remains in beta after five years and its API has changed repeatedly.
- **Cocona** — stable and ergonomic, but output formatting requires a separate library.
- **Spectre.Console.Cli** — stable, rich output built-in, actively maintained. Chosen.
