using Xunit;
using OpenTibia.Client.Graphics;
using System.IO;
using Raylib_cs;
using System;
using OpenTibia.Game.Common;
using System.Threading.Tasks;
using OpenTibia.Threading;

namespace OpenTibia.Client.Tests
{
    public class SpriteLoaderTests
    {
        [Fact]
        public void LoadSprite_ReturnsPromise()
        {
            var dispatcher = new MainThreadDispatcher();
            var cache = new TextureCache(t => { /* dummy unload */ });
            var loader = new SpriteLoader(dispatcher, cache, "nonexistent.spr");

            var promise = loader.LoadSprite(1);

            Assert.NotNull(promise);
            Assert.False(promise.IsCompleted);
        }

        [Fact]
        public async Task LoadSprite_FailsWhenFileNotFound()
        {
            var dispatcher = new MainThreadDispatcher();
            var cache = new TextureCache(t => { /* dummy unload */ });
            var loader = new SpriteLoader(dispatcher, cache, "nonexistent.spr");

            var promise = loader.LoadSprite(1);

            // In tests, we need to wait for the background task to try to load the file
            // and fail.
            try
            {
                await promise;
            }
            catch (Exception ex)
            {
                Assert.Contains("nonexistent.spr", ex.Message);
            }

            Assert.True(promise.IsFaulted);
        }
    }
}
