using UnityEngine;

namespace Wind_
{
    public class Wind1DiractionView : MonoBehaviour
    {
        [SerializeField] private RectTransform _wind1DirView;
        [SerializeField] private RectTransform _wind1DirViewViewport;
        [SerializeField] private float _maxRightValue;

        public void SetSizeDeltaX(float normalizedVelocity)
        {
            float x = _maxRightValue * normalizedVelocity;
            _wind1DirViewViewport.sizeDelta = new Vector2(x, _wind1DirView.sizeDelta.y);
        }
    }
}