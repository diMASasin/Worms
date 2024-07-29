using System;
using UnityEngine;

[Serializable]
public class MapSelectorItem
{
    [field: SerializeField] public string Name { get; private set; } = "Map";
    [field: SerializeField] public Sprite Sprite { get; private set; }
}
