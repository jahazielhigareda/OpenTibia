# Requirements: v1.2 Login & Game Flow

## Protocol 8.60 Login Flow

- **NET-LGN-01**: Support Challenge-response (0x1F) from server.
- **NET-LGN-02**: Implement `LoginOutgoingPacket` (0x0A) without initial XTEA.
- **NET-LGN-03**: RSA encryption of XTEA keys, account, and password.
- **NET-LGN-04**: Adler32 checksum and length header for unencrypted login packet.

## Network Infrastructure

- **NET-CORE-01**: Extend `ClientConnection` to support sending unencrypted payloads (Login).
- **NET-CORE-02**: Implement `RawMessageCollection` for bypass of standard envelope logic.
- **NET-CORE-03**: Update `IncomingCommandHandler` with registration for 0x1F, 0x15, 0x0F.

## Incoming Packets (OpenTibia.Network)

- **PKT-INC-01**: `ConnectionInfoIncomingPacket` (0x1F).
- **PKT-INC-02**: `PendingStateIncomingPacket` (0x15).
- **PKT-INC-03**: `EnterWorldIncomingPacket` (0x0F).

## Incoming Commands (OpenTibia.Client)

- **CMD-INC-01**: `ConnectionInfoCommand` (Handles challenge, triggers login).
- **CMD-INC-02**: `PendingStateCommand` (Transition to `EnteringWorld`).
- **CMD-INC-03**: `EnterWorldCommand` (Transition to `InGame`).

## State & UI

- **UI-LGN-01**: `ClientState` enum (Login, Connecting, LoggingIn, EnteringWorld, InGame).
- **UI-LGN-02**: `LoginScreen` class using Raylib for user input.
- **UI-LGN-03**: `Program.cs` state machine for switching between Login and Game rendering.

## Requirements Table

| ID | Phase | Status |
|----|-------|--------|
| NET-LGN-01 | Phase 12 | Pending |
| NET-LGN-02 | Phase 12 | Pending |
| NET-LGN-03 | Phase 12 | Pending |
| NET-LGN-04 | Phase 12 | Pending |
| NET-CORE-01 | Phase 13 | Pending |
| NET-CORE-02 | Phase 13 | Pending |
| NET-CORE-03 | Phase 13 | Pending |
| PKT-INC-01 | Phase 12 | Pending |
| PKT-INC-02 | Phase 12 | Pending |
| PKT-INC-03 | Phase 12 | Pending |
| CMD-INC-01 | Phase 13 | Pending |
| CMD-INC-02 | Phase 13 | Pending |
| CMD-INC-03 | Phase 13 | Pending |
| UI-LGN-01 | Phase 12 | Pending |
| UI-LGN-02 | Phase 14 | Pending |
| UI-LGN-03 | Phase 14 | Pending |
