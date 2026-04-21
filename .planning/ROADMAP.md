# Roadmap

## Completed Milestones
- [x] **Milestone v1.0: Foundation & Core Logic** — [v1.0-foundation.md](milestones/v1.0-foundation.md) (Shipped 2026-04-21)
- [x] **Milestone v1.1: Prototype Polish** (Shipped 2026-04-21)

## Milestone v1.2: Login & Game Flow
- [x] **Phase 12: Network Protocol & Data Structures** (COMPLETED)
    *   Goal: Define the base types and incoming packets for the 8.60 protocol.
    *   **Requirements**: PKT-INC-01, PKT-INC-02, PKT-INC-03, UI-LGN-01, NET-LGN-02, NET-LGN-03, NET-LGN-04
    *   **Success Criteria**:
        1. Packets serializable/deserializable.
        2. RSA encryption logic for login verified.
    *   **Plans**: 1 plan
        - [x] 12-01-PLAN.md — Define data structures and packets. (COMPLETED)

- [x] **Phase 13: Logic & Connectivity** (COMPLETED)
    *   Goal: Implement connection management and command handling.
    *   **Requirements**: NET-CORE-01, NET-CORE-02, NET-CORE-03, CMD-INC-01, CMD-INC-02, CMD-INC-03
    *   **Success Criteria**:
        1. Connection established to 8.60 server.
        2. Login packet sent and received successfully.
    *   **Plans**: 1 plan
        - [x] 13-01-PLAN.md — Core network and command logic. (COMPLETED)

- [x] **Phase 14: User Interface & State Machine** (COMPLETED)
    *   Goal: Create the user-facing login screen and coordinate states.
    *   **Requirements**: UI-LGN-02, UI-LGN-03
    *   **Success Criteria**:
        1. Login UI captures input correctly.
        2. State machine transitions from Login to InGame seamlessly.
    *   **Plans**: 1 plan
        - [x] 14-01-PLAN.md — UI implementation and main loop update. (COMPLETED)

## Future Milestones
- [ ] **Milestone v1.3: Game Mechanics Expansion** (Outfits, Combat, Inventory)
