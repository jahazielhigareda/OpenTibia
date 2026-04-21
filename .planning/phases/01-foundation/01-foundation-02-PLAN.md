---
phase: 01-foundation
plan: 02
type: execute
wave: 2
depends_on: [01-foundation-01]
files_modified: [OpenTibia.Client/Program.cs, OpenTibia.Client/Commands/UpdatePlayerHealthCommand.cs]
autonomous: true
requirements: [ARCH-01, ARCH-02, D-02, D-04]

must_haves:
  truths:
    - "Raylib window initializes and maintains a frame loop."
    - "OpenTibia.Threading.Dispatcher is integrated and processing commands."
    - "UpdatePlayerHealthCommand correctly modifies the StatusMessage in LocalGameState."
  artifacts:
    - path: "OpenTibia.Client/Program.cs"
      provides: "Main entry point and Raylib game loop"
    - path: "OpenTibia.Client/Commands/UpdatePlayerHealthCommand.cs"
      provides: "Test command for state updates"
  key_links:
    - from: "OpenTibia.Client/Program.cs"
      to: "Raylib-cs"
      via: "API calls"
    - from: "OpenTibia.Client/Program.cs"
      to: "OpenTibia.Threading.Dispatcher"
      via: "instantiation and start"
---

<objective>
Implement the integrated game loop using Raylib and the OpenTibia Dispatcher, and create a test command pipeline.

Purpose: To establish the active runtime of the client where rendering and game logic (commands) coexist.
Output: A functional Raylib application that can process and reflect state changes via commands.
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
@.planning/phases/01-foundation/01-foundation-01-SUMMARY.md
@OpenTibia.Threading/Dispatcher.cs
</context>

<tasks>

<task type="auto">
  <name>Task 1: Implement Raylib main loop</name>
  <files>OpenTibia.Client/Program.cs</files>
  <action>
    Implement the `Main` method in `Program.cs`.
    - Initialize Raylib window (800x600, "OpenTibia Raylib Client").
    - Instantiate and start the `OpenTibia.Threading.Dispatcher`.
    - Create a `ClientContext` and `LocalGameState`.
    - Implement a standard Raylib game loop:
      - Clear background.
      - Draw a status message from `LocalGameState.StatusMessage`.
      - Check for a test key press (e.g., Space) to dispatch `UpdatePlayerHealthCommand`.
    - Ensure clean shutdown of Raylib and Dispatcher.
  </action>
  <verify>
    <automated>dotnet build OpenTibia.Client/OpenTibia.Client.csproj</automated>
  </verify>
  <done>Main game loop is implemented with Raylib and Dispatcher integration.</done>
</task>

<task type="auto" tdd="true">
  <name>Task 2: Implement UpdatePlayerHealthCommand</name>
  <files>OpenTibia.Client/Commands/UpdatePlayerHealthCommand.cs</files>
  <behavior>
    - When executed, the command should update `LocalGameState.StatusMessage` with the new health value.
    - It should accept a health value in its constructor.
  </behavior>
  <action>
    Implement `UpdatePlayerHealthCommand` inheriting from `OpenTibia.Game.Common.Commands.Command` (or `IncomingCommand` as appropriate).
    Implement the `Execute` method to update the `LocalGameState` within the `ClientContext`.
  </action>
  <verify>
    <automated>dotnet build OpenTibia.Client/OpenTibia.Client.csproj</automated>
  </verify>
  <done>UpdatePlayerHealthCommand is implemented and functional.</done>
</task>

</tasks>

<verification>
Verify that the project builds and the game loop correctly initializes the window.
</verification>

<success_criteria>
- Project `OpenTibia.Client` builds without errors.
- `Program.cs` contains a functional Raylib loop.
- `UpdatePlayerHealthCommand` is correctly implemented.
</success_criteria>

<output>
After completion, create `.planning/phases/01-foundation/01-foundation-02-SUMMARY.md`
</output>
