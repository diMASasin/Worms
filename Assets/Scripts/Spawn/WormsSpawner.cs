using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WormsSpawner : MonoBehaviour
{
    [SerializeField] private Land _land;
    [SerializeField] private Camera _camera;
    [SerializeField] private Worm _wormTemplate;
    [SerializeField] private Transform _container;
    [SerializeField] private int _wormsNumber = 4;

    private Vector2[] _points;
    private List<Edge> edges = new();

    private void Start()
    {
        GetEdgesForSpawn();

        for (int i = 0; i < _wormsNumber; i++)
            Instantiate(_wormTemplate, GetRandomPointForSpawn(), Quaternion.identity, _container);
    }

    private void GetEdgesForSpawn()
    {
        int length = _land.PolygonCollider2D.points.Length;
        _points = new Vector2[length];
        Array.Copy(_land.PolygonCollider2D.points, _points, length);

        for (int i = 0; i < _points.Length; i++)
        {
            int j = i + 1;
            if (j >= _points.Length)
                j = 0;

            var newEdge = new Edge(_points[i], _points[j]);
            if (newEdge.IsFloor(_land) && newEdge.IsSuitableSlope())
                edges.Add(newEdge);
        }
    }

    private Vector2 GetRandomPointForSpawn()
    {
        int random = Random.Range(0, edges.Count);
        var randomEdge = edges[random];
        Vector2 randomPoint = Vector2.Lerp(randomEdge.Point1, randomEdge.Point2, Random.value);
        randomPoint.y += _wormTemplate.Collider2D.size.y;
        return randomPoint;
    }
}
