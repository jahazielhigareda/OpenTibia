---
phase: 01-foundation
plan: 03
type: execute
wave: 3
depends_on: [01-foundation-02]
files_modified: [OpenTibia.Client.Tests/OpenTibia.Client.Tests.csproj, OpenTibia.Client.Tests/ClientContextTests.cs, OpenTibia.Client.Tests/CommandPipelineTests.cs]
autonomous: false
requirements: [S-01, S-03, S-04]

must_haves:
  truths:
    - "Unit tests verify that ClientContext initializes correctly with LocalGameState."
    - "Integration tests confirm that dispatching a command updates the local state."
    - "Raylib window successfully displays a message from the LocalGameState."
  artifacts:
    - path: "OpenTibia.Client.Tests/OpenTibia.Client.Tests.csproj"
      provides: "Unit and integration test project"
    - path: "OpenTibia.Client.Tests/ClientContextTests.cs"
      provides: "ClientContext unit tests"
    - path: "OpenTibia.Client.Tests/CommandPipelineTests.cs"
      provides: "Command/Dispatcher integration tests"
  key_links:
    - from: "OpenTibia.Client.Tests/OpenTibia.Client.Tests.csproj"
      to: "OpenTibia.Client/OpenTibia.Client.csproj"
      via: "ProjectRef"
---

<objective>
Verify the client foundation using automated tests and a final visual check.

Purpose: To ensure the architectural integrity and functionality of the core client-side components.
Output: A suite of passing tests and a confirmed visual demonstration of the game loop and state management.
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
@.planning/phases/01-foundation/01-foundation-02-SUMMARY.md
</context>

<tasks>

<task type="auto">
  <name>Task 1: Implement Unit and Integration Tests</name>
  <files>OpenTibia.Client.Tests/OpenTibia.Client.Tests.csproj, OpenTibia.Client.Tests/ClientContextTests.cs, OpenTibia.Client.Tests/CommandPipelineTests.cs</files>
  <action>
    Create a new test project `OpenTibia.Client.Tests`.
    - Reference `OpenTibia.Client` and `xunit`.
    - Implement `ClientContextTests` to verify `LocalGameState` initialization.
    - Implement `CommandPipelineTests` to verify that dispatching `UpdatePlayerHealthCommand` via the `Dispatcher` correctly updates the state in the `ClientContext`.
  </action>
  <verify>
    <automated>dotnet test OpenTibia.Client.Tests/OpenTibia.Client.Tests.csproj</automated>
  </verify>
  <done>Automated tests are implemented and passing.</done>
</task>

<task type="checkpoint:human-verify" gate="blocking">
  <what-built>Visual Raylib client with status display</what-built>
  <how-to-verify>
    1. Run the client application: `dotnet run --project OpenTibia.Client/OpenTibia.Client.csproj`
    2. Verify a Raylib window opens with the title "OpenTibia Raylib Client".
    3. Verify the window displays a default status message from the local game state.
    4. Press the designated test key (e.g., Space) and verify the status message updates (e.g., to "Health: 100").
  </how-to-verify>
  <resume-signal>approved</resume-signal>
</task>

</tasks>

<verification>
Run `dotnet test` and confirm all tests pass, followed by the manual visual verification.
</verification>

<success_criteria>
- All unit and integration tests pass.
- Visual check confirms real-time state updates in the Raylib window.
</success_criteria>

<output>
After completion, create `.planning/phases/01-foundation/01-foundation-03-SUMMARY.md`
</output>
