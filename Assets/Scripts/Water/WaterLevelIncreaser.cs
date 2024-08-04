using DG.Tweening;
using UnityEngine;

namespace Water
{
    public class WaterLevelIncreaser : MonoBehaviour
    {
        private float _step;
        private bool _shouldIncreaseLevel;
    
        public void Init(float step)
        {
            _step = step;
        }
    
        public void IncreaseLevelIfAllowed()
        {
            if (_shouldIncreaseLevel)
                IncreaseLevel();
        }
    
        public void AllowIncreaseWaterLevel()
        {
            _shouldIncreaseLevel = true;
        }

        private void IncreaseLevel()
        {
            transform.DOMove(transform.position + new Vector3(0, 0 + _step, 0), 1);
        }
    }
}
