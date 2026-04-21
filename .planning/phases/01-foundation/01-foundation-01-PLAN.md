---
phase: 01-foundation
plan: 01
type: execute
wave: 1
depends_on: []
files_modified: [OpenTibia.Client/OpenTibia.Client.csproj, OpenTibia.Client/LocalGameState.cs, OpenTibia.Client/ClientContext.cs]
autonomous: true
requirements: [REUSE-01, CORE-01, CORE-02, D-01]

must_haves:
  truths:
    - "OpenTibia.Client project is created and correctly references core libraries."
    - "LocalGameState provides placeholder structures for Map and Creatures."
    - "ClientContext initializes correctly and inherits from OpenTibia.Game.Common.Context."
  artifacts:
    - path: "OpenTibia.Client/OpenTibia.Client.csproj"
      provides: "Client project definition and references"
    - path: "OpenTibia.Client/LocalGameState.cs"
      provides: "In-memory game state buffer"
    - path: "OpenTibia.Client/ClientContext.cs"
      provides: "Context for client-side operations"
  key_links:
    - from: "OpenTibia.Client/ClientContext.cs"
      to: "OpenTibia.Game.Common.Context"
      via: "inheritance"
    - from: "OpenTibia.Client/OpenTibia.Client.csproj"
      to: "OpenTibia.Common.csproj"
      via: "ProjectRef"
---

<objective>
Initialize the OpenTibia Raylib Client project and implement the core state and context objects.

Purpose: To establish the foundational data structures and architectural context for the client, enabling subsequent implementation of the game loop and command pipeline.
Output: A new C# project with core library references and base state/context classes.
</objective>

<execution_context>
@$HOME/.gemini/get-shit-done/workflows/execute-plan.md
@$HOME/.gemini/get-shit-done/templates/summary.md
</execution_context>

<context>
@.planning/PROJECT.md
@.planning/ROADMAP.md
@.planning/STATE.md
@.planning/phases/01-foundation/01-foundation-CONTEXT.md
@OpenTibia.Game.Common/Common/Context.cs
</context>

<tasks>

<task type="auto">
  <name>Task 1: Create OpenTibia.Client project</name>
  <files>OpenTibia.Client/OpenTibia.Client.csproj</files>
  <action>
    Create a new .NET 10.0 project named `OpenTibia.Client`.
    Add project references to:
    - `OpenTibia.Common`
    - `OpenTibia.Network`
    - `OpenTibia.Threading`
    - `OpenTibia.Game.Common`
    Add NuGet package reference for `Raylib-cs`.
    Ensure the project is added to `OpenTibia.sln`.
  </action>
  <verify>
    <automated>dotnet build OpenTibia.Client/OpenTibia.Client.csproj</automated>
  </verify>
  <done>OpenTibia.Client project exists and builds successfully with all core references.</done>
</task>

<task type="auto">
  <name>Task 2: Implement LocalGameState</name>
  <files>OpenTibia.Client/LocalGameState.cs</files>
  <action>
    Implement the `LocalGameState` class in the `OpenTibia.Client` namespace.
    It should hold placeholder structures for `Map` (e.g., a buffer of Tiles) and `Creatures` (e.g., a dictionary of Creature objects from `OpenTibia.Common.Objects`).
    Include a simple `StatusMessage` string field for demonstration purposes.
  </action>
  <verify>
    <automated>dotnet build OpenTibia.Client/OpenTibia.Client.csproj</automated>
  </verify>
  <done>LocalGameState is implemented with basic data structures.</done>
</task>

<task type="auto">
  <name>Task 3: Implement ClientContext</name>
  <files>OpenTibia.Client/ClientContext.cs</files>
  <action>
    Implement `ClientContext` in the `OpenTibia.Client` namespace.
    It must:
    - Inherit from `OpenTibia.Game.Common.Context`.
    - Provide a property to access the `LocalGameState`.
    - Initialize the base `Context` with necessary services (like the Dispatcher from `OpenTibia.Threading`).
    Use DEC-03 and CORE-01.
  </action>
  <verify>
    <automated>dotnet build OpenTibia.Client/OpenTibia.Client.csproj</automated>
  </verify>
  <done>ClientContext is implemented and correctly integrates with the OpenTibia architecture.</done>
</task>

</tasks>

<verification>
Verify that the `OpenTibia.Client` project builds and its core objects can be instantiated.
</verification>

<success_criteria>
- Project `OpenTibia.Client` builds without errors.
- `LocalGameState` and `ClientContext` classes are correctly defined and functional.
</success_criteria>

<output>
After completion, create `.planning/phases/01-foundation/01-foundation-01-SUMMARY.md`
</output>
