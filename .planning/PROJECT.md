# Tibia Client Project (Raylib)

## Current State
- **Shipped Version:** v1.1 (2026-04-21)
- **Status:** Architectural Polish complete. Protocol 8.60 Login & Game Flow implementation starting.

## Next Milestone Goals (v1.2: Login & Game Flow)
- **Login Protocol**: Implement protocol 8.60 Login (0x0A) with Challenge support (0x1F).
- **Network Plumbing**: Extend `ClientConnection` and `IncomingCommandHandler` for non-XTEA/RSA packets.
- **Client State**: Implement a state machine (Login, Connecting, InGame).
- **User Interface**: Create a functional Raylib Login Screen with credential input.
- **Game Transition**: Seamless transition from Login UI to World Rendering on successful authentication.

## Core Value
Create a high-performance Tibia Client using Raylib that mirrors the OpenTibia server's handler-driven architecture.

## Constraints
- **Language:** C# (to match existing server)
- **Graphics:** Raylib
- **Architecture:** Mirror OpenTibia (Dispatcher/Context/Command/Event)
- **Dependencies:** Must reuse `OpenTibia.Common/Network/Security/IO/FileFormats` as is.
