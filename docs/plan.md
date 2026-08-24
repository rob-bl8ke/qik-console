# Plan: Modernize Qik Console App

## TL;DR

Qik is a template-driven document generator with a solid core vision — define projects with inputs, scripts, fragments, and document templates, then generate output files by substituting symbols. The codebase suffers from .NET 7 (EOL), duplicated generators, mixed namespaces, dead code (PowerShell SDK, plugin system, legacy JSON), and low test coverage. We'll modernize to .NET 8, migrate from XML/JSON config to YAML, unify the generation pipeline, strip dead features, and improve the CLI UX — all while keeping this focused as a lean personal productivity tool.

---

## Phase 1: Strip Dead Weight

Remove unused/unwanted features to reduce complexity before refactoring.

1. **Remove QikFunnyFunctions project** from solution — drop plugin system. Remove `PluginLoader` usage from generators (use `new FunctionFactory()` without plugins, or a null/empty loader if the `rob_bl8ke.Qik` API requires one).
2. **Remove legacy JSON support** — delete `JsonLegacyProject.cs`, `JsonLegacyProjectFile.cs`, `JsonLegacyProjectGenerator.cs`, and the `legacyjsonproject` branch in `GenerateCommand.Execute()`.
3. **Remove PowerShell SDK dependencies** — remove `Microsoft.PowerShell.SDK` and `System.Management.Automation` from `QikConsole.csproj`. Remove any post-execution script references from the XML schema/model.
4. **Remove dead code** — `FileSettings.cs` (empty skeleton), `ErrorMessages.resx`/`ErrorMessages.Designer.cs` (unused), `BaseCommand.cs` (duplicated by `ConsoleMessages`).
5. **Remove `postScript`/`postExecutionScripts` from project model** — clean up `Project.cs` and any XML parsing that reads post-script settings.

**Files to delete:** `JsonLegacyProject.cs`, `JsonLegacyProjectFile.cs`, `JsonLegacyProjectGenerator.cs`, `FileSettings.cs`, `ErrorMessages.resx`, `ErrorMessages.Designer.cs`, `BaseCommand.cs`, entire `QikFunnyFunctions/` project.

**Files to modify:** `QikConsole.csproj` (remove deps), `Qik.sln` (remove QikFunnyFunctions), `GenerateCommand.cs`, `CommandFactory.cs`, `Program.cs`, `QikConsoleTests.csproj` (remove plugin build target).

---

## Phase 2: Upgrade to .NET 8

6. **Update `TargetFramework` to `net8.0`** in all `.csproj` files (QikConsole, QikConsoleTests).
7. **Update NuGet packages** to .NET 8 compatible versions — `Microsoft.Extensions.*` to 8.x, `NLog` to latest, `FluentAssertions` to latest, `Moq` to latest or switch to `NSubstitute`, `NUnit` to latest.
8. **Evaluate System.CommandLine** — still in beta after years. Consider switching to `Cocona` or `Spectre.Console.Cli` which are stable and actively maintained. Recommendation: **Spectre.Console.Cli** — provides command parsing + beautiful console output (tables, colors, progress) in one package.
9. **Update `welcome.txt`** version to match `csproj` version. Consider generating version from assembly info instead of hardcoding.

**Depends on:** Phase 1 complete.

---

## Phase 3: Migrate Config from XML to YAML

10. **Add `YamlDotNet` NuGet package.**
11. **Design the YAML project schema.** Proposed format:

```yaml
globals:
  PsReleaseDate: "0000-00-00"
  Uat1ReleaseDate: "0000-00-00"

projects:
  ns:
    title: "New Story"
    script: ./script.qik
    inputs:
      IssueId: "666666"
      IssueTitle: "Metallica"
      ParentIssueId: "777777"
      IssueType: "Story"
    globals: [PsReleaseDate, Uat1ReleaseDate]
    fragments:
      simple-fragment: ./fragments/simple-fragment.qikt
      table-fragment: ./fragments/table-fragment.qikt
    documents:
      - template: ./documents/document.qikt
        outputs:
          - ./output/document.md
```

