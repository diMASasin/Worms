using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind1DiractionView : MonoBehaviour
{
    [SerializeField] private Wind _wind;
    [SerializeField] private RectTransform _wind1DirView;
    [SerializeField] private RectTransform _wind1DirViewViewport;

    private float _maxRightValue;
    private float _minRightValue = 17;

    private void Start()
    {
        _maxRightValue = _wind1DirView.sizeDelta.x;
        _minRightValue = _wind1DirViewViewport.sizeDelta.x;
    }

    public void SetSizeDeltaX(float normalizedVelocity)
    {
        var x = -_maxRightValue * (1 - normalizedVelocity) - _minRightValue;
        _wind1DirViewViewport.sizeDelta = new Vector2(x, 0);
    }
}
