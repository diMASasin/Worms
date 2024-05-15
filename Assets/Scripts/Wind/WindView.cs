using UnityEngine;

public class WindView : MonoBehaviour
{
    [SerializeField] private Wind1DiractionView _windRight;
    [SerializeField] private Wind1DiractionView _windLeft;

    private Wind _wind;

    public void Init(Wind wind)
    {
        _wind = wind;

        _wind.VelocityChanged += OnVelocityChanged;
    }

    private void OnDestroy()
    {
        if (_wind != null)
            _wind.VelocityChanged -= OnVelocityChanged;
    }

    private void OnVelocityChanged(float velocity)
    {
        var normalizedVelocity = Mathf.Abs(velocity / _wind.MaxVelocity);

        _windLeft.gameObject.SetActive(false);
        _windRight.gameObject.SetActive(false);

        if (velocity > 0)
            ChangeVelocityView(_windRight, normalizedVelocity);
        else if (velocity < 0)
            ChangeVelocityView(_windLeft, normalizedVelocity);
    }

    private void ChangeVelocityView(Wind1DiractionView view, float normalizedVelocity)
    {
        view.gameObject.SetActive(true);
        view.SetSizeDeltaX(normalizedVelocity);
    }
}