using SharpDX;

namespace Para_1
{
    class Material
    {
        private Renderer.MaterialProperties _materialProperties;
        public Renderer.MaterialProperties MaterialProperties { get => _materialProperties; }

        private Texture _texture;
        public Texture Texture { get => _texture; }


        public Material(Texture texture, Vector3 emmisiveK, Vector3 ambientK, Vector3 diffuseK, Vector3 specularK, float specularPower)
        {
            _texture = texture;
            _materialProperties = new Renderer.MaterialProperties
            {
                emmisiveK = emmisiveK,
                ambientK = ambientK,
                diffuseK = diffuseK,
                specularK = specularK,
                specularPower = specularPower
            };
        }
    }
}
