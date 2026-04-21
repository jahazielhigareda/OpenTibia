# Phase 1: Foundation - ClientContext, LocalGameState, and Integrated Game Loop

## Goal
Establish the client architecture and core game loop using the OpenTibia handler-driven pattern.

## Requirements
- ARCH-01: Mirror OpenTibia architecture (Dispatcher/Context/Command/Event).
- ARCH-02: Integrated Game Loop using Raylib.
- REUSE-01: Reuse OpenTibia.Common library.
- REUSE-02: Reuse OpenTibia.Network library.
- CORE-01: Implementation of ClientContext.
- CORE-02: Implementation of LocalGameState.

## Decisions
- [D-01] The client project will be named `OpenTibia.Client`.
- [D-02] Use Raylib for high-performance rendering.
- [D-03] Inherit `ClientContext` from `OpenTibia.Common.Context`.
- [D-04] Implement a placeholder `UpdatePlayerHealthCommand` to test the command-driven update mechanism.

## Constraints
- Must reuse existing core libraries (Common, Network, Threading, Game.Common).
- Use `OpenTibia.Threading.Dispatcher` to handle commands.
- Visual check is required to verify the Raylib window displays game state info.

## Success Criteria
- [S-01] Application initializes a Raylib window and maintains a stable frame rate.
- [S-02] `ClientContext` is successfully initialized with a functional `Dispatcher`.
- [S-03] `LocalGameState` updates can be observed when commands are dispatched.
- [S-04] Unit and integration tests verify core architecture.
