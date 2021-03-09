using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace Para_1
{
    class Scene : IDisposable
    {
        private Dictionary<string, Texture> _textures;
        public Dictionary<string, Texture> Textures { get => _textures; }

        private Dictionary<string, Material> _materials;
        public Dictionary<string, Material> Materials { get => _materials; }

        private List<MeshObject> _meshes;
        public List<MeshObject> Meshes { get => _meshes; }

        public Scene()
        {
            _textures = new Dictionary<string, Texture>();
            _materials = new Dictionary<string, Material>();
            _meshes = new List<MeshObject>();
        }

        public void Dispose()
        {
            for (int i = _meshes.Count - 1; i >= 0; i--)
            {
                MeshObject meshObject = _meshes[i];
                _meshes.Remove(meshObject);
                Utilities.Dispose(ref meshObject);
            }


            for (int i = _textures.Count - 1; i >= 0; i--)
            {
                string textureName = _textures.ElementAt(i).Key;
                Texture texture = _textures.ElementAt(i).Value;
                _materials.Remove(textureName);
                Utilities.Dispose(ref texture);
            }
        }
    }
}
