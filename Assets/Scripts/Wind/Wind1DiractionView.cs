using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind1DiractionView : MonoBehaviour
{
    [SerializeField] private Wind _wind;
    [SerializeField] private RectTransform _wind1DirView;
    [SerializeField] private RectTransform _wind1DirViewViewport;
    [SerializeField] private float _maxRightValue;

    private float _minRightValue = 17;

    private void Start()
    {
        _minRightValue = 0;
    }

    public void SetSizeDeltaX(float normalizedVelocity)
    {
        var x = _maxRightValue * normalizedVelocity - _minRightValue;
        _wind1DirView.sizeDelta = new Vector2(x, 0);
    }
}
