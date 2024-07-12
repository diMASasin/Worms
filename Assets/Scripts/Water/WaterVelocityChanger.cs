using UnityEngine;

namespace Water
{
    public class WaterVelocityChanger
    {
        private readonly Material _stylizedWaterMaterial;
        
        private static readonly int WaveDirection = Shader.PropertyToID("_WaveDirection");
        private static readonly int WaveHeight = Shader.PropertyToID("_WaveHeight");

        public WaterVelocityChanger(Material stylizedWaterMaterial)
        {
            _stylizedWaterMaterial = stylizedWaterMaterial;
        }

        public void ChangeVelocity(float normalizedVelocity)
        {
            _stylizedWaterMaterial.SetVector(WaveDirection, new Vector4(-normalizedVelocity, 0));
            _stylizedWaterMaterial.SetFloat(WaveHeight, Mathf.Abs(normalizedVelocity));
        }
    }
}