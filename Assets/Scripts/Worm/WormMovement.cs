using UnityEngine;

public class WormMovement : Movement
{
    [SerializeField] private WormInput _wormInput;

    private void OnEnable()
    {
        _wormInput.InputDisabled += OnInputDisabled;
    }

    private void OnDisable()
    {
        _wormInput.InputDisabled -= OnInputDisabled;
    }

    private void OnInputDisabled()
    {
        base.Reset();
    }
}
