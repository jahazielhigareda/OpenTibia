using Xunit;
using Raylib_cs;
using OpenTibia.Client.Graphics;
using System.Collections.Generic;

namespace OpenTibia.Client.Tests
{
    public class TextureCacheTests
    {
        private void MockUnload(Texture2D texture) { /* No-op */ }

        [Fact]
        public void StoreAndTryGet_Works()
        {
            var cache = new TextureCache(MockUnload);
            var texture = new Texture2D { Id = 1 };

            cache.Store(100, texture);

            Assert.True(cache.TryGet(100, out var retrieved));
            Assert.Equal(1u, retrieved.Id);
        }

        [Fact]
        public void Unload_RemovesFromCache()
        {
            var cache = new TextureCache(MockUnload);
            var texture = new Texture2D { Id = 1 };
            cache.Store(100, texture);

            cache.Unload(100);

            Assert.False(cache.TryGet(100, out _));
        }

        [Fact]
        public void UnloadAll_ClearsCache()
        {
            var cache = new TextureCache(MockUnload);
            cache.Store(1, new Texture2D { Id = 1 });
            cache.Store(2, new Texture2D { Id = 2 });

            cache.UnloadAll();

            Assert.False(cache.TryGet(1, out _));
            Assert.False(cache.TryGet(2, out _));
        }

        [Fact]
        public void Store_SameId_CallsUnloadOnOldTexture()
        {
            bool unloaded = false;
            var cache = new TextureCache(t => unloaded = true);
            
            cache.Store(1, new Texture2D { Id = 1 });
            Assert.False(unloaded);

            cache.Store(1, new Texture2D { Id = 2 });
            Assert.True(unloaded);
        }
    }
}
