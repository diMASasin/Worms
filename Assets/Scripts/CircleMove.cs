using UnityEngine;

public class CircleMove : MonoBehaviour
{
    private Camera _camera;
    private Plane _plane = new Plane(Vector3.forward, Vector3.zero);
    void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out float distance))
            {
                Vector3 position = ray.GetPoint(distance);
                transform.position = position;
            }
        }
    }

}
