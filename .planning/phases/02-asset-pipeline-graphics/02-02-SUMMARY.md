# Phase 2 Plan 02: Sprite Loader Summary

## Frontmatter
- **Phase**: 2
- **Plan**: 02
- **Subsystem**: Graphics
- **Tags**: Asset Pipeline, Sprites, Async
- **Tech Stack**: Raylib-cs, OpenTibia.FileFormats, OpenTibia.Game.Common.Promises
- **Key Files**: 
    - `OpenTibia.Client/Graphics/SpriteLoader.cs`
    - `OpenTibia.Client.Tests/SpriteLoaderTests.cs`
- **Duration**: 1 hour
- **Completed**: 2024-10-24

## One-liner
Asynchronous Tibia sprite loading with RLE decoding, BGRA-to-RGBA conversion, and main-thread GPU upload.

## Deviations from Plan
None - plan executed exactly as written.

## Decisions Made
- [D-06]: Implement BGRA-to-RGBA conversion in `SpriteLoader` to match Raylib's expected pixel format.
- [D-07]: Use a `ConcurrentDictionary` to track pending sprite loads and prevent duplicate requests.

## Known Stubs
- None.

## Self-Check: PASSED
