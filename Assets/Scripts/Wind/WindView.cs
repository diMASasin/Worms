using UnityEngine;
using UnityEngine.UI;

public class WindView : MonoBehaviour
{
    [SerializeField] private Wind _wind;
    [SerializeField] private Wind1DiractionView _windRight;
    [SerializeField] private Wind1DiractionView _windLeft;

    private void OnValidate()
    {
        _wind = FindObjectOfType<Wind>();
    }

    private void OnEnable()
    {
        _wind.VelocityChanged += OnVelocityChanged;
    }

    private void OnDisable()
    {
        _wind.VelocityChanged -= OnVelocityChanged;
    }

    private void OnVelocityChanged(float velocity)
    {
        var normalizedVelocity = Mathf.Abs(velocity / _wind.MaxVelocity);

        if(velocity > 0)
        {
            _windLeft.gameObject.SetActive(false);
            _windRight.gameObject.SetActive(true);
            _windRight.SetSizeDeltaX(normalizedVelocity);
        }
        else if (velocity < 0)
        {
            _windLeft.gameObject.SetActive(true);
            _windRight.gameObject.SetActive(false);
            _windLeft.SetSizeDeltaX(normalizedVelocity);
        }
        else
        {
            _windLeft.gameObject.SetActive(false);
            _windRight.gameObject.SetActive(false);
        }
    }
}
