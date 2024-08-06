using DG.Tweening;
using UnityEngine;

namespace Water
{
    public class WaterVelocityChanger
    {
        private readonly Material _stylizedWaterMaterial;
        private readonly float _maxWaveHeight;
        
        private static readonly int WaveDirection = Shader.PropertyToID("_WaveDirection");
        private static readonly int WaveHeight = Shader.PropertyToID("_WaveHeight");

        public WaterVelocityChanger(Material stylizedWaterMaterial, float maxWaveHeight)
        {
            _maxWaveHeight = maxWaveHeight;
            _stylizedWaterMaterial = stylizedWaterMaterial;
        }

        public void ChangeVelocity(float normalizedVelocity)
        {
            _stylizedWaterMaterial.SetVector(WaveDirection, new Vector4(-normalizedVelocity, 0));
            _stylizedWaterMaterial.SetFloat(WaveHeight, Mathf.Abs(normalizedVelocity * _maxWaveHeight));
        }
    }
}