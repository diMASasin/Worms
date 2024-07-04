using UnityEngine;
using Wind_;

namespace Water
{
    public class WaterVelocityChanger
    {
        private readonly StylizedWater.Scripts.StylizedWater _stylizedWater;
        
        private static readonly int WaveDirection = Shader.PropertyToID("_WaveDirection");
        private static readonly int WaveHeight = Shader.PropertyToID("_WaveHeight");

        public WaterVelocityChanger(StylizedWater.Scripts.StylizedWater stylizedWater)
        {
            _stylizedWater = stylizedWater;
        }

        public void ChangeVelocity(float normalizedVelocity)
        {
            _stylizedWater.material.SetVector(WaveDirection, new Vector4(-normalizedVelocity, 0));
            _stylizedWater.material.SetFloat(WaveHeight, Mathf.Abs(normalizedVelocity));
        }
    }
}