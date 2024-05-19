using UnityEngine;

namespace Wind
{
    public class Wind1DiractionView : MonoBehaviour
    {
        [SerializeField] private Wind _wind;
        [SerializeField] private RectTransform _wind1DirView;
        [SerializeField] private RectTransform _wind1DirViewViewport;
        [SerializeField] private float _maxRightValue;

        // private float _minRightValue = 17;

        public void SetSizeDeltaX(float normalizedVelocity)
        {
            float x = _maxRightValue * normalizedVelocity;
            _wind1DirViewViewport.sizeDelta = new Vector2(x, _wind1DirView.sizeDelta.y);

            // Vector3 windViewPosition = _wind1DirView.position;
            // _wind1DirView.position = new Vector3(-x / 2, windViewPosition.y, windViewPosition.z);
        }
    }
}