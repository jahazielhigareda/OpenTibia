# Phase 2: Asset Pipeline & Graphics - Research

**Researched:** 2024-10-24
**Domain:** Graphics, Asset Loading, Multithreading, Raylib
**Confidence:** HIGH

## Summary

This phase focuses on building a robust, asynchronous asset pipeline for the Tibia Raylib Client. The primary challenge is extracting sprite data from legacy Tibia `.spr` files and uploading them to the GPU without blocking the main rendering thread. We will leverage the existing `OpenTibia.FileFormats` for parsing and `OpenTibia.Game.Common.Promises` for asynchronous flow control.

**Primary recommendation:** Use a dual-dispatcher approach where heavy disk I/O and RLE decompression happen on background threads, while GPU uploads are queued to a `MainThreadDispatcher` executed within the Raylib frame loop.

## Standard Stack

### Core
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| Raylib-cs | 7.0.2 | Graphics API | Industry standard C# wrapper for Raylib. |
| OpenTibia.FileFormats | Current | Sprite/Dat Parsing | Existing project library for Tibia file formats. |
| OpenTibia.Game.Common | Current | Promises/Context | Existing project library for async flow and shared state. |

### Supporting
| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| System.Memory | Current | Span/Memory usage | High-performance pixel manipulation. |

## Architecture Patterns

### Recommended Project Structure
```
OpenTibia.Client/
├── Graphics/
│   ├── TextureCache.cs      # Manages GPU textures and async loading
│   ├── MainThreadDispatcher.cs # Queues actions for the Raylib thread
│   └── CameraManager.cs     # Handles Camera2D and coordinate transforms
├── Loaders/
│   └── SpriteLoader.cs      # Wraps SprFile for on-demand loading
```

### Pattern 1: Asynchronous Texture Loading
**What:** Decouple disk I/O and decompression from GPU upload.
**When to use:** All asset loading to prevent frame drops.
**Example:**
```csharp
public PromiseResult<Texture2D> GetSprite(int spriteId) {
    if (_cache.TryGetValue(spriteId, out var tex)) return Promise.FromResult(tex);
    
    var promise = new PromiseResult<Texture2D>();
    Task.Run(() => {
        // 1. Background: Load pixels
        byte[] rgbaPixels = _spriteLoader.LoadRawPixels(spriteId);
        
        // 2. Main Thread: Upload to GPU
        _mainDispatcher.Post(() => {
            unsafe {
                fixed (byte* p = rgbaPixels) {
                    Image img = new Image {
                        data = p,
                        width = 32,
                        height = 32,
                        format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
                        mipmaps = 1
                    };
                    Texture2D texture = Raylib.LoadTextureFromImage(img);
                    _cache[spriteId] = texture;
                    promise.TrySetResult(texture);
                }
            }
        });
    });
    return promise;
}
```

### Anti-Patterns to Avoid
- **Synchronous LoadTexture:** Never call `Raylib.LoadTexture` or `SprFile.Load` directly in the `Update` or `Draw` loop.
- **Background GPU Upload:** Raylib/OpenGL contexts are thread-local; attempting to create textures on a background thread will fail or cause instability.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| RLE Decoding | Custom RLE parser | `Sprite.Load` | `OpenTibia.FileFormats.Spr.Sprite` already handles Tibia's RLE format. |
| Async/Await | Custom Task system | `PromiseResult<T>` | Already integrated with `ClientServer` and `Command` pipeline. |
| Memory Pooling | Custom Byte Pool | `ArrayPool<byte>` | Standard .NET high-performance pooling. |

## Common Pitfalls

### Pitfall 1: BGRA vs RGBA
**What goes wrong:** Tibia sprites are extracted in BGRA format by `Sprite.cs`, but Raylib defaults to RGBA.
**How to avoid:** Swap R and B channels during the decompression phase in `SpriteLoader` before passing to Raylib.

### Pitfall 2: GPU Memory Leaks
**What goes wrong:** Loading many sprites without unloading them.
**How to avoid:** Implement a `TextureCache.UnloadAll()` and potentially an LRU eviction strategy if sprite count exceeds 10,000.

### Pitfall 3: SprFile Memory Bloat
**What goes wrong:** `SprFile.Load` currently loads *all* sprites into memory at once.
**How to avoid:** Modify `SprFile` or create a `SpriteLoader` that only reads the index table initially and performs `Seek` + `Sprite.Load` on demand.

## Code Examples

### MainThreadDispatcher implementation
```csharp
public class MainThreadDispatcher {
    private readonly ConcurrentQueue<Action> _actions = new();
    public void Post(Action action) => _actions.Enqueue(action);
    public void Execute() {
        while (_actions.TryDequeue(out var action)) action();
    }
}
```

### Camera2D Integration
```csharp
// worldPos is (tileX * 32, tileY * 32)
Camera2D camera = new Camera2D {
    target = playerWorldPosPixels,
    offset = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
    rotation = 0.0f,
    zoom = 1.0f
};

// In Draw loop:
Raylib.BeginMode2D(camera);
// Draw tiles...
Raylib.EndMode2D();
```

## Open Questions

1. **Memory Ownership:** Should `TextureCache` own the `Texture2D` lifecycle entirely, or should it use `RefCounted` textures?
   - Recommendation: Start with simple ownership; implement RefCount only if memory pressure becomes an issue.
2. **Sprite Animation Timing:** How to sync `Item` animations with the global client clock?
   - Recommendation: Use `Raylib.GetTime()` or a dedicated `Clock` component in `LocalGameState`.

## Environment Availability

| Dependency | Required By | Available | Version | Fallback |
|------------|------------|-----------|---------|----------|
| Raylib-cs | Graphics | ✓ | 7.0.2 | — |
| .NET 10.0 | Runtime | ✓ | 10.0 | — |

## Validation Architecture

### Test Framework
| Property | Value |
|----------|-------|
| Framework | xUnit |
| Config file | OpenTibia.Client.Tests.csproj |
| Quick run command | `dotnet test OpenTibia.Client.Tests` |

### Phase Requirements → Test Map
| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| ASSET-01 | Async Sprite Loading | Integration | `dotnet test --filter SpriteLoaderTests` | ❌ Wave 0 |
| ASSET-02 | Texture Cache Hits | Unit | `dotnet test --filter TextureCacheTests` | ❌ Wave 0 |
| CAMERA-01 | Coord Transform | Unit | `dotnet test --filter CameraTests` | ❌ Wave 0 |

## Sources

### Primary (HIGH confidence)
- `OpenTibia.FileFormats/Spr/Sprite.cs` - RLE decoding logic.
- `OpenTibia.Game.Common/Promises/Promise.cs` - Promise implementation.
- `Raylib-cs` Official GitHub Examples - Camera and Texture usage.

### Secondary (MEDIUM confidence)
- OTClient source code - reference for Tibia sprite layering and animation logic.
