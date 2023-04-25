using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Circle : MonoBehaviour
{
    [SerializeField] ColliderRenderer _colliderRenderer;
    [SerializeField] int _sides;
    [SerializeField] PolygonCollider2D _collider;
    [SerializeField] bool _scrollOn = false;
    private void Update()
    {
        if (Application.isPlaying)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0 && _scrollOn)
            {
                _sides += (int)scroll;
                CreateCircle();
            }
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                transform.localScale *= 1.1f;
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                transform.localScale /= 1.1f;
            }
        }
    }

    private void OnValidate()
    {
        CreateCircle();
    }

    void CreateCircle()
    {
        _collider.CreatePrimitive(_sides, Vector2.one);
        _colliderRenderer.CreateMesh();
    }



}
