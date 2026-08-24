# Qik Console

A personal CLI tool that generates documents from template files by substituting symbols resolved from a script and its inputs.

## Language

**Project**:
A named entry in `qik.yaml` that bundles a script, inputs, globals, fragments, and document templates, identified by a short key (e.g. `ns`). The unit of work for a single `qik gen` invocation.
_Avoid_: Job, task, template set, config entry

**Config file**:
The `qik.yaml` file that defines globals and projects. Auto-discovered in the current working directory; overridable with `-f`.
_Avoid_: Projects file, project file, configuration

**Script**:
A `.qik` file that defines symbols by evaluating expressions over inputs and globals. One script per project.
_Avoid_: Program, qik file, template

**Symbol**:
A named placeholder resolved to a value at generation time. Defined in a script; used in fragments and document templates.
_Avoid_: Variable, placeholder, token

**Input**:
A symbol/value pair defined in a project and passed to its script at generation time. Overridable at the CLI level via `-i`.
_Avoid_: Parameter, argument, variable

**Global**:
A symbol/value pair defined at the config-file level with a default value and referenced by name in a project. Distinct from an Input: globals have file-wide scope and are shared across projects. Overridable via `-i` at generation time.
_Avoid_: Shared input, file-level input

**Fragment**:
A named `.qikt` partial template referenced by id and composed into document templates at generation time.
_Avoid_: Partial, include, snippet, template part

**Document**:
A `.qikt` template file that is rendered by the interpreter to produce one or more output files. A project may define multiple documents.
_Avoid_: Template (overloaded — reserve for the `.qikt` file format; use Document for the config concept)

**Generation**:
The end-to-end process of reading a project's config, running its script, and rendering its document templates to produce output files.
_Avoid_: Execution, processing, run
