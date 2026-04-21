using System.Collections.Generic;
using Raylib_cs;

namespace OpenTibia.Client.Graphics
{
    public delegate void UnloadTextureDelegate(Texture2D texture);

    public class TextureCache
    {
        private readonly Dictionary<int, Texture2D> _textures = new Dictionary<int, Texture2D>();
        private readonly UnloadTextureDelegate _unloadAction;

        public TextureCache(UnloadTextureDelegate unloadAction = null)
        {
            _unloadAction = unloadAction ?? Raylib.UnloadTexture;
        }

        public bool TryGet(int id, out Texture2D texture)
        {
            return _textures.TryGetValue(id, out texture);
        }

        public void Store(int id, Texture2D texture)
        {
            if (_textures.TryGetValue(id, out var existing))
            {
                _unloadAction(existing);
            }
            _textures[id] = texture;
        }

        public void Unload(int id)
        {
            if (_textures.TryGetValue(id, out var texture))
            {
                _unloadAction(texture);
                _textures.Remove(id);
            }
        }

        public void UnloadAll()
        {
            foreach (var texture in _textures.Values)
            {
                _unloadAction(texture);
            }
            _textures.Clear();
        }
    }
}