12. **Create `YamlProjectsFile` implementing `IProjectsFile`** — deserialize YAML into the existing `Project` model. This replaces `XmlProjectsFile`.
13. **Simplify the `Project` model** — flatten it now that we have one config format. Drop the `key`/`value` attribute pattern; the YAML key IS the project key, the `title` field replaces `value`. Simplify `Input` to just `Symbol`/`Value` (the `key`/`title` attributes from XML were presentation-only). Simplify `Fragment` to just a dictionary `id → path`. Simplify `Document` to just `template path + output paths[]`.
14. **Delete `XmlProjectsFile.cs`** after migration.
15. **Update simulation/debugging files** — convert `projects.xml` to `projects.yaml`. Update test fixtures.

**Depends on:** Phase 2 complete.

---

## Phase 4: Unify the Generation Pipeline

16. **Extract a single `Generator` class** — the current `XmlProjectsGenerator` and `JsonLegacyProjectGenerator` share ~80% identical code (input parsing, fragment processing, document generation). After removing JSON, consolidate into one clean `DocumentGenerator` class.
17. **Move input parsing (`SetInputs`) out of the generator** — this is CLI-level concern. Parse `-i` inputs in the command handler and pass structured `Input[]` to the generator.
18. **Inject `IInterpreter` properly** — currently `new Interpreter()` is called inside generators. Use the one already registered in DI.
19. **Fix namespace inconsistency** — unify everything under `QikConsole` (or `Qik.Console`). Currently mixed between `QikConsole` and `CygSoft.Qik.QikConsole`.

**Files to create:** `DocumentGenerator.cs`
**Files to modify:** `GenerateCommand.cs`, `CommandFactory.cs`, `Program.cs`
**Files to delete:** `XmlProjectsGenerator.cs` (after unification)

**Depends on:** Phase 3 complete.

---

## Phase 5: CLI UX Improvements

20. **Add a `list` command** — `qik list -f projects.yaml` shows all project keys and titles. Quick way to see what's available.
21. **Add a `validate` command** — `qik validate -f projects.yaml -p ns` checks that all referenced script/fragment/document files exist without generating.
22. **Add `--dry-run` flag to `gen`** — shows what would be generated without writing files.
23. **Improve error messages** — replace generic `ApplicationException` throws with specific, actionable messages (e.g., "Fragment file not found: ./fragments/missing.qikt — referenced by project 'ns'").
24. **Add generation summary output** — after successful generation, show what files were written and where.
25. **Cross-platform path handling** — replace all backslash path separators with `Path.Combine` / `Path.DirectorySeparatorChar`. Remove hardcoded Windows paths from tests.

*Parallel with Phase 4.*

---

## Phase 6: Testing & Quality

26. **Fix existing tests for cross-platform** — `IsDirectoryTests.cs` uses `C:\Windows` and `C:\TotallyFakeFile.exe`. Rewrite with temp directories.
27. **Remove deprecated NUnit assertions** — migrate from `Assert.AreEqual` to `Assert.That(..., Is.EqualTo(...))` or use FluentAssertions consistently.
28. **Add generator integration tests** — test the full pipeline: YAML config → generation → verify output file contents. Use temp directories.
29. **Add command-level tests** — test that CLI args parse correctly and route to the right generator.
30. **Clean up test helpers** — remove `PluginHelpers.cs` (no more plugins), update `FileHelpers` for YAML.
31. **Update README** — remove debug configs (move to `.vscode/launch.json`), remove dev reference links, write clear usage docs for the YAML format. Keep it short since this is a personal tool.

**Depends on:** Phases 4 & 5 complete.

---

## Relevant Files

