using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Raylib_cs;
using OpenTibia.Game.Common;
using OpenTibia.FileFormats.Spr;
using OpenTibia.IO;
using OpenTibia.Threading;

namespace OpenTibia.Client.Graphics
{
    public class SpriteLoader
    {
        private readonly Dispatcher _dispatcher;
        private readonly TextureCache _cache;
        private readonly string _sprPath;
        private readonly bool _isV960OrLater;
        private SprFile _sprFile;
        private readonly ConcurrentDictionary<int, PromiseResult<Texture2D>> _pendingLoads = new ConcurrentDictionary<int, PromiseResult<Texture2D>>();
        private bool _initialized = false;
        private readonly object _initLock = new object();

        public SpriteLoader(Dispatcher dispatcher, TextureCache cache, string sprPath, bool isV960OrLater = false)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _sprPath = sprPath ?? throw new ArgumentNullException(nameof(sprPath));
            _isV960OrLater = isV960OrLater;
        }

        private void Initialize()
        {
            lock (_initLock)
            {
                if (_initialized) return;

                if (!File.Exists(_sprPath))
                    throw new FileNotFoundException($"Sprite file not found at: {Path.GetFullPath(_sprPath)}", _sprPath);

                // Use the official FileFormats library to load the .spr structure
                _sprFile = SprFile.Load(_sprPath, _isV960OrLater);
                Console.WriteLine($"SpriteLoader: Loaded {_sprFile.Sprites.Count} sprites from {_sprPath} using SprFile.Load");
                
                _initialized = true;
            }
        }

        public PromiseResult<Texture2D> LoadSprite(int spriteId)
        {
            // Sprite IDs in Tibia .spr are 1-based index
            if (spriteId < 1) return Promise.Run<Texture2D>((resolve, reject) => reject(new Exception("Invalid Sprite ID")));

            if (_cache.TryGet(spriteId, out var texture))
            {
                return Promise.FromResult(texture);
            }

            if (_pendingLoads.TryGetValue(spriteId, out var pending))
            {
                return pending;
            }

            var promise = new PromiseResult<Texture2D>();
            if (!_pendingLoads.TryAdd(spriteId, promise))
            {
                return _pendingLoads[spriteId];
            }

            Task.Run(() =>
            {
                try
                {
                    if (!_initialized) Initialize();

                    Sprite spr;
                    lock (_initLock)
                    {
                        spr = _sprFile.Sprites.Find(s => s.Id == spriteId);
                    }

                    if (spr == null)
                    {
                        throw new Exception($"Sprite ID {spriteId} not found in spr file.");
                    }

                    byte[] pixels = spr.Pixels; // SprFile already decoded it

                    // Convert BGRA to RGBA for Raylib
                    byte[] rgbaPixels = new byte[pixels.Length];
                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        rgbaPixels[i] = pixels[i + 2];     // R
                        rgbaPixels[i + 1] = pixels[i + 1]; // G
                        rgbaPixels[i + 2] = pixels[i];     // B
                        rgbaPixels[i + 3] = pixels[i + 3]; // A
                    }

                    _dispatcher.QueueForExecution(new DispatcherEvent(() =>
                    {
                        try
                        {
                            unsafe
                            {
                                fixed (byte* pPixels = rgbaPixels)
                                {
                                    Image image = new Image
                                    {
                                        Data = pPixels,
                                        Width = 32,
                                        Height = 32,
                                        Mipmaps = 1,
                                        Format = PixelFormat.UncompressedR8G8B8A8
                                    };

                                    Texture2D tex = Raylib.LoadTextureFromImage(image);
                                    _cache.Store(spriteId, tex);
                                    promise.TrySetResult(tex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            promise.TrySetException(ex);
                        }
                        finally
                        {
                            _pendingLoads.TryRemove(spriteId, out _);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"SpriteLoader Error: {ex.Message}");
                    Console.Error.Flush();
                    promise.TrySetException(ex);
                    _pendingLoads.TryRemove(spriteId, out _);
                }
            });

            return promise;
        }
    }
}
