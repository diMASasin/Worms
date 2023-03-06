using UnityEngine;

public class WormInput : MonoBehaviour
{
    [SerializeField] private Worm _worm;
    [SerializeField] private Throwing _throwing;
    [SerializeField] private int _defaultLayer = 0;
    [SerializeField] private int _currentWormLayer = 6;
    [SerializeField] private Arrow _arrow;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _throwing.EnablePointerLine();
        }

        if(Input.GetMouseButton(0))
        {
            _throwing.SetPointerLinePositionAndScale();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _throwing.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _worm.Jump();
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        _worm.TryMove(horizontal);
    }

    public void EnableInput()
    {
        enabled = true;
        gameObject.layer = _currentWormLayer;
        _arrow.StartMove(); 
    }

    public void DisableInput()
    {
        enabled = false;
        gameObject.layer = _defaultLayer;
    }
}
