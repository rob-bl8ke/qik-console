# Use YAML for project config instead of XML

The existing XML format required verbose attribute-heavy syntax (`key`/`value`/`title` on every element) and was tied to a legacy `System.Xml` parsing path. We replaced it with YAML via `YamlDotNet`: YAML is human-readable, supports inline comments, maps naturally to the `Project` model, and eliminates the dual XML/JSON parsing paths.

## Considered Options

- **XML** — already in use. Rejected: verbose, no comments in most parsers, two separate parsers (XML + JSON) for an identical model.
- **JSON** — already partially in use (legacy path). Rejected: no comments, less readable for config files with many keys.
- **TOML** — readable, but less familiar and `YamlDotNet` is already a well-supported .NET library.
- **YAML** — chosen. Human-readable, supports comments, single parser replaces both XML and JSON paths.