### Core to modify
- `QikConsole/Program.cs` — DI registration, remove dead services, update namespace
- `QikConsole/QikConsole.csproj` — .NET 8, remove PowerShell/plugin deps, add YamlDotNet + Spectre.Console
- `QikConsole/Commands/GenerateCommand.cs` — simplify routing, extract input parsing
- `QikConsole/Commands/CommandFactory.cs` — add list/validate commands
- `QikConsole/Projects/Project.cs` — simplify model
- `QikConsole/Projects/IProjectsFile.cs` — keep interface, change implementation
- `QikConsole/Generators/XmlProjectsGenerator.cs` — refactor into `DocumentGenerator`
- `QikConsole/PlaceholderTerminal.cs` — keep as-is, this is clean
- `QikConsole/FileFunctions.cs` — keep as-is, this is well-abstracted
- `QikConsole/ConsoleMessages.cs` — enhance or replace with Spectre.Console
- `QikConsole/Input.cs` — may simplify to just Symbol+Value

### To delete
- `QikConsole/Projects/JsonLegacyProject.cs`
- `QikConsole/Projects/JsonLegacyProjectFile.cs`
- `QikConsole/Generators/JsonLegacyProjectGenerator.cs`
- `QikConsole/FileSettings.cs`
- `QikConsole/ErrorMessages.resx` + `ErrorMessages.Designer.cs`
- `QikConsole/Commands/BaseCommand.cs`
- `QikFunnyFunctions/` (entire project)

### To create
- `QikConsole/Projects/YamlProjectsFile.cs`
- `QikConsole/Generators/DocumentGenerator.cs`
- `QikConsole/Commands/ListCommand.cs`
- `QikConsole/Commands/ValidateCommand.cs`
- `Debugging/Simulations/yamlprojects/projects.yaml` (new sample)

### Tests to update
- `QikConsoleTests/XmlProjectFileTests.cs` → rename to `YamlProjectFileTests.cs`
- `QikConsoleTests/PlaceholderTerminalTests.cs` — keep, update plugin stubs
- `QikConsoleTests/ExtensionMethods/IsDirectoryTests.cs` — fix cross-platform
- `QikConsoleTests/Helpers/` — remove PluginHelpers, update FileHelpers

---

## Verification

1. `dotnet build` succeeds with zero warnings across all projects
2. `dotnet test` passes all existing tests (adapted for new format) plus new tests
3. Run `qik gen -f projects.yaml -p ns` against simulation data and verify output matches expected `document.md`
4. Run `qik list -f projects.yaml` and verify project listing
5. Run `qik validate -f projects.yaml -p ns` and verify it catches missing files
6. Run `qik gen --dry-run -f projects.yaml -p ns` and verify no files are written
7. Verify on macOS (no Windows path assumptions)

---

## Decisions

- **YAML over TOML/JSON** for project config — user preference, human-readable, supports comments
- **Drop plugins, legacy JSON, post-execution scripts** — simplify for personal use
- **.NET 8 LTS** — supported until Nov 2026, stable
- **Spectre.Console.Cli** recommended over System.CommandLine beta — but this is a soft recommendation; if you prefer to keep System.CommandLine, we can stay with it
- **Qik interpreter treated as frozen dependency** — we won't change its API
- **Scope boundary**: we are NOT changing the `.qik` scripting language, the `.qikt` template format, or the `PlaceholderTerminal` symbol replacement logic — those work well

## Further Considerations

1. **Spectre.Console vs System.CommandLine**: Spectre.Console.Cli gives us command parsing AND rich output (tables, colors, markup) in one package. System.CommandLine has been in beta since 2020. Recommendation: switch to Spectre. But if you'd rather minimize churn, we can keep System.CommandLine and just add Spectre.Console for output formatting only.
2. **Global tool packaging**: Since this is personal, consider publishing as a `dotnet tool` (`dotnet tool install --global qik`) so it's on your PATH everywhere. Just needs `<PackAsTool>true</PackAsTool>` in csproj.
3. **Config discovery**: Should `qik gen -p ns` auto-discover a `qik.yaml` in the current directory (like Docker Compose finds `docker-compose.yml`)? Would save typing the `-f` flag every time.
